using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpUtility : MonoBehaviour
{
    public static string Warning = "Warning";
    public static string Error = "Error";
    public static string Success = "Success!";

    public enum Types { success, warning, error }

    public static string LocalizedType(Player agent, Types type)
    {
        if (Player.language == ML.Lang.en)
        {
            switch (type)
            {
                case Types.success:
                    return "Success!";
                case Types.warning:
                    return "Warning";
                case Types.error:
                    return "Error";
            }
        }
        else if (Player.language == ML.Lang.it)
        {
            switch (type)
            {
                case Types.success:
                    return "Successo!";
                case Types.warning:
                    return "Attenzione";
                case Types.error:
                    return "Errore";
            }
        }
        return "";
    }
    public static void Open(Canvas popupCanvas, string type, string message, int seconds)
    {
        PopUp existing = FindObjectOfType<PopUp>();
        if (existing)
            if (existing.content.text.Equals(message))
                return;

        PopUp.popUpNumber += 1;
        popupCanvas.gameObject.SetActive(true);
        GameObject popObj = Instantiate(Resources.Load<GameObject>("PopUp"));
        PopUp pop = popObj.GetComponent<PopUp>();

        popObj.transform.parent = popupCanvas.transform.GetChild(0).transform;
        popObj.transform.localPosition = Vector3.zero;
        popObj.transform.localRotation = Quaternion.identity;
        popObj.transform.localScale = Vector3.one;
        

        pop.type.text = type;
        pop.content.text = message;
        popupCanvas.enabled = true;
        foreach (RectTransform rt in pop.rects)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }
        if (seconds < 0)
            pop.Dismiss(Mathf.Abs(seconds));
        else if (seconds > 0)
        {
            //pop.DismissOnInput = true;
            pop.Dismiss(seconds);
        }
        else
            pop.DismissOnInput = true;
    }

    public static void Reopen(Canvas popupCanvas)
    {
        popupCanvas.enabled = true;
    }

    public static void Dismiss(Canvas popupCanvas, int seconds)
    {
        popupCanvas.GetComponent<PopUp>().Dismiss(seconds);
    }
}
