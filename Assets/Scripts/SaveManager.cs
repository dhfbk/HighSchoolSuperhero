using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using DataUtilities;
using UnityEngine.Networking;

public interface ISaveable
{
    string State { get; set; }
}

public enum User { user, dev }

[Serializable]
public class StringParts
{
    public List<string> meshes, hairmats, shoesmats, pantsmats, shirtmats, glassesmats, lensesmats, eyesmats, bodymats;
    public StringParts()
    {
        meshes = new List<string>();
        
        hairmats = new List<string>();
        shoesmats = new List<string>();
        pantsmats = new List<string>();
        shirtmats = new List<string>();
        glassesmats = new List<string>();
        lensesmats = new List<string>();
        eyesmats = new List<string>();
        bodymats = new List<string>();
    }

    public StringParts(Parts parts)
    {
        meshes = new List<string>();
        meshes.Add(parts.body.GetComponent<SkinnedMeshRenderer>().sharedMesh.name);
        meshes.Add(parts.hair.GetComponent<SkinnedMeshRenderer>().sharedMesh.name);
        meshes.Add(parts.shirt.GetComponent<SkinnedMeshRenderer>().sharedMesh.name);
        meshes.Add(parts.pants.GetComponent<SkinnedMeshRenderer>().sharedMesh.name);
        meshes.Add(parts.shoes.GetComponent<SkinnedMeshRenderer>().sharedMesh.name);
        if (parts.glasses.GetComponent<SkinnedMeshRenderer>().sharedMesh != null)
            meshes.Add(parts.glasses.GetComponent<SkinnedMeshRenderer>().sharedMesh.name);
        if (parts.lenses.GetComponent<SkinnedMeshRenderer>().sharedMesh != null)
            meshes.Add(parts.lenses.GetComponent<SkinnedMeshRenderer>().sharedMesh.name);
    }
}

[Serializable]
public class ObjectState
{
    public string name;
    public string state;
    public Vector3 position;
    public Quaternion rotation;
    public bool active;
    public ObjectState() { }
    public ObjectState(GameObject obj, bool posRot)
    {
        if (obj.GetComponent<ISaveable>() != null)
        {
            this.name = obj.name;
            this.active = obj.activeSelf;
            if (posRot)
            {
                this.position = obj.transform.position;
                this.rotation = obj.transform.rotation;
            }
            this.state = obj.GetComponent<ISaveable>().State;
        }
    }
}
public class GameState
{
    public int friends;
    public bool loaded;
    public string id;
    public float safety;
    public RCondition rCondition;
    public int totalAnnotatedDialogues, totalAnnotatedGraffiti;
    public bool questionnaireFilled;
    public int storyProgress, area;
    public Vector3 playerPos;
    public Quaternion rotation;
    public PlayerLog playerLog;
    public int level, crystals, battery;
    public float exp, maxexp;
    public float soap, maxSoap;
    public int likes;
    public bool rocket, glider;
    public List<string> playerShirtMats;
    public List<string> playerPantsMats;
    public List<string> playerHairMats;
    public List<string> playerShoesMats;
    public List<string> playerGlassesMats;
    public List<string> playerLensesMats;
    public List<string> playerEyesMats;
    public List<string> playerBodyMats;
    public int dialogueIndex;
    public bool graffitiTutorial;
    public bool dialogueTutorial;
    public List<int> annotatedGraffitiIndeces;
    public List<ObjectState> saveableObjects;
    public User user;
    public List<string> playerParts;
    public List<string> friendParts;
    public QFilledList questionnaireData;

    public GameState()
    {
        annotatedGraffitiIndeces = new List<int>();
        saveableObjects = new List<ObjectState>();
        playerParts = new List<string>();
        playerHairMats = new List<string>();
        playerShoesMats = new List<string>();
        playerPantsMats = new List<string>();
        playerShirtMats = new List<string>();
        playerGlassesMats = new List<string>();
        playerLensesMats = new List<string>();
        playerEyesMats = new List<string>();
        playerBodyMats = new List<string>();
    }
}

public enum TaskType { hate, acc }
public class SaveManager : MonoBehaviour
{
    public delegate void SaveDelegate();
    public static SaveDelegate saveDelegate;
    public GameState gameState;
    public static bool saveChecked;
    public static bool gameLoaded;

