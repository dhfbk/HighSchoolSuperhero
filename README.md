# HighSchoolSuperhero

<h2>Opening the project</h2>
<ol>
  <li>Install Unity Hub.</li>
  <li>Install Unity 2021.3.3f through Unity Hub. Newer versions may work as well.</li>
  <li>Locate the project in your drive and open it. Importing may take up to 5-10 minutes.</li>
  <li>If an empty scene is loaded, just go to File -> Open Scene and navigate to /Assets/Scenes/SampleScene.unity or Main.unity.</li>
</ol>
Done!

<h2>Configuration</h2>
The <i>config.txt</i> file is located under /StreamingAssets/ both in the project and the final build. The file must stay in this folder in order to be editable after building the project.

<ul>
	<li>"guest":"true" - if guest is true, no API is called. Selecting DEMO at the beginning of the game has a similar effect.</li>
<li>"api":"final" - this should remain as it is.</li>
<li>"collectibles":"true" - this changes an option that makes the annotation mechanics rely on limited resources.</li>
	<li>"url":"https://apiurl" - Insert the api url. The default api url is already configured.</li>
<li>"useLocalDatasets":"false" - You can decide to use local sentences (to be put inside <i>dialogues.txt</i> and <i>graffiti.txt</i> under /StreamingAssets/) instead of the API ones.</li>
	</ul>
Note: <i>dialogues.txt</i> and <i>graffiti.txt</i> should contain exactly one sentence per line, without any other information.

<h2>The API</h2>
All changes to the API calls should be made inside the script API.cs located under /Assets/Scripts/Utilities/. Note:
<ul>
<li>All changes should be made inside the coroutines identified by type IEnumerator or by a "C" at the end of the method signatures.</li>
<li>Always refer to methods containing "Final". Methods containing "Dev" are backup/obsolete and do not interact with the API url contained in the configuration file.</li>
</ul>

<h2>The login system</h2>
Changes to the login/authentication system should be made inside the Authenticate() coroutine inside the <i>TitleScreen.cs</i> file.

<h2>Changing the game difficulty</h2>
<ul>
<li>Safety bar. The speed of the safety bar can be changed from Unity's inspector inside the GameObject named "SafetyBar". The <i>SafetyBar.cs</i> script contains an exposed field called "Speed".</li>
<li>Soap bar. The amount of soap that is consumed when erasing can be changed inside the `Erase()` method in the <i>Graffiti.cs</i> file.</li>
<li>Points/rewards. The amount of points given for completed annotations can be set inside the `Annotate()` method either in <i>DialogueInstancer.cs</i> or Graffiti.cs.</li>
</ul>
