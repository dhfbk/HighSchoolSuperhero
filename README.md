# HighSchoolSuperhero

<h2>Opening the project</h2>
<ol>
  <li>Install Unity Hub.</li>
  <li>Install Unity 2021.3.3f through Unity Hub. Newer versions may work as well. If you can't find the right version in Unity Hub, go to the Unity download archive web page.</li>
  <li>Locate the project in your drive and open it. Importing may take up to 5-10 minutes.</li>
  <li>If an empty scene is loaded, just go to File -> Open Scene and navigate to /Assets/Scenes/SampleScene.unity or Main.unity.</li>
</ol>
Done!

<h2>Configuration</h2>
The <i>config.txt</i> file is located under /StreamingAssets/ both in the project and the final build. The file must stay in this folder in order to be editable after building the project.

<ul>
	<li>"guest":"true" - if guest is true, no API is called. Selecting DEMO at the beginning of the game has a similar effect.</li>
<li>"collectibles":"true" - this changes an option that makes the annotation mechanics rely on limited resources.</li>
	<li>"url":"http://localhost:8001/api" - Insert the api url. The default api url is already configured.</li>
<li>"useLocalDatasets":"false" - You can decide to use local sentences (to be put inside <i>dialogues.txt</i> and <i>graffiti.txt</i> under /StreamingAssets/) instead of the API ones.</li>
	</ul>
Note: <i>dialogues.txt</i> and <i>graffiti.txt</i> should contain exactly one sentence per line, without any other information.

<h2>The API</h2>
All changes to the API calls should be made inside the script API.cs located under /Assets/Scripts/Utilities/. Note:
<ul>
<li>All changes should be made inside the coroutines identified by type IEnumerator or by a "C" at the end of the method signatures.</li>
<li>Always refer to methods containing "Final". Methods containing "Dev" are backup/obsolete and do not interact with the API url contained in the configuration file.</li>
Assuming all options, such as the port, are set do default when installing the platform Docker package, the address should be http://localhost:8001/api, while the address to actually open and run the game should be http://localhost:8001/hssh.
</ul>

<h2>The login system</h2>
Changes to the login/authentication system should be made inside the <code>Authenticate()</code> coroutine in the <i>TitleScreen.cs</i> file.

<h2>Language and translations</h2>
<ul>
<li><b>Sample sentences</b>. These sentences should be inteded differently from the ones inside /StreamingAssets/Datasets/. These sentences are for demonstrative purposes only and will be shown when in DEMO mode. These sentences should be put inside a file named sampleSentences_[en/it/fr].txt.</li>
<li><b>System dialogues</b>. Dialogues between in-game characters are different from the dialogues that are shown for annotation purposes. The system dialogues have to be stored in the /StreamingAssets/ folder and have to be named dialogues_[en/it/fr].txt. The file format is .json.</li>
<li><b>System messages</b>. System messages contain messages such as "press ESC to exit" and the like. These messages are stored inside /StreamingAssets/ and are named systemMessages_[en/it/fr].txt. The file format is .json.</li>
</ul>

<h2>Changing the game difficulty/accessibility</h2>
<ul>
<li><b>Safety bar</b>. The speed of the safety bar can be changed from Unity's inspector inside the GameObject named "SafetyBar". The <i>SafetyBar.cs</i> script contains an exposed field called "Speed".</li>
<li><b>Soap bar</b>. The amount of soap that is consumed when erasing can be changed inside the <code>Erase()</code> method in the <i>Graffiti.cs</i> file.</li>
<li><b>Points/rewards</b>. The amount of points given for completed annotations can be set inside the <code>Annotate()</code> method either in <i>DialogueInstancer.cs</i> or <i>Graffiti.cs</i>.</li>
<li><b>Erasing</b>. The eraser can be made bigger or smaller. In order to change its size, search for the "Erase" prefab in the project window. The prefab can be resized in the Transform component within the Inspector.
</ul>

<h2>Useful helper methods</h2>
<ul>
<li><b>Pop-ups</b>. Show pop-ups with the static method <code>PopUpUtility.Open(FindObjectOfType&lt;CameraInterface&gt;().popUpCanvas, [type], [string], [time]);</code>.
Just substitute [type] with <code>PopUpType.Error</code>/<code>Warning</code>/<code>Success</code>; [string] with your message; [time] with how many seconds the pop-up should be displayed for. If you leave this at 0, the pop-up will wait for input.</li>
</ul>
