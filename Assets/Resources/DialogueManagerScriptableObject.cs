using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueManager", menuName = "ScriptableObjects/DialogueManagerScriptableObject", order = 1)]
public class DialogueManagerScriptableObject : ScriptableObject
{
    [SerializeField]
    public List<string> roles;
    [SerializeField]
    public List<string> lines;
    [SerializeField]
    public List<string> annotations;
}
