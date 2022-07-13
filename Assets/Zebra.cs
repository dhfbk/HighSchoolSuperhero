using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zebra : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            other.GetComponentInChildren<Player>().ToggleOnZebra();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            other.GetComponentInChildren<Player>().ToggleOnZebra();
    }
}
