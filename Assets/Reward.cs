using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reward : MonoBehaviour
{
    float speed = 3;
    public GameObject rays;
    Color rayColor = new Color(0f, 0f, 0.0f, 1.0f);
    Color rayColorTransp = new Color(0f, 0f, 0.0f, 0.0f);
    Color rayFillColor = new Color(1.0f, 0.7f, 0.0f, 1.0f);
    Color rayFillColorTransp = new Color(1.0f, 0.7f, 0.0f, 0.0f);
    Color objColor = new Color(1, 1, 1, 1);
    Color objColorTransp = new Color(1, 1, 1, 0);

    private void Start()
    {
        StartCoroutine(Appear());
    }
    IEnumerator Appear()
    {
        float t = 0;
        Vector3 startpos = transform.localPosition;
        while (t < 1)
        {
            t += Time.deltaTime*speed;
            transform.localPosition = Vector3.Lerp(startpos, startpos + new Vector3(0, 50, 0), t);
            foreach (Image img in rays.GetComponentsInChildren<Image>())
            {
                if (img.gameObject.name.Contains("Ray"))
                    img.color = Color.Lerp(rayColorTransp, rayColor, t);
                else
                    img.color = Color.Lerp(rayFillColorTransp, rayFillColor, t);
            }
            yield return null;
        }
        StartCoroutine(Disappear());
    }
    IEnumerator Disappear()
    {
        float t = 0;
        Vector3 startpos = transform.localPosition;
        while (t < 1)
        {
            t += Time.deltaTime*speed;
            transform.localPosition = Vector3.Lerp(startpos, startpos + new Vector3(0, 50, 0), t);
            GetComponent<Image>().color = Color.Lerp(objColor, objColorTransp, t);
            foreach (Image img in rays.GetComponentsInChildren<Image>())
            {
                if (img.gameObject.name.Contains("Ray"))
                    img.color = Color.Lerp(rayColor, rayColorTransp, t);
                else
                    img.color = Color.Lerp(rayFillColor, rayFillColorTransp, t);
            }
            yield return null;
        }
        Destroy(rays);
        Destroy(this.gameObject);
    }
}
