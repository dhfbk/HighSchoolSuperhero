using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationUtilities : MonoBehaviour
{
    public static IEnumerator RotateLinear(bool local, GameObject obj, Quaternion startRot, Quaternion endRot, float speed, float t)
    {
        while (t < 1)
        {
            t += Time.deltaTime*speed;
            if (!local)
                obj.transform.rotation = Quaternion.Lerp(startRot, endRot, t);
            else
                obj.transform.localRotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }
    }

    public static IEnumerator RotateDampened(GameObject obj, Quaternion startRot, Quaternion endRot, float speed)
    {
        while (obj.transform.rotation != endRot) //per lo meno idealmente
        {
            obj.transform.rotation = Quaternion.Lerp(startRot, endRot, Time.deltaTime * speed);
            yield return null;
        }
    }

    public static IEnumerator MoveAndFade(bool local, GameObject obj, Vector3 startPos, Vector3 endPos, float speed)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * speed;
            if (!local)
                obj.transform.position = Vector3.Lerp(startPos, endPos, t);
            else
                obj.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
    }
}
