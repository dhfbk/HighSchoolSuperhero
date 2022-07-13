using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/DialogueScriptableObject", order = 1)]
public class DialogueScriptableObject : ScriptableObject
{
    public List<string> engLines;
    public List<string> itaLines;
}
