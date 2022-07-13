using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUp : MonoBehaviour
{
    public static int popUpNumber;
    public TextMeshProUGUI type;
    public TextMeshProUGUI content;
    public RectTransform[] rects;
    private bool dismissOnInput;
    public bool DismissOnInput { get => dismissOnInput; set => dismissOnInput = value; }

    private void Update()
    {
        if (dismissOnInput)
        {
            if (Input.anyKey)
            {
                Dismiss();
                dismissOnInput = false;
            }
        }
    }
    public void Dismiss()
    {
        Destroy(this.gameObject);
    }
    public void Dismiss(int seconds)
    {
        StopAllCoroutines();
        StartCoroutine(DismissOverFrames(seconds));
    }

    public IEnumerator DismissOverFrames(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        popUpNumber -= 1;
        Destroy(this.gameObject);
    }
}
