using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataUtilities;
using System.IO;
using System.Xml.Serialization;
using UnityEngine.Rendering.PostProcessing;
using System;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public interface IPlayer
{
    Player Agent { get; set; }
}

public enum Condition { W3D, NoW3D }
public enum Design { Between, Within }
public enum RCondition { Restricted, NonRestricted }
public enum Version { Kid, Gram }

[Serializable]
public class Player : MonoBehaviour, IPlayer
{
    public static ML.Lang language;
    public static Design design = Design.Between;
    public static Version version = Version.Kid;
    public static bool demo = false;
    public static bool admin;
    public static RCondition rCondition;
    
    public bool currentlyRestricted;
    public List<AnnotationData> annotationData;
    public bool load;
    private bool scarpeMolla;
    public bool ScarpeMolla
    {
        get { return scarpeMolla; }
        set {
            scarpeMolla = value;
        }
    }

    private bool glider;
    public bool Glider
    {
        get { return glider; }
        set
        {
            glider = value;
        }
    }
    private bool annotating;
    public string playerName;
    public GameObject mapFull;
    private bool onZebra;
    public PlayerLogger playerLogger;
    public PlayerLog playerLog;
    public GameObject avatar;
    public Vector3 playerSize;
    public static int overlays;
    public CameraInterface cameraInterface;
    public string saveName = "PlayerData";
    public int overrideInteraction;
    public Player Agent { get; set; }
    public Vector3 pos;
    public string name;
    public string id;
    public bool initialized;
    public string sessionID;
    public GameState gameState;
    public bool loaded;
    public List<APISentence> dialogueSentences;
    public List<APISentence> graffitiSentences;
    public static int area;
    private bool dissolved;
    public bool graffitiTutorial, dialogueTutorial;
    public static Condition condition;
    public static int pointMultiplier = 2;
    public static int sentenceLimit = 30;
    public bool questionnaireFilled;
    public int friends;
    public string lastFriend;
    public StringParts friendParts;
    public static bool gameOverCalled;
    public bool noMenu;
    private int totalAnnotatedDialogues, totalAnnotatedGraffiti;
    public int TotalAnnotatedDialogues { get => totalAnnotatedDialogues; 
        set 
        {
            totalAnnotatedDialogues = value;
            UpdateProgress();
            if (totalAnnotatedDialogues >= 15 && totalAnnotatedGraffiti >= 15)
            {
                if (Player.demo && !questionnaireFilled)
                    cameraInterface.menuCanvas.GetComponent<Menu>().TakeMeToTheQuestionnaire(this);
            }
        } 
    }
    public int TotalAnnotatedGraffiti
    {
        get => totalAnnotatedGraffiti;
        set
        {
            totalAnnotatedGraffiti = value;
            UpdateProgress();
            if (totalAnnotatedDialogues >= 15 && totalAnnotatedGraffiti >= 15)
            {
                if (Player.demo && !questionnaireFilled)
                    cameraInterface.menuCanvas.GetComponent<Menu>().TakeMeToTheQuestionnaire(this);
            }
        }
    }
    public GameObject interactingNPC;
    public int StoryProgress;

    [SerializeField]
    private int crystals;
    public int Crystals
    {
        get => crystals;
        set
        {
            if (value < 0)
                crystals = 0;
            else
                crystals = value;
            UpdateCrystals();
        }
    }
    private float exp;
    private float likes;
    private int level;
    public int Level 
    { 
        get => level;
        set 
        { 
            level = value;
            UpdateLevel();
            SetAreaAccess();
        }
    }
    public float MaxExp { get; set; }
    public float Exp
    {
        get => exp;
        set
        {
            if (value > MaxExp)
            {
                exp = 0F;
                Level += 1;
                MaxExp = Mathf.Pow(Level, 3)+100f;
                UpdateLevel();
            }
            else
                exp = value;
            UpdateBar();
        }
    }
    public float Likes
    {
        get => likes;
        set
        {
            likes = value;
            UpdateLikes();
        }
    }

    public void UpdateProgress()
    {
        cameraInterface.progress.text = $"Dialoghi: {TotalAnnotatedDialogues}/15 \n Murales: {totalAnnotatedGraffiti}/15";
    }
    private void SetAreaAccess()
    {
        area = Level;
    }
    public void SetEnergy(int amount)
    {
        cameraInterface.battery.GetComponent<Battery>().Set(amount);
    }
    public void SetSoap(float amount)
    {
        cameraInterface.soapBar.Soap = amount;
    }
    public void SetMaxSoap(float amount)
    {
        cameraInterface.soapBar.max = amount;
    }
    public void AddCrystals(int amount)
    {
        Crystals += amount;
        UpdateCrystals();
    }
    public int GetCrystals()=>
        crystals;
    
