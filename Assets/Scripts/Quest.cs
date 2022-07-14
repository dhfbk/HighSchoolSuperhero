using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataUtilities;
using TMPro;
public class Timer
{
    private int time;
    public int Time { get => time; }
    public Timer() { this.time = 60; }
    public Timer(int time) { this.time = time; }
    public IEnumerator Count(TextMeshProUGUI timerText)
    {
        timerText.text = this.time.ToString();
        while (this.time > 0)
        {
            yield return new WaitForSeconds(1);
            this.time -= 1;
            timerText.text = this.time.ToString();
        }
    }
}
public class QuestState
{
    public bool completed;
}
public class Quest : MonoBehaviour, ITriggerable
{
    public Player Agent { get; set; }
    private Player memAgent;
    public string name;
    private List<string> names;
    private bool initialized;
    private bool speaking;
    private bool skirt;
    private bool longhair;
    public QuestScriptableObject engQso;
    public QuestScriptableObject itaQso;
    public QuestScriptableObject qso;
    private bool instructionsGiven;
    public GameObject exclamationPoint;
    public GameObject[] graffiti;
    private bool isActive;
    private Timer timer;
    public bool questCompleted;
    public int timerDefault = 10;
    public QuestState state;
    void Start()
    {
        foreach (GameObject obj in graffiti)
            obj.SetActive(false);
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
                name = LoadUtility.Nomif[Random.Range(0, LoadUtility.Nomif.Count)];
            }
            else
            {
                if (longhair)
                {
                    if (Random.Range(0, 5) > 3)
                        name = LoadUtility.Nomi[Random.Range(0, LoadUtility.Nomi.Count)];
                    else
                        name = LoadUtility.Nomif[Random.Range(0, LoadUtility.Nomif.Count)];
                }
                else
                {
                    if (Random.Range(0, 5) > 3)
                        name = LoadUtility.Nomif[Random.Range(0, LoadUtility.Nomif.Count)];
                    else
                        name = LoadUtility.Nomi[Random.Range(0, LoadUtility.Nomi.Count)];
                }
            }
            initialized = true;
        }
        if (Input.GetKeyUp("e") && !MessageUtility.Speaking)
        {
            if (Agent != null)
            {
                if (Player.language == ML.Lang.en)
                    qso = engQso;
                else if (Player.language == ML.Lang.it)
                    qso = itaQso;
                float dot = Vector3.Dot(Agent.transform.forward, (this.transform.position - Agent.transform.position).normalized);
                if (dot < 1f && dot > 0)
                {
                    if (memAgent == null || memAgent == Agent)
                    {
                        transform.LookAt(new Vector3(Agent.transform.position.x, transform.position.y, Agent.transform.position.z));
                        if (!questCompleted && !isActive)
                        {
                            MessageUtility.BoxedMessageSeriesStart(Agent, this.name, qso.instructions);
                            instructionsGiven = true;
                            memAgent = Agent;
                        }
                        else if (!questCompleted && isActive)
                        {
                            if (CheckNotSatisfied(graffiti) > 0)
                            {
                                if (Player.language == ML.Lang.en)
                                    MessageUtility.SingleBoxedMessage(Agent, this.name, "Hurry up, you still have time!");
                                else if (Player.language == ML.Lang.it)
                                    MessageUtility.SingleBoxedMessage(Agent, this.name, "Hai ancora un po' di tempo!");
                            }
                            else
                            {
                                questCompleted = true;
                                SafetyBar.AddSafety(100);
                                StopTask();
                                isActive = false;
                                MessageUtility.BoxedMessageSeriesStart(Agent, this.name, qso.rewardMessage);
                                GetComponent<SpawnCrystals>().Spawn(Agent);
                                SaveManager.saveDelegate += Save;
                            }
                        }
                        else if (questCompleted)
                        {
                            MessageUtility.SingleBoxedMessage(Agent, this.name, qso.rewardMessage[0]);
                        }
                    }
                    else
                    {
                        MessageUtility.SingleBoxedMessage(Agent, this.name, "Sorry, I'm busy with someone else!");
                    }
                }
            }
        }

        if (instructionsGiven && !MessageUtility.Speaking && !isActive)
        {
            foreach (GameObject obj in graffiti)
                obj.SetActive(true);
            timer = new Timer(timerDefault);
            StartTask(graffiti, timer);
        }

        //if (isActive)
        //{
        //    if (timer.Time <= 0)
        //    {
        //        StopTask();
        //    }
        //}

    }
    private void Save()
    {

    }
    private void StartTask(GameObject[] graffiti, Timer timer)
    {
        exclamationPoint.SetActive(false);
        isActive = true;
        foreach (GameObject obj in graffiti)
        {
            GameObject exc = Instantiate(Resources.Load<GameObject>("ExclamationIconObject"));
            exc.transform.SetParent(obj.transform);
            obj.GetComponent<Graffiti>().exclamationIcon = exc;
            exc.transform.position = obj.transform.position;
        }
        //StartTimer(timer);
    }

    private void StartTimer(Timer timer)
    {
        Agent.cameraInterface.timerText.transform.parent.gameObject.SetActive(true);
        StartCoroutine(timer.Count(Agent.cameraInterface.timerText));
    }

    private void StopTask()
    {
        foreach (GameObject o in graffiti)
        {
            Graffiti g = o.GetComponent<Graffiti>();
            if (g.Agent && g.erasing)
                g.StopAnnotation(g.Agent);
            Destroy(o.GetComponent<Graffiti>().exclamationIcon);
            o.SetActive(false);
        }
        isActive = false;
        instructionsGiven = false;
        memAgent.cameraInterface.timerText.transform.parent.gameObject.SetActive(false);
    }

    private int CheckNotSatisfied(GameObject[] graffiti)
    {
        int notSatisfied = 0;
        foreach (GameObject obj in graffiti)
        {
            if (!obj.GetComponent<Graffiti>().QuestSatisfied)
                notSatisfied += 1;
        }
        return notSatisfied;
    }

    public void TriggerOn(Player agent)
    {
        this.Agent = agent;
    }

    public void TriggerOff()
    {
        this.Agent = null;
    }
}
