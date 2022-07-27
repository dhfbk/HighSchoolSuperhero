using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using DataUtilities;
using System.Linq;
using System.Diagnostics;
using UnityEngine.Networking;

public class GraffitiState
{
    public string name;
    public bool state;
}
public class GraffitiException : Exception
{
    public GraffitiException() { }

    public GraffitiException(string message) : base(message) { }
}

public class Graffiti : MonoBehaviour, ITriggerable
{
    private bool bulkMode = false;
    public LayerMask eraseMask;
    public static Variant variant = Variant.graffiti;
    //Interface
    [SerializeField]
    public bool notTriggeredByArea;
    public int area;
    public Player Agent { get; set; }
    public string State { get; set; }
    private bool calc;
    public bool Calc { get => calc; set => calc = value; }
    public bool Hide { get; set; }
    public GameObject tokenContainer;
    public GameObject eraseContainer;

    List<int> graffitiEscapeLayers = new List<int>() { 13, 15, 21 };
    bool clickWasInitiatedInside;
    bool canErase;
    public LayerMask graffitiTouchMask;
    bool pointsAlreadyGiven;
    public bool erasing;
    public int t;
    public bool reset;
    public string stringa;
    public List<GameObject> tokens;
    public float length;
    public int score;
    public GameObject graffitiArea;
    GameObject firstOfLine;
    GameObject wiper;
    public GameObject camPos;
    List<GameObject> memObst;
    public LayerMask rayMask;
    float planeZ = 0;
    float tokenSize = 0.5f;
    AnnotationData currentAnnSent;
    public Material[] mats;
    public TMP_FontAsset[] fonts;
    int currentStringLength;
    private GameObject enterButton;
    private bool annotated;
    private bool unloaded;
    public static int annotatedGraffitiIndex = 0;
    public static List<int> shownIndeces;
    public static List<int> annotatedIndeces;

    //SQL
    public bool saveAfterAnnotation;
    public static int uniqueGraffitiIndex;
    public static int lastGraffitiID;
    bool sentenceDownloaded;
    bool downloading;
    static bool queueBusy;
    public static GameState gameState;

    //Log
    private Stopwatch sw;

    //Quest
    [Header("Quest")]
    public GameObject exclamationIcon;
    private bool interacted;
    public bool Interacted { get => interacted; }
    private bool questSatisfied;
    public bool QuestSatisfied { get => questSatisfied; }

    public bool graffitiLoaded;
    Vector3 tokenContainerPos;

    public DialogueManagerScriptableObject dialogo;
    // Use this for initialization

    //NEW
    public static List<AnnotationData> DropSentencesLongerThan(List<AnnotationData> lista, int length)
    {
        if (lista.Count == 0)
            throw new GraffitiException("List of sentences is empty");
        if (length <= 0)
            throw new GraffitiException("Length must be greater than zero");

        List<AnnotationData> shortSentences = new List<AnnotationData>();
        foreach (AnnotationData sent in lista)
        {
            if (string.Join(" ", sent.tokens).Length <= length)
            {
                shortSentences.Add(sent);
            }
        }
        return shortSentences;
    }


    void Start()
    {
        tokenContainerPos = tokenContainer.transform.position;
        sw = new Stopwatch();
        shownIndeces = new List<int>();
        annotatedIndeces = new List<int>();
    }



    private void Reload()
    {
        tokenContainer.transform.position = tokenContainerPos;
        graffitiLoaded = false;
        downloading = false;
        sentenceDownloaded = false;
        ResetErase();
    }

    private void ResetErase()
    {
        foreach (Transform erase in eraseContainer.transform)
        {
            Destroy(erase.gameObject);
        }
    }

    bool ConditionsToLoad()
    {
        if (LoadUtility.AllLoaded && 
            API.readyToLoadGame && 
            SaveManager.gameLoaded &&
            //((Player.condition == Condition.W3D && area <= Player.area) ||
                //Player.condition == Condition.NoW3D) &&
            Agent &&
            !queueBusy && 
            !graffitiLoaded)
            return true;
        else
            return false;
    }
    bool ConditionsToInteract()
    {
        if (Agent && 
            DialogueInstancer.deactivateDialoguesAndGraffiti == false && 
            Vector3.Distance(Agent.transform.position, transform.position) < 5)
            return true;
        else
            return false;
    }

