using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointSystem : MonoBehaviour
{
    public static void AddPoints(Player agent, PointType type, int amount)
    {
        agent.StartCoroutine(AddPointsOverFrames(agent, type.ToString(), amount));
    }
    public static void AddPoints(Player agent, string type, int amount)
    {
        agent.StartCoroutine(AddPointsOverFrames(agent, type, amount));
    }
    public static void AddPoints(MonoBehaviour mb, Player agent, string type, int amount)
    {
        mb.StartCoroutine(AddPointsOverFrames(agent, type, amount));
    }
    private static IEnumerator AddPointsOverFrames(Player agent, string type, int amount)
    {
        AudioSource aS = GameObject.Find("FX").GetComponent<AudioSource>();
        AudioClip clip;
        if (type == PointType.heart.ToString())
        {
           clip = Resources.Load<AudioClip>("Pops Up");
        }
        else
        {
            clip = Resources.Load<AudioClip>("Gentle Menu Reminder");
        }
        for (int i = 0; i < amount; i++)
        {
            GameObject p = Instantiate(Resources.Load<GameObject>(type));
            if (Random.Range(0, 3) == 1)
                aS.PlayOneShot(clip);
            p.GetComponent<Point>().player = agent;
            p.transform.parent = agent.cameraInterface.hudCanvas.transform;
            p.transform.localPosition = Vector2.zero;
            p.transform.localRotation = Quaternion.identity;
            p.transform.localScale = Vector2.one;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
