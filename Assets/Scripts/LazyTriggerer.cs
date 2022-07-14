using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazyTriggerer : MonoBehaviour, ITriggerable
{
    public Player Agent { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerOn(Player agent)
    {
        transform.parent.GetComponent<DialogueInstancer>().LazyActivate();
    }

    public void TriggerOff()
    {

    }
}
