using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplatformUtility : MonoBehaviour
{
    public static bool Mobile {
        get {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                return true;
            else
                return false;
            }
    }

    public static bool Web
    {
        get
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                return true;
            else
                return false;
        }
    }

    public static string PrimaryInteractionKey { get => "E/MOUSE CLICK"; }
    public static string SecondaryInteractionKey { get => "F/MOUSE CLICK ON OBJECT"; }
    public static string EscapeKey { get => "ESC/AVATAR"; }
    public static string Cancel { get => "C/RIGHT CLICK"; }

}
