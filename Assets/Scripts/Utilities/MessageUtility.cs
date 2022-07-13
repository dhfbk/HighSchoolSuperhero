using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DataUtilities;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;


public class MessageUtility : MonoBehaviour
{
    public static bool think;
    public LetterByLetter byLetterInstance;
    public static LetterByLetter byLetter;
    private static bool speaking;
    [SerializeField]
    public static bool Speaking { get => speaking; set { speaking = value; } }
    public bool Setup { get; set; }
    public static bool OverrideBusy { get; set; }
    public static bool BoxFinished { get => byLetter.finished; }
    // Start is called before the first frame update
    void Start()
    {
        think = true;
    }

    private void Update()
    {
        if (LoadUtility.AllLoaded && !Setup)
        {
            byLetter = byLetterInstance;
            Setup = true;
        }
    }
    public static void ThinkCloud(Player agent, string role, GameObject obj)
    {
        //Time.timeScale = 0;
        QualityTools.StartTimer();
        //Activates the cloud and starts the dialogue sequence
        if (obj.GetComponent<ChattingAnimation>())
            obj.GetComponent<ChattingAnimation>().enabled = false;
        agent.cameraInterface.thinkCloud.SetActive(true);

        //make sure tokens are destroyed
        DestroyTokens(agent);

        CloseBoxedMessage(agent);
        agent.cameraInterface.nameCloud.SetActive(true);

        if (!String.IsNullOrEmpty(API.sentence) || API.currentSentence.tokens.Length > 0)
        {
            agent.GetComponent<Movement>().Busy = true; //
            agent.cameraInterface.byWord.CallText(role);
            think = false;
        }
        else
        {
            agent.cameraInterface.thinkCloud.SetActive(false);
            agent.cameraInterface.nameCloud.SetActive(false);
            agent.GetComponent<Movement>().Busy = false;
        }
    }

    public static List<GameObject> FindTokens()
    {
        List<GameObject> tokens = new List<GameObject>();
        List<GameObject> lines = GameObject.FindGameObjectsWithTag("Line").ToList();
        if (lines.Count > 0)
        {
            foreach (GameObject line in lines)
            {
                foreach (Transform t in line.transform)
                {
                    tokens.Add(t.gameObject);
                }
            }
        }
        return tokens;
    }

