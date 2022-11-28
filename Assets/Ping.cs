using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ping : MonoBehaviour
{
    float t = 0;
    Player p;
    // Start is called before the first frame update
    void Start()
    {
        p = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Player.admin)
        {
            if (p.initialized == true)
            {
                print(t);
                if (t > 15f)
                {
                    StartCoroutine(API.Ping(GetComponent<Player>()));
                    t = 0;
                }
                else
                {
                    t += Time.deltaTime;
                }
            }
        }
    }
}
