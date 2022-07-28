using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DataUtilities;
using TMPro;

public class Menu : MonoBehaviour
{
    public CameraInterface cameraInterface;
    public TextMeshProUGUI codeText;
    public GameObject instructions;
    public GameObject mainMenu;

    public TextMeshProUGUI controls;
    public Text instructionsT;
    public TextMeshProUGUI bindings;
    public TextMeshProUGUI graphics;
    public TextMeshProUGUI progress;
    public TextMeshProUGUI important;
    public TextMeshProUGUI importantInstructions;
    public TextMeshProUGUI friendsNumber;
    public TextMeshProUGUI friendName;
    public TextMeshProUGUI friendInstructions;
    public Parts friendParts;
    public Text save;
    public Text load;
    public Text exit;
    public Text stuck;
    public TextMeshProUGUI language;

    private bool initiateLoad;
    public void ChangeMusicVolume(float volume)
    {
        cameraInterface.music.volume = AudioUtility.MusicVolume = volume;
    }
    public void ChangeFXVolume(float volume)
    {
        cameraInterface.FX.volume = AudioUtility.FXVolume = volume;
    }

    public void SetGraphics(int value)
    {
        QualitySettings.SetQualityLevel(value);
    }

    public void ChangeLanguage(int value)
    {
        switch (value)
        {
            case 0:
                Player.language = ML.Lang.en;
                break;
            case 1:
                Player.language= ML.Lang.it;
                break;
        }
        UpdateStrings();
    }
    private void Start()
    {
        UpdateStrings();
        UpdateFrient();
    }
    public void UpdateFrient()
    {
        
    }
    public void UpdateStrings()
    {
        Player agent = transform.root.GetComponent<CameraInterface>().player;

        progress.text = $"Dialoghi: {agent.TotalAnnotatedDialogues} \n Murales: {agent.TotalAnnotatedGraffiti}";

        controls.text = ML.systemMessages.controls;
        bindings.text = ML.systemMessages.bindings;
        graphics.text = ML.systemMessages.graphics;
        instructionsT.text = ML.systemMessages.instructionsT;
        important.text = ML.systemMessages.important;
        importantInstructions.text = ML.systemMessages.importantInstructions;
        save.text = ML.systemMessages.save;
        load.text = ML.systemMessages.load;
        exit.text = ML.systemMessages.exit;
        stuck.text = ML.systemMessages.imStuck;
        language.text = ML.systemMessages.language;
        friendsNumber.text = agent.friends.ToString();
        friendName.text = agent.lastFriend.ToString();
        
        agent.cameraInterface.touchIcon.transform.GetChild(0).GetComponent<TextMeshPro>().text = ML.systemMessages.touchIcon;

        if (agent.friends > 0)
        {
            friendParts.gameObject.SetActive(true);
            SaveManager.LoadLook(friendParts, agent.friendParts);
        }
        else
        {
            friendInstructions.text = ML.systemMessages.youCanMakeNewFriends;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            TakeMeToTheQuestionnaire(transform.root.GetComponent<CameraInterface>().player);
        }
        if (API.logged && initiateLoad)
        {
            if (API.isIDRegistered)
            {
                cameraInterface.player.SetID(codeText.text);
                Application.LoadLevel(SceneManager.GetActiveScene().buildIndex);
                //SaveManager.LoadGameState(cameraInterface.player);
            }
            else
            {
                Player agent = transform.root.GetComponent<CameraInterface>().player;
                PopUpUtility.Open(cameraInterface.popUpCanvas, PopUpType.LocalizedType(agent, PopUpType.Types.error), ML.systemMessages.IDNotFound, 0);
            }
            API.logged = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!transform.root.GetComponent<CameraInterface>().player.GetComponent<Player>().IsAnnotating())
            {
                if (!mainMenu.activeSelf && instructions.activeSelf)
                {
                    mainMenu.SetActive(true);
                    instructions.SetActive(false);
                }
            }
        }
    }
    public void Save()
    {
        SaveManager.SaveGameState(cameraInterface.player, true);
    }

    public void Load()
    {
        LoadUtility.AllLoaded = false;
        DialogueInstancer.uniqueLineIndex = 0;
        API.logged = false;
        initiateLoad = true;
        //StartCoroutine(API.CheckID(codeText.text));
        Player agent = FindObjectOfType<Player>();
        SaveManager.DeployGameState(agent, agent.gameState);
    }
    public void Stuck()
    {
        cameraInterface.player.transform.position = new Vector3(0, 0, 0);
        FindObjectOfType<MenuControl>().Hide();
    }
    public void Instructions()
    {
        instructions.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void Exit()
    {
        FindObjectOfType<MenuControl>().Hide();
        cameraInterface.titleScreen.gameObject.SetActive(true);
        cameraInterface.titleScreen.GetComponent<TitleScreen>().ResetTitle();
        cameraInterface.player.Deinitialize();
    }
    public void TakeMeToTheQuestionnaire(Player agent)
    {
        agent.GetComponent<Movement>().Busy = true;
        agent.GetComponent<Movement>().Frozen = true;
        agent.StartCoroutine(QuestionnaireCoroutine(agent));
    }
    public IEnumerator QuestionnaireCoroutine(Player agent)
    {
        DialogueInstancer.deactivateDialoguesAndGraffiti = true;
        MessageUtility.ResetMessages(agent, agent.gameObject);
        agent.GetComponent<Movement>().Busy = true;
        agent.GetComponent<Movement>().Frozen = true;

        string messaggio = "";
        if (SafetyBar.CurrentSafety >= 100 && SafetyBar.CurrentSafety < 500)
            messaggio = "Continua a provare!";
        else if (SafetyBar.CurrentSafety < 100)
            messaggio = "Puoi fare di meglio!";
        else if (SafetyBar.CurrentSafety >= 500 && SafetyBar.CurrentSafety < 800)
            messaggio = "Molto ben fatto, complimenti!";
        else if (SafetyBar.CurrentSafety >= 800)
            messaggio = "Eccellente! Hai fatto un lavoro sensazionale!";

        string faiIlQuestionario = "Ora per favore dedica 5 minuti del tuo tempo per fare il nostro questionario!";
        PopUpUtility.Open(cameraInterface.popUpCanvas, PopUpType.Success, "Hai completato l'annotazione di 30 frasi! \n La qualità linguistica della città!" + (int)SafetyBar.CurrentSafety + "/1000. \n" + messaggio + "\n" + faiIlQuestionario, 10);
        yield return new WaitForSeconds(10);
        
        cameraInterface.menuCanvas.gameObject.SetActive(false);
        cameraInterface.hudCanvas.gameObject.SetActive(false);
        cameraInterface.questionnaireCanvas.gameObject.SetActive(true);
    }

    public void CloseQuestionnaire()
    {
        cameraInterface.menuCanvas.gameObject.SetActive(true);
        cameraInterface.hudCanvas.gameObject.SetActive(true);
        cameraInterface.questionnaireCanvas.gameObject.SetActive(false);
        DialogueInstancer.deactivateDialoguesAndGraffiti = false;
        cameraInterface.player.GetComponent<Movement>().Busy = false;
        cameraInterface.player.GetComponent<Movement>().Frozen = false;
    }
}
