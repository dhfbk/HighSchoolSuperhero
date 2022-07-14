using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject target;
    public float offset;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        offset = 1.5f;
        if (!target)
            target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!target.name.Contains("Player"))
            transform.position = Vector3.Lerp(transform.position, target.transform.position+new Vector3(0, offset, 0), Time.deltaTime * speed);
    }
    private void FixedUpdate()
    {
        if (target.name.Contains("Player"))
            transform.position = Vector3.Lerp(transform.position, target.transform.position + new Vector3(0, offset, 0), Time.deltaTime * speed);

    }
}
