using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using DataUtilities;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

[Serializable]
public class QFilled
{
    public string id;
    public string task;
    public string questionnaire;
    public List<QResponse> entries;

    public QFilled()
    {
        entries = new List<QResponse>();
    }
    public QFilled(string id, string type)
    {
        this.id = id;
        this.questionnaire = type;
        entries = new List<QResponse>();
    }
}

[Serializable]
public class QResponse
{
    public string item;
    public string value;

    public QResponse(string item, string value)
    {
        this.item = item;
        this.value = value;
    }
}

public class Questionnaire : MonoBehaviour
{
    public Player player;
    public string[] types;
    public ML.Lang language;
    public GameObject content;
    public string task;
    public string questionnaire;
    // Start is called before the first frame update
    void Start()
    {
        language = Player.language;
        string dir = Path.Combine(Application.streamingAssetsPath, "Questionnaire.txt");
        if (LoadUtility.Web)
        {
            StartCoroutine(LoadQuestionnaires(transform.parent.GetComponent<QuestionnaireSequence>().questionnaireSequence[0]));
        }
        else
        {
            string language = ML.GetLang(this.language);
            List<string> items = new List<string>();
            foreach (string type in types)
            {
                foreach (string file in Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, $"Questionnaires/{type}/{language}")))
                {
                    if (!file.Contains(".meta"))
                        items.AddRange(File.ReadAllLines(file));
                }
            }
            //InstantiateItems(items);
        }
        List<List<string>> a = new List<List<string>>();
    }

    public IEnumerator LoadQuestionnaires(string type)
    {
        string url = API.urls.getQuestionnaire;
        WWWForm form = new WWWForm();
        form.AddField("Name", type);

        string questionnaireJson ="";
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
            questionnaireJson = www.downloadHandler.text;
        }

        Q q = (Q)JsonUtility.FromJson(questionnaireJson, typeof(Q));

        QuestionnaireSequence manager = transform.parent.GetComponent<QuestionnaireSequence>();
        task = manager.topicTask[manager.GetCurrentIndex()];
        questionnaire = manager.questionnaireSequence[manager.GetCurrentIndex()];

        q.task = task;
        q.questionnaire = questionnaire;

        if (q.random)
        {
            q.items = q.items.OrderBy(a => a.question[0].Length).ToList();
        }

        InstantiateItems(q);
    }

    public void InstantiateItems(Q q)
    {
        foreach (QItem item in q.items)
        {
            GameObject instantiatedItem = Instantiate(Resources.Load<GameObject>(item.type.ToString()));
            if (item.type == QItemType.MultiItem)
            {
                List<string> options = item.options[0].Split('|').ToList();
                //Instantiate all options
                foreach (string option in options)
                {
                    GameObject toggle = Instantiate(Resources.Load<GameObject>("ItemChoice"));
                    toggle.transform.parent = instantiatedItem.transform.GetChild(1);
                    toggle.transform.GetChild(1).GetComponent<Text>().text = option;

                    instantiatedItem.GetComponent<ResetCheckmarks>().toggles.Add(toggle);
                }
            }
            instantiatedItem.transform.position = content.transform.position;
            instantiatedItem.transform.SetParent(content.transform);
            instantiatedItem.transform.rotation = content.transform.rotation;
            instantiatedItem.transform.localScale = Vector3.one;

            int index = (int)Player.language;
            instantiatedItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.question[index];
            instantiatedItem.transform.name = item.question[index];
            //instantiatedItem.GetComponent<ResetCheckmarks>().instance = instance;
        }
    }

    public QFilled FindValues()
    {
        QFilled qfilled = new QFilled();
        qfilled.task = task;
        qfilled.questionnaire = questionnaire;
        foreach (Transform child in content.transform)
        {
            GameObject item = child.gameObject;
            if (item.CompareTag("QuestionnaireItem") || item.CompareTag("MultipleChoiceItem"))
            {
                string answer = "";
                foreach (Toggle t in item.GetComponentsInChildren<Toggle>())
                {
                    if (t.isOn)
                        answer = t.transform.GetChild(1).GetComponent<Text>().text.Trim();
                }
                if (!string.IsNullOrEmpty(answer))
                {
                    string itemText = item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                    qfilled.entries.Add(new QResponse(itemText.Substring(0, 2), answer));
                }
            }
            else if (item.CompareTag("InputFieldItem"))
            {
                string answer = item.transform.GetChild(1).transform.GetChild(2).GetComponent<Text>().text.Trim();
                if (!string.IsNullOrEmpty(answer))
                {
                    string itemText = item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                    qfilled.entries.Add(new QResponse(itemText.Substring(0, 2), answer));
                }
            }
        }
        return qfilled;
    }
}



public class Q
{
    public bool random;
    public string questionnaire;
    public string task;
    public List<QItem> items;

    public Q() { items = new List<QItem>(); }
    public Q(string questionnaire) { this.questionnaire = questionnaire; items = new List<QItem>(); }
}

public enum QItemType { InputItem, MultiItem, LikertItem }

[Serializable]
public class QItem
{
    public List<string> question;
    public QItemType type;
    public List<string> options;
    public QItem() { }
    public QItem(List<string> question, QItemType type, List<string> options = null) { this.question = question; this.type = type; this.options = options; }
}