    public void SetCrystals(int amount)=>
        Crystals = amount;
    public void UpdateCrystals()=>
        cameraInterface.crystalCounterText.text = Crystals.ToString();
    public void AddExp(int points)
    {
        Exp += points;
        UpdateBar();
    }
    public void AddLikes(int points)
    {
        Likes += points;
        UpdateLikes();
    }
    public void UpdateLikes() =>
        cameraInterface.likesText.text = Likes.ToString();
    public void UpdateLevel()=>
        cameraInterface.level.text = $"Lv {Level}";
    public void UpdateBar()=>
        cameraInterface.expBar.fillAmount = (float)(Exp / MaxExp);

    public float GetExp()=>
        Exp;
    public void SetExp(float amount)=>
        Exp = amount;
    public float GetMaxExp()=>
        MaxExp;
    public void SetMaxExp(float amount)=>
        MaxExp = amount;
    public int GetLevel()=>Level;

    public void SetLevel(int amount)=>
        Level = amount;
    public void SetSafety(float amount) => SafetyBar.CurrentSafety = amount;
    public float GetSoap()=>cameraInterface.soapBar.Soap;
    public float GetMaxSoap()=>cameraInterface.soapBar.max;
    public void SubtractSoap(float amount)=>cameraInterface.soapBar.Soap -= amount;
    public void AddSoap(int amount)=>cameraInterface.soapBar.Soap += amount;
    public void ShowSoap()=>cameraInterface.soapBar.gameObject.SetActive(true);
    public void HideSoap()=>cameraInterface.soapBar.transform.parent.gameObject.SetActive(false);
    public void AddEnergy(int amount)=>cameraInterface.battery.GetComponent<Battery>().Change(amount);
    public void SubtractEnergy(int amount)=>cameraInterface.battery.GetComponent<Battery>().Change(-amount);
    public int GetEnergy()=>cameraInterface.battery.GetComponent<Battery>().charge;
    public int GetMaxEnergy()=>7;
    public void ShowBattery()=>cameraInterface.battery.SetActive(true);
    public void HideBattery()=>cameraInterface.battery.SetActive(false);

    public void SetGlider(bool owned)
    {
        Glider = owned;
        cameraInterface.glider.SetActive(owned);
    }
    public void SetRocket(bool owned)
    {
        ScarpeMolla = owned;
        cameraInterface.rocket.SetActive(owned);
    }

    private void Awake()
    {
        gameState = new GameState();
        gameState.id = id;
        gameState.soap = GetSoap();
        condition = SceneManager.GetActiveScene().name == "SampleScene" ? Condition.W3D : Condition.NoW3D;
    }
    private void Start()
    {
        annotationData = new List<AnnotationData>();
        if (GetComponent<Movement>())
        {
            if (scarpeMolla)
                GetComponent<Movement>().jumpForce = 18;
            else
                GetComponent<Movement>().jumpForce = 4;
        }
        Agent = this;
        if (!cameraInterface)
            cameraInterface = FindObjectOfType<CameraInterface>();
        //ObjectState o = new ObjectState(GameObject.Find("Scientist"));
        //gameState.saveableObjects.Add(o);
        //gameState.saveableObjects[0].state = "show";

    }