    public static TaskType type = TaskType.hate;

    public static void SaveState(Player agent, ObjectState os)
    {
        List<ObjectState> oss = agent.gameState.saveableObjects;
        ObjectState x;
        if ((x = oss.Find(x => x.name == os.name)) != null)
        {
            oss.Remove(x);
            oss.Add(os);
        }
        else
        {
            oss.Add(os);
        }
    }
    
    public static void DeployGameState(Player agent, GameState state)
    {
        print("deploy called");
        if (PlayerPrefs.GetString("Name") != null)
            agent.name = PlayerPrefs.GetString("Name");

        agent.gameState = state;
        agent.GetComponent<PlayerLogger>().playerLog = state.playerLog;

        Player.rCondition = state.rCondition;

        SafetyBar.CurrentSafety = state.safety;
        agent.StoryProgress = state.storyProgress;
        agent.friends = state.friends;
        agent.questionnaireFilled = state.questionnaireFilled;
        agent.TotalAnnotatedDialogues = state.totalAnnotatedDialogues;
        agent.TotalAnnotatedGraffiti = state.totalAnnotatedGraffiti;
        agent.SetMaxSoap(state.maxSoap);
        agent.SetSoap(state.soap);
        agent.SetLevel(state.level);
        agent.SetMaxExp(state.maxexp);
        agent.SetExp(state.exp);
        agent.SetEnergy(state.battery);
        agent.SetCrystals(state.crystals);
        agent.SetLikes(state.likes);
        agent.graffitiTutorial = state.graffitiTutorial;
        agent.dialogueTutorial = state.dialogueTutorial;
        agent.questionnaireData = state.questionnaireData;

        Graffiti.gameState = state;
        DialogueInstancer.uniqueLineIndex = state.dialogueIndex;

        //if (Player.condition == Condition.W3D)
        //{
            agent.transform.position = state.playerPos;
        print(state.playerPos);
        print(agent.transform.position);
            agent.SetRocket(state.rocket);
            agent.SetGlider(state.glider);
            agent.transform.rotation = state.rotation;

            LoadLook(agent.GetComponent<Parts>(), state);
            LoadAvatarLook(agent.avatar);
            //LoadLook(agent.avatar.GetComponent<Parts>(), state);

            foreach (ObjectState os in state.saveableObjects)
            {
                GameObject o;
                if ((o = GameObject.Find(os.name)) != null)
                {
                    o.transform.position = os.position;

                    o.transform.rotation = os.rotation;
                    o.SetActive(os.active);
                    o.GetComponent<ISaveable>().State = os.state;
                    o.GetComponentInChildren<ISaveable>().State = os.state;
                }
            }
        //}

        if (Player.demo)
        {
            agent.SetGlider(true);
            agent.SetRocket(true);
            Player.rCondition = RCondition.NonRestricted;
        }
        if (Player.rCondition == RCondition.NonRestricted)
        {
            agent.HideBattery();
            agent.HideSoap();
        }

        //agent.SetSoap(100); //test
        agent.playerLogger.StartGameTimeSW();
        gameLoaded = true;
        FindObjectOfType<Intro>().SetSkipIntro(true);
        LoadUtility.AllLoaded = true;
    }
    public static void LoadAvatarLook(GameObject avatar)
    {
        Parts originalParts = FindObjectOfType<Player>().GetComponent<Parts>();
        avatar.GetComponent<Parts>().body.GetComponent<SkinnedMeshRenderer>().sharedMesh = originalParts.body.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        avatar.GetComponent<Parts>().body.GetComponent<SkinnedMeshRenderer>().material.color = originalParts.body.GetComponent<SkinnedMeshRenderer>().material.color;
        avatar.GetComponent<Parts>().hair.GetComponent<SkinnedMeshRenderer>().sharedMesh = originalParts.hair.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        avatar.GetComponent<Parts>().hair.GetComponent<SkinnedMeshRenderer>().material.color = originalParts.hair.GetComponent<SkinnedMeshRenderer>().material.color;
        avatar.GetComponent<Parts>().shirt.GetComponent<SkinnedMeshRenderer>().sharedMesh = originalParts.shirt.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        avatar.GetComponent<Parts>().shirt.GetComponent<SkinnedMeshRenderer>().material.color = originalParts.shirt.GetComponent<SkinnedMeshRenderer>().material.color;
        avatar.GetComponent<Parts>().glasses.GetComponent<SkinnedMeshRenderer>().sharedMesh = originalParts.glasses.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        avatar.GetComponent<Parts>().glasses.GetComponent<SkinnedMeshRenderer>().material.color = originalParts.glasses.GetComponent<SkinnedMeshRenderer>().material.color;
        avatar.GetComponent<Parts>().lenses.GetComponent<SkinnedMeshRenderer>().sharedMesh = originalParts.lenses.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        avatar.GetComponent<Parts>().lenses.GetComponent<SkinnedMeshRenderer>().material.color = originalParts.lenses.GetComponent<SkinnedMeshRenderer>().material.color;
    }
    public static void LoadLook(Parts parts, StringParts stringParts)
    {
        parts.body.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Parts/Body/" + stringParts.meshes[0]);
        parts.body.GetComponent<SkinnedMeshRenderer>().materials = new Material[] { Resources.Load<Material>("Materials/Characters") };

        parts.hair.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Parts/Hair/" + stringParts.meshes[1]);
        parts.hair.GetComponent<SkinnedMeshRenderer>().materials = new Material[] { Resources.Load<Material>("Materials/Characters") };

        parts.shirt.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Parts/Shirt/" + stringParts.meshes[2]);
        parts.shirt.GetComponent<SkinnedMeshRenderer>().materials = new Material[] { Resources.Load<Material>("Materials/Characters") };
        //parts.eyes.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Parts/Eyes/" + stringParts.meshes[2]);

        if (stringParts.meshes[3].Contains("Pant"))
            parts.pants.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Parts/Pants/" + stringParts.meshes[3]);
        else
            parts.pants.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Parts/Skirt/" + stringParts.meshes[3]);
        parts.pants.GetComponent<SkinnedMeshRenderer>().materials = new Material[] { Resources.Load<Material>("Materials/Characters") };

        parts.shoes.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Parts/Shoes/" + stringParts.meshes[4]);
        parts.shoes.GetComponent<SkinnedMeshRenderer>().materials = new Material[] { Resources.Load<Material>("Materials/Characters") };

    }
    public static void LoadLook(Parts parts, GameState state)
    {
        List<string> meshes = state.playerParts;
        //Meshes
        parts.hair.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Parts/Player/" + meshes[1]);
        parts.eyes.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Parts/Player/" + meshes[2]);
        parts.shoes.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Parts/Player/" + meshes[7]);
        parts.pants.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Parts/Player/" + meshes[6]);
        parts.shirt.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Parts/Player/" + meshes[5]);
        //parts.body.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Parts/Player/" + state.playerParts["body"]);
        parts.lenses.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Parts/Player/" + meshes[4]);
        parts.glasses.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Parts/Player/" + meshes[3]);

        //Materials
        List<Material> hairMats = new List<Material>();
        foreach (string mat in state.playerHairMats)
            hairMats.Add(Resources.Load<Material>("Materials/" + mat));
        parts.hair.GetComponent<SkinnedMeshRenderer>().materials = hairMats.ToArray();

        List<Material> shirtMats = new List<Material>();
        foreach (string mat in state.playerShirtMats)
            shirtMats.Add(Resources.Load<Material>("Materials/" + mat));
        parts.shirt.GetComponent<SkinnedMeshRenderer>().materials = shirtMats.ToArray();

        List<Material> shoesMats = new List<Material>();
        foreach (string mat in state.playerShoesMats)
            shoesMats.Add(Resources.Load<Material>("Materials/" + mat));
        parts.shoes.GetComponent<SkinnedMeshRenderer>().materials = shoesMats.ToArray();

        List<Material> pantsMats = new List<Material>();
        foreach (string mat in state.playerPantsMats)
            pantsMats.Add(Resources.Load<Material>("Materials/" + mat));
        parts.pants.GetComponent<SkinnedMeshRenderer>().materials = pantsMats.ToArray();

        List<Material> glassesMats = new List<Material>();
        foreach (string mat in state.playerGlassesMats)
            glassesMats.Add(Resources.Load<Material>("Materials/" + mat));
        parts.glasses.GetComponent<SkinnedMeshRenderer>().materials = glassesMats.ToArray();

        List<Material> lensesMats = new List<Material>();
        foreach (string mat in state.playerLensesMats)
            lensesMats.Add(Resources.Load<Material>("Materials/" + mat));
        parts.lenses.GetComponent<SkinnedMeshRenderer>().materials = lensesMats.ToArray();

        List<Material> bodyMats = new List<Material>();
        foreach (string mat in state.playerBodyMats)
            bodyMats.Add(Resources.Load<Material>("Materials/" + mat));
        parts.body.GetComponent<SkinnedMeshRenderer>().materials = bodyMats.ToArray();

    }

