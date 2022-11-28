using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTriggerer : MonoBehaviour, IInteractable
{
    
    public void Toggle(Player player, InteractionInput input)
    {
        if (input != InteractionInput.MouseLeft)
            transform.parent.GetComponent<DialogueInstancer>().Toggle(player, false);
        else
            transform.parent.GetComponent<DialogueInstancer>().Toggle(player, true);
    }

    public void Escape(Player player, InteractionInput input)
    {
        transform.parent.GetComponent<DialogueInstancer>().Escape(player);
    }

    public void Enter(Player player, InteractionInput input)
    {
        transform.parent.GetComponent<DialogueInstancer>().Enter();
    }
}
