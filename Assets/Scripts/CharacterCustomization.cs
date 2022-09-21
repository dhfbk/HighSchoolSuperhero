using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using System.Xml.Serialization;
using System;
using System.IO;
using TMPro;
using System.Diagnostics;
using System.Linq;

[Serializable]
public class CharacterCustomization : MonoBehaviour
{
    public FittingRoom fittingScript;
    public bool doNotMoveModel;
    public bool skip;
    public GameObject[] arrows;
    public float value;
    public AudioClip clickSound, hoverSound;
    public AudioSource audio;
    public Color col;
    Material toChange;
    bool move;
    Vector3 dest;
    public GameObject text;
    GameObject currentItem;
    public GameObject model;
    Material[] mats;
    public GameObject hat, hair, shirt, pants, shoes, glasses, lenses, eyes, body;
    int maxHair, maxShirt, maxEyes, maxGlasses, maxPants, maxShoes, maxBody;
    public Text hatT, hairT, bodyT, pantsT, chestT, eyesT, glassesT, shoesT, doneT, createT, confirmT;
    public GameObject nameObj;
    public TMP_InputField namebox;
    public TextMeshProUGUI nameboxPH;
    Dictionary<string, int> currentNumber;

    Mesh mesh;
    Vector3 hairpos, hairrot;
    int i;
    int typeMax;
    bool done;
    public GameObject confirmButton;
    public GameObject sliderObj;
    public GameObject camPivot;
    Vector3 startDest;
    List<Mesh> hairPool = new List<Mesh>();
    List<Mesh> shirtPool = new List<Mesh>();
    List<Mesh> pantsPool = new List<Mesh>();
    List<Mesh> glassesPool = new List<Mesh>();
    List<Mesh> shoesPool = new List<Mesh>();
    List<Mesh> eyesPool = new List<Mesh>();

    private Dictionary<string, Material> memMaterial;

    public static Stopwatch csw = new Stopwatch();
    // Use this for initialization

    void Start()
    {
        memMaterial = new Dictionary<string, Material>();
        memMaterial["Shirt"] = Resources.Load<Material>("Materials/GenericRed");
        memMaterial["Pants"] = Resources.Load<Material>("Materials/GenericWhite");
        memMaterial["Shoes"] = Resources.Load<Material>("Materials/GenericGreen");
        Player agent = transform.root.GetComponent<CameraInterface>().player;
        csw.Start();

            nameboxPH.text = "...";
        UpdateStrings();
        //EditorVocab vocab = SaveManager.Load<EditorVocab>(Application.streamingAssetsPath + "")
        if (skip)
            ConfirmButton();
        else
        {
            dest = new Vector3(2F, 10F, -13F);
            startDest = dest;

            sliderObj = GameObject.FindGameObjectWithTag("Slider");

            //Initialize customization parts
            LoadParts();
            InitializePartIndeces();

        }

    }
    public void InitializePartIndeces()
    {
        currentNumber = new Dictionary<string, int>();
        currentNumber["Hair"] = currentNumber["Eyes"] = currentNumber["Glasses"] = currentNumber["Pants"] = currentNumber["Shoes"] = currentNumber["Shirt"] = 0;

    }
    public void LoadParts()
    {
        maxHair = maxShirt = maxEyes = maxGlasses = maxPants = maxShoes = maxBody = 0;
        Mesh[] objA = Resources.LoadAll<Mesh>("Parts/Player");

        foreach (Mesh obj in objA)
        {
            if (obj.name.Contains("Hair"))
            {
                maxHair += 1;
                hairPool.Add(obj);
            }
            else if (obj.name.Contains("Eyes"))
            {
                maxEyes += 1;
                eyesPool.Add(obj);
            }
            else if (obj.name.Contains("Glasses"))
            {
                maxGlasses += 1;
                glassesPool.Add(obj);
            }
            else if (obj.name.Contains("Shirt"))
            {
                maxShirt += 1;
                shirtPool.Add(obj);
            }
            else if (obj.name.Contains("Pants"))
            {
                maxPants += 1;
                pantsPool.Add(obj);
            }
            else if (obj.name.Contains("Shoes"))
            {
                maxShoes += 1;
                shoesPool.Add(obj);
            }
        }
    }
    public void UpdateStrings()
    {
        hairT.text = ML.systemMessages.hair;
        chestT.text = ML.systemMessages.chest;
        pantsT.text = ML.systemMessages.pants;
        eyesT.text = ML.systemMessages.eyes;
        glassesT.text = ML.systemMessages.glasses;
        shoesT.text = ML.systemMessages.shoes;
        bodyT.text = ML.systemMessages.body;
        doneT.text = ML.systemMessages.editorDone;
        createT.text = ML.systemMessages.editorCreate;
        confirmT.text = ML.systemMessages.editorConfirm;
    }
    public void Hover()
    {
        if (hoverSound)
        {
            audio.clip = hoverSound;
            audio.Play();
        }
    }

