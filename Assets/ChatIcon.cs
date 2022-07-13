using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChatIcon : MonoBehaviour
{
    public Vector3 dest;
    public float speed;
    public bool destroy;
    List<SpriteRenderer> r;
    // Start is called before the first frame update
    void Start()
    {
        speed = 1.0f;
        r = new List<SpriteRenderer>();
        r = GetComponentsInChildren<SpriteRenderer>().ToList();

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, dest, Time.deltaTime*speed);
        for (int i = 0; i < r.Count; i++)
        {
            r[i].color = Color.Lerp(r[i].color, new Color(0.58f, 0.93f, 1, 0), Time.deltaTime * speed);
        }
        if (r[0].color.a < 0.02f)
        {
            if (destroy)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
