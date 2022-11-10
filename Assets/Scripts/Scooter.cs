using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scooter : MonoBehaviour, IPlayer, ITriggerable
{
    public Player Agent { get; set; }
    public ParticleSystem dust;
    public bool overwriteInteraction;
    Rigidbody r;
    public float push;
    public float fastSpeed;
    public float normalSpeed;
    public float speed;
    public float acceleration;
    public float friction;
    public float torque;

    public LayerMask touchMask;
    Vector3 moveVec;
    Vector3 finalVec;
    public float jumpForce;
    bool active;
    public bool jumping;
    AudioSource audio;
    Quaternion initialRot;
    Vector3 trueForward;
    float frictionForce;
    float signedMagnitude;
    public BoxCollider triggerArea;

    GameObject player;

    public float velocityThreshold;
    void Start()
    {
        r = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
        velocityThreshold = 3.0f;
        initialRot = transform.rotation;
        trueForward = transform.forward;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        //SinglePLayer only
        if (Vector3.Distance(player.transform.position, transform.position) < 55) {
            GetComponent<AudioSource>().pitch = r.velocity.magnitude;
            if (Input.GetMouseButtonDown(0)) {
                if (GetComponent<MouseOver>().on) {
                    Player agent;
                    if ((agent = GetComponent<ITriggerable>().Agent) != null)
                        Toggle(agent);
                }
            }
            if (Input.GetKeyDown("f")) {
                Player agent = GetComponent<ITriggerable>().Agent;
                if (agent != null && !agent.GetComponent<Movement>().Busy) {
                    if (!active) {
                        float dot = Vector3.Dot(agent.transform.forward, (this.transform.position - agent.transform.position).normalized);
                        if (dot < 1f && dot > 0)
                        {
                            Toggle(agent);
                        }
                    }
                    else
                        Toggle(agent);
                }
            }
            if (active) 
            {
                //if (Input.GetMouseButton(0))
                //{
                //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //    RaycastHit hit;
                //    if (Physics.Raycast(ray, out hit, Mathf.Infinity, touchMask))
                //    {

                //            Vector3 sameHeightPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                //            transform.LookAt(sameHeightPoint);
                //            Vector3 vector = (sameHeightPoint - transform.position).normalized * 6 * speed;
                //            finalVec = new Vector3(vector.x, r.velocity.y, vector.z);
                //            moveVec = new Vector2(finalVec.x, finalVec.z).normalized;
                //            print(moveVec);
                        
                //    }
                //    else
                //    {
                //        finalVec = Vector3.zero;
                //    }
                //}
                //else
                //{
                //    finalVec = Vector3.zero;
                //}
                if (Input.GetKey(KeyCode.LeftShift)) {
                    if (speed != fastSpeed) {
                        speed = fastSpeed;
                        dust.Play();
                    }
                }
                else {
                    speed = normalSpeed;
                }

                RaycastHit hit0;
                bool rearGrounded = Physics.Raycast(transform.position, Vector3.down, out hit0) && hit0.distance < 0.1f;
                RaycastHit hit1;
                bool frontGrounded = Physics.Raycast(transform.position + transform.forward * 1.25f, Vector3.down, out hit1) && hit1.distance < 0.1f;
                if (rearGrounded || frontGrounded) {
                    if (!jumping) {
                        if (Input.GetKeyUp(KeyCode.Space)){
                            r.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
                            jumping = true;
                            print("jumped");
                        }
                    }
                    else {
                        jumping = false;
                    }
                }
                else
                    jumping = true;
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Agent) {
            transform.rotation = Quaternion.Lerp(transform.rotation, initialRot, Time.fixedDeltaTime * 3);
            trueForward = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z);
            var localVel = transform.InverseTransformDirection(r.velocity);
            if (active && !Agent.GetComponent<Movement>().Busy) {
                if (!jumping) {
                    if (Input.GetMouseButton(0))
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, touchMask))
                        {

                            Vector3 sameHeightPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                            transform.LookAt(sameHeightPoint);
                            Vector3 vector = (sameHeightPoint - transform.position).normalized * 6 * speed;
                            finalVec = new Vector3(vector.x, r.velocity.y, vector.z);
                            moveVec = new Vector2(finalVec.x, finalVec.z).normalized;


                            r.AddForce(finalVec.normalized * 10, ForceMode.Force);
                        }
                    }

                        if (Input.GetAxisRaw("Vertical") > 0)
                        {
                            signedMagnitude = Mathf.Lerp(signedMagnitude, speed, Time.fixedDeltaTime * acceleration * 3);
                            if (r.velocity.magnitude < speed)
                                r.AddForce(trueForward * signedMagnitude, ForceMode.Force);

                            //Raddrizza se non ha niente sotto la ruota davanti
                            if (CalcDistance(transform.position + transform.forward * 1.25f))
                                initialRot = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0));
                            else
                                initialRot = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));
                        }
                        else if (Input.GetAxisRaw("Vertical") < 0)
                        {
                            signedMagnitude = Mathf.Lerp(signedMagnitude, -speed, Time.fixedDeltaTime * acceleration * 3);
                            if (r.velocity.magnitude < speed)
                                r.AddForce(trueForward * signedMagnitude, ForceMode.Force);
                            initialRot = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0));
                        }
                        else
                        {
                            signedMagnitude = Mathf.Lerp(signedMagnitude, 0, Time.fixedDeltaTime * acceleration);
                            initialRot = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));
                        }

                        //
                        if (Input.GetAxisRaw("Horizontal") < 0)
                        {
                            transform.Rotate(0, -torque * Time.fixedDeltaTime * localVel.z / 2, 0);
                            initialRot = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0));
                        }
                        else if (Input.GetAxisRaw("Horizontal") > 0)
                        {
                            transform.Rotate(0, torque * Time.fixedDeltaTime * localVel.z / 2, 0);
                            initialRot = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0));
                        }

                        //Apply friction
                        frictionForce = Vector3.Dot(Vector3.ClampMagnitude(r.velocity, 1), trueForward);
                        float dir = localVel.z > 0 ? 1 : -1;

                        r.velocity = Vector3.ClampMagnitude(r.velocity + trueForward * dir * 5, 1) * r.velocity.magnitude;
                    

                } else {
                    if (Input.GetAxisRaw("Vertical")>0) {
                        if (r.velocity.magnitude < speed / 10) {
                            signedMagnitude = Mathf.Lerp(signedMagnitude, speed, Time.deltaTime * acceleration);
                        }
                    }
                    else if (Input.GetAxisRaw("Vertical") < 0) {
                        if (r.velocity.magnitude < speed / 10) {
                            signedMagnitude = Mathf.Lerp(signedMagnitude, -speed, Time.deltaTime * acceleration);
                        }
                    }
                    else
                        signedMagnitude = Mathf.Lerp(signedMagnitude, 0, Time.deltaTime * acceleration);
                }
            }
        }
    }

    private bool CalcDistance(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos, Vector3.down, out hit)) {
            if (hit.distance < 0.1f) {
                return true;
            } 
            else {
                return false;
            }
        } 
        else {
            return false;
        }
    }

    public void Deactivate()
    {
        active = false;
    }

    public void Activate()
    {
        active = true;
    }

    public void Toggle(Player player)
    {
        if (!active) {
            audio.Play();
            GameObject p = player.gameObject;
            GameObject pmg = GameObject.Find("PlayerModelGroup");
            p.GetComponent<Movement>().Riding = true;
            p.GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().drag = 0.2f;
            p.GetComponent<CapsuleCollider>().enabled = false;
            triggerArea.enabled = false;
            p.GetComponent<Rigidbody>().useGravity = false;
            player.cameraInterface.followPlayer.target = this.transform.gameObject;
            p.transform.parent = this.transform;
            p.transform.localPosition = Vector3.zero + new Vector3(0, 2.25f, 1f);
            p.transform.rotation = transform.rotation;
            active = true;
            this.Agent = player;
            pmg.GetComponent<Animator>().SetBool("Scooter", true);
            transform.tag = "Player";
        }
        else {
            GameObject p = player.gameObject;
            GameObject pmg = GameObject.Find("PlayerModelGroup");
            triggerArea.enabled = true;
            p.GetComponent<Movement>().Riding = false;
            p.GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().drag = 3f;
            p.GetComponent<CapsuleCollider>().enabled = true;
            p.GetComponent<Rigidbody>().useGravity = true;
            player.cameraInterface.followPlayer.target = p;
            p.transform.parent = null;
            active = false;
            this.Agent = null;
            pmg.GetComponent<Animator>().SetBool("Scooter", false);
            pmg.GetComponent<Animator>().SetBool("Walk", true);
            transform.tag = "Scooter";

        }
    }

    public void TriggerOn(Player agent)
    {
        if (!agent.transform.parent) {
            NotificationUtility.ShowString(agent, string.Format(ML.systemMessages.pressFToRide, MultiplatformUtility.SecondaryInteractionKey));
            this.Agent = agent;
        }
    }
    public void TriggerOff()
    {
        this.Agent = null;
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("TriggerArea")) {
            if (col.GetComponent<Triggerer>().overrideInteraction)
                overwriteInteraction = true;
        }
    }

    public void OnTriggerExit(Collider col)
    {
        if (col.transform.CompareTag("TriggerArea")) {
            overwriteInteraction = false;
        }
    }
}