    bool ConditionsToReset()
    {
        if (graffitiLoaded &&
            !Agent &&
            !annotated &&
            !unloaded)
        {
            return true;
        }
        else
            return false;
    }
    void Update()
    {
        //Throw tutorial
        if (Agent && Vector3.Distance(transform.position, Agent.transform.position) < 3.0f)
        {
            if (Agent.graffitiTutorial == false)
            {
                Agent.ShowGraffitiTutorial();
                Agent.graffitiTutorial = true;
            }
        }
        if (ConditionsToLoad())
        {
            if (sentenceDownloaded)
            {
                tokens = InstantiateAllTokens(currentAnnSent);
                try
                {
                    LayoutTokens(tokens, graffitiArea);
                }
                catch (GraffitiException e)
                {
                    print(e.Message);
                }
                finally
                {
                    graffitiLoaded = true;
                }
            }
            else
            {
                if (!downloading)
                {
                    unloaded = false;
                    StartDownload();
                }
            }
        }
        else
        {
            if (ConditionsToReset())
            {
                Unload();               
            }
        }

        if (ConditionsToInteract())
        {
            //Initiate
            if (!erasing)
            {
                if (Input.GetKeyUp("e"))
                {
                    StartGraffiti(Agent);
                    canErase = true;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, graffitiTouchMask))
                    {
                        StartGraffiti(Agent);
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, graffitiTouchMask))
                    {
                        clickWasInitiatedInside = true;
                    }
                    else
                        clickWasInitiatedInside = false;
                }
                if (Input.GetMouseButtonUp(0))
                {

                    canErase = true;
                }
                //Move Camera
                MoveCameraToGraffiti();
                interacted = true;

