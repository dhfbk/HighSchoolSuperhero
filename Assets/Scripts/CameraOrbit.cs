using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AnchorMode { Smooth = 1, Fixed = 2 }
public class CameraOrbit : MonoBehaviour
{
    public bool sm3dworld;
    public Vector3 targetRot;
    public Player targetAgent;
    public GameObject target;
    RaycastHit hit;
    Ray ray;
    public float distance, zoomRange;
    public static Vector3 maxDistanceCoords, startPos;
    public LayerMask layerMask;
    public float camRadius;
    private bool anchored;
    private bool locked;
    public bool IsLocked { get => locked; set { locked = value; } }
    Vector3 overrideCameraAnchor;
    Vector3 camDummy;
    float camOffsety;
    float camOffsetz;

    void Start()
    {
        if (!target)
            target = GameObject.FindGameObjectWithTag("Player");
        maxDistanceCoords = new Vector3(-13f, 13f, 0f);
        transform.position = transform.root.position;
        distance = Vector3.Distance(maxDistanceCoords, Vector3.zero);
        if (anchored)
        {
            transform.parent.eulerAngles = new Vector3(0, -90, -15);
        }
        else
        {         
            startPos = maxDistanceCoords;
        }
        startPos = maxDistanceCoords;
        zoomRange = 12;
    }

    void LateUpdate()
    {
        if (!anchored && !locked) {
            if (sm3dworld) {
                targetRot = new Vector3(0, -90, 0);
                transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, Quaternion.Euler(targetRot), Time.deltaTime);
            }

            if (!targetAgent.GetComponent<Movement>().Busy) {
                if (Input.GetKey(KeyCode.Keypad4)) {
                    transform.parent.transform.eulerAngles += new Vector3(0, -1, 0);
                }
                else if (Input.GetKey(KeyCode.Keypad6)) {
                    transform.parent.transform.eulerAngles += new Vector3(0, 1, 0);
                }
            }

            if (!targetAgent.GetComponent<Movement>().Frozen) {
                if (!sm3dworld && Player.overlays <= 0) {
                    //Rotate
                    if (Input.GetMouseButton(1)) {
                        float valueX = Input.GetAxis("Horizontal") / 5;
                        float valueY = Input.GetAxis("Vertical") / 5;
                        transform.parent.transform.eulerAngles += new Vector3(0, valueX, 0);

                        if (transform.parent.transform.eulerAngles.z + valueY < 85 || transform.parent.transform.eulerAngles.z + valueY > 315) {
                            transform.parent.transform.eulerAngles += new Vector3(0, 0, valueY);
                        }
                    }
                }
            }
            camDummy = transform.position - new Vector3(0, camOffsety, camOffsetz);
            ray = new Ray(target.transform.position, (camDummy - target.transform.position) * distance);
            if (Physics.Raycast(ray, out hit, distance, layerMask)) {
                Pull(hit.point);
            }
            else {
                ResetPos();
            }
        }
        else if (!locked) {
            if (overrideCameraAnchor != null)
                transform.position = Vector3.Lerp(transform.position, overrideCameraAnchor, Time.deltaTime * 2);
            transform.LookAt(target.transform);

        }

        if (!targetAgent.GetComponent<Movement>().Frozen) {
            //Zoom
            if (Input.mouseScrollDelta.y != 0) {
                float delta = Input.mouseScrollDelta.y;
                Vector3 newPos = maxDistanceCoords - new Vector3(-delta, delta, 0).normalized;
                if (Vector3.Distance(newPos, startPos) < zoomRange) {
                    maxDistanceCoords = newPos;
                }
                distance = Vector3.Distance(maxDistanceCoords, Vector3.zero);
            }
        }
    }

    public void Pull(Vector3 point)
    {
        Vector3 newPoint = point + Vector3.ClampMagnitude(target.transform.position - point, camRadius);
        if (Vector3.Distance(point, target.transform.position) > 3.0f) {
            camOffsety = 0;
            camOffsetz = 0;
            transform.position = newPoint;
        }
        else {
            camOffsety = 5;
            camOffsetz = camRadius/2;
            transform.position = newPoint + new Vector3(0, camOffsety, camOffsetz);
        }
        transform.LookAt(target.transform);
    }

    public void ResetPos()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, maxDistanceCoords, Time.deltaTime * 3);
        transform.LookAt(target.transform);

    }

    public void SetCameraLock(bool state)
    {
        locked = state;
    }

    public void SetCameraAnchor(Vector3 anchor, AnchorMode mode, bool pan)
    {
        overrideCameraAnchor = anchor;
        if (mode == AnchorMode.Fixed) {
            target = transform.root.gameObject.GetComponent<FollowPlayer>().target.gameObject;
            transform.root.gameObject.GetComponent<FollowPlayer>().enabled = false;
        }
        if (!pan)
            transform.position = overrideCameraAnchor;
        anchored = true;
    }
    public void SetCameraAnchor(bool set)
    {
        if (set) {
            anchored = true;
            transform.root.GetComponent<FollowPlayer>().enabled = false;
            target = target.GetComponent<FollowPlayer>().target;
        }
        else {
            anchored = false;
            transform.root.GetComponent<FollowPlayer>().enabled = true;
            target = transform.root.gameObject;
        }
    }
    public void CameraLook(GameObject newTarget, int seconds)
    {
        targetAgent.GetComponent<Movement>().Busy = true;
        GameObject oldTarget = target;
        target = newTarget;
        StartCoroutine(WaitForLook(oldTarget, seconds));
    }
    private IEnumerator WaitForLook(GameObject oldTarget, int seconds)
    {
        yield return new WaitForSeconds(seconds);
        target = oldTarget;
        targetAgent.GetComponent<Movement>().Busy = false;
    }

    public void CameraPan(GameObject newTarget, int seconds, bool busy = false)
    {
        targetAgent.GetComponent<Movement>().Busy = true;
        targetAgent.GetComponent<Movement>().Frozen = true;
        GameObject oldTarget = transform.root.GetComponent<FollowPlayer>().target;
        transform.root.GetComponent<FollowPlayer>().target = newTarget;
        transform.root.GetComponent<FollowPlayer>().speed /= 5.0f; 
        StartCoroutine(WaitForPan(oldTarget, seconds, busy));
    }
    private IEnumerator WaitForPan(GameObject oldTarget, int seconds, bool busy)
    {
        yield return new WaitForSeconds(seconds);
        transform.root.GetComponent<FollowPlayer>().target = oldTarget;
        Invoke("ResetFollowSpeed", 1.5f);
        targetAgent.GetComponent<Movement>().Busy = busy;
        targetAgent.GetComponent<Movement>().Frozen = busy;
    }

    private void ResetFollowSpeed()
    {
        //targetAgent.GetComponent<Movement>().Busy = false;
        transform.root.GetComponent<FollowPlayer>().speed *= 10.0f;
    }
}