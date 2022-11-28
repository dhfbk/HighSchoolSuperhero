using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataUtilities;

public class DialogueNames : MonoBehaviour
{
    public static List<string> BadTags = new List<string>() { "Bullo", "Bullo1", "Bullo2", "SupportoBullo1", "SupportoBullo2", "SupportoBullo3" };
    public static List<string> GoodTags = new List<string>() { "Vittima", "Vittima1", "SupportoVittima1", "SupportoVittima2", "SupportoVittima3"};

    private void Start()
    {
        LoadUtility.Nomi = new List<string>() { "Mark", "Lukas", "Michael", "Louis", "Brad", "Akim", "Marco" };
        LoadUtility.Nomif = new List<string>() { "Adele", "Susan", "Sofia", "Yasmin", "Ingrid", "Jennifer", "Giorgia" };
    }

}