    void Update()
    {
        if (admin || Application.isEditor)
        {
            //DEV COMMANDS
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKey("r"))
                {
                    print("rocket");
                    SetRocket(true);
                }
                else if (Input.GetKey("g"))
                {
                    SetGlider(true);
                }
            }
        }
        if (SaveManager.gameLoaded)
        {
            if (!dissolved)
            {
                cameraInterface.transitionCanvas.gameObject.SetActive(false);
                dissolved = true;
            }
            if (Input.GetKeyUp("m") && !GetComponent<Movement>().Busy)
            {
                mapFull.SetActive(!mapFull.activeSelf);
            }

            if (Input.GetKeyUp("t") && !this.GetComponent<Movement>().Busy)
            {
                if (!PhoneUtility.phoneOut)
                {
                    PhoneUtility.Up(GetComponent<Player>());
                }
                else
                {
                    PhoneUtility.Down(GetComponent<Player>());
                }

            }
#if UNITY_EDITOR
            //if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyUp("p"))
            //{
            //    SaveManager.SaveGameState(this);
            //}
#endif


        }
    }

    public static string Md5(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        byte[] hashBytes;
        using (System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }

    void OnApplicationQuit()
    {
        //SaveManager.SaveGameState(this, gameState);
        //SaveManager.SaveDelegate(this);
    }
    public void Initialize(bool load)
    {
        //Setup player transform data
        transform.parent = null;
        GetComponent<CharacterCustomizationSetup>().enabled = false;
        GetComponent<Rigidbody>().useGravity = true;
        //transform.gameObject.layer = 10;
        //foreach (Transform child in transform)
        //{
        //    if (child.tag != "UI")
        //    {
        //        child.gameObject.layer = 10;
        //    }
        //}
        //Control.Instance.GetComponent<UpdateAvatar>().Up();
        cameraInterface.audioManager.ChangeBGM(AudioManager.TownMusic);

        //Setup cameras
        if (Player.condition == Condition.W3D)
        {
            GraphicsUtility.DisableDof();
            cameraInterface.cameraOrbit.enabled = true;
            cameraInterface.followPlayer.enabled = true;
        }
        cameraInterface.uicam.orthographic = true;
        cameraInterface.hudCanvas.gameObject.SetActive(true);
        cameraInterface.inventory.SetActive(false);


        //Setup start parameters
        transform.localScale = playerSize;
        transform.position = pos;
        transform.rotation = Quaternion.identity;
        GetComponent<Movement>().enabled = true;
        GetComponent<Movement>().Busy = false;

        //Setup control
        //Movement.Busy = false;
        initialized = true;

        //Temporary logger
        playerLogger = GetComponent<PlayerLogger>();
        //print(JsonUtility.ToJson(quest));

        if (load)
        {
            SetID(PlayerPrefs.GetString("ID"));
            API.GetSave(this);

            //List<string> list = new List<string>();
            //list.Add("lol");
            //list.Add("mah");

            //DialogueLines d = new DialogueLines(list);
            //print(JsonUtility.ToJson(d));
        }
        else
        {
            StartCoroutine(NewPlayer());
        }
    }

    public void Deinitialize()
    {
        GraphicsUtility.EnableDof();
        cameraInterface.cameraOrbit.enabled = false;
        cameraInterface.followPlayer.enabled = false;
        cameraInterface.hudCanvas.gameObject.SetActive(false);
        SequenceManager.sequence = Sequence.load;
        GetComponent<Movement>().enabled = false;
        GetComponent<Movement>().Busy = true;
        GetComponent<Rigidbody>().useGravity = false;
        initialized = false;
        this.enabled = false;
    }

    public IEnumerator NewPlayer()
    {
        SetEnergy(GetMaxEnergy());
        SetMaxSoap(100);
        SetSoap(100);
        SetLevel(1);
        SetMaxExp(100);
        SetExp(0);
        SetSafety(1000);
        
        while (API.readyToLoadGame == false)
        {
            yield return null;
        }
        area = 1;
        playerLog = new PlayerLog();
        transform.position = GameObject.Find("PlayerPos").transform.position;
        SaveManager.gameLoaded = true;
        if (LoadingManager.config.guest == true)
            SetID(Password.Generate() + "Guest");
        else
            SetID(Password.Generate());
        //PopUpUtility.Open(cameraInterface.popUpCanvas, PopUpType.LocalizedType(Agent, PopUpType.Types.success), string.Format(ML.systemMessages.yourID, id, MultiplatformUtility.EscapeKey), 5);
        cameraInterface.transitionCanvas.gameObject.SetActive(false);
        playerLogger.StartGameTimeSW();

        if (Player.demo)
        {
            SetGlider(true);
            SetRocket(true);
        }

        playerLogger.playerLog.CustomizationTime = CharacterCustomization.csw.ElapsedMilliseconds / 1000f;
        yield return StartCoroutine(DecideCondition());
        LoadUtility.AllLoaded = true;
    }
    private IEnumerator DecideCondition()
    {
        if (Player.demo)
        {
            HideSoap();
            HideBattery();
            Player.rCondition = RCondition.NonRestricted;
        }
        else
        {
            if (rCondition == RCondition.NonRestricted)
            {
                HideSoap();
                HideBattery();
            }

            if (API.currentApi == Api.dev)
            {
                int[] vals = new int[2];

                using (UnityWebRequest www = UnityWebRequest.Get(API.urls.getConditions))
                {
                    yield return www.SendWebRequest();
                    vals[0] = int.Parse(www.downloadHandler.text.Split(',')[0]);
                    vals[1] = int.Parse(www.downloadHandler.text.Split(',')[1]);
                }

                int newValue;
                if (vals[0] <= vals[1])
                {
                    rCondition = RCondition.Restricted;
                    newValue = vals[0] + 1;
                }
                else
                {
                    rCondition = RCondition.NonRestricted;
                    newValue = vals[1] + 1;
                }

                WWWForm form = new WWWForm();
                form.AddField("Type", rCondition.ToString());
                form.AddField("Value", newValue);

                using (UnityWebRequest www = UnityWebRequest.Post(API.urls.updateConditions, form))
                {
                    yield return www.SendWebRequest();
                }
            }

        }
        //SaveManager.SaveGameState(this);
    }
    public void SetID(string id)
    {
        this.id = id;
        PlayerPrefs.SetString("ID", id);
        cameraInterface.currentID.text = "Current ID: " + id;
    }

    public void ToggleOnZebra()
    {
        onZebra = !onZebra;
    }
    public bool IsOnZebra()
    {
        return onZebra;
    }

    public void ShowGraffitiTutorial()
    {
        GetComponent<Movement>().Busy = true;
        StartCoroutine(TaskTutorial(cameraInterface.graffitiTutorialContainer));
    }
    public void ShowDialogueTutorial()
    {
        GetComponent<Movement>().Busy = true;
        StartCoroutine(TaskTutorial(cameraInterface.dialogueTutorialContainer));
    }

    public void SetAnnotating(bool annotating)
    {
        this.annotating = annotating;
    }
    public bool IsAnnotating()
    {
        return annotating;
    }

    IEnumerator TaskTutorial(GameObject container)
    {
        noMenu = true;
        DialogueInstancer.deactivateDialoguesAndGraffiti = true;
        string type;
        if (container.name.Contains("Dialogue"))
        {
            type = "d";
            dialogueTutorial = true;
        }
        else
        {
            type = "g";
            graffitiTutorial = true;
        }

        container.gameObject.SetActive(true);

        Image b = container.transform.GetChild(0).GetComponent<Image>();
        float t = 0;
        Image[] imgs = container.GetComponentsInChildren<Image>();
        TextMeshProUGUI[] txts = container.GetComponentsInChildren<TextMeshProUGUI>();

        if (type == "d")
        {
            txts[0].text = string.Format(ML.systemMessages.tutorialD0, MultiplatformUtility.PrimaryInteractionKey);
            txts[1].text = MultiplatformUtility.Mobile ? ML.systemMessages.tutorialD1Tap : ML.systemMessages.tutorialD1;
            txts[2].text = ML.systemMessages.tutorialD2;
            txts[3].text = ML.systemMessages.tutorialD3;
        }
        else
        {
            txts[0].text = string.Format(ML.systemMessages.tutorialG0, MultiplatformUtility.PrimaryInteractionKey);
            txts[1].text = MultiplatformUtility.Mobile ? ML.systemMessages.tutorialG1Tap : ML.systemMessages.tutorialG1;
            txts[2].text = ML.systemMessages.tutorialG2;
        }

        while (t < 1)
        {
            b.color = Color.Lerp(new Color(0, 0, 0, 0f), new Color(0, 0, 0, 0.45f), t);
            t += Time.deltaTime;
            yield return null;
        }

        for(int i = 0; i < txts.Length; i++)
        {

            while (t < 1.2f)
            {
                imgs[i+1].color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, t);
                txts[i].color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, t);
                t += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(0.75f);
            t = 0;
        }

        while (!Input.anyKey)
        {
            yield return null;
        }

        t = 0;

        while (t < 1.2f)
        {
            for (int i = 0; i < txts.Length; i++)
            {
                imgs[i + 1].color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), t);
                txts[i].color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), t);
                t += Time.deltaTime;
            }
            yield return null;
        }


        container.gameObject.SetActive(false);
        if (type == "d")
            cameraInterface.participateButton.GetComponent<ParticipateButton>().Show();
        GetComponent<Movement>().Busy = false;
        DialogueInstancer.deactivateDialoguesAndGraffiti = false;
        noMenu = false;
    }

    public void GameOver()
    {
        cameraInterface.gameOver.SetActive(true);
        cameraInterface.gameOver.GetComponent<GameOver>().Activate();
    }
}