    public void ShowArrows()
    {
        arrows[0].SetActive(true);
        arrows[1].SetActive(true);
    }
    public void HideArrows()
    {
        arrows[0].SetActive(false);
        arrows[1].SetActive(false);
    }

    public void ShowSkin()
    {
        HideArrows();
        text.transform.GetChild(0).GetComponent<Text>().text = "Corpo";
        

        text.transform.GetChild(0).GetComponent<Text>().text = "Altezza";
        //sliderObj.SetActive(true);
        text.GetComponent<Text>().text = "";
        dest = startDest + (transform.position - startDest).normalized;// new Vector3(1.5F, 0F, 7.0F);
        currentItem = body;

        typeMax = maxBody;
        confirmButton.SetActive(false);

        audio.clip = clickSound;
        audio.Play();
    }

    public void ShowHead()
    {
        ShowArrows();
        text.transform.GetChild(0).GetComponent<Text>().text = "Capelli";
        text.transform.GetComponent<Text>().text = currentNumber["Hair"].ToString();
        i = currentNumber["Hair"];

        dest = startDest + ((transform.position-new Vector3(0,1.5f,0)) - startDest).normalized*2;
        currentItem = hair;
        typeMax = maxHair;
        confirmButton.SetActive(false);

        audio.clip = clickSound;
        audio.Play();
    }

    public void ShowTorso()
    {
        ShowArrows();
        text.transform.GetChild(0).GetComponent<Text>().text = "Maglia";
        text.transform.GetComponent<Text>().text = currentNumber["Shirt"].ToString();
        i = currentNumber["Shirt"];

        dest = startDest + ((transform.position + new Vector3(0, 1, 0)) - startDest).normalized*2;
        currentItem = shirt;
        
        typeMax = maxShirt;
        confirmButton.SetActive(false);

        audio.clip = clickSound;
        audio.Play();
    }

    public void ShowLegs()
    {
        ShowArrows();
        text.transform.GetChild(0).GetComponent<Text>().text = "Pantaloni";
        text.transform.GetComponent<Text>().text = currentNumber["Pants"].ToString();
        i = currentNumber["Pants"];

        dest = startDest + ((transform.position + new Vector3(0, 1.5f, 0)) - startDest).normalized*2;
        currentItem = pants;
        typeMax = maxPants;
        confirmButton.SetActive(false);

        audio.clip = clickSound;
        audio.Play();
    }

    public void ShowShoes()
    {
        ShowArrows();
        text.transform.GetChild(0).GetComponent<Text>().text = "Scarpe";
        text.transform.GetComponent<Text>().text = currentNumber["Shoes"].ToString();
        i = currentNumber["Shoes"];

        dest = startDest + ((transform.position + new Vector3(0, 2, 0)) - startDest).normalized*2;
        currentItem = shoes;
        typeMax = maxShoes;
        confirmButton.SetActive(false);

        audio.clip = clickSound;
        audio.Play();
    }

