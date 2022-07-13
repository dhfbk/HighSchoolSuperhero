using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public float rotation;
    public bool rotate;
    public int dir;
    public Transform d;
    // Start is called before the first frame update
    void Start()
    {
        d = transform.GetChild(0);
        if (d.gameObject.name.Contains("SX"))
        {
            dir = -1;
        }
        else
        {
            dir = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rotate)
        {
            /*if (rotation < 2.0F)
            {
                d.eulerAngles = new Vector3(d.eulerAngles.x, d.eulerAngles.y, dir*rotation);
                rotation += Time.deltaTime;
            }*/
           // d.eulerAngles = new Vector3(d.eulerAngles.x, d.eulerAngles.y, Mathf.LerpAngle(d.eulerAngles.z, dir * 2.0F, Time.deltaTime*10));
            d.transform.localRotation = Quaternion.Lerp(d.transform.localRotation, Quaternion.Euler(d.transform.localRotation.x, d.transform.localRotation.y, dir*80), Time.deltaTime*2);
        }
        else
        {
            /*if (rotation > 0)
            {
                d.eulerAngles = new Vector3(d.eulerAngles.x, d.eulerAngles.y, -dir * rotation);
                rotation -= Time.deltaTime;
            }*/
            d.transform.localRotation = Quaternion.Lerp(d.transform.localRotation, Quaternion.Euler(d.transform.localRotation.x, d.transform.localRotation.y, 0), Time.deltaTime*2);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.gameObject.tag == "Player")
        {
            rotate = true;
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (col.transform.gameObject.tag == "Player")
        {
            rotate = false;
        }
    }
}
