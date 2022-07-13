using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reactions : MonoBehaviour
{
    public static void Create(GameObject emitter, string type)
    {
        GameObject reaction = Instantiate(Resources.Load<GameObject>($"{type}Reaction"), emitter.transform);
        reaction.transform.rotation = Quaternion.identity;
        reaction.transform.localPosition = Vector3.zero;
        reaction.transform.localScale = Vector3.one;
    }
}
