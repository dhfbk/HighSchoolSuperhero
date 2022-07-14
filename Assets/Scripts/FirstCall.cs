using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DataUtilities;

public class FirstCall : MonoBehaviour, ITriggerable, ISaveable
{
    public Player Agent { get; set; }
    public string State { get; set; }
    List<string> lines;
    TextAsset dialogue;
    private bool answered;
    public DialogueScriptableObject phone1;

    void Update()
    {
        if (State == "Taken")
        {
            Destroy(this.gameObject);
        }
        else if (Agent && !MessageUtility.Speaking)
        {
            if (!answered)
            {
                if ((GetComponent<MouseOver>().on == true && Input.GetMouseButtonDown(0)) || Input.GetKeyUp("e"))
                {
                    GetComponent<Vibration>().enabled = false;

                    MessageUtility.BoxedMessageSeriesStart(Agent, "Professor", API.systemDialogues.scientist.dialogueLines[0].lines);
                    answered = true;
                }
            }
            //else
            //{
            //    if (GetComponent<MouseOver>().on == true)
            //    {
            //        MessageUtility.SingleBoxedMessage(Agent, PlayerPrefs.GetString("Name"), "I should go meet the professor.");
            //    }
            //}
        }
        if (answered && !MessageUtility.Speaking)
        {
            Agent.GetComponent<Player>().cameraInterface.phone.SetActive(true);
            State = "Taken";
            Agent.gameState.saveableObjects.Add(new ObjectState(this.gameObject, false));
            Destroy(this.gameObject);
        }
    }
    public void TriggerOn(Player agent)
    {
        this.Agent = agent;
        HintUtility.ShowHint(agent, "E", ML.systemMessages.answerThePhone, 5);
    }
    public void TriggerOff()
    {
        this.Agent = null;
        HintUtility.HideHint();
    }
}
