using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataUtilities;
using UnityEngine.Networking;
using System;
using TMPro;
public class NPCInteraction : MonoBehaviour, ITriggerable
{
    public bool interactable;
    public Player Agent { get; set; }
    public string name;
    private List<string> names;
    private bool initialized;
    private bool speaking;
    private bool skirt;
    private bool longhair;
    private string sqlSentence;
    private bool thinking;
    private float agreement;
    bool reacted;
    private bool engaged;

    bool Restriction;
    string tasktype;
    void Start()
    {
    }
    void Update()
    {
        if (LoadUtility.AllLoaded && !initialized)
        {
            names = new List<string>();
            if (GetComponent<Parts>().pants.GetComponent<SkinnedMeshRenderer>().sharedMesh.name.Contains("Skirt"))
                skirt = true;

            if (GetComponent<Parts>().hair.GetComponent<SkinnedMeshRenderer>().sharedMesh.name.Contains("long"))
                longhair = true;

            if (skirt)
            {
                name = LoadUtility.Nomif[UnityEngine.Random.Range(0, LoadUtility.Nomif.Count)];
            }
            else
            {
                if (longhair)
                {
                    if (UnityEngine.Random.Range(0, 5) > 3)
                        name = LoadUtility.Nomi[UnityEngine.Random.Range(0, LoadUtility.Nomi.Count)];
                    else
                        name = LoadUtility.Nomif[UnityEngine.Random.Range(0, LoadUtility.Nomif.Count)];
                }
                else
                {
                    if (UnityEngine.Random.Range(0, 5) > 3)
                        name = LoadUtility.Nomif[UnityEngine.Random.Range(0, LoadUtility.Nomif.Count)];
                    else
                        name = LoadUtility.Nomi[UnityEngine.Random.Range(0, LoadUtility.Nomi.Count)];
                }
            }
            initialized = true;
        }
        if (Agent && DialogueInstancer.deactivateDialoguesAndGraffiti == false)
        {
            if (Input.GetKeyUp("e") && interactable)
            {
                if (Agent) //&& Agent.overrideInteraction == 0)
                {
                    if (!thinking)
                    {
                        if (!Agent.GetComponent<Movement>().Busy)
                        {
                            if (Dot.Calc(Agent, this.gameObject))
                            {
                                if (Agent.interactingNPC == null)
                                {
                                    Stop();
                                    DialogueInstancer.inDialogue = true;
                                    StartCoroutine(GetSentenceAndShowCloud(DialogueInstancer.uniqueLineIndex));
                                    thinking = true;
                                    Agent.interactingNPC = this.gameObject;
                                }
                                //StartCoroutine(WaitForEndOfMessageAndGo());
                            }
                        }
                        else
                        {
                            if (MouseOverToken.Changed)
                            {
                                if (MessageUtility.BoxFinished && !reacted)
                                {
                                    ShowReaction();
                                }
                                else if (MessageUtility.BoxFinished && reacted)
                                {
                                    ExitDialogue();
                                }
                            }
                        }
                    }
                    else
                    {
                        //StartCoroutine(Annotate());
                        if (!MouseOverToken.Changed)
                            ExitDialogue();
                    }
                }
            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (Agent && Agent.interactingNPC == this.gameObject)
                {
                    if (thinking)
                    {
                        if (!GameObject.Find("IfBG"))
                            ExitDialogue();
                        else
                            DeactivateIf();
                    }
                }
            }

            if ((Input.GetKeyUp(KeyCode.RightArrow) || (Input.GetKeyDown(KeyCode.Return) && !GameObject.Find("IfBG"))) && interactable)
            {
                if (Agent && Agent.interactingNPC == this.gameObject)
                {
                    if (thinking)
                    {
                        float agreement = 0;
                        StartCoroutine(DialogueInstancer.Annotate(Agent, tasktype, this));
                        //AnnotationData anndata = Annotate(out agreement);
                        List<string> messageTokens = Annotation.GetNewTokens(tokens: MessageUtility.FindTokens());
                        string joiner = Player.admin == true ? "" : " ";
                        MessageUtility.SingleBoxedMessageStay(Agent, this.name, string.Join(joiner, messageTokens));
                        thinking = false;
                        Agent.cameraInterface.eButton.SetActive(false);
                        Agent.cameraInterface.mouseButton.SetActive(false);
                        Agent.cameraInterface.enterButton.SetActive(false);
                    }
                    else
                    {
                        if (MouseOverToken.Changed)
                        {
                            if (MessageUtility.BoxFinished && !reacted)
                            {
                                ShowReaction();
                            }
                            else if (MessageUtility.BoxFinished && reacted)
                            {
                                ExitDialogue();
                            }
                        }
                        else if (MessageUtility.BoxFinished)
                        {
                            ExitDialogue();
                        }
                    }
                }
            }
        }
        else
        {
            if (engaged)
                ExitDialogue();
        }
    }
    void DeactivateIf()
    {
        GameObject ifo = GameObject.Find("IfBG");
        if (ifo)
        {
            ifo.GetComponentInChildren<TMP_InputField>().text = "";
            ifo.SetActive(false);
        }
        Agent.cameraInterface.mouseButton.SetActive(true);
        Agent.cameraInterface.enterButton.SetActive(false);
    }
    private void ShowReaction()
    {
        StartCoroutine(Jump(this.gameObject));
        Reactions.Create(emitter: this.gameObject, type: "Exclamation");
        MessageUtility.SingleBoxedMessageStay(Agent, this.name, ML.systemMessages.didntMeanToSayThis);
        reacted = true;
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
    private void ExitDialogue()
    {
        DialogueInstancer.inDialogue = false;
        DeactivateIf();
        Agent.cameraInterface.enterButton.SetActive(false);
        Agent.cameraInterface.eButton.SetActive(false);
        Agent.cameraInterface.mouseButton.SetActive(false);
        engaged = false;
        Agent.playerLogger.StopDialogueAnnotationSW();
        Agent.playerLogger.StopSentenceAnnotationSW();
        //if (!Restriction)
        //    Agent.cameraInterface.battery.GetComponent<Rainbow>().StopRainbow();
        if (thinking || MessageUtility.Speaking)
            Player.overlays -= 1;
        MessageUtility.OverrideBusy = false;
        MessageUtility.ResetMessages(Agent, this.gameObject);
        Agent.GetComponent<Movement>().Busy = false;
        //talking = false;
        //toReset = false;
        PhoneUtility.Down(Agent);
        Go();
        thinking = false;
        reacted = false;
        Agent.interactingNPC = null;
    }

    public IEnumerator GetSentenceAndShowCloud(int i)
    {

        engaged = true;
        //WebDebug.Print("Retrieveing sql sentence...");

        if (API.currentApi == Api.dev)
        {
            WWWForm form = new WWWForm();
            form.AddField("ID", i);

            //WebDebug.Print("Form created...");
            using (UnityWebRequest www = UnityWebRequest.Post(API.urls.getSentence, form))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {

                    NotificationUtility.ShowString(Agent, www.error);
                    //WebDebug.Print(www.error);
                }
                else
                {
                    if (!String.IsNullOrEmpty(www.downloadHandler.text))
                    {

                        sqlSentence = www.downloadHandler.text;
                        MessageUtility.ThinkCloud(Agent, this.name, this.gameObject);
                        MouseOverToken.Changed = false;
                        thinking = true;
                        Player.overlays += 1;
                        Agent.cameraInterface.enterButton.SetActive(true);
                        Agent.cameraInterface.mouseButton.SetActive(true);
                        Agent.cameraInterface.eButton.SetActive(true);
                        Agent.playerLogger.StartSentenceAnnotationSW();
                        //WebDebug.Print("Retrieved!");
                    }
                }
            }
        }
        else
        {
            yield return StartCoroutine(API.GetSentenceSingleC(Agent, DialogueInstancer.uniqueLineIndex, "ch"));
            MessageUtility.ThinkCloud(Agent, this.name, this.gameObject);
            MouseOverToken.Changed = false;
            thinking = true;
            Player.overlays += 1;
            Agent.cameraInterface.enterButton.SetActive(true);
            Agent.cameraInterface.mouseButton.SetActive(true);
            Agent.cameraInterface.eButton.SetActive(true);
            Agent.playerLogger.StartSentenceAnnotationSW();
        }
    }

