using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DataUtilities;
using System;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Linq;

public class Config
{
    public string lang;
    public string domain;
    public bool guest;
    public bool collectibles;
    public string api;
    public string url;
    public bool useLocalDatasets;
}

public class LoadingManager : MonoBehaviour
{
    public bool SaveFound { get; set; }
    public GameObject customizeCanvas;
    public GameObject text;
    public GameObject percentage;
    public CharacterCustomizationSetup playerCustomization;
    public GameObject bullyCanvas;
    public GameObject black;
    public Player player;
    public CameraInterface cameraInterface;
    public static bool skipCustomization;
    static TextMeshPro PercentageText { get; set; }
    static float Percentage { get; set; }
    public static int sampleNumber;
    public static int sentenceCount;
    public static Config config;

    public void Activate()
    {
        SafetyBar.TimeAttack = true;
#if UNITY_EDITOR
        PlayerPrefs.DeleteAll();
#endif
        if (GameObject.FindGameObjectWithTag("ProgressPercentage"))
            LoadingManager.PercentageText = GameObject.FindGameObjectWithTag("ProgressPercentage").GetComponent<TextMeshPro>();
        StartCoroutine(DecideNewOrLoad());
    }

    IEnumerator DecideNewOrLoad()
    {
        //Player.language = (ML.Lang)Enum.Parse(typeof(ML.Lang), config.lang);
        //API.domain = config.domain;
        //API.url = config.url;
        API.currentApi = (Api)Enum.Parse(typeof(Api), config.api);

        if (API.currentApi == Api.dev)
        {
            yield return StartCoroutine(API.GetUrls(Player.language));
            string url;
            url = API.urls.getSentenceDialogues;
            WWWForm form = new WWWForm();
            form.AddField("Count", "true");
            int count = 0;
            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                yield return www.SendWebRequest();
                if (!String.IsNullOrEmpty(www.downloadHandler.text))
                {
                    count = int.Parse(www.downloadHandler.text);
                }
            }
            sentenceCount = count;
        }
        else
        {
            //API.urlsRetrieved = true;
        }
        API.readyToLoadGame = true;
        yield return StartCoroutine(API.GetSystemMessagesC());

