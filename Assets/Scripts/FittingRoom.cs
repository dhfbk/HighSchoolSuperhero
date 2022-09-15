using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FittingRoom : MonoBehaviour, ITriggerable
{
    private Vector3 memPos;
    public Player Agent { get; set; }
    private bool activated;
    private void Update()
    {
        if (Agent != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (GetComponent<MouseOver>().on)
                {
                    if (!activated && Agent != null)
                    {//altrimenti prende il click anche sotto i pulsanti
                        Toggle(Agent);
                    }
                }
            }
            if (Input.GetKeyDown("e") && !Agent.cameraInterface.menuCanvas.gameObject.activeSelf)
            {

                float dot = Vector3.Dot(Agent.transform.forward, (this.transform.position - Agent.transform.position).normalized);
                if (dot < 1f && dot > 0)
                {
                    Toggle(Agent);
                }
                
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (activated)
                {
                    Toggle(Agent);
                }
            }

        }
    }
    public void Toggle(Player player)
    {
        CameraInterface ci = Agent.cameraInterface;
        CharacterCustomization cc = ci.editorCanvas.GetComponent<CharacterCustomization>();

        if (activated == false)
        {
            activated = true;
            ci.hudCanvas.gameObject.SetActive(false);
            ci.editorCanvas.gameObject.SetActive(true);
            

            cc.doNotMoveModel = true;
            cc.nameObj.SetActive(false);
            cc.fittingScript = this;
            cc.LoadParts();
            cc.InitializePartIndeces();

            //Place player
            Agent.GetComponent<CharacterCustomizationSetup>().enabled = true;
            Agent.GetComponent<Rigidbody>().useGravity = false;
            ci.GetComponent<FollowPlayer>().enabled = false;
            memPos = Agent.transform.position;

            //Agent.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
            Agent.transform.eulerAngles = new Vector3(-45, 180, 0);
            Agent.transform.SetParent(Camera.main.transform);
            Agent.transform.localPosition = new Vector3(1.5f, 0.0f, 7.0f);

            Player.overlays += 1;
            
            player.cameraInterface.cameraOrbit.SetCameraLock(true);
            player.GetComponent<Movement>().Busy = true;
            player.cameraInterface.terminalWindow.GenerateItems();
        }
        else
        {
            //Agent.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.On;
            Agent.GetComponent<CharacterCustomizationSetup>().enabled = false;
            Agent.transform.eulerAngles = new Vector3(0, 0, 0);
            Agent.GetComponent<Rigidbody>().useGravity = true;
            Agent.transform.SetParent(null);
            ci.GetComponent<FollowPlayer>().enabled = true;
            Agent.transform.position = memPos;
            ci.hudCanvas.gameObject.SetActive(true);
            ci.editorCanvas.gameObject.SetActive(false);
            Player.overlays -= 1;
            activated = false;
            player.cameraInterface.cameraOrbit.SetCameraLock(false);
            player.GetComponent<Movement>().Busy = false;
        }
    }

    public void TriggerOn(Player agent)
    {
        this.Agent = agent;
    }
    public void TriggerOff()
    {
        if (activated == false)
            this.Agent = null;
    }
}
