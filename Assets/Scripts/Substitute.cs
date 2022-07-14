using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Substitute : MonoBehaviour
{
    public GameObject canvas;
    public InputField iF;
    public GameObject iFobj;
    public GameObject token;
    public List<GameObject> tokens;
    public bool update;
    public bool subbing;
    public TouchScreenKeyboard kb;
    public DialogueInstancer bully;
    public WordByWord byWord;
    public AnnotatedDoc anndoc;
    public string memtoken;
    public char[] separators = new char[] { ' ', ',', '\n' };
    public AnnotationData dummySentence;

    void Start()
    {
        bully = GetComponent<DialogueInstancer>();
        byWord = GetComponent<WordByWord>();
        canvas = transform.GetChild(0).gameObject;
        //ripristino canvas = transform.parent.parent.GetComponent<WordByWord>().canvas;
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (EventSystem.current.currentSelectedGameObject.name.Contains("Token"))
            {
                token = EventSystem.current.currentSelectedGameObject;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (EventSystem.current.currentSelectedGameObject.name.Contains("Token"))
                {
                    Sub();
                }
            }
        }
        if (Input.GetKeyDown("return"))
        {
            if (subbing)
            {
                //Substitute
                //store token
                token.GetComponent<Annotation>().memtoken = token.transform.GetChild(0).GetComponent<Text>().text;
                memtoken = token.GetComponent<Annotation>().memtoken;
                token.GetComponent<Annotation>().modified = true;

                //Substitute word with input field text
                iF = iFobj.GetComponent<InputField>();
                token.transform.GetChild(0).GetComponent<Text>().text = iF.text;

                //Destroy iF
                token.GetComponent<Annotation>().modified = true;
                Destroy(GameObject.Find("InputField"));
                ContentSizeFitter cont;
                cont = GameObject.FindGameObjectWithTag("ThinkCloud").GetComponent<ContentSizeFitter>();
                LayoutRebuilder.ForceRebuildLayoutImmediate(GameObject.FindGameObjectWithTag("ThinkCloud").GetComponent<RectTransform>());


                subbing = false;
            }
        }      
    }

    public void Sub()
    {
        kb = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        if (iFobj)
        {
            Destroy(iFobj);
        }
        iFobj = Instantiate(Resources.Load<GameObject>("InputField"));
        iFobj.name = "InputField";
        iFobj.transform.parent = canvas.transform;
        iFobj.transform.localScale = new Vector3(1F, 1F, 1F);
        //iFobj.transform.localPosition = new Vector3(iFobj.transform.localPosition.x, iFobj.transform.localPosition.y, 0);
        iFobj.transform.position = new Vector3(token.transform.position.x, token.transform.position.y+40, token.transform.position.z);
        //iFobj.GetComponent<RectTransform>().anchoredPosition = new Vector3(token.GetComponent<RectTransform>().anchoredPosition.x, token.GetComponent<RectTransform>().anchoredPosition.y + 60);
        //Control.player.GetComponent<Control>().token = this.gameObject;
        subbing = true;
    }
}