    public void ShowGlasses()
    {
        ShowArrows();
        text.transform.GetChild(0).GetComponent<Text>().text = "Occhiali";
        text.transform.GetComponent<Text>().text = currentNumber["Glasses"].ToString();
        i = currentNumber["Glasses"];

        dest = startDest + ((transform.position - new Vector3(0, 1, 0)) - startDest).normalized*2;
        currentItem = glasses;
        typeMax = maxGlasses;
        confirmButton.SetActive(false);

        audio.clip = clickSound;
        audio.Play();
    }

    public void ShowEyes()
    {
        ShowArrows();
        text.transform.GetChild(0).GetComponent<Text>().text = "Occhi";
        text.transform.GetComponent<Text>().text = currentNumber["Eyes"].ToString();
        i = currentNumber["Eyes"];

        dest = startDest + ((transform.position - new Vector3(0, 1, 0)) - startDest).normalized*2;
        currentItem = eyes;
        typeMax = maxEyes;
        confirmButton.SetActive(false);

        audio.clip = clickSound;
        audio.Play();
    }

    public void Back()
    {
        dest = startDest;
        foreach (Transform button in transform)
        {
            if (button.gameObject.tag == "EditButton" || button.gameObject.tag == "Slider")
            {
                button.gameObject.SetActive(false);
            }
        }
        confirmButton.SetActive(true);
        done = true;
    }

    public void ChangeColor(Material mat)
    {
        SkinnedMeshRenderer smr = currentItem.GetComponent<SkinnedMeshRenderer>();
        List<string> notChangeable = new List<string>() { "Tex", "Jeans" };
        if (!notChangeable.Any(smr.sharedMesh.name.Contains))
        {
            if (currentItem == eyes)
                smr.materials = new Material[4] { smr.materials[0],
                smr.materials[1],
                mat,
                smr.materials[3]};
            else if (currentItem == shirt)
            {
                if (smr.sharedMesh.name.Contains("Open"))
                    smr.materials = new Material[2] { mat, (Material)Resources.Load<Material>("Materials/GenericWhite") };
                else
                    smr.materials = new Material[1] { mat };
            }
            else
                smr.materials = new Material[1] { mat };
            memMaterial[currentItem.name] = mat;
        }
        
    }

    public void TypeRight()
    {
        if (i == typeMax-1)
        {
            i = 0;
        }
        else
        {
            i += 1;
        }
        text.GetComponent<Text>().text = i.ToString();
        currentNumber[currentItem.name] = i;
        if (typeMax > 0)
        {
            switch (currentItem.name)
            {
                case "Eyes":
                    mesh = eyesPool[i];
                    break;
                case "Hair":
                    mesh = hairPool[i];
                    break;
                case "Shirt":
                    mesh = shirtPool[i];
                    break;
                case "Pants":
                    mesh = pantsPool[i];
                    break;
                case "Shoes":
                    mesh = shoesPool[i];
                    break;
                case "Glasses":
                    mesh = glassesPool[i];
                    break;
            }
            //mesh = Resources.Load<Mesh>("Parts/Player/" + currentItem.name + i);
            SkinnedMeshRenderer smr = currentItem.GetComponent<SkinnedMeshRenderer>();
            smr.sharedMesh = mesh;
            if (smr.sharedMesh.name.Contains("Fant"))
            {
                smr.materials = new Material[1] { (Material)Resources.Load<Material>("Materials/Fant") };
            }
            else if (smr.sharedMesh.name.Contains("Tex") && smr.sharedMesh.name.Contains("Open"))
            {
                smr.materials = new Material[2] { (Material)Resources.Load<Material>("Materials/Characters"), (Material)Resources.Load<Material>("Materials/GenericWhite") };
            }
            else if (smr.sharedMesh.name.Contains("Open") && smr.sharedMesh.name.Contains("Jeans"))
            {
                smr.materials = new Material[2] { (Material)Resources.Load<Material>("Materials/Jeans"), (Material)Resources.Load<Material>("Materials/GenericWhite") };
            }
            else if (smr.sharedMesh.name.Contains("Open"))
            {
                smr.materials = new Material[2] { memMaterial[currentItem.name], (Material)Resources.Load<Material>("Materials/GenericWhite") };
            }
            else if (smr.sharedMesh.name.Contains("Jeans"))
            {
                smr.materials = new Material[1] { (Material)Resources.Load<Material>("Materials/Jeans") };
            }
            else
                smr.materials = new Material[1] { memMaterial[currentItem.name] };
        }
        if (currentItem.name == "Glasses")
            CheckGlasses();
    }

