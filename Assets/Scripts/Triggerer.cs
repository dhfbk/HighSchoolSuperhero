using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triggerer : MonoBehaviour
{
    public bool overrideInteraction;
    public GameObject triggerableObject;
    ITriggerable triggerScript;

    void Start()
    {
        transform.gameObject.layer = 2;
        if (triggerableObject)
            triggerScript = triggerableObject.GetComponent<ITriggerable>();
        else
            triggerScript = transform.parent.GetComponent<ITriggerable>();

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            Player player = col.GetComponentInChildren<Player>();
            //if (player.inTriggerArea == 0)
            //{
                triggerScript.TriggerOn(player);
            //}
        }
        else if (col.CompareTag("GoalArea"))
            RewardUtility.DisplayReward(transform.parent.GetComponent<Ball>().memAgent, transform.parent.GetComponent<Ball>().rewardInfo);
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            triggerScript.TriggerOff();
        }
    }
}
