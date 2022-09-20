using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;
using System;
using System.Text;
using UnityEngine.Android;

public class AudioUtility
{
    public static float FXVolume = 1.0F;
    public static float MusicVolume = 1.0f;

    public static void PlaySound(AudioSource source)
    {
        source.volume = AudioUtility.FXVolume;
        source.Play();
    }
    public static void PlayMusic(AudioSource source)
    {
        source.volume = AudioUtility.MusicVolume/3;
        source.Play();
    }
}

public class Movement : MonoBehaviour
{
    private float walkSpeed = 1.25f;
    private float runSpeed = 2.25f;

    //Touch
    bool tap_once;
    float tPressed;
    float lastClickDuration;
    float lastPauseDuration;
    bool doubleTapped;
    float pauseT;
    bool clickRegistered;

    private bool tap_gliderOnce;
    private bool tap_readyToGlide;
    public float glideGravity;
    public GameObject glider;
    public GameObject gliderInstance;
    public AudioClip[] stepAsphalt;
    public AudioClip[] stepGround;
    private bool busy;
    private Vector3 moveVec;
    public Arrow arrow;
    public ParticleSystem[] trails;
    public bool Busy { get => busy;
        set
        {
            busy = value;
            if (GetComponent<Animator>().GetBool("Jump"))
                GetComponent<Animator>().SetBool("Jump", false);
            if (busy == true)
                GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
    public bool Riding { get; set; }
    public bool Frozen { get; set; }
    public bool MouseFrozen { get; set; }
    public bool KeyboardFrozen { get; set; }
    float x, y, z;
    public float Speed { get; set; }
    public LayerMask Mask;
    public LayerMask UIMask;
    Rigidbody r;
    public Camera camera;
    bool artificialRelease;
    bool jumping;
    public float jumpForce;
    public float landingDistance;
    private float downForce;
    private Animator anim;
    private int step;
    Player agent;
    AudioSource audio;
    float inertiaT;
    float smoothSpeed;
    bool jump;
    bool getJAxisOnce;
    bool getGAxisOnce;
    bool readyToGlide;
    bool gliding;
    float jt;
    bool pressingJump;
    Vector3 finalVec;
    public LayerMask jumpMask;

    void Start()
    {
        if (Player.condition == Condition.NoW3D)
            GetComponent<Rigidbody>().useGravity = false;
        audio = GetComponent<AudioSource>();
        agent = GetComponent<Player>();
        anim = GameObject.Find("PlayerModelGroup").GetComponent<Animator>();
        print("name" + anim.transform.gameObject.name);
        print("name" + anim.name);
        if (Speed == 0)
            Speed = 1;
        r = GetComponent<Rigidbody>();

        if (transform.name != "NetworkPlayer")
            camera = Camera.main;
    }

    void Update()
    {
        if (anim.GetBool("Walk"))
        {
            float normTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (normTime > 0.1f && normTime < 0.6f && step == 0)
            {
                audio.clip = stepAsphalt[0];
                audio.pitch = UnityEngine.Random.Range(1.5f, 2.0f);
                AudioUtility.PlaySound(audio);
                step = 1;
            }
            else if (normTime > 0.6f && step == 1)
            {
                audio.clip = stepAsphalt[0];
                audio.pitch = UnityEngine.Random.Range(1.5f, 2.0f);
                AudioUtility.PlaySound(audio);
                step = 0;
            }

        }
        if (!Busy && !Riding)
        {
            if (Input.GetAxisRaw("Fire3")>0)
            {
                Speed = runSpeed;
                anim.SetFloat("Speed", 2.0F);
            }
            else
            {
                Speed = walkSpeed;
                anim.SetFloat("Speed", 1);
            }

            //Glider
            if (Input.GetAxisRaw("Fire2") > 0)
            {
                if (!getGAxisOnce && readyToGlide)
                {
                    ToggleGlider();
                }
            }
            else if (Input.GetAxisRaw("Fire2") == 0)
            {
                readyToGlide = true;
                getGAxisOnce = false;
            }


            //Jump keyboard
            if (Input.GetAxisRaw("Jump") != 0)
            {
                pressingJump = true;
            }

            //Jump touch
            if (Input.touchCount > 0 || Input.GetMouseButton(0))
            {
                tap_once = false;
                tPressed += Time.deltaTime;

                if (!clickRegistered)
                {
                    lastPauseDuration = pauseT;
                    pauseT = 0;

                    if (lastPauseDuration < 0.25f && lastClickDuration < 0.25f)
                        doubleTapped = true;
                    clickRegistered = true;
                }

                if (doubleTapped)
                {
                    pressingJump = true;
                    if (tap_readyToGlide && !tap_gliderOnce)
                    {
                        ToggleGlider();
                        tap_gliderOnce = true;
                    }
                }
                
                RaycastHit hit;
                RaycastHit hitUI;
                Ray ray;
                Ray rayUI;
                Vector3 inputPosition;
                if (Input.GetMouseButton(0))
                {
                    inputPosition = Input.mousePosition;
                    ray = camera.ScreenPointToRay(Input.mousePosition);
                    rayUI = agent.cameraInterface.uicam.ScreenPointToRay(Input.mousePosition);
                }
                else
                {
                    int id = Input.touches[Input.touchCount - 1].fingerId;
                    inputPosition = Input.touches[id].position;
                    ray = camera.ScreenPointToRay(Input.touches[id].position);
                    rayUI = agent.cameraInterface.uicam.ScreenPointToRay(Input.touches[id].position);
                }

                if (Physics.Raycast(rayUI, Mathf.Infinity, UIMask))
                    artificialRelease = true;

                if (!artificialRelease)
                {
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, Mask))
                    {
                        //if (mouseMovementMask.All(x=>x != hit.transform.gameObject.layer))
                        //{
                        anim.SetBool("Walk", true);
                        Vector3 sameHeightPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                        transform.LookAt(sameHeightPoint);
                        //transform.position = Vector3.MoveTowards(transform.position, new Vector3(hit.point.x, transform.position.y, hit.point.z), Time.deltaTime * 4 * Speed);

                        Vector2 customMousePos = new Vector2(Mathf.Abs(Input.mousePosition.x - (Screen.width / 2)), Mathf.Abs(Input.mousePosition.y - (Screen.height / 2)));
                        customMousePos = new Vector2((customMousePos.x * Screen.height / 2) / (Screen.width / 2), customMousePos.y);
                        float p = Vector2.Distance(customMousePos, Vector3.zero);
                        float d = Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2), new Vector2(Screen.width, Screen.height));

                        float s = (p * 4) / d;
                        Vector3 vector = (sameHeightPoint - transform.position).normalized * 6 * Speed;
                        finalVec = new Vector3(vector.x * s, r.velocity.y, vector.z * s);
                        anim.SetFloat("Speed", s);
                        moveVec = new Vector2(finalVec.x, finalVec.z).normalized * s;
                        //}
                    }
                }
            }
            else
            {
                artificialRelease = false;
                tap_gliderOnce = false;
                if (!IsGrounded(this.gameObject))
                    tap_readyToGlide = true;
                if (!tap_once)
                {
                    lastClickDuration = tPressed;
                    tPressed = 0;
                    tap_once = true;
                }

                pauseT += Time.deltaTime;
                doubleTapped = false;

                //Reset clicks
                clickRegistered = false;

                if (Input.GetAxisRaw("Jump") == 0)
                    pressingJump = false;

                if (!KeyboardFrozen)
                {
                    Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                    if (!jumping)
                        anim.SetBool("Walk", true);
                    moveVec = input;
                    Vector3 sameHeightCamera = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
                    Vector3 dir = (transform.position - sameHeightCamera).normalized;
                    Vector3 forwardVector = dir * 6 * Speed * moveVec.y;
                    Vector3 rightVector = Camera.main.transform.right * 6 * Speed * moveVec.x;

                    float mod;
                    if (Mathf.Abs(moveVec.x) + Mathf.Abs(moveVec.y) >= 2)
                        mod = Mathf.Sqrt(2);
                    else
                        mod = 1;

                    finalVec = new Vector3(forwardVector.x + rightVector.x, r.velocity.y, forwardVector.z + rightVector.z) / mod;

                    //DirectionDummy
                    Vector3 directionDummy = transform.position + new Vector3(finalVec.normalized.x, 0, finalVec.normalized.z);
                    transform.LookAt(directionDummy);
                }
            }
            if (pressingJump)
            {
                if (GetComponent<Player>().ScarpeMolla)
                {
                    if (trails[0].isPlaying)
                    {
                        if (jt <= 0)
                        {
                            foreach (ParticleSystem trail in trails)
                                trail.Stop();
                        }
                    }
                    else
                    {
                        if (jt > 0)
                        {
                            foreach (ParticleSystem trail in trails)
                                trail.Play();
                        }
                    }
                }
                if (IsGrounded(this.gameObject))
                {
                    if (!getJAxisOnce)
                    {
                        jt = 1;
                        jump = true;
                        getJAxisOnce = true;
                    }
                }
            }
            else
            {
                if (GetComponent<Player>().ScarpeMolla)
                    foreach (ParticleSystem trail in trails)
                        trail.Stop();
                if (IsGrounded(this.gameObject))
                    getJAxisOnce = false;
            }

