using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;
using Microsoft.CSharp;
using System.IO;
using System.Text;

[Serializable]
public class LoginResult //{"result":"OK","save_game":"","language":"it","true":"1T!","false":"0F!","session_id":""}
{
    public string result;
    public string save_game;
    public string language;
    public string trueLabel;
    public string falseLabel;
    public string session_id;
}

[Serializable]
public class ErrorResult
{
    public string result;
    public string error;
}

[Serializable]
public class SentenceResult
{
    public string result;
    public List<APISentence> sentences;
}

[Serializable]
public class SentenceResultSingle
{
    public string result;
    public APISentence sentence;
}

[Serializable]
public class Urls
{
    public string getID, getSentenceAcc, getGoldAcc, getSentence, getConditions, getSentenceGraffiti, getSentenceGraffitiEng, getGoldDialogues, getGoldGraffiti,
        getSentenceDialoguesEng, getParticipants, getOffset, getSentenceDialogues, getSentenceDemo, getSave, getQuestionnaire,
        postAnnotationAcc, postAnnotation, postParticipants, postSave, postQuestionnaire, postLog,
        updateConditions, getSystemMessages;
}

[Serializable]
public class APISentence
{
    public int id;
    public string[] tokens;
    public bool annotated;

    public APISentence() { }
    public APISentence(string sent)
    {
        this.id = 0;
        this.tokens = WordByWord.RegexTokenizer(sent).ToArray();
        this.annotated = false;
    }
    public APISentence(string sent, int id)
    {
        this.id = id;
        this.tokens = WordByWord.RegexTokenizer(sent).ToArray();
        this.annotated = false;
    }
}

[Serializable]
public class APISentences
{
    public List<APISentence> sentences;
}

[Serializable]
public class StudentLogin
{
    public string sessionID;
    public string language;
    public List<APISentence> graffiti;
    public List<APISentence> dialogues;
    public GameState save;
}
[Serializable]
public class SystemDialogues
{
    public Dialogues scientist;
    public Dialogues trappedStudent;
    public Dialogues questNPC;
}
[Serializable]
public class Dialogues
{
    public List<DialogueLines> dialogueLines;

    public Dialogues() { dialogueLines = new List<DialogueLines>(); }
}
[Serializable]
public class DialogueLines
{
    public int id;

    public List<string> lines;

    public DialogueLines() { lines = new List<string>(); }
}


public enum Api { dev = 0, final = 1 }
public enum Variant { graffiti, dialogues }

public class API : MonoBehaviour
{
    public static Api currentApi = Api.final;

    public static bool useLocalDatasets;

    //Urls
    public static Urls urls;
    public static string domain = "https://www.spacewords.altervista.org";
    public static string dialogueUrl = domain+"/HSSH/getDialogues.php";
    public static string getUrls = domain+"/HSSH/getUrls.php";
    public static bool urlerror;
    public static bool readyToLoadGame;
    public static bool systemMessagesRetrieved;

    //ID - old API
    public static bool isIDRegistered;
    public static bool logged;

    //Data - old API
    public static string sentence;

    //New API
    public static string url;
    public static int LoginState;
    public static APISentence currentSentence;
    public static List<APISentence> dialogueSentences;
    public static List<APISentence> graffitiSentences;
    public static List<string> sampleSentences;
    public static bool graffitiFinishedWarningShown;
    public static bool dialoguesFinishedWarningShown;
    public static SystemDialogues systemDialogues;
    public static bool graffitiFinished;
    public static bool dialoguesFinished;

    //Login
    public static bool loggedIn;
    public static string sessionID;

    //Old
    public static IEnumerator RegisterNewParticipant(int sentenceCount)
    {
        string url;
        int participantN = 0;

        if (currentApi == Api.dev)
        {
            url = API.urls.getParticipants;
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();
                if (!String.IsNullOrEmpty(www.downloadHandler.text))
                {
                    participantN = int.Parse(www.downloadHandler.text);
                }
            }
            url = API.urls.postParticipants;
            WWWForm form = new WWWForm();
            form.AddField("Value", participantN + 1);
            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                yield return www.SendWebRequest();
            }

