using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataUtilities;

public class Intro : MonoBehaviour
{
    public GameObject scientist;
    public GameObject scientistHouseHandle;
    private Vector3 houseCamPos;
    GameObject eyes;
    public GameObject camPos;
    float t;
    bool blockTime;
    public GameObject zzz;
    public float sleepTime;
    private bool allSet;
    bool introSet;
    public bool overrideAdminSkip;
    Quaternion sitUp = new Quaternion(0, 1, 0, 0);
    Quaternion turn = new Quaternion(0, 1, 0, 0);
    private bool noPhone;
    private bool skipIntro;
    public bool skipIntroForAll;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void SetSkipIntro(bool v)
    {
        skipIntro = v;
    }

    // Update is called once per frame
    void Update()
    {
        if (LoadUtility.AllLoaded && !skipIntro)
        {
            if ((!Player.admin || overrideAdminSkip) && !skipIntroForAll) //intro shown
            {
                if (!introSet)
                {
                    //this.enabled = false;
                    scientistHouseHandle.SetActive(true);
                    houseCamPos = camPos.transform.position;
                    transform.position = GameObject.Find("PlayerPos").transform.position;
                    scientist.SetActive(true);
                    introSet = true;
                }

                if (GetComponent<Player>().StoryProgress < 1)
                {
                    if (!noPhone)
                    {
                        GetComponent<Player>().cameraInterface.phone.SetActive(false);
                        GetComponent<Player>().HideBattery();
                        noPhone = true;
                    }
                    if (GetComponent<Player>().initialized && !allSet)
                    {
                        transform.rotation = new Quaternion(-0.481617659f, 0.517730117f, 0.481617659f, 0.517730117f);
                        Camera.main.GetComponent<CameraOrbit>().enabled = false;
                        Camera.main.transform.position = this.transform.position;
                        GetComponent<CapsuleCollider>().isTrigger = true;
                        GetComponent<Rigidbody>().useGravity = false;
                        eyes = GameObject.FindGameObjectWithTag("ModelEyeL");
                        eyes.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("EyesClosed");
                        zzz.SetActive(true);
                        GetComponent<Movement>().Busy = true;
                        GetComponent<Movement>().enabled = false;
                        GetComponent<Animator>().SetBool("Sleep", true);

                        GetComponent<Player>().cameraInterface.cameraOrbit.SetCameraAnchor(houseCamPos, AnchorMode.Fixed, pan: true);
                        allSet = true;
                    }
                    if (allSet)
                    {
                        Camera.main.GetComponent<CameraOrbit>().enabled = true;
                        Camera.main.GetComponent<CameraOrbit>().zoomRange = 7;
                        if (!blockTime)
                        {
                            t += Time.deltaTime;
                        }
                        if (t > sleepTime)
                        {
                            //suona bzz
                            eyes.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Eyes0");
                            zzz.SetActive(false);
                            GetComponent<Animator>().SetBool("Sleep", false);
                            GetComponent<Animator>().SetBool("Sit", true);
                            this.enabled = false;
                            StartCoroutine(SitUp());
                        }
                    }
                }
            }
            else
            {
                transform.position = GameObject.Find("PlayerPosAdmin").transform.position;
                this.enabled = false;
            }           
        }
    }

    IEnumerator SitUp()
    {
        Quaternion startRot = transform.rotation;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2;
            transform.rotation = Quaternion.Lerp(startRot, sitUp, t);
            yield return null;
        }
        //yield return new WaitForSeconds(0.2f);
        //StartCoroutine(Turn());
        StartCoroutine(GetOffBed());

    }

    IEnumerator Turn()
    {
        Quaternion startRot = transform.rotation;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2;
            transform.rotation = Quaternion.Lerp(startRot, turn, t);
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(GetOffBed());
        
    }

    IEnumerator GetOffBed()
    {
        GetComponent<Animator>().SetBool("Sit", false);
        GetComponent<Animator>().SetBool("GetUp", true);

        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position - new Vector3(0, 0, 2);
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        GetComponent<Animator>().SetBool("GetUp", false);
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<CapsuleCollider>().isTrigger = false;
        GetComponent<Movement>().Busy = false;
        GetComponent<Movement>().enabled = true;
        yield return new WaitForSeconds(0.2f);
        MessageUtility.SingleBoxedMessage(GetComponent<Player>(), PlayerPrefs.GetString("Name"), ML.systemMessages.iShouldAnswer);

        GetComponent<Player>().StoryProgress = 1;
        SaveManager.SaveGameState(GetComponent<Player>());

        HintUtility.ShowHint(GetComponent<Player>(), "WASD", ML.systemMessages.WASDToMove, 10);
    }
}
