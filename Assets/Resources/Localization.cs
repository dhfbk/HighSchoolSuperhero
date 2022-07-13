using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Localization : MonoBehaviour
{
    public static string GetLanguage()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Language/Language.txt");
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        else
        {
            return "En";
        }
    }
}
