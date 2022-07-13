using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Car : MonoBehaviour
{
    public Player Agent;
    //Oggetti
    [Header("Destinations")]
    public List<Vector3> destinations;
    
    [Header("Steering wheel")]
    public GameObject leftWheel;
    public GameObject rightWheel;
    public GameObject leftRearWheel;
    public GameObject rightRearWheel;
    public float speed;
     
    //Quaternioni
    Quaternion carTargetRotation;
    Quaternion carStartRotation;

    //Logic
    Coroutine lastCoroutine;
    public int i;
    float time;
    float rotOffset;
    float turnOffset;
    static float maxSpeed = 17;

    GameObject player;

    //Sound FX
    public AudioClip horn;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        speed = maxSpeed;
        if (transform.parent)
        {
            destinations = new List<Vector3>();
            foreach (Transform t in transform.parent)
                if (t.tag == "Node")
                    destinations.Add(t.gameObject.transform.position);
        }

        carTargetRotation = transform.rotation; //target direction è uguale a current direction all'inizio
    }

    void Update()
    {
        //SinglePlayer only
        if (Vector3.Distance(player.transform.position, transform.position) < 100)
        {
            rotOffset += speed / 2;
            transform.position += transform.forward * Time.deltaTime * speed; //vai in direzione forward

            if (Vector3.Distance(transform.position, destinations[i]) < 3.5f) //Se raggiungi una tappa
            {
                carTargetRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 90, 0)); //Turn more

                if (i == destinations.Count - 1)
                    i = 0;
                else
                    i++;
                time = 0; //reset prencetuale di rotazione macchina e ruote
                carStartRotation = transform.rotation; //setta rotazione attuale come iniziale
            }

            //Rotazione macchina
            if (time < 1)
            {
                time += Time.deltaTime * speed / 10;
                transform.rotation = Quaternion.Lerp(carStartRotation, carTargetRotation, time);
                //Usando percentuale. Uso mix di quaternion (carStartRotation) e Vector3 trasformato in Quaternion (carTargetRotation)
                //per decidere i gradi a piacere, difficile solo coi quaternion
            }
            else
            {
                transform.LookAt(destinations[i]);
            }
            //Rotazione ruote
            if (time < 1) //uso il tempo di rotazione della macchina anche se per le ruote uso un tipo di rotazione non basata sulla percentuale
                turnOffset = Mathf.Lerp(turnOffset, 30, Time.deltaTime * 3); //Usando interpolazione morbida
            else
                turnOffset = Mathf.Lerp(turnOffset, 0, Time.deltaTime * 3); //Usando interpolazione morbida

            leftWheel.transform.localRotation = rightWheel.transform.localRotation = Quaternion.Euler(new Vector3(rotOffset, turnOffset, 0));
            leftRearWheel.transform.localRotation = rightRearWheel.transform.localRotation = Quaternion.Euler(new Vector3(rotOffset, 0, 0));
        }
    }


    //Stop
    public void Stop()
    {
        if (Agent)
        {
            PlayHorn();
        }
        if (lastCoroutine != null)
            StopCoroutine(lastCoroutine);
        lastCoroutine = StartCoroutine(Decelerate());
    }
    public void Restart()
    {
        if (lastCoroutine != null)
            StopCoroutine(lastCoroutine);
        lastCoroutine = StartCoroutine(Accelerate());
    }
    private IEnumerator Accelerate()
    {
        float startSpeed = speed;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime/(5);
            speed = Mathf.Lerp(startSpeed, maxSpeed, t);
            yield return null;
        }
    }
    private IEnumerator Decelerate()
    {
        float startSpeed = speed;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime*(speed/2);
            speed = Mathf.Lerp(startSpeed, 0, t);
            yield return null;
        }
    }

    private void PlayHorn()
    {
        if (!Agent.IsOnZebra())
        {
            Agent.cameraInterface.FX.clip = horn;
            Agent.cameraInterface.FX.Play();
        }
    }
}
