using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseOver : MonoBehaviour
{
    Renderer r;
    float originalOutline;
    public bool on;
    Component interact;
    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Renderer>();
    }

    private void OnMouseOver()
    {
        Player agent;
        if ((agent = GetComponent<ITriggerable>().Agent) != null)
        {
            if (Vector3.Distance(transform.position, agent.transform.position) < 3)
            {
                agent.GetComponent<Movement>().MouseFrozen = true;
                on = true;
                for (int i = 0; i < r.materials.Length; i++)
                {
                    r.materials[i].SetColor("_OutlineColor", new Color(1, 0.4F, 0F, 1));
                }
            }
        }
    }
    private void OnMouseExit()
    {
        Player agent;
        if ((agent = GetComponent<ITriggerable>().Agent) != null)
        {
            agent.GetComponent<Movement>().MouseFrozen = false;
            on = false;
            for (int i = 0; i < r.materials.Length; i++)
            {
                r.materials[i].SetColor("_OutlineColor", new Color(0, 0, 0, 1));
            }
        }
    }
}
