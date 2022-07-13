using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class NotificationException : Exception
{
    public NotificationException() { }
    public NotificationException(string message) : base(message) { }
}
public class NotificationUtility : MonoBehaviour
{
    public Player player;
    public GameObject notBG;
    public AudioClip notificationSound;
    public static AudioClip notClip;
    
    static float t;
    static Vector2 startPos;
    static Vector2 destPos;

    static Vector2 showPos;
    static Vector2 hidePos;

    static Vector2 initialPos;
    public static bool Showing { get; set; }

    private void Start()
    {
        notClip = notificationSound;
        //initialPos = Control.Instance.notBG.transform.localPosition;
        //showPos = Control.Instance.notBG.transform.localPosition;
        initialPos = showPos = transform.localPosition;
        hidePos = showPos + new Vector2(0, 60);
        Hide(player);
        PhoneUtility.phoneUp += MoveUp;
        PhoneUtility.phoneDown += MoveDown;
    }
    public static string ShowString(Player agent, string text) //may not work properly in multiplayer yet
    {
        t = 0;
        if (!agent || agent.cameraInterface.notText == null)
            return "Error: notification object not set";
        agent.cameraInterface.notText.text = text;
        destPos = showPos;
        Showing = true;
        agent.cameraInterface.notBG.GetComponent<NotificationUtility>().StopAllCoroutines();
        agent.cameraInterface.notBG.GetComponent<NotificationUtility>().StartCoroutine(Move(agent, true));
        return "Done";
    }
    public static string ShowString(Player agent, string text, bool hide) //may not work properly in multiplayer yet
    {
        t = 0;
        if (agent.cameraInterface.notText == null)
            return "Error: notification object not set";
        agent.cameraInterface.notText.text = text;
        destPos = showPos;
        Showing = true;
        agent.cameraInterface.notBG.GetComponent<NotificationUtility>().StopAllCoroutines();
        agent.cameraInterface.notBG.GetComponent<NotificationUtility>().StartCoroutine(Move(agent, hide));
        return "Done";
    }

    public static void Hide(Player agent)
    {
        t = 0;
        destPos = hidePos;
        Showing = false;
        agent.cameraInterface.notBG.GetComponent<NotificationUtility>().StopAllCoroutines();
        agent.cameraInterface.notBG.GetComponent<NotificationUtility>().StartCoroutine(Move(agent, false));
    }

    public static bool IsShowing()
    {
        return Showing;
    }

    public static void MoveUp(Player agent)
    {
        showPos = initialPos + new Vector2(0, 90);
        hidePos = showPos + new Vector2(0, 60);
        if (Showing)
        {
            agent.cameraInterface.notBG.transform.localPosition = showPos;
            destPos = showPos;
        }
        else
        {
            agent.cameraInterface.notBG.transform.localPosition = hidePos;
            destPos = hidePos;
        }
    }
    public static void MoveDown(Player agent)
    {
        showPos = initialPos;
        hidePos = showPos + new Vector2(0, 60);
        if (Showing)
        {
            agent.cameraInterface.notBG.transform.localPosition = showPos;
            destPos = showPos;
        }
        else
        {
            agent.cameraInterface.notBG.transform.localPosition = hidePos;
            destPos = hidePos;
        }
    }

    private static IEnumerator Move(Player agent, bool hide)
    {
        if (Showing)
        {
            agent.cameraInterface.FX.clip = notClip;
            agent.cameraInterface.FX.pitch = 1;
            agent.cameraInterface.FX.Play();
            while (agent.cameraInterface.notBG.transform.localPosition.y >= destPos.y + 0.1f)
            {
                agent.cameraInterface.notBG.transform.localPosition = Vector3.Lerp(agent.cameraInterface.notBG.transform.localPosition, destPos, Time.deltaTime * 5);
                yield return null;
            }
            if (hide)
            {
                yield return new WaitForSeconds(3);
                Hide(agent);
            }
        }
        else if (!Showing)
        {
            while (agent.cameraInterface.notBG.transform.localPosition.y <= destPos.y - 0.1f)
            {
                agent.cameraInterface.notBG.transform.localPosition = Vector3.Lerp(agent.cameraInterface.notBG.transform.localPosition, destPos, Time.deltaTime * 5);
                yield return null;
            }
        }

    }
}
