using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CameraScriptableObject", menuName = "ScriptableObjects/CameraScriptableObject", order = 1)]
public class CameraScriptableObject : ScriptableObject
{
    [SerializeField]
    public FollowPlayer followPlayer;
}
