using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class RightClick : MonoBehaviour, IPointerClickHandler
{
    public List<GameObject> tokens;
    void Start()
    {
        tokens = new List<GameObject>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Left click");
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            Debug.Log("Middle click");
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                if (transform.parent.GetChild(i).name.Contains("Token") && transform.parent.GetChild(i).name != this.gameObject.name)
                {
                    tokens.Add(transform.parent.GetChild(i).gameObject);
                }
            }
            transform.parent.parent.GetComponent<WordByWord>().tokens = tokens;
            //transform.parent.parent.GetComponent<WordByWord>().Center(false);
            Destroy(this.gameObject);
        }
    }
}