    private void Stop()
    {
        GetComponent<NPCController>().enabled = false;
        GetComponent<Animator>().SetBool("Walk", false);
        transform.LookAt(new Vector3(Agent.transform.position.x, transform.position.y, Agent.transform.position.z));
    }
    private void Go()
    {
        transform.LookAt(GetComponent<NPCController>().dest.transform);
        GetComponent<NPCController>().enabled = true;
        GetComponent<Animator>().SetBool("Walk", true);
    }

    private IEnumerator WaitForEndOfMessageAndGo()
    {
        while (MessageUtility.Speaking)
            yield return null;
        Go();
    }

    public void ColorStudent()
    {
        GetComponent<CombineChildren>().Colorize();
    }

    public void TriggerOn(Player agent)
    {
        agent.cameraInterface.eButton.SetActive(false);
        agent.cameraInterface.enterButton.SetActive(false);
        agent.cameraInterface.mouseButton.SetActive(false);
        this.Agent = agent;
        NotificationUtility.ShowString(agent, $"Press {MultiplatformUtility.PrimaryInteractionKey} to talk!");

        Restriction = Player.rCondition == RCondition.NonRestricted ? false : true;
        tasktype = Restriction == true ? "NPC_R" : "NPC";
        if (Restriction)
            agent.currentlyRestricted = true;
        else
            agent.currentlyRestricted = false;
    }

    public void TriggerOff()
    {
        print("prova");
        ExitDialogue();
        this.Agent = null;
    }
}
