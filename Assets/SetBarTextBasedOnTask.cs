using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetBarTextBasedOnTask : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SaveManager.type == TaskType.acc)
        {
            GetComponent<TextMeshProUGUI>().text = "School quality";
        }
        else
        {
            GetComponent<TextMeshProUGUI>().text = "School safety";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
