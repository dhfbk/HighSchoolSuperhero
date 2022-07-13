# HighSchoolSuperhero

<h2>Opening the project</h2>
<ol>
  <li>Install Unity Hub.</li>
  <li>Install Unity 2021.3.3f through Unity Hub. Newer versions may work as well.</li>
  <li>Locate the project in your drive and open it. Importing may take up to 5-10 minutes.</li>
  <li>If an empty scene is loaded, just go to File -> Open Scene and navigate to Assets/Scenes/SampleScene.unity or Main.unity.</li>
</ol>
Done!

<h2>The Configuration file</h2>
The <i>config.txt</i> file is located under StreamingAssets both in the project and the final build. The file must stay in this folder in order to be editable after building the project.

<ul>
	<li>"guest":"true"
		if guest is true, no API is called.</li>
<li>"api":"final", //this should remain as it is.</li>
<li>"collectibles":"true", //this changes an option that makes the annotation mechanics rely on limited resources.</li>
	<li>"url":"https://apiurl", //Insert the api url. The default api url is already configured.</li>
<li>"useLocalDatasets":"false" //You can decide to use local sentences (under StreamingAssets/dialogues.txt or graffiti.txt) instead of the API ones.</li>
	</ul>

<h2>The API</h2>
All changes to the API calls and systems should be made inside the script API.cs located under Assets/Scripts/Utilities. Note:
<ul>
<li>All changes should be made inside the coroutines identified by type IEnumerator or by a "C" at the end of the method signatures.</li>
<li>Always refer to methods containing "Final". Methods containing "Dev" are backup/obsolete and do not interact with the API url contained in the configuration file.</li>
</ul>
