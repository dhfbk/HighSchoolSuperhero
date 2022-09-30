using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Xml;
using TMPro;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;
using UnityEngine.Events;
using DataUtilities;
using System.Net;
using System.Text;
using UnityEngine.Rendering;
using System.Diagnostics;


public class DialogueInstancer : MonoBehaviour, ITriggerable
{
    public static Variant variant = Variant.dialogues;
    public delegate void MessageDelegate();
    public bool saveAfterAnnotation;
    public Player Agent { get; set; }
    public bool spawned;
    public bool talking;
    public bool update;
    public int i;
    public InputField iF;
    public GameObject token;
    public Mesh prova;
    private Stopwatch sw;
    private float cloudBlock;
    public GameObject generator;
    public GameObject npccombine;
    //Corpus loading variables
    public string text;
    public string messaggio;
    public int bullyNum, victimNum;
    public List<GameObject> bullySpawns, victimSpawns;
    public DialogueManagerScriptableObject dialogo;
    public List<string> tokens;
    public List<string> roles;
    public List<string> bulli, vittime;
    public Dictionary<string, string> namedb;

    private GameObject enterButton;
    private GameObject mouseButton;
    private GameObject eButton;
    public LayerMask dialogueTouchMask;
    public LayerMask dialogueEscapeLayers;// = 1 << 28;
    public LayerMask dialogueEscapeLayersUI;
    public int t;
    public int iLine;
    public static int uniqueLineIndex;
    public GameObject author, msgbox, thinkcloud, namecloud;
    bool toReset;
    public Mesh mesh;
    public GameObject currentItem;
    public bool annotated;
    bool endConversation;
    public bool accorto;
    public int charge;
    public int groupChanges;
    private List<GameObject> children;
    MessageDelegate nextTurn;
    public bool near;
    bool noTags;
    string currentAuthor;
    bool lazyTrigger;
    public GameObject lazyTriggerArea;
    float agreement;

    public bool Restriction;
    string tasktype;
    List<string> BadTags;
    List<string> GoodTags;

    public static int maxLineIndex;
    public static int firstIndex;
    public static bool deactivateDialoguesAndGraffiti;

    void Start()
    {
        sw = new Stopwatch();
        dialogo = Resources.Load<DialogueManagerScriptableObject>("DialogueManager");
        GetComponent<Renderer>().enabled = false;
        bullyNum = 0;
        victimNum = 0;
        bullySpawns = new List<GameObject>();
        victimSpawns = new List<GameObject>();
        roles = new List<string>();
        children = new List<GameObject>();

        LoadUtility.Nomi = new List<string>() { "Mark", "Lukas", "Michael", "Louis", "Brad", "Akim", "Marco" };
        LoadUtility.Nomif = new List<string>() { "Adele", "Susan", "Sofia", "Yasmin", "Ingrid", "Jennifer", "Giorgia" };


        BadTags = new List<string>();
        GoodTags = new List<string>();
        BadTags.Add("Bullo");
        BadTags.Add("Bullo1");
        BadTags.Add("Bullo2");
        BadTags.Add("SupportoBullo1");
        BadTags.Add("SupportoBullo2");
        BadTags.Add("SupportoBullo3");
        GoodTags.Add("Vittima");
        GoodTags.Add("Vittima1");
        GoodTags.Add("SupportoVittima1");
        GoodTags.Add("SupportoVittima2");
        GoodTags.Add("SupportoVittima3");
    }
    public void CreateDialogues()
    {
        WebDebug.Print($"{transform.name} Initializing...");
        //dialogo.turns.Clear();
        dialogo.roles.Clear();
        dialogo.lines.Clear();
        dialogo.annotations.Clear();
        tokens = new List<string>();
        WebDebug.Print($"{transform.name} Creating dialogues...");

        ExtractTurns();
        WebDebug.Print($"{transform.name} Turns extracted!");

        SubstituteNames();
        WebDebug.Print($"{transform.name} Names substituted!");
    }