    public static IEnumerator SaveExists(Player agent)
    {
        string json;
        WWWForm form = new WWWForm();
        form.AddField("ID", agent.id);

        using (UnityWebRequest www = UnityWebRequest.Post(API.urls.getSave, form))
        {
            yield return www.SendWebRequest();
            saveChecked = true;
            if (String.IsNullOrEmpty(www.downloadHandler.text))
            {
                LoadingManager.skipCustomization = false;
            }
            else
                LoadingManager.skipCustomization = true;
        }
    }

    public static void SavePlayerStats(Player agent) //to be moved
    {
        Save(agent, agent.saveName);
    }

    public static void Save<T>(T data, string saveName)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (FileStream stream = new FileStream(Path.Combine(Application.streamingAssetsPath, $"{saveName}.hssh"), FileMode.Create))
        {
            serializer.Serialize(stream, data);
        }
    }
    public static string ConvertToXmlString<T>(T data)
    {
        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

        ns.Add("", "");
        var settings = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = true,
        };
        using (var stringWriter = new StringWriter())
        {
            using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(xmlWriter, data, ns);
                return stringWriter.ToString();
            }
        }
    }
    public static void SaveGameState(Player agent, bool showMessage = false)
    {
        //Fill the game state
        if (agent.gameState == null)
            agent.gameState = new GameState();
        GameState state = agent.gameState;

        PlayerLogger pl = agent.GetComponent<PlayerLogger>();
        pl.playerLog.GameTime += pl.gametimesw.ElapsedMilliseconds / 1000f;
        pl.gametimesw.Restart();

        //Transfer from logger to player
        //agent.playerLog = pl.playerLog;

        state.playerLog = pl.playerLog;

        state.friends = agent.friends;
        state.id = agent.id;
        state.graffitiTutorial = agent.graffitiTutorial;
        state.dialogueTutorial = agent.dialogueTutorial;
        state.safety = SafetyBar.CurrentSafety;
        state.totalAnnotatedDialogues = agent.TotalAnnotatedDialogues;
        state.totalAnnotatedGraffiti = agent.TotalAnnotatedGraffiti;
        state.rCondition = Player.rCondition;
        state.questionnaireFilled = agent.questionnaireFilled;
        state.storyProgress = agent.StoryProgress;
        state.soap = agent.GetSoap();
        state.maxSoap = agent.GetMaxSoap();

        state.level = agent.GetLevel();
        state.exp = agent.GetExp();
        state.maxexp = agent.GetMaxExp();
        state.crystals = agent.GetCrystals();
        state.likes = agent.GetLikes();

        state.rocket = agent.ScarpeMolla;
        state.glider = agent.Glider;
        state.playerPos = agent.transform.position;
        state.rotation = agent.transform.rotation;
        state.battery = agent.GetEnergy();
        state.playerParts = new List<string>();
        state.dialogueIndex = DialogueInstancer.uniqueLineIndex;

        state.questionnaireData = agent.questionnaireData;

        Parts parts = agent.GetComponent<Parts>();
        state.playerParts.Add(parts.body.GetComponent<SkinnedMeshRenderer>().sharedMesh.name);
        state.playerParts.Add(parts.hair.GetComponent<SkinnedMeshRenderer>().sharedMesh.name);
        state.playerParts.Add(parts.eyes.GetComponent<SkinnedMeshRenderer>().sharedMesh.name);
        state.playerParts.Add(parts.glasses.GetComponent<SkinnedMeshRenderer>().sharedMesh.name);
        state.playerParts.Add(parts.lenses.GetComponent<SkinnedMeshRenderer>().sharedMesh.name);
        state.playerParts.Add(parts.shirt.GetComponent<SkinnedMeshRenderer>().sharedMesh.name);
        state.playerParts.Add(parts.pants.GetComponent<SkinnedMeshRenderer>().sharedMesh.name);
        state.playerParts.Add(parts.shoes.GetComponent<SkinnedMeshRenderer>().sharedMesh.name);

        state.playerShirtMats.Clear();
        foreach (Material mat in parts.shirt.GetComponent<SkinnedMeshRenderer>().materials)
            state.playerShirtMats.Add(mat.name.Replace(" (Instance)", ""));

        state.playerShoesMats.Clear();
        foreach (Material mat in parts.shoes.GetComponent<SkinnedMeshRenderer>().materials)
            state.playerShoesMats.Add(mat.name.Replace(" (Instance)", ""));

        state.playerPantsMats.Clear();
        foreach (Material mat in parts.pants.GetComponent<SkinnedMeshRenderer>().materials)
            state.playerPantsMats.Add(mat.name.Replace(" (Instance)", ""));

        state.playerGlassesMats.Clear();
        foreach (Material mat in parts.glasses.GetComponent<SkinnedMeshRenderer>().materials)
            state.playerGlassesMats.Add(mat.name.Replace(" (Instance)", ""));

        state.playerLensesMats.Clear();
        foreach (Material mat in parts.lenses.GetComponent<SkinnedMeshRenderer>().materials)
            state.playerLensesMats.Add(mat.name.Replace(" (Instance)", ""));

        state.playerHairMats.Clear();
        foreach (Material mat in parts.hair.GetComponent<SkinnedMeshRenderer>().materials)
            state.playerHairMats.Add(mat.name.Replace(" (Instance)", ""));

        state.playerEyesMats.Clear();
        foreach (Material mat in parts.eyes.GetComponent<SkinnedMeshRenderer>().materials)
            state.playerEyesMats.Add(mat.name.Replace(" (Instance)", ""));

        state.playerBodyMats.Clear();
        foreach (Material mat in parts.body.GetComponent<SkinnedMeshRenderer>().materials)
            state.playerBodyMats.Add(mat.name.Replace(" (Instance)", ""));

#if UNITY_EDITOR
        state.user = User.dev;
#endif
        //Save the dam game state
        string savestring = SaveJson(state);

        API.PostSave(agent, showMessage);
    }

    public static string SaveJson<T>(T data)
    {
        return JsonUtility.ToJson(data);
    }
    public static void Save<T>(T data, string saveName, string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (FileStream stream = new FileStream(Path.Combine(path, saveName), FileMode.Create))
        {
            serializer.Serialize(stream, data);
        }
    }

    public static T Load<T>(string saveName) where T : new()
    {
        if (File.Exists(Path.Combine(Application.streamingAssetsPath, $"{saveName}.hssh")))
        {
            T data = new T();
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream stream = new FileStream(Path.Combine(Application.streamingAssetsPath, $"{saveName}.hssh"), FileMode.Open))
            {
                data = (T)serializer.Deserialize(stream);
            }
            return data;
        }
        else
        {
            return default;
        }
    }
    public static T Load<T>(string saveName, string path) where T : new()
    {
        if (File.Exists(Path.Combine(path, saveName)))
        {
            T data = new T();
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream stream = new FileStream(Path.Combine(path, saveName), FileMode.Open))
            {
                data = (T)serializer.Deserialize(stream);
            }
            return data;
        }
        else
        {
            return default;
        }
    }

    public static void AppendToXml(AnnotationData anndata, string agentid)
    {
        SqlAnnotatedSentence sqlsent = new SqlAnnotatedSentence(anndata);
        string content = ConvertToXmlString(sqlsent);
        content = "\n" + content;
        File.AppendAllText(Path.Combine(Application.streamingAssetsPath, "AnnotationData/" + agentid), content);
    }
}