    public void TypeLeft()
    {
        if (i == 0)
        {
            i = typeMax-1;
        }
        else
        {
            i -= 1;
        }
        text.GetComponent<Text>().text = i.ToString();
        currentNumber[currentItem.name] = i;
        if (typeMax > 0)
        {
            switch (currentItem.name)
            {
                case "Eyes":
                    mesh = eyesPool[i];
                    break;
                case "Hair":
                    mesh = hairPool[i];
                    break;
                case "Shirt":
                    mesh = shirtPool[i];
                    break;
                case "Pants":
                    mesh = pantsPool[i];
                    break;
                case "Shoes":
                    mesh = shoesPool[i];
                    break;
                case "Glasses":
                    mesh = glassesPool[i];
                    break;
            }
            SkinnedMeshRenderer smr = currentItem.GetComponent<SkinnedMeshRenderer>();
            smr.sharedMesh = mesh;
            if (smr.sharedMesh.name.Contains("Fant"))
            {
                smr.materials = new Material[1] { (Material)Resources.Load<Material>("Materials/Fant") };
            }
            else if (smr.sharedMesh.name.Contains("Tex") && smr.sharedMesh.name.Contains("Open"))
            {
                smr.materials = new Material[2] { (Material)Resources.Load<Material>("Materials/Characters"), (Material)Resources.Load<Material>("Materials/GenericWhite") };
            }
            else if (smr.sharedMesh.name.Contains("Open") && smr.sharedMesh.name.Contains("Jeans"))
                smr.materials = new Material[2] { (Material)Resources.Load<Material>("Materials/Jeans"), (Material)Resources.Load<Material>("Materials/GenericWhite") };
            else if (smr.sharedMesh.name.Contains("Open"))
                smr.materials = new Material[2] { memMaterial[currentItem.name], (Material)Resources.Load<Material>("Materials/GenericWhite") };
            else if (smr.sharedMesh.name.Contains("Jeans"))
            {
                smr.materials = new Material[1] { (Material)Resources.Load<Material>("Materials/Jeans") };
            }
            else
                smr.materials = new Material[1] { memMaterial[currentItem.name] };
        }
        if (currentItem.name == "Glasses")
            CheckGlasses();
    }

    public void CheckGlasses()
    {
        lenses.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Parts/Player/Lenses" + currentNumber["Glasses"]);
    }

