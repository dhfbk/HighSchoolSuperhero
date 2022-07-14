using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;
using DataUtilities;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void SendLoginRequestToPage();

#endif
    public bool move;
    public bool google;
    public bool forceLogin;
    public Player player;
    float tM = 0f;
    float tD = 0f;
    float startDilate = -1;
    float endDilate = 0;
    Vector3 startPos;
    Vector3 nextPos;
    int range = 100;
    public TextMeshProUGUI pressStart;
    public GameObject nicknameField;
    public GameObject domainField;
    public GameObject passwordField;
    public GameObject googleLogin;
    public GameObject passLogin;
    GameObject login;
    public GameObject loginButton;
    public GameObject demoLoginButton;
    public TMP_InputField nickname;
    public TMP_InputField password;
    public TMP_InputField domain;
    public GameObject pressStartBG;
    public GameObject loadingCanvas;
    public GameObject languageChoiceBox;
    public GameObject info;
    private Vector3 campos;
    private Quaternion camrot;

    private int activeIF;

    void Start()
    {
        campos = Camera.main.transform.position;
        camrot = Camera.main.transform.rotation;
        if (SceneManager.GetActiveScene().name == "NoWorld")
            Player.condition = Condition.NoW3D;
        else
            Player.condition = Condition.W3D;
        //Player agent;
        //if ((agent = transform.root.GetComponent<CameraInterface>().player) != null)
        //{
        //    pressStart.text = ML.LocalizedStr(agent, ML.Str.pressStart);
        //}
        if (Player.condition == Condition.W3D)
            GraphicsUtility.EnableDof();

        if (API.currentApi == Api.final)
            forceLogin = true;

        if (!forceLogin)
        {
            loadingCanvas.SetActive(true);
            this.gameObject.SetActive(false);
            player.GetComponent<Player>().enabled = true;
            player.GetComponent<SequenceManager>().Next();
        }
        ResetMovement();
        if (google)
            login = googleLogin;
        else
            login = passLogin;
    }
    void Update()
    {
        //Camera movement
        if (move)
        {
            Camera.main.transform.position = Vector3.Lerp(startPos, nextPos, tM);
            tM += Time.deltaTime / range;
            if (tM >= 1)
            {
                ResetMovement();
            }
        }

        //Press start animation
        pressStart.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, Mathf.Lerp(startDilate, endDilate, tD));
        tD += Time.deltaTime;
        if (tD >= 1)
        {
            ReverseDilate();
        }

        if (Input.anyKey && !login.activeSelf)
        {
            LoginDisplay(false);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (login.activeSelf)
                Login();
        }

        if (login.GetComponent<Login>())
        {
            if (login.GetComponent<Login>().done)
            {
                Begin();
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (languageChoiceBox.activeSelf == true)
            {
                languageChoiceBox.SetActive(false);
            }
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (nicknameField.GetComponent<TMP_InputField>().isFocused)
            {
                passwordField.GetComponent<TMP_InputField>().ActivateInputField();
            }
            else if (passwordField.GetComponent<TMP_InputField>().isFocused)
            {
                nicknameField.GetComponent<TMP_InputField>().ActivateInputField();
            }
            else
            {
                nicknameField.GetComponent<TMP_InputField>().ActivateInputField();
            }
        }
    }

    void ResetMovement()
    {
        startPos = Camera.main.transform.position;
        nextPos = Camera.main.transform.position + new Vector3(Random.Range(-range, range+1), startPos.y, Random.Range(-range, range+1));
        tM = 0;
    }

    void ReverseDilate()
    {
        if (endDilate == 0)
        {
            endDilate = -1;
            startDilate = 0;
        }
        else
        {
            endDilate = 0;
            startDilate = -1;
        }
        tD = 0;
    }

    public void Login()
    {
        StartCoroutine(Authenticate(demo: false));
    }

    public void DemoLogin()
    {
        AskLanguage();
        
    }

    public void AskLanguage()
    {
        languageChoiceBox.SetActive(true);
    }

    public void ChangeLanguage(int lang)
    {
        if (lang == 0)
            Player.language = ML.Lang.en;
        else if (lang == 1)
            Player.language = ML.Lang.it;
        else if (lang == 2)
            Player.language = ML.Lang.fr;
    }

    public void AllAdminOptionsSet()
    {
        languageChoiceBox.SetActive(false);
        StartCoroutine(Authenticate(demo: true));
    }

    void LoginDisplay(bool google)
    {
        if (LoadingManager.config.guest == true && forceLogin == false)
            player.GetComponent<SequenceManager>().Next();
        else
        {
            pressStartBG.SetActive(false);
            info.SetActive(false);

            if (google)
            {
                //nicknameField.SetActive(true);
                //passwordField.SetActive(true);
                nickname.text = "";
                password.text = "";
                login.SetActive(true);
            }
            else
            {
                nickname.text = "";
                password.text = "";
                nicknameField.SetActive(true);
                passwordField.SetActive(true);
                loginButton.SetActive(true);
                demoLoginButton.SetActive(true);

                if (MultiplatformUtility.Mobile)
                {
                    domainField.SetActive(true);
                }
            }
        }
    }
    void Begin()
    {
        loadingCanvas.SetActive(true);
        this.gameObject.SetActive(false);
        player.GetComponent<Player>().enabled = true;
    }


    public IEnumerator Authenticate(bool demo)
    {
        //Admin
        if (demo == true || (nickname.text == "admin" && password.text == "admin"))
        {
            Player.admin = true;
            player.sessionID = "no_session";
            Player.demo = false;
            yield return StartCoroutine(LoadingManager.LoadSampleSentences());
            player.GetComponent<SequenceManager>().Next();

        }
        else //User
        {
            Player.admin = false;

            //If mobile, manual input of the API domain is needed
            if (MultiplatformUtility.Mobile)
            {
                if (string.IsNullOrEmpty(domain.text))
                    PopUpUtility.Open(player.cameraInterface.popUpCanvas, PopUpType.Error, "Please specify the API domain (https://www...)", 5);
                else
                    API.domain = domain.text;
            }
                

            //Login attempt
            yield return StartCoroutine(API.SendLoginRequest(player, nickname.text, password.text));
            if (API.LoginState == 1)
            {
                player.GetComponent<SequenceManager>().Next();
            }
            else if (API.LoginState == 0)
            {
                PopUpUtility.Open(player.cameraInterface.popUpCanvas, PopUpType.Error, "Wrong username or password, or server is offline", 5);
            }
        }
    }

    public void ResetTitle()
    {
        Camera.main.transform.position = campos;
        Camera.main.transform.rotation = camrot;
    }