            if (!IsGrounded(this.gameObject))
            {
                jumping = true;
                //getJAxisOnce = false;
                anim.SetBool("Walk", false);
                if (!gliding)
                    anim.SetBool("Jump", true);
                else
                    anim.SetBool("Jump", false);
                downForce = -1f;

            }
            else
            {
                tap_readyToGlide = false;
                readyToGlide = false;
                if (gliderInstance)
                {
                    DestroyGlider();
                }
                if (jumping == true)
                {
                    jumping = false;
                    //getJAxisOnce = false;
                    AudioUtility.PlaySound(audio);
                }
                anim.SetBool("Jump", false);
                downForce = 0;
            }

            if (!Input.GetMouseButton(0) && Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("Horizontal") == 0)
            {
                anim.SetBool("Walk", false);
                ResetInertia();
            }
        }
        else
        {
            if (anim.GetBool("Walk") == true)
                anim.SetBool("Walk", false);
        }
    }

    private void FixedUpdate()
    {
        if (!Busy && !Riding)
        {
            if (!jumping)
                Move();
            else
            {
                if (GetComponent<Player>().ScarpeMolla)
                {
                    if (pressingJump)
                    {
                        AddToJump();
                    }
                }
                if (!gliding)
                    r.AddForce(Vector3.down*3000, ForceMode.Force);
                FallMove();
            }

            if (jump)
                Jump();
        }
    }

    private void Jump()
    {
        AudioUtility.PlaySound(audio);
        r.velocity = new Vector3(r.velocity.x, 0, r.velocity.z);
        r.AddForce(new Vector3(0, 900 * jumpForce, 0), ForceMode.Impulse);
        jump = false;
        jumping = true;
    }
    private void AddToJump()
    {
        GetComponent<Animator>().SetBool("Walk", false);
        if (jt > 0)
            jt -= Time.deltaTime;
        else
        {
            jt = 0;
        }
        r.AddForce(new Vector3(0, 700 * jt, 0), ForceMode.Impulse);
    }

    private void ToggleGlider()
    {
        getGAxisOnce = true;
        if (GetComponent<Player>().Glider)
        {
            if (!gliding)
            {
                Transform pmg = GameObject.Find("PlayerModelGroup").transform;
                pmg.localRotation = Quaternion.Euler(pmg.localEulerAngles + new Vector3(60, 0, 0));
                gliding = true;
                gliderInstance = Instantiate(glider);
                StartCoroutine(GliderPopUp());
                gliderInstance.transform.position = this.transform.position;
                gliderInstance.transform.parent = this.gameObject.transform;
                gliderInstance.transform.localPosition += new Vector3(0, 0, 0.33f);
                gliderInstance.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                
                GetComponent<Rigidbody>().drag = 7;
            }
            else
            {
                DestroyGlider();
            }
        }
    }

    IEnumerator GliderPopUp()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2;
            gliderInstance.transform.localScale = Vector3.one * (Mathf.Sin(16 * t) / (20 * t) + 1);
            yield return null;
        }
    }

    private void DestroyGlider()
    {
        Transform pmg = GameObject.Find("PlayerModelGroup").transform;
        pmg.localRotation = Quaternion.identity;
        gliding = false;
        Destroy(gliderInstance);
        gliderInstance = null;
        GetComponent<Rigidbody>().drag = 3;
    }
    private void ResetInertia()
    {
        inertiaT = Mathf.Lerp(inertiaT, 0, Time.deltaTime * 3);
        smoothSpeed = Mathf.Lerp(smoothSpeed, 0, Time.deltaTime * 3);
    }
    private void Move()
    {

        GetComponent<Rigidbody>().velocity = finalVec;


    }
    private void FallMove()
    {
        if (!gliding)
            GetComponent<Rigidbody>().AddForce(new Vector3(moveVec.x * 4000, 0, moveVec.y * 4000), ForceMode.Force);
        else
            GetComponent<Rigidbody>().AddForce(new Vector3(moveVec.x * 15000, 0, moveVec.y * 15000), ForceMode.Force);
    }
    private bool IsGrounded(GameObject obj)
    {
        if (r.velocity.y != 0)
        {
            bool[] points = { false, false };
            RaycastHit hit;
            if (Physics.Raycast(obj.transform.position + obj.transform.forward * 0.4f, Vector3.down, out hit, Mathf.Infinity, jumpMask))
            {
                if (hit.distance < landingDistance)
                    points[0] = true;
            }
            else if (Physics.Raycast(obj.transform.position - obj.transform.forward * 0.4f, Vector3.down, out hit, Mathf.Infinity, jumpMask))
            {
                if (hit.distance < landingDistance)
                    points[0] = true;
            }
            return points[0] || points[1];
        }
        else
            return true;
    }

    public void OnTriggerEnter(Collider other)
    {
        //if (SaveManager.gameLoaded)
        //{
        //    if (other.tag.Contains("MapArea"))
        //    {
        //        string num = other.tag.Substring(other.tag.IndexOf("MapArea") + 7);
        //        int n = int.Parse(num);
        //        if (n > Player.area && Player.area > 0)
        //        {
        //            arrow.gameObject.SetActive(true);
        //            arrow.target = GameObject.FindGameObjectWithTag("MapArea" + Player.area).transform;
        //            //PopUpUtility.Open(GetComponent<Player>().cameraInterface.popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.warning), ML.LocalizedMess(agent, ML.Mess.goBackToGraffitiArea), 2);
        //        }
        //        else
        //        {
        //            arrow.gameObject.SetActive(false);
        //        }
        //    }
        //}
    }
}