            int offset = 2; //default
            url = API.urls.getOffset;
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();
                if (!String.IsNullOrEmpty(www.downloadHandler.text))
                {
                    offset = int.Parse(www.downloadHandler.text);
                }
            }

            OffsetSingle(participantN, offset, sentenceCount);
        }
    }
    public static void OffsetSingle(int npart, int offset, int count)
    {
        //int blocks = (int)((float)count / offset);
        //int mod = count % offset;
        //int loops = (int)((float)npart / blocks);
        //npart -= (loops * blocks);
        //int newIndex = npart * offset;
        //newIndex += offset - mod;

        int newIndex = (npart * offset) - (count * ((npart * offset) / count));

        DialogueInstancer.uniqueLineIndex = newIndex;
        Graffiti.uniqueGraffitiIndex = 0;
    }
    public static int GetParticipants()
    {
        return 0;
    }
    public static IEnumerator CheckID(string id)
    {
        WWWForm form = new WWWForm();
        form.AddField("ID", id);
        string uri = API.urls.getID;
        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            yield return www.SendWebRequest();

            if (www.downloadHandler.text == "1")
            {
                isIDRegistered = true;
            }
            else
            {
                isIDRegistered = false;
            }
            logged = true;
        }
    }
    public static IEnumerator GetUrls(ML.Lang lang)
    {

        if (API.currentApi == Api.dev)
        {
            domain = "http://www.spacewords.altervista.org";
        }
        Urls n = new Urls();
        string urljson;
        string url = lang == ML.Lang.en ? domain + "/HSSH/getUrlTableEng.php" : domain + "/HSSH/getUrlTable.php";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.ConnectionError)
            {
                urljson = www.downloadHandler.text;
                urls = JsonUtility.FromJson<Urls>(urljson);
            }
            else
            {
                urlerror = true;
            }
            readyToLoadGame = true;
        }
        
    }

    //----------------------------------------
    //
    // LOGIN
    //
    //----------------------------------------
    public static IEnumerator SendLoginRequest(Player agent, string user, string pass)
    {
        LoginState = -1;
        string uri = $"{API.url}?action=userLogin&username={user}&password={pass}";
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            www.timeout = 5;
            yield return www.SendWebRequest();
            string text = www.downloadHandler.text;
            if (www.timeout > 5)
                PopUpUtility.Open(FindObjectOfType<CameraInterface>().popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.error), "Timeout", 5);
            if (www.result == UnityWebRequest.Result.Success)
            {
                LoginState = 0;

                LoginState = 1;
                LoginResult loginres = JsonUtility.FromJson<LoginResult>(text);
                API.sessionID = loginres.session_id;
                agent.sessionID = loginres.session_id;
                int offset;
                int limit;

                try
                {
                    agent.gameState = JsonUtility.FromJson<GameState>(loginres.save_game);
                }
                catch
                {
                    agent.gameState = null;
                }

                if (agent.gameState != null)
                    agent.loaded = true;
                else
                    agent.loaded = false;
                offset = 0;//
                limit = 300;//


                yield return agent.StartCoroutine(GetSentenceBulk(agent, 0, Variant.dialogues, limit, offset));
                agent.graffitiSentences = graffitiSentences;

                Player.language = (ML.Lang)Enum.Parse(typeof(ML.Lang), loginres.language);
                Annotation.trueLabel = loginres.trueLabel;
                Annotation.falseLabel = loginres.falseLabel;

            }
            else
            {
                ErrorResult erres = JsonUtility.FromJson<ErrorResult>(text);
                if (erres != null)
                    PopUpUtility.Open(FindObjectOfType<CameraInterface>().popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.error), www.error + "\n" + "Action: Login" + "\n" + erres.error, 5);
                else
                    PopUpUtility.Open(FindObjectOfType<CameraInterface>().popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.error), www.error + "\n" + "Action: Login", 5);
            }
        }
    }

    //----------------------------------------
    //
    // GET SAVE STATE
    //
    //----------------------------------------
    public static void LoadGame(Player agent)
    {
        if (API.currentApi == Api.dev)
            agent.StartCoroutine(GetSaveC(agent));
        else
        {
            if (agent.loaded == true)
            {
                SaveManager.DeployGameState(agent, agent.gameState);
            }
            else
            {
                SaveManager.gameLoaded = true;
            }
        }
    }
   
    public static IEnumerator GetSaveC(Player agent)
    {
        GameState state = null;
        string json;

        WWWForm form = new WWWForm();
        form.AddField("ID", agent.id);

        using (UnityWebRequest www = UnityWebRequest.Post(API.urls.getSave, form))
        {
            yield return www.SendWebRequest();
            json = www.downloadHandler.text;
        }

        if (!String.IsNullOrEmpty(json))
        {
            state = JsonUtility.FromJson<GameState>(json);
        }

        if (state != null)
        {
            SaveManager.DeployGameState(agent, state);
        }
        else
        {
            SaveManager.gameLoaded = true;
        }
    }

    //----------------------------------------
    //
    // POST SAVE STATE
    //
    //----------------------------------------

    public static void PostSave(Player agent, bool showMessage)
    {
        if (currentApi == Api.dev)
            agent.StartCoroutine(PostSaveDevC(agent, showMessage));
        else
            agent.StartCoroutine(PostSaveFinalC(agent, showMessage));
    }
    public static IEnumerator PostSaveDevC(Player agent, bool showMessage)
    {
        WWWForm form = new WWWForm();
        form.AddField("ID", agent.id);
        form.AddField("GameState", JsonUtility.ToJson(agent.gameState));

        using (UnityWebRequest www = UnityWebRequest.Post(API.urls.postSave, form))
        {
            yield return www.SendWebRequest();
        }
        if (showMessage)
            PopUpUtility.Open(agent.cameraInterface.popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.success), ML.systemMessages.gameSaved, 2);
    }
    public static IEnumerator PostSaveFinalC(Player agent, bool showMessage) //?action=task&session_id=cb3b3dff39a8e828572536cbb5d1d92e&type=hssh&sub=saveGame&save_game=pippo
    {
        if (Player.admin)
        {
            //PopUpUtility.Open(agent.cameraInterface.popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.warning), "Demo mode: game not saved", 2);
            yield break;
        }

        WWWForm form = new WWWForm();
        string action = "task";
        form.AddField("action", action);
        form.AddField("session_id", agent.sessionID);
        form.AddField("type", "hssh");
        form.AddField("sub", "saveGame");
        form.AddField("save_game", JsonUtility.ToJson(agent.gameState));

        print("save: "+JsonUtility.ToJson(agent.gameState));

        //TEMP
        string u = $"http://kidactions.fbk.eu/api/?action=task&session_id={agent.sessionID}&type=hssh&sub=saveGame&save_game={JsonUtility.ToJson(agent.gameState)}";
        string u2 = $"action=task&session_id={agent.sessionID}&type=hssh&sub=saveGame&save_game={JsonUtility.ToJson(agent.gameState)}";


        byte[] bytes = Encoding.UTF8.GetBytes(u2);
        string encoded = Encoding.UTF8.GetString(bytes);
        //
        using (UnityWebRequest www = UnityWebRequest.Post(API.url, form))
        {
            yield return www.SendWebRequest();
            string text = www.downloadHandler.text;

            if (www.result == UnityWebRequest.Result.Success)
            {
                if (showMessage && API.currentApi == Api.dev)
                    PopUpUtility.Open(agent.cameraInterface.popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.success), ML.systemMessages.gameSaved, 2);
            }
            else
            {
                ErrorResult erres = JsonUtility.FromJson<ErrorResult>(text);
                if (erres != null)
                    PopUpUtility.Open(agent.cameraInterface.popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.error), www.error + "\n" + "Action: Save game" + "\n" + erres.error, 2);
                else
                    PopUpUtility.Open(agent.cameraInterface.popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.error), www.error + "\n" + "Action: Save game", 2);

                if (www.responseCode == 401)
                    FindObjectOfType<Menu>().Exit();
            }
        }      
    }

    //----------------------------------------
    //
    // GET SENTENCES
    //
    //----------------------------------------

    public static IEnumerator GetSentence(Player agent, int index, Variant variant, int offset=0, int limit=0)
    {

        if (currentApi == Api.dev)
        {
            string url;
            if (variant == Variant.dialogues)
                url = API.urls.getSentenceDialogues;
            else
                url = API.urls.getSentenceGraffiti;
            WWWForm form = new WWWForm();
            form.AddField("ID", index);

            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError && Player.demo)
                {
                    int rand = UnityEngine.Random.Range(0, 2);
                    if (variant == Variant.dialogues)
                    {
                        if (rand == 0)
                            sentence = "Shut up you nerd";
                        else
                            sentence = "Get lost fat loser";
                    }
                    else
                    {
                        if (rand == 0)
                            sentence = "Shut up you nerd";
                        else
                            sentence = "Get lost fat loser";
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(www.downloadHandler.text))
                    {
                        sentence = www.downloadHandler.text;
                    }
                }
            }
        }
        else
        {
            currentSentence = new APISentence(API.sampleSentences[UnityEngine.Random.Range(0, API.sampleSentences.Count)]);
            //if (variant == Variant.dialogues)
            //{
            //    currentSentence = agent.dialogueSentences[index];
            //}
            //else
            //{
            //    currentSentence = agent.graffitiSentences[index];
            //}
            yield return null;
        }
    }

    
    public static IEnumerator GetSentenceSingleC(Player agent, int lastID, string set)
    {
//         nextSentence (GET)
// session_id
// set = ch/gr
// action = task
// sub = nextSentence
// [last_id = null]
// RET sentence = {“id”: 1, tokens: [“Bla”, “bla”, “bla”], annotated: true/false}

        if (Player.admin)
        {
            if (API.useLocalDatasets)
            {
                if (set == "ch")
                    currentSentence = dialogueSentences[UnityEngine.Random.Range(0, dialogueSentences.Count)];
                else
                    currentSentence = graffitiSentences[UnityEngine.Random.Range(0, graffitiSentences.Count)];

            }
            else
            {
                currentSentence = new APISentence(API.sampleSentences[UnityEngine.Random.Range(0, API.sampleSentences.Count)]);            
            }
            yield break;
        }

        string sub = "nextSentence";

        string uri = $"{API.url}?action=task&sub={sub}&set={set}&type=hssh&session_id={agent.sessionID}&last_id={lastID}";
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();
            string text = www.downloadHandler.text;

            if (www.result == UnityWebRequest.Result.Success)
            {
                SentenceResultSingle sentres = JsonUtility.FromJson<SentenceResultSingle>(text);
                APISentence s = sentres.sentence;
                if (s.annotated == true) //If the server response contains an already annotated sentence it means that there are no more sentences to annotate
                {
                    yield return agent.StartCoroutine(LoadingManager.LoadSampleSentences());
                    if (set == "gr")
                    {
                        if (!API.graffitiFinishedWarningShown)
                        {
                            PopUpUtility.Open(agent.cameraInterface.popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.warning), ML.systemMessages.finishedGraffiti, 2);
                            API.graffitiFinishedWarningShown = true;
                            API.graffitiFinished = true;
                            
                        }
                    }
                    else
                    {
                        if (!API.dialoguesFinishedWarningShown)
                        {
                            PopUpUtility.Open(agent.cameraInterface.popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.warning), ML.systemMessages.finishedDialogues, 2);
                            API.dialoguesFinishedWarningShown = true;
                            API.dialoguesFinished = true;
                            DialogueInstancer.uniqueLineIndex = 0;
                        }
                    }
                }
                currentSentence = s; //This is the final objective of this function
            }
            else
            {
                ErrorResult erres = JsonUtility.FromJson<ErrorResult>(text);
                if (erres != null)
                    PopUpUtility.Open(agent.cameraInterface.popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.error), www.error + "\n" + erres.error, 2);
                else
                    PopUpUtility.Open(agent.cameraInterface.popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.error), www.error, 2);

                if (www.responseCode == 401)
                    FindObjectOfType<Menu>().Exit();
                
            }
        }
    }

    public static IEnumerator GetSentenceBulk(Player agent, int index, Variant variant, int limit = 0, int offset = 0) //Executed only once at the beginning of the game
    {
        string set = variant == Variant.graffiti ? "gr" : "ch";
        //final API ?action=task&sub=sentences&session_id=cb3b3dff39a8e828572536cbb5d1d92e&type=hssh&set=ch
        string uri = $"{API.url}?action=task&sub=sentences&session_id={agent.sessionID}&type=hssh&set={set}&limit={limit}&offset={offset}";
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();
            string text = www.downloadHandler.text;
            if (www.result == UnityWebRequest.Result.Success)
            {
                SentenceResult sentres = JsonUtility.FromJson<SentenceResult>(text);
                if (set == "gr")
                {
                    graffitiSentences = sentres.sentences;
                }
                else
                {
                    dialogueSentences = sentres.sentences;
                    DialogueInstancer.maxLineIndex = sentres.sentences.Last().id;
                    DialogueInstancer.firstIndex = sentres.sentences[0].id;
                }
            }
            else
            {
                ErrorResult erres = JsonUtility.FromJson<ErrorResult>(text);
                if (erres != null)
                    PopUpUtility.Open(agent.cameraInterface.popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.error), www.error + "\n" + "Action: Get sentence block" + "\n" + erres.error, 2);
                else
                    PopUpUtility.Open(agent.cameraInterface.popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.error), www.error + "\n" + "Action: Get sentence block", 2);
            
            
            }
        }
    }

    //----------------------------------------
    //
    // POST ANNOTATIONS
    //
    //----------------------------------------

    public static void PostAnnotation(Player agent, AnnotationData anndata)
    {
        SqlAnnotatedSentence sqlsent = new SqlAnnotatedSentence(anndata);


        if (currentApi == Api.dev)
            agent.GetComponent<Annotation>().StartCoroutine(API.PostAnnotationDevC(agent, sqlsent));
        else
            agent.GetComponent<Annotation>().StartCoroutine(API.PostAnnotationFinalC(agent, sqlsent));
    }
    public static void PostAnnotation(Player agent, SqlAnnotatedSentence sqlsent)
    {
        if (currentApi == Api.dev)
            agent.GetComponent<Annotation>().StartCoroutine(API.PostAnnotationDevC(agent, sqlsent));
        else
            agent.GetComponent<Annotation>().StartCoroutine(API.PostAnnotationFinalC(agent, sqlsent));
    }
    public static IEnumerator PostAnnotationFinalC(Player agent, SqlAnnotatedSentence sqlsent)
    {
        //final API
        //string jsent = new JSONSentence(sqlsent).json;

        if (Player.admin)
        {
            //NotificationUtility.ShowString(agent, "Demo mode: annotation not saved");
            yield break;
        }
        WWWForm form = new WWWForm();
        form.AddField("action", "task");
        form.AddField("session_id", agent.sessionID);
        form.AddField("type", "hssh");
        form.AddField("sentence_id", sqlsent.id);
        form.AddField("sub", "saveAnnotation");

        string tokenJson = JsonUtility.ToJson(sqlsent.multiTypeTokens);
        //tokenJson = tokenJson.Substring(1, tokenJson.Length-2);
        form.AddField("tokens", tokenJson);
        print(tokenJson);
        using (UnityWebRequest www = UnityWebRequest.Post(API.url, form))
        {
            yield return www.SendWebRequest();
            string text = www.downloadHandler.text;
            
            if (www.result == UnityWebRequest.Result.Success)
            {
                //if (erres == null)//Provvisorio
                //{
                    NotificationUtility.ShowString(agent, "Annotated!");
                //}
                //else
                //{
                //    print("Error detected");
                //    PopUpUtility.Open(agent.cameraInterface.popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.error), "Could not send data, the server may be offline", 5);
                //}
            }
            else
            {

                ErrorResult erres = JsonUtility.FromJson<ErrorResult>(text);
                if (erres != null)
                //PopUpUtility.Open(agent.cameraInterface.popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.error), "Could not send data to the server, check your connection", 5);
                    PopUpUtility.Open(agent.cameraInterface.popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.error), www.error + "\n" + "Action: Save annotation" + "\n" + erres.error, 10);
                else
                    PopUpUtility.Open(agent.cameraInterface.popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.error), www.error + "\n" + "Action: Save annotation", 10);

                NotificationUtility.ShowString(agent, www.error);

                if (www.responseCode == 401)
                    FindObjectOfType<Menu>().Exit();
            }
        }
    }
    public static IEnumerator PostAnnotationDevC(Player agent, SqlAnnotatedSentence sqlsent)
    {
        string url;
        url = API.urls.postAnnotation;
        WWWForm form = new WWWForm();
        try
        {
            Annotation.ValidateAnnotatedSentence(sqlsent);
            form.AddField("UserID", agent.id);
            form.AddField("SentenceID", sqlsent.id);
            form.AddField("Tokens", sqlsent.tokens);
            form.AddField("NewTokens", sqlsent.newTokens);
            form.AddField("Annotations", sqlsent.annotations);
            form.AddField("TimePerToken", sqlsent.timePerToken);
            form.AddField("Task", sqlsent.task);
            form.AddField("Gold", sqlsent.gold);
        }
        catch (AnnotationException e)
        {
            NotificationUtility.ShowString(agent, e.Message);
        }

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.error != null)
            {

                NotificationUtility.ShowString(agent, www.error);
            }
            else
            {

                NotificationUtility.ShowString(agent, "Annotated!");
            }
        }     
    }

    //System messages
    public static void GetSystemMessages(Player agent) => agent.StartCoroutine(GetSystemMessagesC());
    public static IEnumerator GetSystemMessagesC()
    {
        SystemMessages formattedData = new SystemMessages();
        if (currentApi == Api.dev)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(API.urls.getSystemMessages))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.ConnectionError)
                {
                    string messjson = www.downloadHandler.text;
                    formattedData = JsonUtility.FromJson<SystemMessages>(messjson);
                }
            }
        }
        else
        {
            //SET PATH
            string path = Path.Combine(Application.streamingAssetsPath, $"systemMessages_{Player.language}.txt");
            //
            
            string file;
            if (path.Contains("://") || path.Contains(":///"))
            {
                using (UnityWebRequest www = UnityWebRequest.Get(path))
                {
                    yield return www.SendWebRequest();

                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        file = www.downloadHandler.text;
                        formattedData = JsonUtility.FromJson<SystemMessages>(file);
                    }
                    else
                    {
                        PopUpUtility.Open(FindObjectOfType<Player>().cameraInterface.popUpCanvas, PopUpType.LocalizedType(FindObjectOfType<Player>(), PopUpType.Types.error), www.error + "\n" + "Could not load the system messages at path " + path, 10);
                    }
                }
            }
            else
            {
                file = File.ReadAllText(path);
                formattedData = JsonUtility.FromJson<SystemMessages>(file);
            }
        }
        ML.systemMessages = formattedData;
        systemMessagesRetrieved = true;
    }
}
