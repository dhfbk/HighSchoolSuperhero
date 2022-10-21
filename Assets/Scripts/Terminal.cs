using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : MonoBehaviour, ITriggerable
{
    public Player Agent { get; set; }
    private bool activated;
    private void Update()
    {
        if (Agent != null)
        {
            float dot = Vector3.Dot(Agent.transform.forward, (this.transform.position - Agent.transform.position).normalized);
            if (Input.GetMouseButtonDown(0))
            {
                if (GetComponent<MouseOver>().on)
                {
                    if (!activated)
                    {//altrimenti prende il click anche sotto la finestra
                        Toggle(Agent);
                    }
                }
            }
            if (Input.GetKeyDown("e") && !Agent.cameraInterface.menuCanvas.gameObject.activeSelf)
            {
                if (dot < 1f && dot > 0)
                {
                    Toggle(Agent);
                }     
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (activated)
                {
                    print(Player.overlays);
                    Toggle(Agent);
                }
            }
            if (!(dot < 1f) || !(dot > 0))
            {
                if (activated)
                    Toggle(Agent);
            }
        }
        else
        {        
            if (activated)
            {
                Toggle(Agent);
            }
        }
    }
    public void Toggle(Player player)
    {
        if (activated == false)
        {
            player.cameraInterface.terminalWindow.transform.gameObject.SetActive(true);
            Player.overlays += 1;
            activated = true;
            player.cameraInterface.cameraOrbit.SetCameraLock(true);
            player.GetComponent<Movement>().Busy = true;
            player.cameraInterface.terminalWindow.GenerateItems();
        }
        else
        {
            player.cameraInterface.terminalWindow.transform.gameObject.SetActive(false);
            Player.overlays -= 1;
            activated = false;
            player.cameraInterface.cameraOrbit.SetCameraLock(false);
            player.GetComponent<Movement>().Busy = false;
            print("fire");
        }
    }

    public void TriggerOn(Player agent)
    {
        this.Agent = agent;
    }
    public void TriggerOff()
    {
        this.Agent = null;
    }
}
