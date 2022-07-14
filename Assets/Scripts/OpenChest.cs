using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class ChestState
{
    public string saveName;
    public bool spawned;

    public ChestState() { }
    public ChestState(string name) { saveName = name; spawned = false; }
}
public class OpenChest : MonoBehaviour, ITriggerable, ISaveable
{
    public Player Agent { get; set; }

    public Quaternion openRot;
    private Quaternion closeRot;
    private bool open;

    private ChestState state;
    public string State { get; set; }
    static int chestIndex;
    // Start is called before the first frame update
    void Start()
    {
        chestIndex += 1;
        closeRot = transform.localRotation;
        State = "full";
        this.name = transform.parent.name + "Lid";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GetComponent<MouseOver>().on)
            {
                Open();
            }
        }
        if (Input.GetKeyDown("e"))
        {
            if (Agent != null)
            {
                float dot = Vector3.Dot(Agent.transform.forward, (this.transform.position - Agent.transform.position).normalized);
                if (Agent != null && dot < 1f && dot > 0)
                {
                    Open();
                }
            }
        }
    }

    void Open()
    {
        if (!open)
        {
            open = true;
            StopAllCoroutines();
            StartCoroutine(AnimationUtilities.RotateLinear(true, this.gameObject, transform.localRotation, openRot, 2, 0));
            if (State == "full")
            {
                SpawnTreasure();
            }
        }
        else
        {
            open = false;
            StopAllCoroutines();
            StartCoroutine(AnimationUtilities.RotateLinear(true, this.gameObject, transform.localRotation, closeRot, 2, 0));
        }
    }

    void SpawnTreasure()
    {
        GetComponent<SpawnCrystals>().Spawn(Agent);
        Save();
    }

    public void Save()
    {
        State = "empty";
        Agent.gameState.saveableObjects.Add(new ObjectState(this.gameObject, false));
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
