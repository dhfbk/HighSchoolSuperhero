using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChattingAnimation : MonoBehaviour
{
    public float rate;
    float t;
    List<GameObject> spritePool;
    int poolSize;
    int index;

    void Start()
    {
        rate = 1.0f;
        spritePool = new List<GameObject>();
        poolSize = 5;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject icon = Instantiate(Resources.Load<GameObject>("ChatIcon"));
            icon.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            spritePool.Add(icon);
        }
    }

    void Update()
    {
        t += Time.deltaTime;
        if (t > rate)
        {
            CreateIcon();
            index++;
            if (index == spritePool.Count)
            {
                index = 0;
            }
            t = 0;
        }
    }

    public void CreateIcon()
    {
        spritePool[index].transform.position = transform.position + new Vector3(Random.Range(-0.75f, 0.75f), 2, Random.Range(-0.75f, 0.75f));
        spritePool[index].GetComponent<ChatIcon>().dest = spritePool[index].transform.position + new Vector3(0, 3, 0);
        spritePool[index].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        spritePool[index].GetComponent<SpriteRenderer>().color = new Color(0.58f, 0.93f, 1, 1);
    }
}
