using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RewardInfo", menuName = "ScriptableObjects/RewardInfo", order = 1)]
public class RewardInfo : ScriptableObject
{
    [SerializeField]
    public Sprite sprite;
    [SerializeField]
    public int amount;
    [SerializeField]
    public AudioClip clip;
    [SerializeField]
    public float scale;
}