#if UNITY_WEBGL
    void GoogleLogin()
    {
        if (LoadUtility.Web)
        SendLoginRequestToPage();
        else
            login.GetComponent<Login>().StartLogin(player);
    }
#endif
}
/*
< !DOCTYPE html >
 < html lang = "en-us" >
  
    < head >
  
      < meta charset = "utf-8" >
   
       < meta http - equiv = "Content-Type" content = "text/html; charset=utf-8" >
        

            < script src = "https://apis.google.com/js/platform.js" async defer></script>

    <title>Unity WebGL Player | HighSchoolSuperHero</title>
    <link rel="shortcut icon" href="TemplateData/favicon.ico">
    <link rel="stylesheet" href="TemplateData/style.css">
    <script src="TemplateData/UnityProgress.js"></script>
    <script src="Build/UnityLoader.js"></script>
    <script>
      var unityInstance = UnityLoader.instantiate("unityContainer", "Build/game.json", {onProgress: UnityProgress});
    </ script >
  </ head >
  < body >
      < div class= "webgl-content" >
 
           < div id = "unityContainer" style = "width: 960px; height: 600px" ></ div >
    
              < div class= "footer" >
     
                   < div class= "webgl-logo" ></ div >
      
                    < div class= "fullscreen" onclick = "unityInstance.SetFullscreen(1)" ></ div >
        
                      < div class= "title" > HighSchoolSuperHero </ div >
         
                   </ div >
         
               </ div >
         
               < script >
                   var GoogleAuth;
var SCOPE = 'https://www.googleapis.com/auth/userinfo.profile';
function handleClientLoad()
{
    // Load the API's client and auth2 modules.
    // Call the initClient function after the modules load.
    gapi.load('client:auth2', initClient);
}

function initClient()
{
    // In practice, your app can retrieve one or more discovery documents.
    var discoveryUrl = 'https://www.googleapis.com/discovery/v1/apis/drive/v3/rest';

    // Initialize the gapi.client object, which app uses to make API requests.
    // Get API key and client ID from API Console.
    // 'scope' field specifies space-delimited list of access scopes.
    gapi.client.init({
        'apiKey': 'AIzaSyAKTMaDyE9RK-qaSCWz28vzJa1-OGsIAQU',
                'clientId': '637987137452-fd0q9chij0fsjdt06cfd78onuro3vf0m.apps.googleusercontent.com',
                'discoveryDocs': [discoveryUrl],
                'scope': SCOPE
                }).then(function() {
    GoogleAuth = gapi.auth2.getAuthInstance();

    // Listen for sign-in state changes.
    GoogleAuth.isSignedIn.listen(updateSigninStatus);

    // Handle initial sign-in state. (Determine if user is already signed in.)
    var user = GoogleAuth.currentUser.get();
    setSigninStatus();

                    // Call handleAuthClick function when user clicks on
                    //      "Sign In/Authorize" button.
                    $('#sign-in-or-out-button').click(function() {
        handleAuthClick();
    });
                $('#revoke-access-button').click(function() {
        revokeAccess();
    });
});
          }

          function handleAuthClick()
{
    if (GoogleAuth.isSignedIn.get())
    {
        // User is authorized and has clicked "Sign out" button.
        GoogleAuth.signOut();
    }
    else
    {
        // User is not signed in. Start Google auth flow.
        GoogleAuth.signIn();
    }
}

function revokeAccess()
{
    GoogleAuth.disconnect();
}

function setSigninStatus()
{
    var user = GoogleAuth.currentUser.get();

    var profile = user.getBasicProfile();
    var email = profile.getEmail();
    SendEmailToUnity(email);

    var isAuthorized = user.hasGrantedScopes(SCOPE);
    if (isAuthorized)
    {
                $('#sign-in-or-out-button').html('Sign out');
                 $('#revoke-access-button').css('display', 'inline-block');
                $('#auth-status').html('You are currently signed in and have granted ' +
                'access to this app.');
    }
    else
    {
                $('#sign-in-or-out-button').html('Sign In/Authorize');
                 $('#revoke-access-button').css('display', 'none');
                $('#auth-status').html('You have not authorized this app or you are ' +
                'signed out.');
    }
}

function updateSigninStatus()
{
    setSigninStatus();
}

function SendEmailToUnity(email)
{
    unityInstance.SendMessage("HTMLReceiver", "StoreEmail", email);
}
      </ script >

      < button id = "sign-in-or-out-button"
              style = "margin-left: 25px" >
          Sign In / Authorize
      </ button >
      < button id = "revoke-access-button"
              style = "display: none; margin-left: 25px" >
          Revoke access
      </ button >

      < div id = "auth-status" style = "display: inline; padding-left: 25px" ></ div >< hr >
   

         < script src = "https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js" ></ script >
    
          < script async defer src = "https://apis.google.com/js/api.js"
              onload="this.onload=function(){};handleClientLoad()"
              onreadystatechange="if (this.readyState === 'complete') this.onload()">
      </script>
  </body>
</html>
*/