    private static void BoxedMessageSingle(Player agent, string aut, string message, bool single)
    {
        agent.cameraInterface.eButtonHigh.SetActive(true);
        agent.GetComponent<Movement>().Busy = true;
        agent.GetComponent<Rigidbody>().velocity = Vector3.zero;
        DestroyTokens(agent);
        agent.cameraInterface.thinkCloud.SetActive(false);
        agent.cameraInterface.msgBox.SetActive(true);
        byLetter.CallText(message, aut);
        speaking = true;
        if (single)
        {
            agent.StopAllCoroutines();
            agent.StartCoroutine(WaitForKeyAndClose(agent));
        }
        else
        {
            agent.StopAllCoroutines();
            agent.StartCoroutine(WaitForFinish(agent));
        }
    }
    private static void BoxedMessageForSeries(Player agent, string aut, string message)
    {
        agent.cameraInterface.eButtonHigh.SetActive(true);
        agent.GetComponent<Movement>().Busy = true;
        agent.GetComponent<Rigidbody>().velocity = Vector3.zero;
        DestroyTokens(agent);
        agent.cameraInterface.thinkCloud.SetActive(false);
        agent.cameraInterface.msgBox.SetActive(true);
        byLetter.CallText(message, aut);
        speaking = true;
    }
    private static bool GetMessageInputFinish()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp("e"))
        {
            if (!byLetter.finished)
            {
                byLetter.Finish();
                return false;
            }
            else
            {
                return true;
            }
        }
        else
            return false;
    }
    private static bool GetMessageInput()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp("e"))
        {
            return true;
        }
        else
            return false;
    }

    private static IEnumerator BoxedMessageSeries(Player agent, List<string> lines)
    {
        if (lines.Count <= 0)
            print("No strings found!");
        speaking = true;

        string aut, mess;
        aut = lines[0].Split('#')[0];
        mess = lines[0].Split('#')[1];
        MessageUtility.BoxedMessageForSeries(agent, aut, mess);

        for (int i = 1; i < lines.Count;)
        {
            aut = lines[i].Split('#')[0];
            mess = lines[i].Split('#')[1];
            if (aut == "NAME")
            {
                aut = PlayerPrefs.GetString("Name");
            }
            if (GetMessageInputFinish())
            {
                i++;
                if (i == lines.Count)
                    break;
                MessageUtility.BoxedMessageForSeries(agent, aut, mess);
            }
            yield return null;
        }
        CloseBoxedMessage(agent);
    }
    public static void SingleBoxedMessageStay(Player agent, string aut, string line)
    {
        agent.cameraInterface.eButtonHigh.SetActive(true);
        MessageUtility.CloseBoxedMessage(agent);
            BoxedMessageSingle(agent, aut, line, false);
    }
    public static void SingleBoxedMessage(Player agent, string aut, string line)
    {
        agent.cameraInterface.eButtonHigh.SetActive(true);
        if (!Speaking)
            BoxedMessageSingle(agent, aut, line, true);
    }
    public static void BoxedMessageSeriesStart(Player agent, string aut, List<string> lines)
    {
        agent.cameraInterface.eButtonHigh.SetActive(true);
        if (!Speaking)
            agent.StartCoroutine(BoxedMessageSeries(agent, aut, lines));
    }
    //public static void BoxedMessageSeriesStart(Player agent, List<string> lines)
    //{
    //    agent.cameraInterface.eButtonHigh.SetActive(true);
    //    if (!Speaking)
    //        agent.StartCoroutine(BoxedMessageSeries(agent, lines));
    //}
    private static IEnumerator BoxedMessageSeries(Player agent, string aut, List<string> lines)
    {
        if (lines.Count <= 0)
            print("No strings found!");
        speaking = true;

        MessageUtility.BoxedMessageForSeries(agent, aut, lines[0]);
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < lines.Count;)
        {
            if (GetMessageInputFinish())
            {
                i++;
                if (i == lines.Count)
                    break;
                MessageUtility.BoxedMessageForSeries(agent, aut, lines[i]);
            }
            yield return null;
        }
        CloseBoxedMessage(agent);
    }
    private static IEnumerator WaitForFinish(Player agent)
    {
        yield return new WaitForSeconds(0.1f);
        bool inputGiven = false;
        while (!inputGiven)
        {
            if (GetMessageInput())
            {
                if (!byLetter.finished)
                {
                    byLetter.Finish();
                    inputGiven = true;
                }
            }
            yield return null;
        }
    }

    private static IEnumerator WaitForKeyAndClose(Player agent)
    {
        yield return new WaitForSeconds(0.1f);
        bool inputGiven = false;
        while (!inputGiven)
        {
            if (GetMessageInput())
            {
                if (!byLetter.finished)
                {
                    byLetter.Finish();
                }
                else
                {
                    CloseBoxedMessage(agent);
                    inputGiven = true;
                }
            }
            yield return null;
        }
    }

    public static void CloseBoxedMessage(Player agent)
    {
        agent.cameraInterface.eButtonHigh.SetActive(false);
        speaking = false;
        byLetter.finished = false;
        if (!OverrideBusy)
            agent.GetComponent<Movement>().Busy = false;
        byLetter.gameObject.SetActive(false);
    }

    public static void DestroyTokens(Player agent)
    {
        //Time.timeScale = 1;
        List<GameObject> lines = GameObject.FindGameObjectsWithTag("Line").ToList();
        if (lines.Count > 0)
        {
            foreach (GameObject line in lines)
            {
                Destroy(line);
            }
            //GameObject.FindGameObjectWithTag("InputField").SetActive(false);
            agent.cameraInterface.nameCloud.SetActive(false);
            Canvas.ForceUpdateCanvases();
            QualityTools.StopTimer();
        }
    }

    public static void ResetMessages(Player agent, GameObject obj)
    {
        DestroyTokens(agent);
        agent.cameraInterface.eButtonHigh.SetActive(false);
        agent.cameraInterface.thinkCloud.SetActive(false);
        agent.cameraInterface.msgBox.SetActive(false);
        if (obj.GetComponent<ChattingAnimation>())
            obj.GetComponent<ChattingAnimation>().enabled = true;
    }
}
