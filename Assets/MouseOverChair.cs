using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverChair : MonoBehaviour
{
    Renderer r;
    bool on;
    bool sitting;
    bool gettingUp;
    bool up;
    Vector3 dest;
    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            if (Control.Instance.player && Vector3.Distance(Control.Instance.player.transform.position, transform.position) < 3)
            {
                if (on == true)
                {
                    if (gameObject.name == "ChairWheels")
                    {
                        if (!sitting)
                        {
                            Chair(true);
                        }
                    }
                }
                else
                {
                    if (gameObject.name == "ChairWheels")
                    {
                        if (sitting)
                        {
                            Chair(false);
                        }
                    }
                }
            }
        }

        //Sit
        if (sitting)
        {
            if (up)
            {
                Control.Instance.player.transform.position = Vector3.MoveTowards(Control.Instance.player.transform.position, dest, Time.deltaTime * 5);
                if (Control.Instance.player.transform.position == dest)
                {
                    up = false;
                    dest = new Vector3(transform.position.x, transform.position.y + 0.75F, transform.position.z + 0.2F);
                }
            }
            else
            {
                Control.Instance.player.transform.position = Vector3.MoveTowards(Control.Instance.player.transform.position, dest, Time.deltaTime * 5);
            }
            Control.Instance.player.transform.LookAt(new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z - 1));
        }
        //GetUp
        else if (gettingUp)
        {
            if (up)
            {
                Control.Instance.player.transform.position = Vector3.MoveTowards(Control.Instance.player.transform.position, dest, Time.deltaTime * 5);
                if (Control.Instance.player.transform.position == dest)
                {
                    up = false;
                    dest = new Vector3(transform.position.x + 1, transform.position.y + 0.2f, transform.position.z);
                }
            }
            else
            {
                Control.Instance.player.transform.position = Vector3.MoveTowards(Control.Instance.player.transform.position, dest, Time.deltaTime * 5);
                if (Control.Instance.player.transform.position == dest)
                {
                    gettingUp = false;
                    Control.Instance.player.GetComponent<Control>().enabled = true;
                    Control.Instance.player.GetComponent<Rigidbody>().useGravity = true;
                    Control.Instance.player.GetComponent<CapsuleCollider>().enabled = true;
                    Control.Instance.player.GetComponent<Animator>().SetBool("GetUp", false);
                }
            }
        }
    }

    private void OnMouseOver()
    {
        for (int i = 0; i < r.materials.Length; i++)
        {
            r.materials[i].SetColor("_OutlineColor", new Color(1, 0.4F, 0F, 1));
            //r.materials[i].SetColor("_EmissionColor", new Color(4, 4, 0, 1));
            on = true;
        }
    }
    private void OnMouseExit()
    {
        for (int i = 0; i < r.materials.Length; i++)
        {
            r.materials[i].SetColor("_OutlineColor", new Color(0, 0, 0, 1));
            //r.materials[i].SetColor("_EmissionColor", new Color(0, 0, 0, 1));
            on = false;
        }
    }

    void Chair(bool sit)
    {
        if (sit)
        {
            Control.Instance.player.GetComponent<Movement>().enabled = false;
            Control.Instance.player.GetComponent<Animator>().SetBool("Sit", true);
            Control.Instance.player.GetComponent<Animator>().SetBool("GetUp", false);
            Control.Instance.player.GetComponent<Rigidbody>().useGravity = false;
            Control.Instance.player.GetComponent<CapsuleCollider>().enabled = false;
            dest = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z + 0.2F);
            sitting = true;
            gettingUp = false;
            up = true;
        }
        else
        {
            Control.Instance.player.transform.eulerAngles = new Vector3(0,90,0);
            Control.Instance.player.GetComponent<Animator>().SetBool("GetUp", true);
            Control.Instance.player.GetComponent<Animator>().SetBool("Sit", false);
            dest = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            sitting = false;
            gettingUp = true;
            up = true;
        }
    }
}
