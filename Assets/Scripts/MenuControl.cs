using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControl : MonoBehaviour
{
    CameraInterface ci;
    // Start is called before the first frame update
    void Start()
    {
        ci = GetComponent<Player>().cameraInterface;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ci.player.noMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape) ||
                MultiplatformUtility.Mobile &&
                        Input.GetMouseButtonDown(0) && Input.mousePosition.x < Screen.width / 10 && Input.mousePosition.y > (Screen.height - Screen.height / 5))
            {
                if (GetComponent<Player>().initialized && !GetComponent<Player>().IsAnnotating())
                {
                    if (!ci.menuCanvas.gameObject.activeSelf && Player.overlays < 1)
                    {
                        Show();
                    }
                    else
                    {
                        if (ci.menuCanvas.gameObject.activeSelf && ci.menuCanvas.GetComponent<Menu>().mainMenu.activeSelf)
                        {
                            Hide();
                        }
                    }
                }
            }
        }
    }

    public void Show()
    {
        DialogueInstancer.deactivateDialoguesAndGraffiti = true;
        ci.menuCanvas.gameObject.SetActive(true);
        GetComponent<Movement>().Busy = true;
        ci.menuCanvas.GetComponent<Menu>().UpdateStrings();
        if (GetComponent<Movement>().Busy)
            Time.timeScale = 1;
        else
            Time.timeScale = 1;
    }

    public void Hide()
    {
        DialogueInstancer.deactivateDialoguesAndGraffiti = false;
        ci.menuCanvas.gameObject.SetActive(false);
        GetComponent<Movement>().Busy = false;
    }
}