        if (player.loaded == true)
        {
            SaveFound = true;
            SequenceManager.sequence = Sequence.initialize;
        }
        else
        {
            SaveFound = false;
        }
        API.logged = true;
        player.GetComponent<SequenceManager>().Next();
        this.gameObject.SetActive(false);
        
    }
    public static IEnumerator LoadConfig()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "config.txt");
        string file = "";
        if (path.Contains("://") || path.Contains(":///"))
        {
            using (UnityWebRequest www = UnityWebRequest.Get(path))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.ConnectionError)
                {
                    file = www.downloadHandler.text;
                }
                else
                {

                }
            }
        }
        else
        {
            file = File.ReadAllText(path);
        }

        //Setup / Load CONFIG
        LoadingManager.config = JsonUtility.FromJson<Config>(file);
        API.currentApi = (Api)Enum.Parse(typeof(Api), LoadingManager.config.api);
        API.useLocalDatasets = LoadingManager.config.useLocalDatasets;
        Player.rCondition = LoadingManager.config.collectibles == true ? RCondition.Restricted : RCondition.NonRestricted;
        API.url = LoadingManager.config.url;

        if (API.useLocalDatasets == true)
        {
            string dpath = Path.Combine(Application.streamingAssetsPath, "Datasets/dialogues.txt");
            string gpath = Path.Combine(Application.streamingAssetsPath, "Datasets/graffiti.txt");
            string dfile = "";
            string gfile = "";
            API.dialogueSentences = new List<APISentence>();
            API.graffitiSentences = new List<APISentence>();
            int dID = 0;
            int gID = 0;

            if (dpath.Contains("://") || dpath.Contains(":///"))
            {
                using (UnityWebRequest www = UnityWebRequest.Get(dpath))
                {
                    yield return www.SendWebRequest();

                    if (www.result != UnityWebRequest.Result.ConnectionError)
                        dfile = www.downloadHandler.text;
                }
                using (UnityWebRequest www = UnityWebRequest.Get(gpath))
                {
                    yield return www.SendWebRequest();

                    if (www.result != UnityWebRequest.Result.ConnectionError)
                        gfile = www.downloadHandler.text;
                }
            }
            else
            {
                dfile = File.ReadAllText(dpath);
                gfile = File.ReadAllText(gpath);
            }


            foreach (string line in dfile.Split(new[] { "\r\n", "\r", "\n"}, StringSplitOptions.None))
            {
                print(line);
                API.dialogueSentences.Add(new APISentence(line, dID));
                dID += 1;
            }

            foreach (string line in gfile.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None))
            {
                print(line);
                API.graffitiSentences.Add(new APISentence(line, gID));
                gID += 1;
            }
        }
        
    }

    public static IEnumerator LoadSystemDialogues()
    {
        string path = Path.Combine(Application.streamingAssetsPath, $"dialogues_{Player.language}.txt");
        if (path.Contains("://") || path.Contains(":///"))
        {
            using (UnityWebRequest www = UnityWebRequest.Get(path))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string text = www.downloadHandler.text;
                    API.systemDialogues = JsonUtility.FromJson<SystemDialogues>(text);
                }
                else
                {
                    print("Could not load the sample sentences");
                }
            }
        }
        else
        {
            API.systemDialogues = JsonUtility.FromJson<SystemDialogues>(File.ReadAllText(path));
        }
    }

    public static IEnumerator LoadSampleSentences()
    {
        string path = Path.Combine(Application.streamingAssetsPath, $"sampleSentences_{Player.language}.txt");
        if (path.Contains("://") || path.Contains(":///"))
        {
            using (UnityWebRequest www = UnityWebRequest.Get(path))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    API.sampleSentences = www.downloadHandler.text.Split(new[] {'\r','\n'}).ToList();
                }
                else
                {
                    PopUpUtility.Open(FindObjectOfType<CameraInterface>().popUpCanvas, PopUpType.Error, "Could not load sample sentences", 5);
                }
            }
        }
        else
        {
            API.sampleSentences = File.ReadAllLines(path).ToList();
        }
    }

    IEnumerator PrepareNewGame()
    {
        yield return StartCoroutine(API.RegisterNewParticipant(sentenceCount));
    }

    IEnumerator CheckSavedGame()
    {
        yield return StartCoroutine(API.CheckID(PlayerPrefs.GetString("ID")));
        if (API.isIDRegistered == true)
        {
            SaveFound = true;
            SequenceManager.sequence = Sequence.initialize; //skip character creation
        }
        else
        {
            yield return StartCoroutine(API.RegisterNewParticipant(sentenceCount));
            SaveFound = false;
        }
    }

    void Update()
    {
        //print(PlayerLogger.casw.ElapsedMilliseconds);
        //if (SaveManager.urlsRetrieved)
        //{
        //    if (!checkingID)
        //    {
        //        StartCoroutine(Password.CheckID(PlayerPrefs.GetString("ID")));
        //        checkingID = true;
        //    }
        //    else if (!samplesCounted)
        //    {
        //        StartCoroutine(GetSampleNumber());
        //        countingSamples = true;
        //    }
        //    if (samplesCounted)
        //    {
        //        this.gameObject.SetActive(false);
        //        if (Password.IDChecked)
        //        {
        //            LoadUtility.AllLoaded = true;
        //            if (Password.isIDRegistered == true)
        //                load = true;
        //            else
        //                load = false;
        //            StartGame();
        //        }
        //    }
        //}
    }


    private IEnumerator GetSampleNumber()
    {
        //string url = SaveManager.getSentenceUrl;
        //WWWForm form = new WWWForm();
        //form.AddField("Count", "true");
        //using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        //{
        //    yield return www.SendWebRequest();
        //    if (!String.IsNullOrEmpty(www.downloadHandler.text))
        //    {
        //        sampleNumber = int.Parse(www.downloadHandler.text);
        //        DialogueInstancer.uniqueLineIndex = sampleNumber / 2;
        //        Graffiti.uniqueGraffitiIndex = 0;
        //        //samplesCounted = true;



        //        //
        //    }
        //}

        string url;
        //if (Player.version == Version.Gram)
        //{
            //quanti particpanti?
            int participantN = 0;
            url = API.urls.getParticipants;
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();
                if (!String.IsNullOrEmpty(www.downloadHandler.text))
                {
                    participantN = int.Parse(www.downloadHandler.text);
                }
            }


            int participantsNormalized;
            if (participantN > 10)
            {
                string pn = participantN.ToString();
                participantsNormalized = int.Parse(pn.Substring((pn.Length - 1) - 2, 2));
            }
            else
            {
                participantsNormalized = participantN;
            }

            int offset = 0;
    }

    public static void OffsetPair(int npart, int offset)
    {
        if (npart % 2 == 0)
            DialogueInstancer.uniqueLineIndex = (npart / 2) * offset;
        else
            DialogueInstancer.uniqueLineIndex = ((npart -1)/2) * offset;

        Graffiti.uniqueGraffitiIndex = 0;
    }
    public static void UpdatePercentage(float progress, float total)
    {
        LoadingManager.Percentage = progress / total;
        LoadingManager.Percentage *= 100f;
        LoadingManager.PercentageText.text = Mathf.Floor(LoadingManager.Percentage).ToString() + "%";
    }
}