    public void ConfirmButton()
    {
        Player player = transform.root.GetComponent<CameraInterface>().player;
        csw.Stop();

        if (!doNotMoveModel)
        {
            if (String.IsNullOrEmpty(namebox.text))
            {
                PopUpUtility.Open(transform.root.GetComponent<CameraInterface>().popUpCanvas, 
                    PopUpType.LocalizedType(player, PopUpType.Types.warning),
                    ML.systemMessages.insertYourNameFirst, 
                    2);
                return;
            }

            PlayerPrefs.SetString("Name", namebox.text);
        }

        transform.gameObject.SetActive(false);
        player.avatar.GetComponent<Parts>().body.GetComponent<SkinnedMeshRenderer>().sharedMesh = body.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        player.avatar.GetComponent<Parts>().body.GetComponent<SkinnedMeshRenderer>().material.color = body.GetComponent<SkinnedMeshRenderer>().material.color;
        player.avatar.GetComponent<Parts>().hair.GetComponent<SkinnedMeshRenderer>().sharedMesh = hair.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        player.avatar.GetComponent<Parts>().hair.GetComponent<SkinnedMeshRenderer>().material.color = hair.GetComponent<SkinnedMeshRenderer>().material.color;

        player.avatar.GetComponent<Parts>().shirt.GetComponent<SkinnedMeshRenderer>().sharedMesh = shirt.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        player.avatar.GetComponent<Parts>().shirt.GetComponent<SkinnedMeshRenderer>().materials[0].mainTexture = shirt.GetComponent<SkinnedMeshRenderer>().materials[0].mainTexture;
        player.avatar.GetComponent<Parts>().shirt.GetComponent<SkinnedMeshRenderer>().materials[0].color = shirt.GetComponent<SkinnedMeshRenderer>().materials[0].color;
        
        player.avatar.GetComponent<Parts>().glasses.GetComponent<SkinnedMeshRenderer>().sharedMesh = glasses.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        player.avatar.GetComponent<Parts>().glasses.GetComponent<SkinnedMeshRenderer>().material.color = glasses.GetComponent<SkinnedMeshRenderer>().material.color;
        player.avatar.GetComponent<Parts>().lenses.GetComponent<SkinnedMeshRenderer>().sharedMesh = lenses.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        player.avatar.GetComponent<Parts>().lenses.GetComponent<SkinnedMeshRenderer>().material.color = lenses.GetComponent<SkinnedMeshRenderer>().material.color;

        player.GetComponent<Parts>().hair.GetComponent<SkinnedMeshRenderer>().sharedMesh = hair.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        player.GetComponent<Parts>().hair.GetComponent<SkinnedMeshRenderer>().material.color = hair.GetComponent<SkinnedMeshRenderer>().material.color;
        player.GetComponent<Parts>().shirt.GetComponent<SkinnedMeshRenderer>().sharedMesh = shirt.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        player.GetComponent<Parts>().shirt.GetComponent<SkinnedMeshRenderer>().material.color = shirt.GetComponent<SkinnedMeshRenderer>().material.color;
        player.GetComponent<Parts>().glasses.GetComponent<SkinnedMeshRenderer>().sharedMesh = glasses.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        player.GetComponent<Parts>().glasses.GetComponent<SkinnedMeshRenderer>().material.color = glasses.GetComponent<SkinnedMeshRenderer>().material.color;
        player.GetComponent<Parts>().lenses.GetComponent<SkinnedMeshRenderer>().sharedMesh = lenses.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        player.GetComponent<Parts>().lenses.GetComponent<SkinnedMeshRenderer>().material.color = lenses.GetComponent<SkinnedMeshRenderer>().material.color;

        if (!doNotMoveModel)
        {
            player.GetComponent<SequenceManager>().Next();
        }
        else
        {
            fittingScript.Toggle(fittingScript.Agent);
        }
    }
    
    public void SetActive(bool slideract)
    {
        //if (done == true)
        //{
            foreach (Transform button in transform)
            {
                if (button.gameObject.tag == "EditButton")
                {
                    button.gameObject.SetActive(true);
                }
            }
            if (slideract)
            {
                //sliderObj.SetActive(true);
                GameObject.Find("ArrowLeft").SetActive(false);
                GameObject.Find("ArrowRight").SetActive(false);
            }
            else
            {
                //sliderObj.SetActive(false);
            }
        //}
        confirmButton.SetActive(false);
    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    col = eventData.pointerCurrentRaycast.gameObject.GetComponent<Image>().color;
    //}
    void Update()
    {
        if (!doNotMoveModel)
            model.transform.position = Vector3.MoveTowards(model.transform.position, dest, Time.deltaTime * 5);

        if (text.transform.GetChild(0).GetComponent<Text>().text != "Altezza")
        {
            //sliderObj.SetActive(false);
        }
    }

    public void ChangeHeight(float value)
    {
        //Control.Instance.player.transform.localScale = new Vector3(value / 4 + 1f, value / 4 + 1f, value / 4 + 1f);
    }
}