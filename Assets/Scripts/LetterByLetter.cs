using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterByLetter : MonoBehaviour
{
    public bool write;
    public GameObject textobj;
    public Text textField;   
    public string aut;
    public GameObject msgbox;
    public bool speaking;
    public GameObject nametextobj;
    public GameObject scrollfield;

    public string messaggio;
    public bool finished;

    void Start ()
    {
    }

    public void Finish()
    {
        finished = true;
        textField.text = messaggio;
        StopAllCoroutines();
    }
    public void AddLetter(Text textfield, string messaggio, int i)
    {
        textfield.text += messaggio.Substring(i-1, 1);
    }

    public void CallText(string stringa, string author)
    {
        finished = false;
        textobj.GetComponent<Text>().text = ""; //reset
        messaggio = stringa;
        aut = author;
        nametextobj.GetComponent<Text>().text = aut;
        StartCoroutine(WriteText(textField, messaggio));
    }

    public void Active(Player agent, bool state)
    {
        gameObject.SetActive(state);
        agent.GetComponent<Movement>().Busy = state;
    }

    private IEnumerator WriteText(Text textField, string messaggio)
    {
        int i = 0;
        while (i < messaggio.Length)
        {
            yield return new WaitForSeconds(0.05f);
            i++;
            AddLetter(textField, messaggio, i);
        }
        finished = true;
    }
}