                //ERASE
                if (Input.GetMouseButton(0) && canErase)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, eraseMask))
                    {
                        if (!hit.transform.name.Contains("Erase") && hit.transform.CompareTag("GraffitiToken"))
                        {
                            if (!reset)
                            {
                                Erase(hit, tokens[0].GetComponent<TMP_Text>().fontSize / 3);
                                reset = true;
                            }
                            else
                            {
                                if (t > 3)
                                {
                                    Erase(hit, tokens[0].GetComponent<TMP_Text>().fontSize / 3);
                                    t = 0;
                                }
                                else
                                {
                                    t += 1;
                                }
                            }
                        }
                        else if (graffitiEscapeLayers.All(x => x != hit.transform.gameObject.layer))
                        {
                            if (!clickWasInitiatedInside)
                            {
                                StartCoroutine(Annotate());
                                StopAnnotation(Agent);
                            }
                        }
                    }
                }
                else
                {
                    if (reset)
                        reset = false;
                }

                if (Input.GetKeyUp("return") || Input.GetKeyUp("e"))
                {
                    if (Hide)
                    {
                        if (memObst.Count > 0)
                            ShowHiddenObstacles(memObst);
                    }

                    if (Input.GetKeyUp("return"))
                    {
                        StartCoroutine(Annotate());
                    }
                    StopAnnotation(Agent);
                }

                if (Input.GetKeyUp(KeyCode.Escape) && Player.condition == Condition.W3D)
                {
                    StopAnnotation(Agent);
                }

                if (Input.GetKeyUp("c"))
                {
                    Unerase(eraseContainer.transform);
                }
            }
        }
    }
    private void StartDownload()
    {
        if (tokenContainer.transform.childCount > 0)
            foreach (Transform t in tokenContainer.transform)
                Destroy(t.gameObject);
        StartCoroutine(LoadGraffitiTokens());
        queueBusy = true;
        downloading = true;
    }

    private void Unload()
    {
        foreach (Transform token in tokenContainer.transform)
            Destroy(token.gameObject);

        Unerase(eraseContainer.transform);

        graffitiLoaded = false;
        sentenceDownloaded = false;
        downloading = false;

        //annotated = false;

        unloaded = true;
    }
    public IEnumerator LoadGraffitiTokens() //This creates a series of AnnotationData objects with tokens and annotations from LoadUtility.annSO
    {
        if (API.currentApi == Api.dev)
        {
            int selfIndex = uniqueGraffitiIndex; //API dev
            if (gameState != null)
            {
                while (gameState.annotatedGraffitiIndeces.Contains(selfIndex))
                {
                    selfIndex += 1;
                    yield return null;
                }
            }

            uniqueGraffitiIndex = selfIndex;

            API.sentence = null;
            yield return StartCoroutine(API.GetSentence(Agent, uniqueGraffitiIndex, variant));
            sentenceDownloaded = true;
            queueBusy = false;

            if (API.sentence != null)
            {
                currentAnnSent = new AnnotationData();
                currentAnnSent.id = uniqueGraffitiIndex;
                currentAnnSent.tokens = WordByWord.RegexTokenizer(API.sentence);

                uniqueGraffitiIndex += 1;
                currentStringLength = API.sentence.Length;
            }
        }
        else
        {
            currentAnnSent = new AnnotationData();
            if (bulkMode == true)
            {

                yield return StartCoroutine(API.GetSentence(FindObjectOfType<Player>(), uniqueGraffitiIndex, Variant.graffiti));
                //while (API.currentSentence.annotated == true)
                //{
                //    print("ignored: " + API.currentSentence.id + API.currentSentence.tokens[0]);
                //    yield return StartCoroutine(API.GetSentence(FindObjectOfType<Player>(), uniqueGraffitiIndex, Variant.graffiti));
                //    uniqueGraffitiIndex += 1;
                //}
                sentenceDownloaded = true;
                
                //int selfIndex = uniqueGraffitiIndex;
                ////Check if already annotated
                //if (gameState != null)
                //{
                //    while (gameState.annotatedGraffitiIndeces.Contains(selfIndex))
                //    {
                //        selfIndex += 1;
                //        yield return null;
                //    }
                //}
                //uniqueGraffitiIndex = selfIndex;
                currentAnnSent.id = FindObjectOfType<Player>().graffitiSentences[uniqueGraffitiIndex].id;
                
                
            }
            else
            {
                yield return StartCoroutine(API.GetSentenceSingleC(FindObjectOfType<Player>(), lastGraffitiID, "gr"));

                if (shownIndeces.Count > 0)
                {
                    if (shownIndeces.Contains(API.currentSentence.id))
                    {
                        lastGraffitiID = shownIndeces.Max();
                        yield return StartCoroutine(API.GetSentenceSingleC(FindObjectOfType<Player>(), lastGraffitiID, "gr"));
                    }
                }

                sentenceDownloaded = true;

                currentAnnSent.id = API.currentSentence.id;
#if UNITY_EDITOR
                print("New sentence found that is not shown: " + currentAnnSent.id + " " + string.Join(" ", currentAnnSent.tokens));
#endif

            }
            currentAnnSent.tokens = API.currentSentence.tokens.ToList();//WordByWord.RegexTokenizer(API.sentence);
            currentStringLength = API.currentSentence.tokens.Length;
            queueBusy = false;
        }
    }
    void SetRainbow()
    {
        if (Player.rCondition == RCondition.NonRestricted)
        {
            Agent.currentlyRestricted = false;
            if (Player.design == Design.Between)
                Agent.HideSoap();
            else
                Agent.cameraInterface.soapBar.GetComponent<Rainbow>().StartRainbow();

            if (Player.condition == Condition.NoW3D)
                Agent.cameraInterface.soapBar.GetComponent<SoapBar>().NormalSize();
        }
        else
        {
            Agent.currentlyRestricted = true;
            if (Player.design == Design.Within)
                Agent.cameraInterface.soapBar.GetComponent<Rainbow>().StopRainbow();

            if (Player.condition == Condition.NoW3D)
                Agent.cameraInterface.soapBar.GetComponent<SoapBar>().SetNewMax(currentStringLength / 2f);
        }
    }
    public List<GameObject> InstantiateAllTokens(AnnotationData currentAnnSent)
    {
        //Instantiate all tokens
        List<GameObject> tokens = new List<GameObject>();
        int randMat = UnityEngine.Random.Range(0, mats.Length);
        for (int i = 0; i < currentAnnSent.tokens.Count; i++)
        {
            GameObject token = Instantiate(Resources.Load<GameObject>(this.gameObject.tag + "Token"));
            token.name = transform.name + "Token" + i;
            token.transform.SetParent(tokenContainer.transform);
            //aggiungere progressione delle coordinate
            if (i == 0)
            {
                token.transform.localPosition = new Vector3(0.69F, -1.68F, 0.09F);
            }
            else
            {
                //token.transform.localPosition = new Vector3(tokens[i - 1].transform.localPosition.x + tokens[i - 1].GetComponent<RectTransform>().sizeDelta.x + 2, -1.68F, 10F);
            }
            token.transform.rotation = tokenContainer.transform.rotation;
            token.transform.localScale = new Vector3(tokenSize, tokenSize, tokenSize);
            token.GetComponent<TMP_Text>().text = currentAnnSent.tokens[i];
            token.GetComponent<ContentSizeFitter>().enabled = true;
            Canvas.ForceUpdateCanvases(); //da usare SEMPRE insieme ai cambiamenti dei rect transform
            token.GetComponent<BoxCollider>().center = new Vector3(token.GetComponent<RectTransform>().sizeDelta.x / 2, 0, 0);
            token.GetComponent<BoxCollider>().size = new Vector3(token.GetComponent<RectTransform>().sizeDelta.x, token.GetComponent<RectTransform>().sizeDelta.y, 1);

            token.GetComponent<Renderer>().material = mats[randMat];
            token.GetComponent<TextMeshPro>().fontSharedMaterial = mats[randMat];
            token.GetComponent<TextMeshPro>().font = fonts[randMat];
            token.GetComponent<TextMeshPro>().UpdateFontAsset();

            if (Player.condition == Condition.NoW3D)
                StartGraffiti(Agent);
            tokens.Add(token);
        }
        return tokens;
    }
    public void LayoutTokens(List<GameObject> tokens, GameObject graffitiArea)
    {
        lastGraffitiID = currentAnnSent.id;
        shownIndeces.Add(lastGraffitiID);
#if UNITY_EDITOR
        print("Load tokens with id: " + lastGraffitiID + "annotated: " + API.currentSentence.annotated + " " + string.Join(" ", currentAnnSent.tokens));
#endif
        Vector2 graffitiSize = new Vector2(0, 0);
        if (!graffitiArea || tokens.Count == 0)
        {
            throw new GraffitiException($"Area or tokens not defined: {transform.name} ID: {this.gameObject.GetInstanceID()}");
        }
        float space = 0.5f;
        //Fitter
        currentStringLength = 0;
        for (int i = 0; i < tokens.Count; i++) //dispongo
        {
            RectTransform rt = tokens[i].GetComponent<RectTransform>();
            if (i == 0)
            {
                tokens[i].transform.localPosition = new Vector3(0, 0, planeZ);
                length = rt.rect.width * tokenSize;
                firstOfLine = tokens[i];
            }
            else
            {
                if (length + rt.rect.width * tokenSize + space * tokenSize <= graffitiArea.transform.localScale.x * 10)
                {
                    tokens[i].transform.localPosition = new Vector3(length + space * tokenSize, tokens[i - 1].transform.localPosition.y, planeZ);
                    length += space * tokenSize + rt.rect.width * tokenSize;
                    if (length > graffitiSize.x)
                    {
                        graffitiSize = new Vector2(length, graffitiSize.y);
                    }
                }
                else
                {
                    tokens[i].transform.localPosition = new Vector3(firstOfLine.transform.localPosition.x, firstOfLine.transform.localPosition.y - firstOfLine.GetComponent<RectTransform>().rect.height, planeZ);
                    //reset length
                    length = rt.rect.width * tokenSize;
                    firstOfLine = tokens[i];
                    graffitiSize += new Vector2(0, firstOfLine.GetComponent<RectTransform>().rect.height);
                }
            }

        }
        //Center relative to rotation using transform.up/right/forward
        tokenContainer.transform.position = transform.position;
        tokenContainer.transform.position -= tokenContainer.transform.right * (graffitiSize.x / 2) * transform.localScale.x;
        tokenContainer.transform.position += tokenContainer.transform.up * (graffitiSize.y / 2) * transform.localScale.x;
    }

    private void HidePlayer()
    {
        foreach (Transform t in Agent.transform)
            if (t.gameObject.layer != 27)
                t.gameObject.layer = 29;
    }
    private void ShowPlayer()
    {
        foreach (Transform t in Agent.transform)
            if (t.gameObject.layer != 27)
                t.gameObject.layer = 10;
    }

    //Button press
    public void StartGraffiti(Player Agent)
    {
        Agent.GetComponent<PlayerLogger>().playerLog.NumberOfGraffitiActivated++;
        HidePlayer();
        if (Agent != null && DialogueInstancer.deactivateDialoguesAndGraffiti == false)
        {
            Agent.SetAnnotating(true);
            if (!MultiplatformUtility.Mobile)
            {
                enterButton = Agent.cameraInterface.enterButton;
                enterButton.SetActive(true);
                Agent.cameraInterface.eButton.SetActive(true);
                Agent.cameraInterface.cButton.SetActive(true);
            }
            //else
            //    enterButton = Agent.cameraInterface.okCircle;


            SetRainbow();
            Agent.GetComponent<Rigidbody>().velocity = Vector3.zero;

            Agent.playerLogger.StartGraffitiAnnotationSW();
            if (!GetComponent<No3DGraffiti>())
            {
                Camera.main.GetComponent<CameraOrbit>().enabled = false;
                Agent.GetComponent<Movement>().Busy = true;
            }
            wiper = Instantiate(Resources.Load<GameObject>("Wiper"));
            wiper.transform.eulerAngles = new Vector3(wiper.transform.eulerAngles.x, 90, wiper.transform.eulerAngles.z);
            //NASCONDI OSTACOLI
            //HideObstacles(tokens, hideMask);

            erasing = true;
        }
    }

    public void StopAnnotation(Player agent)
    {
        ShowPlayer();
        Agent.SetAnnotating(false);
        if (!MultiplatformUtility.Mobile)
        {
            enterButton.SetActive(false);
            Agent.cameraInterface.eButton.SetActive(false);
            Agent.cameraInterface.cButton.SetActive(false);
        }
        if (Player.rCondition == RCondition.NonRestricted)
        {
            Agent.cameraInterface.soapBar.GetComponent<Rainbow>().StopRainbow();
        }
        //Agent.cameraInterface.soapBar.transform.parent.gameObject.SetActive(true);
        agent.playerLogger.StopGraffitiAnnotationSW();
        if (Player.condition == Condition.W3D)
        {
            agent.GetComponent<Movement>().Busy = false;
            Destroy(wiper);
            agent.cameraInterface.cameraOrbit.enabled = true;
        }
        else
        {
            //Next graffiti
            uniqueGraffitiIndex += 1;
            Reload();
        }
        erasing = false;
        canErase = false;
    }

    //----------------------------
    //
    // ANNOTATION
    //
    //----------------------------
    private IEnumerator Annotate()
    {
        Player agent = Agent;
        agent.GetComponent<PlayerLogger>().playerLog.NumberOfAnnotatedGraffiti++;
        List<int> occludedTokens = CalculateOcclusion(tokens);
        float timePerToken = PlayerLogger.CalculateTimePerToken(currentAnnSent.tokens.Count, Agent.playerLogger.GetGraffitiAnnotationTime());
        string tasktype = Player.rCondition == RCondition.Restricted ? "GR" : "G";
        AnnotationData anndata = new AnnotationData(currentAnnSent.id, currentAnnSent.tokens, occludedTokens, timePerToken, tasktype);
        anndata.CleanSpaces();
        string goldann = "";
        float agreement = 0;
        //if (currentAnnSent.annotations.Count > 0) //if gold annotation data was found in LoadUtility.annSO at currentAnnSent index
        //    agreement = Annotation.GoldCompare(anndata, currentAnnSent);
        //else
        //    agreement = Annotation.SilverCompare(currentAnnSent);
        if (API.currentApi == Api.dev)
        {
            WWWForm form = new WWWForm();
            form.AddField("ID", anndata.id);
            using (UnityWebRequest www = UnityWebRequest.Post(API.urls.getGoldGraffiti, form))
            {
                yield return www.SendWebRequest();

                if (www.error == null && !www.downloadHandler.text.Contains("errore"))
                    goldann = www.downloadHandler.text;
            }
        }

        int selfAnnotated = anndata.annotations.Contains(1) ? 0 : 1;

        if (!String.IsNullOrEmpty(goldann))
        {
            //AnnotationData goldSentence = new AnnotationData(iLine, sqlSentence, goldann);
            //agreement = Annotation.GoldCompare(anndata, goldSentence);
            anndata.gold = goldann;

            if (int.Parse(goldann) == selfAnnotated)
                agreement = 1;
            else
                agreement = 0;
        }
        else
        {
            anndata.gold = "1";
        }

        if (exclamationIcon)
        {
            DestroyImmediate(exclamationIcon);
            questSatisfied = true;
        }

        if (!agent.gameState.annotatedGraffitiIndeces.Contains(currentAnnSent.id))
            agent.gameState.annotatedGraffitiIndeces.Add(currentAnnSent.id);
        
        API.PostAnnotation(agent, anndata);

        if (!pointsAlreadyGiven)
        {
            GetComponent<SpawnCrystals>().Spawn(agent);
            int points = 5 + (int)(15 * agreement);
            points *= Player.pointMultiplier;
            agent.TotalAnnotatedGraffiti += 1;
            PointSystem.AddPoints(this, agent, "Point", points);
            
            SafetyBar.AddSafety((20 + 20 * agreement) * Player.pointMultiplier);
            if (Player.condition == Condition.W3D)
                pointsAlreadyGiven = true;
        }

        annotated = true;
        uniqueGraffitiIndex += 1;

        lastGraffitiID = currentAnnSent.id;
        annotatedIndeces.Add(currentAnnSent.id);
        
        //annotatedGraffitiIndex += 1;

        API.PostSave(agent, false);
    }

    private void MoveCameraToGraffiti()
    {
        if (!GetComponent<No3DGraffiti>())
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, camPos.transform.position, Time.deltaTime * 3);
            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, camPos.transform.rotation, Time.deltaTime * 3);
        }
    }

    public void ShowHiddenObstacles(List<GameObject> memObst)
    {
        foreach (GameObject obst in memObst)
        {
            obst.GetComponent<MeshRenderer>().enabled = true;
            obst.GetComponent<MeshCollider>().enabled = true;
        }
        memObst.Clear();
    }

    public void Erase(RaycastHit hit, float tokenSize)
    {
        Agent.playerLogger.StopGraffitiAnnotationSW();
        if (Player.rCondition == RCondition.NonRestricted || (Player.rCondition == RCondition.Restricted && Agent.GetSoap() > 0))
        {
            GameObject sprite = Instantiate(Resources.Load<GameObject>("Erase"));
            sprite.transform.parent = eraseContainer.transform;
            sprite.transform.localPosition = transform.InverseTransformPoint(hit.point);
            sprite.transform.localPosition = new Vector3(sprite.transform.localPosition.x, sprite.transform.localPosition.y, planeZ - 0.05f);
            sprite.transform.localScale = new Vector3(tokenSize, tokenSize, tokenSize)/2;
            sprite.transform.rotation = sprite.transform.parent.rotation;
            if(Player.rCondition == RCondition.Restricted)
                Agent.SubtractSoap(1);
        }
        else
        {
            NotificationUtility.ShowString(Agent, ML.systemMessages.notEnoughSoap);
            PopUpUtility.Open(Agent.cameraInterface.popUpCanvas, PopUpType.Warning, "Sembra che tu non abbia più sapone. \n Raccogli cristalli e vai a un terminale (T) per comprarne ancora!", 4);
        }
    }

    public void EraseExperimental(RaycastHit hit, float tokenSize)
    {
        GameObject sprite = Instantiate(Resources.Load<GameObject>("EraseMesh"));
        sprite.transform.parent = eraseContainer.transform;
        sprite.transform.localPosition = transform.InverseTransformPoint(hit.point);
        sprite.transform.localPosition = new Vector3(sprite.transform.localPosition.x, sprite.transform.localPosition.y, planeZ - 0.05f);
        sprite.transform.localScale = new Vector3(tokenSize/100, tokenSize/100, tokenSize/100);
        sprite.transform.rotation = Quaternion.Euler(sprite.transform.parent.eulerAngles + new Vector3(-90,0,0));
    }

    public List<int> CalculateOcclusion(List<GameObject> tokens)
    {
        List<int> results = new List<int>();
        RaycastHit hit;
        int score = 0;
        int parts = 5;
        int cuts = parts - 1;
        for (int i = 0; i < tokens.Count; i++)
        {
            float xOffset = 0f;
            for (int i2 = 0; i2 < parts; i2++)
            {
                float part = tokens[i].GetComponent<TMP_Text>().GetRenderedValues(true).x*transform.localScale.x / parts;

                part /= 2.5f;

                if (i2 == 0)
                    xOffset = part;
                else if (i2 == 1)
                    xOffset = part * 2;
                else if (i2 == 2)
                    xOffset = part * 3;
                else if (i2 == 3)
                    xOffset = part * 4;
                else if (i2 == 4)
                    xOffset = part * 5;

                Vector3 point = tokens[i].transform.position + (tokens[i].transform.right * (xOffset-part/2));

                UnityEngine.Debug.DrawRay(Camera.main.transform.position, point-Camera.main.transform.position, Color.green, 1000);
                if (Physics.Raycast(Camera.main.transform.position, 
                    point - Camera.main.transform.position, 
                    out hit, 
                    Mathf.Infinity,
                    rayMask))
                {
                    //GameObject v = Instantiate(Resources.Load<GameObject>("test"));
                    //v.transform.position = hit.point;
                    //v.name = tokens[i].name + "testCube";
                    //if a word is hit
                    if (hit.collider.tag != "GraffitiToken")
                    {
                        score += 1;
                    }
                }
            }
            if (score >= 3)
            {
                //il token è considerato annotato!
                results.Add(1);
            }
            else
            {
                results.Add(0);
            }
            score = 0;
        }
        return results;
    }

    public void Unerase(Transform t)
    {

        List<GameObject> eraseChildren = new List<GameObject>();
        for (int i = 0; i < t.childCount; i++)
        {
            if (t.GetChild(i).gameObject.layer == 21)
                eraseChildren.Add(t.GetChild(i).gameObject);
            Agent.AddSoap(1);
        }

        foreach (GameObject obj in eraseChildren)
        {
            Destroy(obj);
        }
    }

    public void TriggerOn(Player agent)
    {

        this.Agent = agent;
        NotificationUtility.ShowString(agent, string.Format(ML.systemMessages.eraseOrReset, MultiplatformUtility.PrimaryInteractionKey, MultiplatformUtility.Cancel));
        agent.overrideInteraction += 1;
        
    }
    public void TriggerOff()
    {

        Unload();

        shownIndeces.Remove(currentAnnSent.id);

        lastGraffitiID = 0;
        annotated = false;

        Agent.overrideInteraction -= 1;
        if (erasing)
        {
            StopAnnotation(this.Agent);
        }
        this.Agent = null;
        
    }
}
