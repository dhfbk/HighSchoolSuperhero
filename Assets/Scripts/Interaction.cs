using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionInput { Enter, Esc, E, MouseLeft, MouseRight }

public interface IInteractable
{
    void Toggle(Player player, InteractionInput input);
    void Escape(Player player, InteractionInput input);
    void Enter(Player player, InteractionInput input);
}

public class Interaction : MonoBehaviour
{
    public LayerMask mask;
    public float distance;
    private void Start()
    {
        if (distance == 0)
            distance = 3.0f;
    }
    private void Update()
    {
        
        if (Input.anyKeyDown)
        {
            if (!GetComponent<Player>().cameraInterface.menuCanvas.gameObject.activeSelf)
            {
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
                {
                    print(hit.transform.name);
                    IInteractable interactableScript;
                    if ((interactableScript = hit.transform.GetComponent<IInteractable>()) != null)
                    {
                        if (Vector3.Distance(transform.position, hit.transform.position) < distance)
                        {
                            if (Input.GetKeyDown(KeyCode.E))
                                interactableScript.Toggle(this.GetComponent<Player>(), InteractionInput.E);
                            else if (Input.GetKeyDown(KeyCode.Return))
                                interactableScript.Enter(this.GetComponent<Player>(), InteractionInput.Enter);
                            else if (Input.GetKeyDown(KeyCode.Escape))
                                interactableScript.Escape(this.GetComponent<Player>(), InteractionInput.Esc);
                            else if (Input.GetMouseButtonDown(0))
                                interactableScript.Toggle(this.GetComponent<Player>(), InteractionInput.MouseLeft);
                        }
                    }
                }
            }
        }
    }
}
