using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataUtilities;
using UnityEngine.Networking;
using System.Web;

public class Password
{
    public static string glyphs = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!?";
    public static List<string> passwords = new List<string>(){ 
    "3*^h-nN#Z^", "ccAXce4^*B", "uFT=5bL&SJ",
    "&7xwgTd3!u", "_6azEyNDQG", "*aNC_8*pc!",
    "T9EGZa8**z", "n-4D?Y63L%", "rQ8h#EmLg7",
    "p$S3YwCsZw", "J3e5e=g=6v", "Lpu284k%N_",
    "5J=B5#mN-q", "t+p-Ard26+", "6y%m!H&8Ef",
    "sFA%+3w-=q", "hvY9xP&_ZC", "d+Y9%S$v8y",
    "RbQC#46sS6", "MNpqN?^_2+", "*4q5L6rxpC",
    "Kk=uD56JKM", "P!Qr-b3FjZ", "zV@3J6MF&#",
    "npF%h8y!ER", "T&x44mEskL", "Ts!Y4aB+2W",
    "nZczt-g7##", "f_B2$8Ft#X", "u@@Js5#f?G",

    "mLV8n9^-p$", "C*vLda=2HE", "3F%2zC+v#M",
    "j4p4%^hA*e", "ra#8AfBcXy", "nLDQ8!%&p5",
    "e6?62@q-Gz", "Z^CZh#5te8", "ww2Z^q%jjp",
    "qFt&2t%@$B", "3B+ZA5n%z$", "Us72-@JAxq",
    "u^C?nRF22c", "yz8&4LPRtn", "c+Skw^*H9=",
    "b^!Gu72wwm", "cY4G-J&xEu", "Qfx$Za5_BS",
    "&yd74xbpQx", "Lf93sT*X%_", "suhN7P?gW=",
    "K&*^3uxYhJ", "^Be5LU&XJ9", "M-PJ7r=ve$",
    "&QexpK3^w!", "ur&kCee2T9", "qeR5a%-+$7",
    "w6wUc-Y6D#", "9k-csU9UFD", "nFV%Xsg5X5",

    "*7SAS5xG=r", "rHq9f$JV69", "nZrr&n&3Ga",
    "^xmB!U9kR+", "bSprL^5SF@", "n?BzR4ZTZ!",
    "-2A^A6=aKA", "!Kv^5zD-V-", "!Kv^5zD-V-",
    "h273A?k#f+", "$cB4NYVy2T", "SQUM8$B7zz",
    "r8yc!U#E8j", "#jZD7v-rQm", "kMUDNb2V&2",
    "fem3P4&pja", "=X=5y*RW^P", "?wcn5qG5gb",
    "pk+%4YEfKA", "k^8?E34epT", "3U^%6Z-Cb6",
    "m$$sGP5b3*", "UJ*u#6nV2n", "?eP+P&4r4N",
    "^8!A+_!2m3", "wtn+fM9mFP", "!U5Rm5-ss8",
    "H9pPB*5^??", "5Mv^c*MsPT", "h&f*MWKe6!",
    };
    public static string Generate(int length=8)
    {
        string password = "";
        for (int i = 0; i < length; i++)
        {
            char glyph = glyphs[UnityEngine.Random.Range(0, glyphs.Length)];
            password += glyph;
        }

        return password;
    }

}

public class Login : MonoBehaviour
{
    public string id;
    public Player agent;
    public bool done;

    const string clientID = "637987137452-rk1gfmpm7a2k6mocb5f7lr2p99mnjtmu.apps.googleusercontent.com";
    const string clientSecret = "Se4Xlh5zBwGsYylmzFrvIFKK";
    const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth/userinfo.profile";
    const string tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
    const string userInfoEndpoint = "https://accounts.google.com/.well-known/openid-configuration";

    //private IntPtr unityWindow;
    //[DllImport("user32.dll")]
    //static extern IntPtr GetActiveWindow();
    //[DllImport("user32.dll")]
    //static extern bool SetForegroundWindow(IntPtr hWnd);

#if UNITY_WEBGL

    public void StartLogin(Player agent)
    {
        //unityWindow = GetActiveWindow();
        this.agent = agent;
        Setup();
    }


    public static int GetRandomUnusedPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
 
    private async void Setup()
    {
        // Generates state and PKCE values.
        string state = randomDataBase64url(32);
        string code_verifier = randomDataBase64url(32);
        string code_challenge = base64urlencodeNoPadding(sha256(code_verifier));
        const string code_challenge_method = "S256";

        // Creates a redirect URI using an available port on the loopback address.
        string redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, GetRandomUnusedPort());
        output("redirect URI: " + redirectURI);

        // Creates an HttpListener to listen for requests on that redirect URI.
        var http = new HttpListener();
        http.Prefixes.Add(redirectURI);
        output("Listening...");
        http.Start();

        // Creates the OAuth 2.0 authorization request.
        string authorizationRequest = string.Format("{0}?response_type=code&scope=openid%20email&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
            authorizationEndpoint,
            System.Uri.EscapeDataString(redirectURI),
            clientID,
            state,
            code_challenge,
            code_challenge_method);

        // Opens request in the browser.
        System.Diagnostics.Process.Start(authorizationRequest);

