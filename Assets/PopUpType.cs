using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpType : MonoBehaviour
{
    public static string Warning = "Warning";
    public static string Error = "Error";
    public static string Success = "Success!";

    public enum Types { success, warning, error }

    public static string LocalizedType(Player agent, Types type)
    {
        if (Player.language == ML.Lang.en)
        {
            switch (type)
            {
                case Types.success:
                    return "Success!";
                case Types.warning:
                    return "Warning";
                case Types.error:
                    return "Error";
            }
        }
        else if (Player.language == ML.Lang.it)
        {
            switch (type)
            {
                case Types.success:
                    return "Successo!";
                case Types.warning:
                    return "Attenzione";
                case Types.error:
                    return "Errore";
            }
        }
        return "";
    }
}
