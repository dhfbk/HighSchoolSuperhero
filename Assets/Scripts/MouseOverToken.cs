using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MouseOverToken : MonoBehaviour
{
    public Player agent;
    public bool on;
    static GameObject currentToken;
    public static bool Changed { get; set; }
    public static bool Editing
    {
        get => currentToken;
    }
    // Start is called before the first frame update
    void Start()
    {
        agent = transform.root.GetComponent<FollowPlayer>().target.GetComponent<Player>();
    }
    // Update is called once per frame
    void Update()
    {

        
            if (Input.GetMouseButtonUp(0))
            {
                Player agent = FindObjectOfType<Player>();
                GameObject ifParent = agent.cameraInterface.ifParent;
                GameObject iF = ifParent.transform.GetChild(0).gameObject;
                if (agent.cameraInterface.okCircle.GetComponent<DialogueOkButton>().IsOver() && GameObject.Find("IfBG"))
                {
                    ChangeToken(false);
                }
                if (on && (currentToken != this.gameObject || currentToken == null))
                {
                    if (agent.currentlyRestricted == false || (agent.currentlyRestricted == true && agent.GetEnergy() > 0))
                    {

                        GameObject ifo = GameObject.Find("IfBG");
                        if (ifo)
                        {
                            ifo.GetComponentInChildren<TMP_InputField>().text = "";
                            ifo.SetActive(false);
                        }
                        List<GameObject> tokens = MessageUtility.FindTokens();
                        foreach (GameObject tok in tokens)
                            tok.GetComponent<TextMeshPro>().faceColor = new Color(1f, 1f, 1f, 1f);

                        //activate iF
                        if (!MultiplatformUtility.Mobile)
                        {
                            agent.cameraInterface.mouseButton.SetActive(false);
                            agent.cameraInterface.enterButton.SetActive(true);
                        }
                        else
                        {
                            agent.cameraInterface.touchIcon.SetActive(false);
                            agent.cameraInterface.okCircle.SetActive(true);
                        }
                        agent.playerLogger.StopSentenceAnnotationSW();
                        //agent.AddEnergy(-1);
                        ifParent.SetActive(true);
                        currentToken = this.gameObject;

                        TMP_InputField i = FindObjectOfType<TMP_InputField>();

                        if (!i.isFocused)
                        {
                            i.ActivateInputField();
                        }
                        //iF.transform.localPosition = new Vector3(transform.localPosition.x - 15f, transform.parent.localPosition.y + 100f, transform.localPosition.z - 0.1f);
                    }
                    else
                    {
                        NotificationUtility.ShowString(agent, "Not enough charge!");
                        PopUpUtility.Open(agent.cameraInterface.popUpCanvas, PopUpType.Warning, "Sembra che tu abbia esaurito l'energia del Bully-o-meter. \n Raccogli cristalli e vai a un terminale (T) per comprarne ancora!", 4);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                ChangeToken(false);
            }
            else if (Input.GetKeyDown(KeyCode.Delete))
            {
                ChangeToken(delete: true);
            }
            UpdateBox();
        
    }

    private void ChangeToken(bool delete)
    {
        if (this.gameObject == currentToken)
        {
            Player agent = FindObjectOfType<Player>();
            GameObject iF = GameObject.Find("IfBG").transform.GetChild(0).gameObject;
            //agent.cameraInterface.eButton.SetActive(false);
            //Annotate
            GetComponent<Annotation>().memtoken = GetComponent<TextMeshPro>().text;
            if (delete)
                GetComponent<TextMeshPro>().text = "";
            else
                GetComponent<TextMeshPro>().text = iF.GetComponent<TMP_InputField>().text + "\u00A0";
            //Deactivate if
            iF.GetComponent<TMP_InputField>().text = "";
            iF.transform.parent.gameObject.SetActive(false);


            transform.parent.GetComponent<ContentSizeFitter>().enabled = false;
            transform.parent.GetComponent<ContentSizeFitter>().enabled = true;
            LayoutRebuilder.ForceRebuildLayoutImmediate(agent.cameraInterface.thinkCloud.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(agent.cameraInterface.thinkCloud.GetComponent<RectTransform>());


            //Subtract if token not yet modified
            if (GetComponent<Annotation>().modified == false)
            {
                if (Player.rCondition == RCondition.Restricted)
                {
                    agent.SubtractEnergy(1);
                }
            }
            GetComponent<Annotation>().modified = true;
            Changed = true;
            currentToken = null;
        }
        on = false;
        TurnWhite();
        GetComponent<Fluctuate>().StartFluctuate();
    }

    private void UpdateBox()
    {
        GetComponent<BoxCollider>().size = new Vector3(GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height, 1);
        GetComponent<BoxCollider>().center = new Vector3(GetComponent<RectTransform>().rect.width / 2, 0, 1);
    }

    private void OnMouseOver()
    {
        on = true;
        TurnRed();
    }

    private void OnMouseExit()
    {
        if (currentToken != this.gameObject || currentToken == null)
        {
            TurnWhite();
        }
        on = false;
    }

    private void TurnRed()
    {
        GetComponent<TextMeshPro>().faceColor = new Color(0.75f, 0.25f, 0.35f, 1);
    }

    private void TurnWhite()
    {
        GetComponent<TextMeshPro>().faceColor = new Color(1f, 1f, 1f, 1f);
    }
}
