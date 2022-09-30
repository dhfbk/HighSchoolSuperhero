using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCrystal : MonoBehaviour, ISaveable, ITriggerable
{
    public Player Agent { get; set; }
    public string State { get; set; } = "available";
    Coroutine lastcoroutine;
    public RewardInfo rewardInfo;
    [SerializeField]
    private float speed;
    public float Speed { get=>speed; set => speed = value; }
    public bool FromChest { get; set; }
    private bool visible;
    float t;
    Vector3 startPos;
    Vector3 endPos;
    bool collecting;
    int dir;
    private void Start()
    {
        dir = 1;
        startPos = transform.position;
        endPos = transform.position + new Vector3(0, 0.5f, 0);

        //name = transform.name + transform.position.x + transform.position.z;

        //if (State == "taken")
        //    Destroy(this.gameObject);
        //if (!FromChest)
        //    lastcoroutine = StartCoroutine(Animate());
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            if (lastcoroutine != null)
                StopCoroutine(lastcoroutine);
            Collect(other.GetComponentInChildren<Player>());
        }
    }

    public void Collect(Player agent)
    {        //StopAllCoroutines();
        collecting = true;
        StartCoroutine(CollectOverFrames(agent));
        RewardUtility.DisplayReward(agent, rewardInfo, 0.6f);

        ObjectState s = new ObjectState(this.gameObject, false);
        s.active = false;

        //agent.gameState.saveableObjects.Add(s);
    }

    IEnumerator CollectOverFrames(Player agent)
    {
        while (Vector2.Distance(transform.position, agent.transform.position) > 0.2f)
        {
            transform.position = Vector3.MoveTowards(transform.position, agent.transform.position, Time.deltaTime*Speed);
            yield return null;
        }
        agent.AddCrystals(rewardInfo.amount);
        Destroy(this.gameObject);
    }

    //private void Update()
    //{
    //    if (visible && !FromChest && !collecting)
    //    {
    //        if (t < 1.0f)
    //        {
    //            transform.position = Vector3.Lerp(startPos, endPos, t);
    //            t += Time.deltaTime;
    //        }
    //        else
    //        {
    //            startPos = transform.position;
    //            t = 0;
    //            dir *= -1;

    //            if (dir == 1)
    //                endPos = new Vector3(transform.position.x, starty + 0.5f, transform.position.z);
    //            else
    //                endPos = new Vector3(transform.position.x, starty, transform.position.z);
    //        }
    //    }
    //}

    public void Update()
    {
        if (visible && !collecting)
        {
            if (dir == 1)
            {
                if (transform.position.y < endPos.y)
                    transform.position = Vector3.MoveTowards(transform.position, endPos, Time.deltaTime);
                else
                    dir = 0;
            }
            else if (dir == 0)
            {
                if (transform.position.y > startPos.y)
                    transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime);
                else
                    dir = 1;
            }
        }
    }

    //IEnumerator Animate()
    //{
    //    if (visible)
    //    {
    //        float range = 0.5f;
    //        Vector3 endPos = transform.position + new Vector3(0, range, 0);
    //        float random = Random.Range(0.0f, range - 0.1f);
    //        transform.position += new Vector3(0, random);
    //        Vector3 startPos = transform.position;
    //        float t = random;
    //        int dir = 1;
    //        while (true)
    //        {
    //            while (t < 1.0f)
    //            {
    //                transform.position = Vector3.Lerp(startPos, endPos, t);
    //                t += Time.deltaTime;
    //                yield return null;
    //            }
    //            t = 0;
    //            dir *= -1;
    //            startPos = transform.position;
    //            endPos = transform.position + new Vector3(0, range, 0) * dir;
    //            yield return null;
    //        }
    //    }
    //}

    public void TriggerOn(Player agent)
    {      
        Agent = agent;
        visible = true;
        //StartCoroutine(Animate());
    }

    public void TriggerOff()
    {
        Agent = null;
        visible = false;
        //StopAllCoroutines();
    }
}
