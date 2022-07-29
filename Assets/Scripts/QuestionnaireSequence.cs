using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class QuestionnaireSequence : MonoBehaviour
{
    public string[] questionnaireSequence;
    public string[] topicTask;
    public GameObject questionnaireObject;
    public GameObject sendButton;
    private int i;

    private void Start()
    {
        transform.root.GetComponent<CameraInterface>().player.GetComponent<Movement>().Busy = true;
        transform.root.GetComponent<CameraInterface>().player.GetComponent<Movement>().Frozen = true;

        i = 0;
        questionnaireObject.SetActive(true);
    }

    public void Next()
    {

        Questionnaire q = questionnaireObject.GetComponent<Questionnaire>();
        QFilled qfilled = q.FindValues();
        string qjson = JsonUtility.ToJson(qfilled);
        print(qjson);

        Player agent = transform.root.GetComponent<CameraInterface>().player;
        if (AllFilled(q, qfilled))
        {
            if (agent.questionnaireData == null)
                agent.questionnaireData = new QFilledList();
            agent.questionnaireData.qlist.Add(qfilled);
            print(JsonUtility.ToJson(qfilled));

            //StartCoroutine(SendFilledQuestionnaire(agent, qfilled));
            foreach (Transform t in q.content.transform)
            {
                Destroy(t.gameObject);
            }

            if (i < questionnaireSequence.Length - 1)
            {
                i += 1;
                StartCoroutine(q.LoadQuestionnaires(questionnaireSequence[i]));
            }
            else
            {
                CameraInterface ci = transform.root.GetComponent<CameraInterface>();

                ci.menuCanvas.GetComponent<Menu>().CloseQuestionnaire();

                agent.questionnaireFilled = true;
                SaveManager.SaveGameState(agent);
            }
        }
        else
        {
            PopUpUtility.Open(transform.root.GetComponent<CameraInterface>().popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.warning), ML.systemMessages.fillOutAllItems, 2);
        }
    }

    public bool AllFilled(Questionnaire q, QFilled qfilled)
    {
        int items = 0;
        foreach (Transform t in q.content.transform)
        {
            items += 1;
        }
        if (qfilled.entries.Count >= items)
        {
            return true;
        }
        return false;
    }

    IEnumerator SendFilledQuestionnaire(Player agent, QFilled qfilled)
    {
        WWWForm form = new WWWForm();
        form.AddField("ID", agent.id);
        form.AddField("Data", JsonUtility.ToJson(qfilled));

        string url = API.urls.postQuestionnaire;
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
        }
    }

    public int GetCurrentIndex()
    {
        return i;
    }
}
