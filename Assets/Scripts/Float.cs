using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float : MonoBehaviour
{
    public Vector3 upper;
    public Vector3 lower;
    bool up;
    float t;
    float speed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        upper = transform.position;
        lower = transform.position - new Vector3(0, 0.5f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (up && transform.position.y < upper.y - 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, upper, t);
            t += Time.deltaTime * speed;
        }
        else if (!up && transform.position.y > lower.y+0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, lower, t);
            t += Time.deltaTime * speed;
        }
        else
        {
            t = 0;
            up = !up;
        }
    }
}
