using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class Control : Singleton<Control>
{
    public Vector3 startPos;
    public Quaternion startRot;
    public GameObject token;
    public GameObject bully;

    //Camera
    public FollowPlayer followPlayer;
    public CameraOrbit cameraOrbit;

    //UI
    public WordByWord byWord;
    public LetterByLetter byLetter;
    public TerminalWindow terminalWindow;

    //Parameters
    public GameObject msgbox;
    public GameObject player;
    public Camera uicam;
    public GameObject hudCanvas;
    public GameObject customizeCanvas;
    public GameObject loadingCanvas;
    public AudioManager audioManager;
    public GameObject bullyCanvas;
    public GameObject thinkCloud;
    public GameObject nameCloud;
    public GameObject webDebugWindow;
    public GameObject ifParent;
    public GameObject expBar;
    public GameObject safeBar;
    public TextMeshProUGUI level;

    //Inventory
    public GameObject inventory;
    public InventoryS inv;

    //Phone
    public GameObject phone;
    public Image notBG;
    public TextMeshProUGUI notText;

    void Start ()
    {
        QualitySettings.SetQualityLevel(1, true);
    }

    private void OnApplicationQuit()
    {

    }


}
