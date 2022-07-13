using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

public enum Sequence { title, load, character, initialize }

public class SequenceManager : MonoBehaviour
{
    public static Sequence sequence;
    public LoadingManager loadingManager;
    public CharacterCustomization characterCustomization;
    public GameObject bullyCanvas;
    public GameObject customizeCanvas;
    public CameraInterface cameraInterface;
    public CharacterCustomizationSetup characterCustomizationSetup;

    void Start()
    {
        StartCoroutine(LoadingManager.LoadConfig());
        BeginSequence();
    }



    private void BeginSequence()
    {
        //StartCoroutine(API.SendLoginRequest(GetComponent<Player>(), "t4-user1", "t4-user1"));
        //StartCoroutine(API.PostAnnotationFinalC(GetComponent<Player>(), new SqlAnnotatedSentence()));
        //StartCoroutine(API.PostSaveFinalC(GetComponent<Player>(), true));
        cameraInterface.transitionCanvas.gameObject.SetActive(false);
        sequence = Sequence.title;
        Next();
    }

    private void StartCreation()
    {
        ChangeLayer(cameraInterface.gameObject, 26);
        cameraInterface.uicam.depth = 3;
        bullyCanvas.SetActive(true);
  
        customizeCanvas.SetActive(true);
        characterCustomizationSetup.Setup();
        cameraInterface.transitionCanvas.gameObject.SetActive(false);
        cameraInterface.uicam.enabled = false;
        cameraInterface.uicam.enabled = true;

        loadingManager.gameObject.SetActive(false);

        cameraInterface.uicam.Render();

        FindObjectOfType<CharacterCustomization>().UpdateStrings();
    }

    private void StartGame()
    {
        StartCoroutine(LoadingManager.LoadSystemDialogues());
        cameraInterface.uicam.depth = 1;
        GetComponent<Player>().enabled = true;
        ChangeLayer(cameraInterface.gameObject, 10);
        GetComponent<Player>().Initialize(loadingManager.SaveFound);
        bullyCanvas.SetActive(true);
    }

    public void Next()
    {
        if (sequence == Sequence.title)
        {
            cameraInterface.titleScreen.SetActive(true);
            cameraInterface.transitionCanvas.gameObject.SetActive(false);
            cameraInterface.loadingCanvas.gameObject.SetActive(false);
        }
        else if (sequence == Sequence.load)
        {
            cameraInterface.titleScreen.SetActive(false);
            cameraInterface.loadingCanvas.gameObject.SetActive(true);
            loadingManager.Activate();
        }
        else if (sequence == Sequence.character)
        {
            StartCreation();
        }
        else if (sequence == Sequence.initialize)
        {
            StartGame();
        }
        sequence += 1;
    }

    public void ChangeLayer(GameObject player, int layer)
    {
        player.layer = layer;
        foreach (Transform child in transform)
        {
            if (!child.name.Contains("Invisible"))
            {
                if (child.tag != "UI")
                {
                    child.gameObject.layer = layer;
                }
            }
        }
    }
}
