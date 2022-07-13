using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AnnotatedDialogueManager", menuName = "ScriptableObjects/AnnotatedSO", order = 1)]
public class AnnotatedSO : ScriptableObject
{
    [SerializeField]
    public List<string> turns;
    [SerializeField]
    public List<string> roles;
    [SerializeField]
    public List<string> lines;
    [SerializeField]
    public List<string> annotations;
}
