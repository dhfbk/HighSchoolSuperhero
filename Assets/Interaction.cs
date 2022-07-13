using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IInteractable
{
    void Toggle(Player player);
}

public class Interaction : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyUp("e"))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                IInteractable interactableScript;
                if ((interactableScript = hit.transform.GetComponent<IInteractable>()) != null)
                {
                    if (Vector3.Distance(transform.position, hit.transform.position) < 2.0F)
                        interactableScript.Toggle(this.GetComponent<Player>());
                }
            }
        }

        //else if (Input.GetKeyUp(KeyCode.DownArrow))
        //{
        //    if (PhoneUtility.phoneOut)
        //    {
        //        PhoneUtility.Down(GetComponent<Player>());
        //    }
        //}
    }

}
