using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : MonoBehaviour, IInteractable
{
    private bool activated;
    public void Toggle(Player player, InteractionInput input)
    {
        if (activated == false)
        {
            ToggleOn(player);
        }
        else
        {
            ToggleOff(player);
        }
    }

    public void ToggleOn(Player player)
    {
        if (activated == false)
        {
            player.currentTerminalInstance = GetComponent<Terminal>();
            player.cameraInterface.terminalWindow.transform.gameObject.SetActive(true);
            Player.overlays += 1;
            activated = true;
            player.cameraInterface.cameraOrbit.SetCameraLock(true);
            player.GetComponent<Movement>().Busy = true;
            player.cameraInterface.terminalWindow.GenerateItems();
            player.Freeze(true);
        }
    }

    public void ToggleOff(Player player)
    {
        if (activated == true)
        {
            player.currentTerminalInstance = null;
            player.cameraInterface.terminalWindow.transform.gameObject.SetActive(false);
            Player.overlays -= 1;
            activated = false;
            player.cameraInterface.cameraOrbit.SetCameraLock(false);
            player.GetComponent<Movement>().Busy = false;
            player.Freeze(false);
        }
    }

    public void Escape(Player player, InteractionInput input)
    {
        ToggleOff(player);
    }
    public void Enter(Player player, InteractionInput input)
    {
        
    }
}
