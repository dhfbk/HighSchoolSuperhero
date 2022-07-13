using UnityEngine;

public class Ball : MonoBehaviour, ITriggerable
{
    public Player Agent { get; set; }
    public Player memAgent;
    public RewardInfo rewardInfo;
    Rigidbody r;
    bool pressed;
    float t;
    Vector3 initialPos;
    float ballDistance;
    bool taken;
    GameObject target;
    bool shooting;
    float w;
    void Start()
    {
        r = GetComponent<Rigidbody>();
        initialPos = transform.position;
        ballDistance = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position + target.transform.forward, Time.deltaTime * 10);
            if (Input.GetKeyDown("f"))
            {
                pressed = true;
                t = 0;
            }
        }
        else
        {
            if (Agent != null)
            {
                if (!shooting)
                {
                    target = Agent.gameObject;
                    r.velocity = Vector3.zero;
                }
            }
            else
            {
                shooting = false;
            }
        }


        if (pressed)
        {
            if (t <= 5f)
                t += Time.deltaTime*7;
        }
        if (Input.GetKeyUp("f"))
        {
            if (pressed)
            {
                pressed = false;
                Shoot(t);
            }
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
            //if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            //{
            //    if (hit.transform.name == "Socfield")
            //    {
            //        if (Vector3.Distance(hit.point, transform.position) < Vector3.Distance(hit.point, target.transform.position))
            //        {
            //            r.AddForce(Vector3.ClampMagnitude(hit.point - target.transform.position, 1)*10*t + Vector3.up*5*t, ForceMode.Impulse);
            //        ignore = true;
            //            target = null;
            //            t = 0;
            //        }
            //    }
            //}
            //else
            //{
            //}
        }

        if (r.velocity != Vector3.zero)
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.name != "Socfield" && hit.transform.name != "Player")
                {
                    Bounce();
                }
            }
        }
    }

    private void Shoot(float t)
    {
        r.AddForce(Vector3.ClampMagnitude(Agent.transform.forward, 1) * 10 * t + Vector3.up * 3 * t, ForceMode.Impulse);
        target = null;
        shooting = true;
        t = 0;
    }

    private void Bounce()
    {
        r.velocity = -r.velocity;
    }

    //private void OnCollisionEnter(Collision other)
    //{
    //    if (other.transform.CompareTag("Player") && other.transform.name == "Player")
    //    {
    //        target = other.transform.gameObject;
    //        taken = true;
    //    }
    //}
    public void TriggerOn(Player agent)
    {
        this.Agent = agent;
        memAgent = agent;
        HintUtility.ShowHint(agent, "F", "shoot", 2);
    }
    public void TriggerOff()
    {
        this.Agent = null;
    }
}
