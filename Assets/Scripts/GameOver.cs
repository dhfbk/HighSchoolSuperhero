using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image black;
    public CameraInterface cameraInterface;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        cameraInterface.player.GetComponent<Movement>().Busy = true;
        DialogueInstancer.deactivateDialoguesAndGraffiti = true;

        StopAllCoroutines();
        StartCoroutine(ActivateFade());
    }

    public IEnumerator ActivateFade()
    {
        Player p = cameraInterface.player;
        if (p.currentDialogueInstance != null)
            p.currentDialogueInstance.ExitDialogue(p);

        if (p.currentGraffitiInstance != null)
            p.currentGraffitiInstance.StopAnnotation(cameraInterface.player);

        cameraInterface.participateButton.GetComponent<ParticipateButton>().Hide();

        if (p.currentTerminalInstance != null)
            p.currentTerminalInstance.ToggleOff(p);

        MessageUtility.CloseBoxedMessage(p);

        Color currentColor = text.color;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime;
            text.color = Color.Lerp(currentColor, new Color(currentColor.r, currentColor.g, currentColor.b, 1), t);
            black.color = Color.Lerp(new Color(0,0,0,0), new Color(0,0,0,1), t);
            yield return null;
        }
        yield return new WaitForSeconds(3);
        cameraInterface.player.AddCrystals(-20);
        cameraInterface.player.transform.position = new Vector3(0,0,0);

        text.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0);
        black.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0);

        //while (t < 1.0f)
        //{
        //    text.faceColor = Color.Lerp(currentColor, new Color(currentColor.r, currentColor.g, currentColor.b, 0), t);
        //    black.color = Color.Lerp(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), t);
        //    yield return null;
        //}

        Deactivate();
        if (cameraInterface.player.transform.parent)
        {
            Scooter s = cameraInterface.player.transform.parent.GetComponent<Scooter>();
            if (s)
                s.Toggle(s.Agent);
        }




        ShowPlayerIfHidden();
    }

    public void Deactivate()
    {
        this.gameObject.SetActive(false);
        cameraInterface.player.GetComponent<Movement>().Busy = false;
        DialogueInstancer.deactivateDialoguesAndGraffiti = false;
        SafetyBar.CurrentSafety = SafetyBar.SafetyMax;
        Player.gameOverCalled = false;
    }

    public void ShowPlayerIfHidden()
    {
        GameObject p = GameObject.Find("PlayerModelGroup");
        foreach (Transform t in p.transform)
            if (t.gameObject.layer != 27)
                t.gameObject.layer = 10;
    }
}
