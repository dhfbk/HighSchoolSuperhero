using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ReactionIcon : MonoBehaviour
{
    Vector3 startPos;
    Vector3 endPos;
    Vector3 startScale;
    Vector3 endScale;
    float t;
    List<SpriteRenderer> r;
    private void Start()
    {
        r = new List<SpriteRenderer>();
        r = GetComponentsInChildren<SpriteRenderer>().ToList();
        startPos = transform.localPosition;
        endPos = startPos + new Vector3(0, 3.5f, 0);
        startScale = transform.localScale / 2;
        endScale = startScale * 2;
    }
    void Update()
    {
        transform.LookAt(Camera.main.transform.position, Vector3.up);
        transform.localPosition = Vector3.Lerp(startPos, endPos, t);
        transform.localScale = Vector3.Lerp(startScale, endScale, t);
        t += Time.deltaTime*2;
        if (t >= 1F)
            StartCoroutine(WaitAndDestroy());
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(3);
        float t = 0;
        Color[] startColors = new Color[r.Count];
        Color[] endColors = new Color[r.Count];
        for (int i = 0; i < startColors.Length; i++)
        {
            startColors[i] = r[i].color;
                Color temp = startColors[i];
                temp.a = 0;
                endColors[i] = temp;
            i++;
        }

        while (t <= 1)
        {
            for (int i = 0; i < r.Count; i++)
            {
                r[i].color = Color.Lerp(startColors[i], endColors[i], t);
            }
            t += Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