    public void SearchTurns(bool defined)
    {
        for (int i = 0; i < LoadUtility.annSO.lines.Count; i++)
        {
            string line = LoadUtility.annSO.lines[i];
            string role = LoadUtility.annSO.roles[i];
            string annotation = LoadUtility.annSO.annotations[i];
            line = Regex.Replace(line, @"\p{C}+", string.Empty); //remove annoying non-printable characters
            if (!string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(line))
            {
                if (defined)
                {
                    if (BadTags.Contains(role) || GoodTags.Contains(role))
                    {
                        dialogo.roles.Add(role);
                        dialogo.lines.Add(line);
                        if (!String.IsNullOrEmpty(annotation))
                            dialogo.annotations.Add(annotation);
                        else
                            dialogo.annotations.Add("");
                    }
                }
                else
                {
                    dialogo.roles.Add(role);
                    dialogo.lines.Add(line);
                    if (!String.IsNullOrEmpty(annotation))
                        dialogo.annotations.Add(annotation);
                    else
                        dialogo.annotations.Add("");
                }
            }
        }
    }

    public void ExtractTurns()
    {
        SearchTurns(true);

        //Se non sono stati trovati ruoli...
        if (dialogo.roles.Count == 0)
        {
            SearchTurns(false);
            noTags = true;
        }
        
        foreach (string role in dialogo.roles)
        {
            if (!roles.Contains(role))
                roles.Add(role);
        }
    }

    public void SubstituteNames()
    {
        bulli = new List<string>();
        vittime = new List<string>();
        namedb = new Dictionary<string, string>();

        for (int i = 0; i < roles.Count; i++) //per ogni ruolo estratto
        {
            namedb.Add(roles[i], LoadUtility.Nomi[i]); //assegna uno dei nomi. Tabella di corrispondenze
            if (BadTags.Contains(roles[i]))
                bulli.Add(LoadUtility.Nomi[i]);
            else
                vittime.Add(LoadUtility.Nomi[i]);
        }

        for (int i = 0; i < dialogo.roles.Count; i++) //per ogni turno
            dialogo.roles[i] = namedb[dialogo.roles[i]]; //sostituisci ruolo con nome

        for (int i = 0; i < roles.Count; i++) //per ogni ruolo estratto controlla ogni linea di messaggio e sostituisce le chiamate dei nomi
        {
            for (int i2 = 0; i2 < dialogo.lines.Count; i2++)
            {
                //dialogoAnn.lines[i2] = dialogo.lines[i2];
                if (dialogo.lines[i2].Contains(roles[i]))
                {
                    if (!String.IsNullOrEmpty(dialogo.lines[i2]))
                        dialogo.lines[i2] = dialogo.lines[i2].Replace(roles[i], namedb[roles[i]]);
                }
            }
        }
    }

    public void SpawnCharacters(List<string> roles)
    {

        //Restriction = UnityEngine.Random.Range(0, 2) == 1 ? false : true;
        Restriction = Player.rCondition == RCondition.NonRestricted ? false : true;

        tasktype = Restriction == true ? "DR" : "D";
        //if (!Restriction)
        //    generator.SetActive(true);
        //WebDebug.Print($"{transform.name} Spawning...");
        //Get spawn points
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            if (transform.GetChild(i).tag == "BullySpawn")
            {
                bullySpawns.Add(transform.GetChild(i).gameObject);
                transform.GetChild(i).GetComponent<Renderer>().enabled = false;
            }
            else if (transform.GetChild(i).tag == "VictimSpawn")
            {
                victimSpawns.Add(transform.GetChild(i).gameObject);
                transform.GetChild(i).GetComponent<Renderer>().enabled = false;
            }
        }


