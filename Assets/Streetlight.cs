using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Streetlight : MonoBehaviour
{
    public GameObject light;
    private void Start()
    {
        GameObject.Find("Sun").GetComponent<DayNight>().change.AddListener(Trigger);
    }
    public void Trigger()
    {
        light.SetActive(!light.activeSelf);
    }
}
