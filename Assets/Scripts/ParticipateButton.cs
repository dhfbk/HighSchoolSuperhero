using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticipateButton : MonoBehaviour
{
    private bool show;
    private bool clicked;
    private bool over;

    void Update()
    {
        if (show)
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, -370, transform.localPosition.z), Time.deltaTime*3);
        else
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, -750, transform.localPosition.z), Time.deltaTime*3);
    }

    public void Show()
    {
        show = true;
    }

    public void Hide()
    {
        show = false;
    }

    public void Click()
    {
        clicked = true;
    }

    public bool IsClicked()
    {
        return clicked;
    }
    public bool IsOver()
    {
        return over;
    }

    private void OnMouseUpAsButton()
    {
        clicked = true;
    }
    private void OnMouseOver()
    {
        over = true;
    }
    private void OnMouseExit()
    {
        over = false;
    }
}
