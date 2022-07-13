using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptiveClip : MonoBehaviour
{
    public float targetFPS;
    public float[] samples;
    public static float mean;
    private int i;

    private void Awake()
    {
        if (samples.Length == 0)
            samples = new float[10];
    }
    private void Update()
    {
        if (i < samples.Length)
        {
            samples[i] = 1.0f / Time.deltaTime;
            i++;
        }
        else
        {
            mean = Mean(samples);
            if (mean < targetFPS)
                Decrease();
            else
                Increase();
            i = 0;
        }
    }

    private void Decrease()
    {
        if (GetComponent<Camera>().farClipPlane > 100)
            GetComponent<Camera>().farClipPlane -= (targetFPS-mean)/50;
    }

    private void Increase()
    {
        if (GetComponent<Camera>().farClipPlane < 500)
            GetComponent<Camera>().farClipPlane += (mean-targetFPS)/50;
    }

    private float Mean(float[] samples)
    {
        float sum = 0f;
        float length = samples.Length;
        foreach (float sample in samples)
        {
            sum += sample;
        }
        return sum / length;
    }
}
