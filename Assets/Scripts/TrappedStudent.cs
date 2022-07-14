using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class TrappedStudent : MonoBehaviour
{
    GameObject npc;
    Locker locker;
    bool msgCalled;
    Player agent;
    List<string> lines;
    public TextMeshPro help;
    public AudioClip tada;
    // Start is called before the first frame update
    void Start()
    {
        //Locker
        locker = GetComponentInChildren<Locker>();

        //create student
        


        //Pos
        
    }

    // Update is called once per frame
    void Update()
    {
        if (API.systemMessagesRetrieved == true)
        {
            help.text = ML.systemMessages.help;
        }
        if (locker.IsOpen() && !msgCalled)
        {
            StartCoroutine(ShowMessages());
            msgCalled = true;
        }
    }

    void GetDialogue()
    {

    }

    IEnumerator ShowMessages()
    {
        npc = NPCUtilities.CreateNPC(true, this.transform.position, free: true, silent: true, combine: false);
        npc.transform.SetParent(this.transform);
        npc.transform.localPosition += new Vector3(0, 3.25f, 0.5f);

        //Rot
        npc.transform.localEulerAngles = new Vector3(0, 0, 0);

        //Other
        npc.GetComponent<Rigidbody>().useGravity = false;
        npc.GetComponent<CapsuleCollider>().enabled = false;


        Player agent = locker.Agent;

        agent.friendParts = new StringParts(npc.GetComponent<Parts>());

        MessageUtility.OverrideBusy = true;
        agent.GetComponent<Movement>().Busy = true;

        //WWWForm form = new WWWForm();
        //form.AddField("Entity", "TrappedStudent");
        //using (UnityWebRequest www = UnityWebRequest.Post(API.dialogueUrl, form))
        //{
        //    print(API.domain + API.dialogueUrl);
        //    yield return www.SendWebRequest();
        //    string json = www.downloadHandler.text;
        //    print("JSON:"+json);
        //    Dialogues d = JsonUtility.FromJson<Dialogues>(json);
        //    DialogueLines dl = d.dialogueLines.Find(x => x.id == 0 && x.lang == ML.Lang.it);
        //    if (dl != null)
        //        lines = dl.lines;
        //    //instrR = d.dialogueLines.Find(x => x.id == 3 && x.lang == ML.Lang.ita).lines;

        //}
        MessageUtility.BoxedMessageSeriesStart(agent, "Trapped student", API.systemDialogues.trappedStudent.dialogueLines[0].lines);

        while (MessageUtility.Speaking)
            yield return null;
        agent.GetComponent<Movement>().Busy = true;

        npc.transform.SetParent(Camera.main.transform);

        GameObject star = agent.cameraInterface.star;
        star.SetActive(true);
        star.transform.localScale = Vector3.zero;

        GameObject text = agent.cameraInterface.youSavedAStudent;
        text.SetActive(true);
        text.transform.localScale = Vector3.zero;
        text.GetComponent<TextMeshPro>().text = ML.systemMessages.youSavedAStudent;

        float t;

        //Animation start
        npc.layer = 26;
        foreach (Transform tr in npc.transform)
        {
            if (tr.name.Contains("Eyes"))
            {
                foreach (Transform e in tr)
                    e.gameObject.layer = 26;
            }
            tr.gameObject.layer = 26;
        }
        t = 0;
        Vector3 startPos = npc.transform.localPosition;
        Quaternion startRot = npc.transform.localRotation;
        while (t < 1)
        {
            t += Time.deltaTime*3;
            npc.transform.localPosition = Vector3.Lerp(startPos, new Vector3(0, -0.5f, 5), t);

            npc.transform.localRotation = Quaternion.Lerp(startRot, Quaternion.Euler(new Vector3(20f, 200, 6f)), t);
            yield return null;
        }

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 3;
            star.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(34, 34, 34), t);
            text.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1, 1, 1), t);
            yield return null;
        }

        agent.cameraInterface.FX.volume = 0.75f;
        agent.cameraInterface.FX.PlayOneShot(tada);
        yield return new WaitForSeconds(2);

        //Disappear
        t = 0;
        Vector3 starStartScale = star.transform.localScale;
        Vector3 npcStartScale = npc.transform.localScale;
        while (t < 1)
        {
            t += Time.deltaTime * 4;
            star.transform.localScale = Vector3.Lerp(starStartScale, Vector3.zero, t);
            npc.transform.localScale = Vector3.Lerp(npcStartScale, Vector3.zero, t);
            text.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            yield return null;
        }
        Destroy(npc);
        star.SetActive(false);
        text.SetActive(false);
        MessageUtility.OverrideBusy = false;
        agent.GetComponent<Movement>().Busy = false;
        help.transform.parent.gameObject.SetActive(false);

        //POINTS
        PointSystem.AddPoints(agent, PointType.heart, 50);
        agent.friends += 1;
    }
}
