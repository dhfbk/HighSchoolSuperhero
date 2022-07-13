using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/QuestScriptableObject", order = 1)]
public class QuestScriptableObject : ScriptableObject
{
    [SerializeField]
    public List<string> instructions;
    [SerializeField]
    public List<string> rewardMessage;
}