        //Spawn the participants
        int randomvar2; //random position
        GameObject participant;
        foreach (string role in roles)
        {
            if (BadTags.Contains(role))
            {
                if (bullySpawns.Count > 0)
                {
                    randomvar2 = UnityEngine.Random.Range(0, bullySpawns.Count);
                    GameObject bully = Instantiate(npccombine);
                    NPCUtilities.RandomLook(bully);
                    bully.GetComponent<CombineChildren>().Combine();
                    //bully.SendMessage("Combine");
                    bully.tag = "Bully";
                    bully.transform.name = role;
                    bully.transform.position = bullySpawns[randomvar2].transform.position;
                    bully.transform.parent = transform;
                    bully.GetComponent<GoBackToPlace>().place = bullySpawns[randomvar2];
                    Destroy(bully.GetComponent<GoBackToPlace>().triggerArea);
                    bullySpawns.Remove(bullySpawns[randomvar2]);
                    bully.transform.LookAt(transform.position); //look at center
                    participant = bully;
                    //NPCUtilities.RandomLook(participant);
                    transform.GetComponent<DialogueInstancer>().children.Add(participant);
                }
            }
            else if (noTags == true || (noTags == false && GoodTags.Contains(role)))
            {
                if (victimSpawns.Count > 0)
                {
                    randomvar2 = UnityEngine.Random.Range(0, victimSpawns.Count);
                    GameObject victim = Instantiate(npccombine);
                    NPCUtilities.RandomLook(victim);
                    victim.GetComponent<CombineChildren>().Combine();
                    //victim.SendMessage("Combine");
                    victim.tag = "Victim";
                    victim.transform.name = role;
                    victim.transform.position = victimSpawns[randomvar2].transform.position;
                    victim.transform.parent = transform;
                    victim.GetComponent<GoBackToPlace>().place = victimSpawns[randomvar2];
                    Destroy(victim.GetComponent<GoBackToPlace>().triggerArea);
                    victimSpawns.Remove(victimSpawns[randomvar2]);
                    victim.transform.LookAt(transform.position);
                    participant = victim;
                    //NPCUtilities.RandomLook(participant);
                    transform.GetComponent<DialogueInstancer>().children.Add(participant);
                }
            }
        }

    }

    public void AssignNamesToChildren(List<string> bullyNames, List<string> victimNames)
    {

        //Assign names
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Bully"))
            {
                int randomvar2 = UnityEngine.Random.Range(0, bullyNames.Count);
                child.name = bullyNames[randomvar2];
                bullyNames.Remove(bullyNames[randomvar2]); //exclude from further renaming the name that was just given
            }
            else if (child.CompareTag("Victim"))
            {
                int randomvar2 = UnityEngine.Random.Range(0, victimNames.Count);
                child.name = victimNames[randomvar2];
                victimNames.Remove(victimNames[randomvar2]);
            }
        }
    }

    public void StartDialogue()
    {
        iLine = uniqueLineIndex;
        //messaggio = dialogo.lines[iLine];
        //author = GameObject.Find(dialogo.roles[iLine]);
    }

    public void Activate()
    {
        //CreateDialogues();
        noTags = true;
        SpawnCharacters(LoadUtility.Nomi);
        //Combine();
        //AssignNamesToChildren(LoadUtility.Nomi, LoadUtility.Nomi);
        StartDialogue();
        //print(dialogo.lines[0]);
        spawned = true;
    }

    void Update()
    {
        if (LoadUtility.AllLoaded && API.readyToLoadGame && !spawned && lazyTrigger)
        {           
            Activate();
        }
        if (spawned)
        {
            if (Agent && deactivateDialoguesAndGraffiti == false)
            {
                cloudBlock -= Time.deltaTime;
                if (Input.GetKeyUp("e"))
                {
                    if (!talking)
                    {
                        StartCoroutine(EnterDialogue());
                    }
                    else if (nextTurn == ShowReaction || nextTurn == ShowCloud)
                    {
                        if (MessageUtility.BoxFinished)
                        {
                            StartCoroutine(ExecuteTurn(nextTurn));
                            //Erase iF...
                        }
                    }
                    else//if (cloudBlock <= 0)
                    {
                        if (!MouseOverToken.ifParent.activeSelf)
                        {
                            //StartCoroutine(Annotate());
                            if (!MouseOverToken.Changed)
                                ExitDialogue();
                        }
                    }
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Ray rayUI = Agent.cameraInterface.uicam.ScreenPointToRay(Input.mousePosition);

                    RaycastHit hit;
                    RaycastHit hitUI;
                    if (!talking)
                    {
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, dialogueTouchMask) ||
                            Agent.cameraInterface.participateButton.GetComponent<ParticipateButton>().IsOver())
                        {
                            StartCoroutine(EnterDialogue());
                        }
                    }
                    else
                    {
                        Physics.Raycast(ray, out hit, Mathf.Infinity, dialogueEscapeLayers);
                        Physics.Raycast(rayUI, out hitUI, Mathf.Infinity, dialogueEscapeLayersUI);

                        if (hit.transform == null &&
                            hitUI.transform == null)
                        {
                            if (nextTurn == ShowBox)
                                ExitDialogue();
                            else
                                StartCoroutine(ExecuteTurn(nextTurn));
                        }
                        else

                        {

                            if (Agent.cameraInterface.okCircle.GetComponent<DialogueOkButton>().IsOver() && !GameObject.Find("IfBG"))
                            {
                                StartCoroutine(ExecuteTurn(nextTurn));
                            }
                        }
                    }
                }
                else if ((Input.GetKeyDown(KeyCode.Return)) && !GameObject.Find("IfBG"))
                {
                    if (talking && cloudBlock <= 0)
                    {
                        if (!MessageUtility.Speaking)
                        {
                            StartCoroutine(ExecuteTurn(nextTurn));
                            //Erase iF...
                        }
                        else
                        {
                            if (MessageUtility.BoxFinished)
                            {
                                StartCoroutine(ExecuteTurn(nextTurn));
                            }
                        }
                    }
                }
                else if (Input.GetKeyUp(KeyCode.Escape))
                {
                    if (talking)
                    {
                        if (GameObject.Find("IfBG"))
                            DeactivateIf();
                        ExitDialogue();
                    }
                    
                }
            }
        }
    }

    void DeactivateIf()
    {
        GameObject ifo = GameObject.Find("IfBG");
        if (ifo != null)
        {
            ifo.GetComponentInChildren<TMP_InputField>().text = "";
            ifo.SetActive(false);
        }
        mouseButton.SetActive(true);
        enterButton.SetActive(false);
    }

    private IEnumerator ExecuteTurn(MessageDelegate nextTurn)
    {
        if (nextTurn == ShowCloud)
        {
            if (!API.dialoguesFinished)
                yield return StartCoroutine(API.GetSentenceSingleC(Agent, uniqueLineIndex, "ch"));
            else
                yield return StartCoroutine(API.GetSentence(Agent, 1, Variant.dialogues));
            print(Agent.dialogueSentences.Count);

        }
        else
            yield return null;
        nextTurn();
    }

    private void ShowCloud()
    {
        if (noTags)
        {
            iLine = uniqueLineIndex;
            //iLine = UnityEngine.Random.Range(0, dialogo.lines.Count);
            currentAuthor = this.children[UnityEngine.Random.Range(0, this.children.Count)].name;
        }
        else
        {
            //lineUniqueIndex += 1;
            iLine = uniqueLineIndex;
            currentAuthor = dialogo.roles[iLine];
        }

        if (API.currentApi == Api.dev)
            MessageUtility.ThinkCloud(Agent, currentAuthor, this.gameObject);
        else
            MessageUtility.ThinkCloud(Agent, currentAuthor, this.gameObject);
        cloudBlock = 1;
        nextTurn = ShowBox;
        Agent.playerLogger.StartSentenceAnnotationSW();

        if (MultiplatformUtility.Mobile)
        {
            mouseButton = Agent.cameraInterface.touchIcon;
            mouseButton.transform.GetChild(0).GetComponent<TextMeshPro>().text = ML.systemMessages.touchIcon;
            eButton = null;
            enterButton = Agent.cameraInterface.okCircle;
        }
        else
        {
            mouseButton = Agent.cameraInterface.mouseButton;
            eButton = Agent.cameraInterface.eButton;
            enterButton = Agent.cameraInterface.enterButton;
        }
        mouseButton.SetActive(true);
        if (eButton != null)
            eButton.SetActive(true);
        enterButton.SetActive(true);
    }
    private void ShowBox()
    {
        if (eButton != null)
            eButton.SetActive(false);
        enterButton.SetActive(false);
        mouseButton.SetActive(false);
        float agreement = 0;
        StartCoroutine(Annotate(Agent, tasktype, this));
        //AnnotationData anndata = Annotate(out agreement);
        List<string> messageTokens = Annotation.GetNewTokens(tokens: MessageUtility.FindTokens());
        //if (API.currentApi == Api.dev)
        //    MessageUtility.SingleBoxedMessageStay(Agent, currentAuthor, string.Join("", messageTokens));
        //else
        string joiner = Player.admin == true ? "" : " ";
        MessageUtility.SingleBoxedMessageStay(Agent, currentAuthor, string.Join(joiner, messageTokens));
        if (MouseOverToken.Changed)
        {            
            nextTurn = ShowReaction;
        }
        else
            nextTurn = ShowCloud;
    }
    private void ShowReaction()
    {
        MouseOverToken.Changed = false;
        GameObject bully = this.children.Find(x => x.name == currentAuthor);
        StartCoroutine(Jump(bully));
        Reactions.Create(emitter: bully, type: "Exclamation");
        MessageUtility.SingleBoxedMessageStay(Agent, currentAuthor, ML.systemMessages.didntMeanToSayThis);
        nextTurn = ShowCloud;
    }
    private IEnumerator EnterDialogue()
    {
        Agent.cameraInterface.participateButton.GetComponent<ParticipateButton>().Hide();
        Agent.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Agent.playerLogger.StartDialogueAnnotationSW();
        if (!Restriction)
        {
            Agent.currentlyRestricted = false;
            if (Player.design == Design.Within)
                Agent.cameraInterface.battery.GetComponent<Rainbow>().StartRainbow();
        }
        else
            Agent.currentlyRestricted = true;
        Player.overlays += 1;
        NotificationUtility.ShowString(Agent, string.Format(ML.systemMessages.closeConversation, MultiplatformUtility.PrimaryInteractionKey));

        if (!API.dialoguesFinished)
            yield return StartCoroutine(API.GetSentenceSingleC(Agent, uniqueLineIndex, "ch"));
        else
            yield return StartCoroutine(API.GetSentence(Agent, 1, Variant.dialogues));

        print("kal " + API.dialoguesFinished + " " + 1);

        ShowCloud();
        nextTurn = ShowBox;
        charge = Agent.GetEnergy();
        talking = true;
        Agent.GetComponent<Movement>().Busy = true;
        toReset = true;
        PhoneUtility.Hide(Agent);
        MessageUtility.OverrideBusy = true;
    }

    private void ExitDialogue()
    {
        DeactivateIf();
        enterButton.SetActive(false);
        if (eButton != null)
            eButton.SetActive(false);
        mouseButton.SetActive(false);
        Agent.playerLogger.StopDialogueAnnotationSW();
        if (!Restriction)
        {
            if (Player.design == Design.Within)
                Agent.cameraInterface.battery.GetComponent<Rainbow>().StopRainbow();
        }
        Player.overlays -= 1;
        MessageUtility.OverrideBusy = false;
        MessageUtility.ResetMessages(Agent, this.gameObject);
        Agent.GetComponent<Movement>().Busy = false;
        talking = false;
        toReset = false;
        PhoneUtility.Down(Agent);

        Agent.cameraInterface.participateButton.GetComponent<ParticipateButton>().Show();

        //if (API.currentApi == Api.final && Player.admin == false)
        //    uniqueLineIndex -= 1;
    }
    public IEnumerator Jump(GameObject bully)
    {
        int i = 0;
        while (i < 4)
        {
            bully.GetComponent<Rigidbody>().AddForce(new Vector3(0, 300, 0), ForceMode.Impulse);
            i++;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public static IEnumerator Annotate(Player Agent, string taskType, MonoBehaviour mb)
    {
        Agent.playerLogger.playerLog.NumberOfAnnotatedSentences++;
        float agreement = 0;
        List<GameObject> tokens = MessageUtility.FindTokens();
        float timePerToken = PlayerLogger.CalculateTimePerToken(tokens.Count, Agent.playerLogger.StopSentenceAnnotationSW());
        //AnnotationData anndata = Annotation.AnnotateModifiedText(DialogueInstancer.uniqueLineIndex, tokens, timePerToken, taskType);
        AnnotationData anndata = Annotation.AnnotateModifiedText(API.currentSentence.id, tokens, timePerToken, taskType);

        string goldann = "";

        if (API.currentApi == Api.dev)
        {
            WWWForm form = new WWWForm();
            form.AddField("ID", anndata.id);
            using (UnityWebRequest www = UnityWebRequest.Post(API.urls.getGoldDialogues, form))
            {
                yield return www.SendWebRequest();

                if (www.error == null && !www.downloadHandler.text.Contains("errore"))
                    goldann = www.downloadHandler.text;
            }

            int annotated = anndata.annotations.Contains(1) ? 0 : 1;


            if (!String.IsNullOrEmpty(goldann))
            {
                //AnnotationData goldSentence = new AnnotationData(iLine, sqlSentence, goldann);
                //agreement = Annotation.GoldCompare(anndata, goldSentence);
                anndata.gold = goldann;
                if (int.Parse(goldann) == annotated)
                    agreement = 1;
                else
                    agreement = 0;
            }
            else
            {
                anndata.gold = "1";
            }
        }

        int points = 5 + (int)(10 * agreement);

        if (timePerToken > 0.15f)
        {
            points *= Player.pointMultiplier;
            PointSystem.AddPoints(mb, Agent, "Point", points);
            PointSystem.AddPoints(mb, Agent, "Heart", points);
            //mb.GetComponent<SpawnCrystals>().Spawn(Agent);
        }
        SafetyBar.AddSafety((20 + 20 * agreement) * Player.pointMultiplier);
        Agent.TotalAnnotatedDialogues += 1;

        if (mb.GetComponent<DialogueInstancer>())
            mb.GetComponent<DialogueInstancer>().ColorRandomStudent();
        else
            mb.GetComponent<NPCInteraction>().ColorStudent();

        //Save
        if (!API.dialoguesFinished)
        {
            API.PostAnnotation(Agent, anndata);
        }

        DialogueInstancer.IncrementIndex();

        SaveManager.SaveGameState(Agent);
        API.PostSave(Agent, false);
    }

    public void ColorRandomStudent()
    {
        List<CombineChildren> bw = new List<CombineChildren>();
        List<CombineChildren> col = new List<CombineChildren>();
        foreach (Transform t in transform)
        {
            if (t.CompareTag("Bully") || t.CompareTag("Victim"))
            {
                if (!t.GetComponent<CombineChildren>().colorized)
                {
                    //t.GetComponent<CombineChildren>().Colorize();
                    bw.Add(t.GetComponent<CombineChildren>());
                }
                else
                    col.Add(t.GetComponent<CombineChildren>());
            }
        }
        if (bw.Count > 0)
        {
            bw[UnityEngine.Random.Range(0, bw.Count)].Colorize();
            if (bw.Count <= 1)
                GetComponent<AudioSource>().Play();
        }

    }

    public static void IncrementIndex()
    {
        if (uniqueLineIndex + 1 >= DialogueInstancer.maxLineIndex)
        {
            uniqueLineIndex = DialogueInstancer.firstIndex;
        }
        else
        {
            uniqueLineIndex += 1;
        }
    }

    public void TriggerOn(Player agent)
    {
        NotificationUtility.ShowString(agent, string.Format(ML.systemMessages.enterConversation, MultiplatformUtility.PrimaryInteractionKey));
        //if (MultiplatformUtility.Mobile)
            //agent.cameraInterface.participateButton.GetComponent<ParticipateButton>().Show();
        this.Agent = agent;

        if (!agent.dialogueTutorial)
        {
            agent.ShowDialogueTutorial();
            agent.dialogueTutorial = true;
        }
        else
        {
            agent.cameraInterface.participateButton.GetComponent<ParticipateButton>().Show();
        }
    }

    public void TriggerOff()
    {
        Agent.cameraInterface.participateButton.GetComponent<ParticipateButton>().Hide();
        NotificationUtility.Hide(this.Agent);
        this.Agent = null;
        toReset = false;
    }

    public void LazyActivate()
    {
        lazyTrigger = true;
    }

}
