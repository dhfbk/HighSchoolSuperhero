using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dot
{
    public static bool Calc(Player agent, GameObject target)
    {
        float dot = Vector3.Dot(agent.transform.forward, (target.transform.position - agent.transform.position).normalized);
        if (dot < 1f && dot > 0)
            return true;
        else
            return false;
    }
}

public class TeleportManager
{
    public delegate void DoAfterDissolve();
    private static GameObject anchor;
    public static bool done;
    public static void Teleport(Player agent, Vector3 dest, GameObject anchor, bool busy = false)
    {
        //Setup
        agent.GetComponent<Movement>().Busy = true;
        agent.cameraInterface.transitionCanvas.gameObject.SetActive(true);
        TeleportManager.anchor = anchor;
        agent.StartCoroutine(DissolveTeleport(agent, agent.cameraInterface.transitionCanvas.transform.GetChild(0).GetComponent<Image>(), dest, busy));
    }
    public static void Teleport(Player agent, Vector3 dest, bool busy = false)
    {
        //Setup
        agent.GetComponent<Movement>().Busy = true;
        agent.cameraInterface.transitionCanvas.gameObject.SetActive(true);
        anchor = null;
        agent.StartCoroutine(DissolveTeleport(agent, agent.cameraInterface.transitionCanvas.transform.GetChild(0).GetComponent<Image>(), dest, busy));
    }
    private static IEnumerator DissolveTeleport(Player agent, Image black, Vector3 dest, bool busy)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2f;
            black.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), t);
            yield return null;
        }
        agent.gameObject.transform.position = dest;
        if (anchor)
            agent.cameraInterface.cameraOrbit.SetCameraAnchor(anchor.transform.position, AnchorMode.Fixed, false);
        else
            agent.cameraInterface.cameraOrbit.SetCameraAnchor(false);
        agent.cameraInterface.gameObject.transform.position = dest + new Vector3(0, 1.5f, 0);
        t = 0;
        done = true;
        while (t < 1)
        {
            t += Time.deltaTime * 2;
            black.color = Color.Lerp(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), t);
            yield return null;
        }
        agent.GetComponent<Movement>().Busy = busy;
        agent.cameraInterface.transitionCanvas.gameObject.SetActive(false);
        done = false;
    }
    public static void Teleport(GameObject obj, Vector3 dest)
    {
        obj.transform.position = dest;
    }
}
public class TeleportDoor : MonoBehaviour, ITriggerable
{
    public GameObject dest;
    public GameObject phone;
    public Player Agent { get; set; }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (GetComponent<MouseOver>().on)
            {
                TeleportManager.Teleport(GetComponent<ITriggerable>().Agent, dest.transform.position, false);
                GetComponent<MouseOver>().on = false;
            }
        }
        else if (Input.GetKeyUp("e"))
        {
            if (Agent != null)
            {
                if (Dot.Calc(Agent, this.gameObject))
                {
                    if (phone == null)
                        TeleportManager.Teleport(Agent, dest.transform.position, false);
                    else
                        MessageUtility.SingleBoxedMessage(Agent, PlayerPrefs.GetString("Name"), ML.systemMessages.iShouldAnswer);
                }
            }
        }
    }

    public void TriggerOn(Player agent)
    {
        Agent = agent;
        HintUtility.ShowHint(agent, "E", ML.systemMessages.openDoor, 5);
    }

    public void TriggerOff()
    {
        Agent = null;
        HintUtility.HideHint();
    }
}
