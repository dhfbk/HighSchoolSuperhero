using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DayNight : MonoBehaviour
{
    public UnityEvent change;
    public enum DTime { day, night }
    DTime time = DTime.day;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        change.AddListener(Trigger);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, speed*Time.deltaTime/5), Space.World);


        float dot = Vector3.Dot(transform.forward, Vector3.down);

        if (dot >= 0)
        {
            GetComponent<Light>().color = new Color(1, 1f * (float)System.Math.Tanh(dot) + 0.25f, 0.92f * (float)System.Math.Tanh(dot) + 0.3f, 1);
            RenderSettings.fogColor = new Color(2 - (dot * 1.5f), dot, dot, 1);
            RenderSettings.ambientIntensity = 1.2f - (0.5f - dot / 2);
        }
        else
        {

            RenderSettings.fogColor = Color.Lerp(new Color(0.8f, 0f, 0f, 1), new Color(0.25f, 0.25f, 0.25f, 1), Mathf.Abs(dot));
        }

        if (dot < 0)
        {
            if (time == DTime.day)
            {
                change.Invoke();
                time = DTime.night;
            }
        }
        else if (dot > 0)
        {
            if (time == DTime.night)
            {
                change.Invoke();
                time = DTime.day;
            }
        }
    }

    void Trigger()
    {
        GetComponent<Light>().intensity = GetComponent<Light>().intensity == 0 ? 1 : 0;
    }
}