        // Waits for the OAuth authorization response.
        var context = await http.GetContextAsync();

        // Brings this app back to the foreground.
        //SetForegroundWindow(unityWindow);

        // Sends an HTTP response to the browser.
        var response = context.Response;
        string responseString = string.Format("<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>");
        var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        var responseOutput = response.OutputStream;
        Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
        {
            responseOutput.Close();
            http.Stop();
            Debug.Log("HTTP server stopped.");
        });

        // Checks for errors.
        if (context.Request.QueryString.Get("error") != null)
        {
            string error = String.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error"));
            PopUpUtility.Open(agent.cameraInterface.popUpCanvas, PopUpType.Error, error, 10);
            output(error);
            return;
        }
        if (context.Request.QueryString.Get("code") == null
            || context.Request.QueryString.Get("state") == null)
        {
            string error = "Malformed authorization response. " + context.Request.QueryString;
            PopUpUtility.Open(agent.cameraInterface.popUpCanvas, PopUpType.Error, error, 10);
            output(error);
            return;
        }
        // extracts the code
        var code = context.Request.QueryString.Get("code");
        var incoming_state = context.Request.QueryString.Get("state");

        // Compares the receieved state to the expected value, to ensure that
        // this app made the request which resulted in authorization.
        if (incoming_state != state)
        {
            output(String.Format("Received request with invalid state ({0})", incoming_state));
            return;
        }
        output("Authorization code: " + code);
        // Starts the code exchange at the Token Endpoint.
        performCodeExchange(code, code_verifier, redirectURI);
    }

    async void performCodeExchange(string code, string code_verifier, string redirectURI)
    {
        output("Exchanging code for tokens...");

        // builds the  request
        string tokenRequestURI = "https://www.googleapis.com/oauth2/v4/token";
        string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
            code,
            System.Uri.EscapeDataString(redirectURI),
            clientID,
            code_verifier,
            clientSecret
            );

        // sends the request
        HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestURI);
        tokenRequest.Method = "POST";
        tokenRequest.ContentType = "application/x-www-form-urlencoded";
        tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
        tokenRequest.ContentLength = _byteVersion.Length;
        Stream stream = tokenRequest.GetRequestStream();
        await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
        stream.Close();

        try
        {
            // gets the response
            WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
            using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
            {
                // reads response body
                string responseText = await reader.ReadToEndAsync();
                output(responseText);

                // converts to dictionary
                Dictionary<string, string> tokenEndpointDecoded = JsonUtility.FromJson<Dictionary<string, string>>(responseText);

                string access_token = tokenEndpointDecoded["access_token"];
                userinfoCall(access_token);
            }
        }
        catch (WebException ex)
        {
            if (ex.Status == WebExceptionStatus.ProtocolError)
            {
                var response = ex.Response as HttpWebResponse;
                if (response != null)
                {
                    output("HTTP: " + response.StatusCode);
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        // reads response body
                        string responseText = await reader.ReadToEndAsync();
                        output(responseText);
                    }
                }

            }
        }
    }


    async void userinfoCall(string access_token)
    {
        output("Making API Call to Userinfo...");

        string oldRequest = "https://www.googleapis.com/oauth2/v3/userinfo";
        string newRequest = "https://www.googleapis.com/oauth2/v3/userinfo";
        // builds the  request
        string userinfoRequestURI = newRequest;

        // sends the request
        HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(userinfoRequestURI);
        userinfoRequest.Method = "GET";
        userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
        userinfoRequest.ContentType = "application/x-www-form-urlencoded";
        userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

        // gets the response
        WebResponse userinfoResponse = await userinfoRequest.GetResponseAsync();
        using (StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream()))
        {
            // reads response body
            string userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();
            output(userinfoResponseText);
        }
    }

    /// <summary>
    /// Appends the given string to the on-screen log, and the debug console.
    /// </summary>
    /// <param name="output">string to be appended</param>
    public void output(string output)
    {
//textBoxOutput.Text = textBoxOutput.Text + output + Environment.NewLine;
        if (output.Contains("email"))
        {
            string content = JsonExtractor.ElementExtractor(output, "email");
            agent.id = Player.Md5(content);
            done = true;
        }
    }


    /// <summary>
    /// Returns URI-safe data with a given input length.
    /// </summary>
    /// <param name="length">Input length (nb. output will be longer)</param>
    /// <returns></returns>
    public static string randomDataBase64url(uint length)
    {
        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        byte[] bytes = new byte[length];
        rng.GetBytes(bytes);
        return base64urlencodeNoPadding(bytes);
    }

    /// <summary>
    /// Returns the SHA256 hash of the input string.
    /// </summary>
    /// <param name="inputStirng"></param>
    /// <returns></returns>
    public static byte[] sha256(string inputStirng)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
        SHA256Managed sha256 = new SHA256Managed();
        return sha256.ComputeHash(bytes);
    }

    /// <summary>
    /// Base64url no-padding encodes the given input buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static string base64urlencodeNoPadding(byte[] buffer)
    {
        string base64 = Convert.ToBase64String(buffer);

        // Converts base64 to base64url.
        base64 = base64.Replace("+", "-");
        base64 = base64.Replace("/", "_");
        // Strips padding.
        base64 = base64.Replace("=", "");

        return base64;
    }
#endif
}
