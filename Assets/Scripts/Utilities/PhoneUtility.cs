using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class PhoneUtility : MonoBehaviour
{
    //Phone
    static Vector2 phonePosUp;
    static Vector2 phonePosDown;
    static Vector2 phonePosDownExt;
    static Vector2 phonePosUpExt;
    static Vector2 phonePosHidden;
    static Vector2 destCoords1;
    static Vector2 destCoords2;
    static Vector2 phoneStartPos;
    static Vector2 phoneStartScale;
    static Vector2 phoneDestScale;
    public static bool phoneOut;
    static float t;

    //Event
    public delegate void PhoneAction(Player agent);
    public static event PhoneAction phoneUp;
    public static event PhoneAction phoneDown;
    public static event PhoneAction phoneHide;

    // Start is called before the first frame update
    void Start()
    {
        if (Player.condition == Condition.NoW3D)
            this.gameObject.SetActive(false);
        else
        {
            phonePosDown = transform.localPosition;
            phonePosDownExt = phonePosDown - new Vector2(0, 30);

            phonePosUpExt = new Vector2(phonePosDown.x, -150);
            phonePosUp = new Vector2(phonePosDown.x, -180);

            phonePosHidden = phonePosDown - new Vector2(0, 100);

            destCoords1 = phonePosDownExt;
            destCoords2 = phonePosDown;
            phoneStartPos = phonePosDown;
            phoneStartScale = transform.localScale;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (t < 1)
        {
            t += Time.deltaTime * 7;
            transform.localPosition = Vector3.Lerp(phoneStartPos, destCoords1, t);
            transform.localScale = Vector3.Lerp(phoneStartScale, phoneDestScale, t);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, destCoords2, Time.deltaTime * 10);
            transform.localScale = Vector3.Lerp(transform.localScale, phoneStartScale, Time.deltaTime * 10);
        }
    }

    public static void Up(Player agent)
    {
        phoneOut = true;
        t = 0;
        destCoords1 = phonePosUpExt;
        destCoords2 = phonePosUp;
        phoneStartPos = agent.cameraInterface.phone.transform.localPosition;
        phoneDestScale = phoneStartScale * 1.2f;
        agent.cameraInterface.phone.GetComponent<PhoneComponents>().content.transform.localPosition = Vector2.zero;
        phoneUp(agent);
    }

    public static void Down(Player agent)
    {
        phoneOut = false;
        t = 0;
        destCoords1 = phonePosDownExt;
        destCoords2 = phonePosDown;
        phoneStartPos = agent.cameraInterface.phone.transform.localPosition;
        phoneDestScale = phoneStartScale * 0.8f;
        agent.cameraInterface.phone.GetComponent<PhoneComponents>().content.transform.localPosition = new Vector2(0, 90);
        phoneDown(agent);
    }

    public static void Hide(Player agent)
    {
        t = 0;
        destCoords1 = phonePosHidden;
        destCoords2 = phonePosHidden;
        phoneStartPos = agent.cameraInterface.phone.transform.localPosition;
        phoneDestScale = phoneStartScale * 0.5f;
    }
}
