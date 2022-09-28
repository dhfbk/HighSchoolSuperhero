using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazyTriggerer : MonoBehaviour, ITriggerable
{
    public Player Agent { get; set; }
    GameObject obj;
    Collider objCollider;
    bool activated;
    Camera cam;
    Plane[] planes;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        planes = GeometryUtility.CalculateFrustumPlanes(cam);
        objCollider = GetComponent<SphereCollider>();

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenpos = Camera.main.WorldToScreenPoint(transform.position);
        if (!activated)
        {
            if (screenpos.x > 0 &&
                screenpos.x < Screen.width &&
                screenpos.y > 0 &&
                screenpos.y < Screen.height)
            {
                Activate();
            }
        }

    }

    public void TriggerOn(Player agent)
    {
        //if (!activated)
        //    LazyActivate();
    }

    public void TriggerOff()
    {

    }

    private void LazyActivate()
    {
        transform.parent.GetComponent<DialogueInstancer>().LazyActivate();
        activated = true;
    }

    private void Activate()
    {
        transform.parent.GetComponent<DialogueInstancer>().Activate();
        activated = true;
    }
}
