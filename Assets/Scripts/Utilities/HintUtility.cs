using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintUtility : MonoBehaviour
{
    public static void ShowHint(Player agent, string key, string action, int sec)
    {
        Hint hint = agent.cameraInterface.hint;
        if (Player.language == ML.Lang.en)
        {
            hint.press.text = "Press";
            hint.to.text = "to";
        }
        else if (Player.language == ML.Lang.it)
        {
            hint.press.text = "Premi";
            hint.to.text = "per";
        }
        hint.hintKey.text = key;
        hint.hintAction.text = action;
        hint.StartCoroutine(Show(hint, sec));
    }

    public static void HideHint()
    {

    }

    static IEnumerator Show(Hint hint, int sec)
    {
        float t1 = 0;
        while (Vector3.Distance(hint.gameObject.transform.localScale, hint.dest) != 0)
        {
            t1 += hint.speed;
            hint.gameObject.transform.localScale = Vector3.Lerp(hint.origin, hint.dest, t1);
            yield return null;
        }
        if (sec > 0)
        {
            float t2 = 0;
            yield return new WaitForSeconds(sec);
            while (Vector3.Distance(hint.gameObject.transform.localScale, hint.origin) != 0)
            {
                t2 += hint.speed;
                hint.gameObject.transform.localScale = Vector3.Lerp(hint.dest, hint.origin, t2);
                yield return null;
            }
        }
    }
}
