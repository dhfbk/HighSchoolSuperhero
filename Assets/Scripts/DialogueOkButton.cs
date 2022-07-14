using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueOkButton : MonoBehaviour
{
    bool over;
    public void Update()
    {
        
    }
    public bool IsOver()
    {
        return over;
    }

    public void OnMouseEnter()
    {
        over = true;
    }
    public void OnMouseOver()
    {
        over = true;
    }

    public void OnMouseExit()
    {
        over = false;
    }

    public void OnDisable()
    {
        over = false;
    }
}
