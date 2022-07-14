using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Linq;



public class ScientistInteraction : MonoBehaviour, ITriggerable, ISaveable
{
    public Player Agent { get; set; }
    public string Name { get; set; }
    public string State { get; set; }
    public Vector3 Position { get => transform.position; set => transform.position = value; }
    public Quaternion Rotation { get => transform.rotation; set => transform.rotation = value; }

    //public DialogyeScriptableObject 

    public DialogueScriptableObject beginSO;
    public DialogueScriptableObject showSO;
    public DialogueScriptableObject gonowSO;
    public DialogueScriptableObject instructionsR;
    public DialogueScriptableObject instructionsNonR;

    public List<string> begin;
    public List<string> show;
    public List<string> gonow;
    public List<string> instrR;
    public List<string> instrNR;

    public RewardInfo deviceRewardInfo;
    public GameObject teleportDestination;
    public GameObject teleportDestinationScientist;
    public GameObject TDScientistOut;
    public GameObject TDPlayerOut;
    public GameObject cameraPos;
    public GameObject triggerPanBullies;
    public GameObject exitTrigger;
    public GameObject firstDialogue;
    private bool teleport;
    private bool deviceGiven;
    private bool go;
    private bool scientistTeleported;
    private bool blockInteraction;
    public static bool scientistAutoStart;

    //Gender
    public GameObject hair;
    public GameObject glasses;
    public GameObject pants;

    public Mesh whair;
    public Mesh wglasses;
    public Mesh wpants;

    void Start()
    {
        

            //LoadLines();

        State = "begin";

        //Randomize gender
        if (UnityEngine.Random.Range(0,2) == 0)
        {
            Woman();
        }
    }

    void LoadLines()
    {

        begin = API.systemDialogues.scientist.dialogueLines[1].lines;
        show = API.systemDialogues.scientist.dialogueLines[2].lines;
        gonow = API.systemDialogues.scientist.dialogueLines[3].lines;
        
    }

    void Woman()
    {
        hair.GetComponent<SkinnedMeshRenderer>().sharedMesh = whair;
        glasses.GetComponent<SkinnedMeshRenderer>().sharedMesh = wglasses;
        pants.GetComponent<SkinnedMeshRenderer>().sharedMesh = wpants;
    }

    void Update()
    {
        if (State == "begin")
        {

            if ((Input.GetKeyUp("e") && !MessageUtility.Speaking && !blockInteraction) || scientistAutoStart)
            {
                if (Agent)
                {
                    if (Dot.Calc(Agent, this.gameObject) || scientistAutoStart)
                    {
                        scientistAutoStart = false;
                        transform.LookAt(new Vector3(Agent.transform.position.x, transform.position.y, Agent.transform.position.z));
                        MessageUtility.BoxedMessageSeriesStart(Agent, "Professor", begin);
                        teleport = true;
                    }
                }
            }
            if (teleport == true && !MessageUtility.Speaking)
            {
                blockInteraction = true;
                if (Agent)
                {
                    TeleportManager.Teleport(Agent, teleportDestination.transform.position, cameraPos);
                }
                if (TeleportManager.done)
                {
                    if (!scientistTeleported)
                    {
                        TeleportManager.Teleport(this.gameObject, teleportDestinationScientist.transform.position);
                        transform.LookAt(new Vector3(teleportDestination.transform.position.x, transform.position.y, teleportDestination.transform.position.z));
                        scientistTeleported = true;
                        State = "show";
                        teleport = false;
                        SaveState();
                    }
                }
            }           
        }
        else if (State == "show")
        {
            SetPosition();
            if (Input.GetKeyUp("e") && !MessageUtility.Speaking)
            {
                if (Agent)
                {
                    if (Dot.Calc(Agent, this.gameObject))
                    {
                        transform.LookAt(new Vector3(Agent.transform.position.x, transform.position.y, Agent.transform.position.z));
                        
                        MessageUtility.BoxedMessageSeriesStart(Agent, "Professor", show);
                        deviceGiven = true;
                    }
                }
            }
            if (Agent)
            {
                if (deviceGiven == true && !MessageUtility.Speaking)
                {
                    RewardUtility.DisplayReward(Agent, deviceRewardInfo);
                    State = "given";
                    SaveState();
                    triggerPanBullies.SetActive(true);
                    exitTrigger.SetActive(true);
                    firstDialogue.SetActive(true);
                    if (Player.rCondition == RCondition.Restricted)
                        Agent.ShowBattery();
                    SafetyBar.TimeAttack = true;
                }
            }
        }
        else if (State == "given")
        {
            SetPosition();
            if (Input.GetKeyUp("e") && !MessageUtility.Speaking)
            {
                if (Agent)
                {
                    if (Dot.Calc(Agent, this.gameObject))
                    {
                        transform.LookAt(new Vector3(Agent.transform.position.x, transform.position.y, Agent.transform.position.z));
                        
                        MessageUtility.BoxedMessageSeriesStart(Agent, "Professor", gonow);
                        //go = true;
                    }
                }
            }
            if (Agent)
            {
                if (go && !MessageUtility.Speaking)
                {
                    State = "finished";
                    SaveState();
                    this.enabled = false;
                }
            }
        }
    }

    public void SetPosition()
    {
        if (!scientistTeleported)
        {
            TeleportManager.Teleport(this.gameObject, teleportDestinationScientist.transform.position);
            transform.LookAt(new Vector3(teleportDestination.transform.position.x, transform.position.y, teleportDestination.transform.position.z));
            scientistTeleported = true;
        }

        if (State == "given" || State == "finished")
        {
            exitTrigger.SetActive(true);
        }
    }

    public void SaveState()
    {
        SaveManager.SaveState(Agent, new ObjectState(this.gameObject, true));
        //Agent.gameState.saveableObjects.Add(os);
        print(JsonUtility.ToJson(Agent.gameState));
    }
    public void TriggerOn(Player agent)
    {
        LoadLines();
        this.Agent = agent;
        NotificationUtility.ShowString(agent, ML.systemMessages.pressToTalk);
        if (Player.rCondition == RCondition.Restricted)
            showSO = instructionsR;
        else
            showSO = instructionsNonR;
    }

    public void TriggerOff()
    {
        this.Agent = null;
    }
}
