using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTrigger : MonoBehaviour
{
    private int obstacles;
    public Car car;
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") || col.CompareTag("Scooter"))
        {
            obstacles += 1;
            car.Agent = col.GetComponent<Player>();
            car.Stop();
        }
        else if (col.CompareTag("Car") || col.CompareTag("NPC"))
        {
            obstacles += 1;
            car.Stop();
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player") || col.CompareTag("Scooter"))
        {
            obstacles -= 1;
            car.Agent = null;
            if (obstacles <= 0)
                car.Restart();
        }
        else if (col.CompareTag("Car") || col.CompareTag("NPC"))
        {
            obstacles -= 1;
            if (obstacles <= 0)
                car.Restart();
        }
    }
}
