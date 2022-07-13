using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResetCheckmarks : MonoBehaviour
{
    public QFilled qfilled;
    public TextMeshProUGUI tmpro;
    public List<GameObject> toggles;

    private void Start()
    {
        
    }
    // Start is called before the first frame update
    public void ResetAllCheckmarks(GameObject caller)
    {
        foreach (GameObject toggle in toggles)
        {
            if (toggle != caller)
                toggle.GetComponent<Toggle>().isOn = false;
        }
        QResponse entry = new QResponse(tmpro.text.Substring(0, 2), caller.name);
        //UpdateQuestionnaire(instance, entry);
    }

    public void UpdateQuestionnaire(QFilled qfilled, QResponse entry)
    {
        QResponse found;
        if ((found = qfilled.entries.Find(x => x.item == entry.item)) != null)
        {
            int i = qfilled.entries.IndexOf(found);
            qfilled.entries[i].value = entry.value;
        }
        else
        {
            qfilled.entries.Add(entry);
        }
    }
}
