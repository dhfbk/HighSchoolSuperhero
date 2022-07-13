using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Linq;
using System.Xml.Serialization;
using System.Xml;
using System;
using UnityEngine.UI;
using System.Net;
using System.Text;
using TMPro;

namespace DataUtilities
{
    public class LoadUtility : MonoBehaviour
    {
        //Web load
        string localhost;
        public static string StreamingAssetsPath;
        public static bool Web { get; set; }
        public static int AllowedMilliseconds { get; set; } = 250;
        public static int BufferSize { get; set; } = 2048;

        //Corpus loading variables
        char[] separators;
        public List<string> txtFiles, texts, txtTexts;
        public string webcorptext;
        public string totalTurns;
        public string lang;
        public AnnotatedDoc anndoc;
        public static bool AllLoaded { get; set; }

        //Interface
        public static AnnotatedSO annSO;
        public static List<int> Offsets { get; set; }
        public static List<string> LTxtManifest { get; set; }
        public static List<string> LJsonManifest { get; set; }
        public static List<string> LJsonTagsManifest { get; set; }
        public static List<string> LXmlManifest { get; set; }
        public static List<string> LXmlTagsManifest { get; set; }
        public static List<string> LCsvManifest { get; set; }
        public static List<string> LCsvTagsManifest { get; set; }

        public static string LoadedText { get; set; }
        public static string CorporaPath { get; set; }
        public static int Progress { get; set; }
        public static int ProgressSteps { get; set; } = 5;

        public static List<string> BadTags { get; set; }
        public static List<string> GoodTags { get; set; }
        public static List<string> Nomi { get; set; }
        public static List<string> Nomif { get; set; }
        public static List<string> TurnsByLine { get; set; }
        public static List<string> AnnotationsByLine { get; set; }


        public static bool WebloadFinished { get; set; }

        void Awake()
        {
            //if (Application.platform == RuntimePlatform.OSXPlayer)
            //    StreamingAssetsPath = Application.streamingAssetsPath;
            //else
            //    StreamingAssetsPath = Application.streamingAssetsPath;
            StreamingAssetsPath = Application.streamingAssetsPath;

            CorporaPath = StreamingAssetsPath;

            if (StreamingAssetsPath.Contains("://") || StreamingAssetsPath.Contains(":///"))
            {
                LoadUtility.Web = true;
                
                WebDebug.Print(StreamingAssetsPath);
            }
            else
            {
            }

        }
        void Start()
        {
            annSO = Resources.Load<AnnotatedSO>("AnnotatedDialogueManager");
            QualitySettings.SetQualityLevel(1, true);
            GoodTags = new List<string>();
            BadTags = new List<string>();
            BadTags.Add("Bullo");
            BadTags.Add("Bullo1");
            BadTags.Add("Bullo2");
            BadTags.Add("SupportoBullo1");
            BadTags.Add("SupportoBullo2");
            BadTags.Add("SupportoBullo3");
            GoodTags.Add("Vittima");
            GoodTags.Add("Vittima1");
            GoodTags.Add("SupportoVittima1");
            GoodTags.Add("SupportoVittima2");
            GoodTags.Add("SupportoVittima3");


            anndoc = new AnnotatedDoc()
            {
                anndata = new List<AnnotationData>()
            };
            //PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("lang", "Ita");
        }

        void Update()
        {
            //if (XmlExtractor.Loaded && JsonExtractor.Loaded && CsvExtractor.Loaded)
            //{
            //    LoadUtility.AllLoaded = true;
            //}
        }

        public static IEnumerator WebLoadAll()
        {
            //Smart manifest
            //XmlDocument manifest = new XmlDocument();
            //manifest.Load(Application.streamingAssetsPath + "/Corpora/Manifest.xml");
            //XmlNodeList txts = manifest.GetElementsByTagName("TxtFile");
            //XmlNodeList xmls = manifest.GetElementsByTagName("XmlFile");
            //XmlNodeList jsons = manifest.GetElementsByTagName("JsonFile");
            //XmlNodeList csvs = manifest.GetElementsByTagName("CsvFile");
            //WebDebug.Print(txts.Item(0).InnerText);

            //MANIFESTS
            //Load file list

            //Txt Manifest
            yield return Control.Instance.StartCoroutine(WebTextLoad("Corpora/TxtManifest.txt"));
            if (!string.IsNullOrEmpty(LoadUtility.LoadedText)) LoadUtility.LTxtManifest = Regex.Split(LoadUtility.LoadedText, "\r\n|\r|\n").ToList();
            Debug.Log("Loaded txt manifest");

            //Json Manifest
            yield return Control.Instance.StartCoroutine(WebTextLoad("Corpora/JsonTagsManifest.txt"));
            if (!string.IsNullOrEmpty(LoadUtility.LoadedText)) LoadUtility.LJsonTagsManifest = Regex.Split(LoadUtility.LoadedText, "\r\n|\r|\n").ToList();
            yield return Control.Instance.StartCoroutine(WebTextLoad("Corpora/JsonManifest.txt"));
            if (!string.IsNullOrEmpty(LoadUtility.LoadedText)) LoadUtility.LJsonManifest = Regex.Split(LoadUtility.LoadedText, "\r\n|\r|\n").ToList();
            Debug.Log("Loaded json manifest");

            //Xml Manifest
            yield return Control.Instance.StartCoroutine(WebTextLoad("Corpora/XmlTagsManifest.txt"));
            if (!string.IsNullOrEmpty(LoadUtility.LoadedText)) LoadUtility.LXmlTagsManifest = Regex.Split(LoadUtility.LoadedText, "\r\n|\r|\n").ToList();
            yield return Control.Instance.StartCoroutine(WebTextLoad("Corpora/XmlManifest.txt"));
            if (!string.IsNullOrEmpty(LoadUtility.LoadedText)) LoadUtility.LXmlManifest = Regex.Split(LoadUtility.LoadedText, "\r\n|\r|\n").ToList();
            Debug.Log("Loaded xml manifest");

            //Csv Manifest
            //yield return Control.Instance.StartCoroutine(WebTextLoad("Corpora/CsvTagsManifest.txt"));
            //if (!string.IsNullOrEmpty(LoadUtility.LoadedText)) LoadUtility.LCsvTagsManifest = Regex.Split(LoadUtility.LoadedText, "\r\n|\r|\n").ToList();
            yield return Control.Instance.StartCoroutine(WebTextLoad("Corpora/CsvManifest.txt"));
            if (!string.IsNullOrEmpty(LoadUtility.LoadedText)) LoadUtility.LCsvManifest = Regex.Split(LoadUtility.LoadedText, "\r\n|\r|\n").ToList();
            Debug.Log("Loaded csv manifest");

            WebDebug.Print("Beginning to load...");
            TextMeshPro lt = GameObject.FindGameObjectWithTag("LoadingText").GetComponent<TextMeshPro>();
            string file = "";

            //TXTS (no need for tags)
            WebDebug.Print("Loading TXTs...");
            TxtExtractor.Docs = new List<string>();
            TxtExtractor.Tagsets = new List<string>();
            TxtExtractor.AnnotatedDocs = new List<string>();
            for (int i = 0; i < LTxtManifest.Count; i++)
            {
                //Docs
                file = Path.Combine("Corpora/TXTCORPORA/", LoadUtility.LTxtManifest[i]+ ".txt");
                yield return Control.Instance.StartCoroutine(WebTextLoad(file));
                if (!string.IsNullOrEmpty(LoadUtility.LoadedText)) TxtExtractor.Docs.Add(LoadUtility.LoadedText);
                WebDebug.Print("Txt docs done");

                //Anndocs
                file = Path.Combine("Corpora/Annotated/TXT/", LoadUtility.LTxtManifest[i]+ ".txt");
                yield return Control.Instance.StartCoroutine(WebTextLoad(file));
                if (!string.IsNullOrEmpty(LoadUtility.LoadedText)) TxtExtractor.AnnotatedDocs.Add(LoadUtility.LoadedText);
                else TxtExtractor.AnnotatedDocs.Add("empty");
                WebDebug.Print("Txt anndocs done");
            }
            lt.text = "TXTs loaded. Loading Jsons...";
                LoadUtility.Progress += 1;
                LoadingManager.UpdatePercentage(LoadUtility.Progress, LoadUtility.ProgressSteps);

            //LOAD JSONS
            WebDebug.Print("Loading JSONs...");
            JsonExtractor.Docs = new List<string>();
            JsonExtractor.AnnotatedDocs = new List<string>();
            JsonExtractor.Tagsets = new List<string>();
            for (int i = 0; i < LJsonManifest.Count; i++)
            {
                //Tags
                file = Path.Combine("Corpora/JSONCORPORA/", LoadUtility.LJsonTagsManifest[i]+ ".txt");
                yield return Control.Instance.StartCoroutine(WebTextLoad(file));
                if (!string.IsNullOrEmpty(LoadUtility.LoadedText)) JsonExtractor.Tagsets.Add(LoadUtility.LoadedText);
                WebDebug.Print("json tags done");

                //Docs
                file = Path.Combine("Corpora/JSONCORPORA/", LoadUtility.LJsonManifest[i]+ ".json");
                yield return Control.Instance.StartCoroutine(WebTextLoad(file));
                if (!string.IsNullOrEmpty(LoadUtility.LoadedText)) JsonExtractor.Docs.Add(LoadUtility.LoadedText);
                WebDebug.Print("json docs done");

                //Anndocs
                file = Path.Combine("Corpora/Annotated/JSON/", LoadUtility.LJsonManifest[i]+ ".txt");
                yield return Control.Instance.StartCoroutine(WebTextLoad(file));
                if (!string.IsNullOrEmpty(LoadUtility.LoadedText)) JsonExtractor.AnnotatedDocs.Add(LoadUtility.LoadedText);
                else JsonExtractor.AnnotatedDocs.Add("empty");
                WebDebug.Print("json anndocs done");
            }
            lt.text = "Jsons loaded. Loading Xmls...";
                LoadUtility.Progress += 1;
                LoadingManager.UpdatePercentage(LoadUtility.Progress, LoadUtility.ProgressSteps);

            //LOAD XMLS
            WebDebug.Print("Loading Xmls...");
            XmlExtractor.Docs = new List<XmlDocument>();
            XmlExtractor.AnnotatedDocs = new List<string>();
            XmlExtractor.Tagsets = new List<string>();
            for (int i = 0; i < LXmlManifest.Count; i++)
            {
                //Tags
                file = Path.Combine("Corpora/XMLCORPORA/", LoadUtility.LXmlTagsManifest[i] + ".txt");
                yield return Control.Instance.StartCoroutine(WebTextLoad(file));
                if (!string.IsNullOrEmpty(LoadUtility.LoadedText)) XmlExtractor.Tagsets.Add(LoadUtility.LoadedText);
                WebDebug.Print("xml tags done");

                //Docs
                file = Path.Combine("Corpora/XMLCORPORA/", LoadUtility.LXmlManifest[i]+ ".xml");
                yield return Control.Instance.StartCoroutine(WebTextLoad(file));
                XmlDocument xmlToLoad = new XmlDocument();
                xmlToLoad.LoadXml(LoadUtility.LoadedText);
                if (!string.IsNullOrEmpty(LoadUtility.LoadedText)) XmlExtractor.Docs.Add(xmlToLoad);
                WebDebug.Print("xml docs done");

                //Anndocs
                file = Path.Combine("Corpora/Annotated/XML/", LoadUtility.LXmlManifest[i]+ ".txt");
                yield return Control.Instance.StartCoroutine(WebTextLoad(file));
                if (!string.IsNullOrEmpty(LoadUtility.LoadedText)) XmlExtractor.AnnotatedDocs.Add(LoadUtility.LoadedText);
                else XmlExtractor.AnnotatedDocs.Add("empty");
                WebDebug.Print("xml anndocs done");
            }
            lt.text = "Xmls loaded. Loading Csvs...";
                LoadUtility.Progress += 1;
                LoadingManager.UpdatePercentage(LoadUtility.Progress, LoadUtility.ProgressSteps);

            //LOAD CSVS
            WebDebug.Print("Loading CSVs...");
            CsvExtractor.Docs = new List<string>();
            CsvExtractor.AnnotatedDocs = new List<string>();
            CsvExtractor.Tagsets = new List<string>();
            for (int i = 0; i < LCsvManifest.Count; i++)
            {
                ////Tags
                //if (LoadUtility.LCsvTagsManifest.Count > 0)
                //{
                //    file = Path.Combine("Corpora/CSVCORPORA/", LoadUtility.LCsvTagsManifest[i] + ".txt");
                //    yield return Control.Instance.StartCoroutine(WebTextLoad(file));
                //    if (!string.IsNullOrEmpty(LoadUtility.LoadedText)) CsvExtractor.Tagsets.Add(LoadUtility.LoadedText);
                //    WebDebug.Print("csv tags done");
                //}

                //Docs
                file = Path.Combine("Corpora/CSVCORPORA/", LoadUtility.LCsvManifest[i]+ ".csv");
                yield return Control.Instance.StartCoroutine(WebTextLoad(file));
                if (!string.IsNullOrEmpty(LoadUtility.LoadedText)) CsvExtractor.Docs.Add(LoadUtility.LoadedText);
                WebDebug.Print("csv docs done");

                //Anndocs
                file = Path.Combine("Corpora/Annotated/CSV/", LoadUtility.LCsvManifest[i]+ ".txt");
                yield return Control.Instance.StartCoroutine(WebTextLoad(file));
                if (!string.IsNullOrEmpty(LoadUtility.LoadedText)) CsvExtractor.AnnotatedDocs.Add(LoadUtility.LoadedText);
                else CsvExtractor.AnnotatedDocs.Add("empty");
                WebDebug.Print("cslv anndocs done");
            }
            lt.text = "Csvs loaded. Cleaning...";
                LoadUtility.Progress += 1;
                LoadingManager.UpdatePercentage(LoadUtility.Progress, LoadUtility.ProgressSteps);

            //CLEAN
            annSO.turns.Clear();
            annSO.roles.Clear();
            annSO.lines.Clear();
            annSO.annotations.Clear();
            WebDebug.Print("Cleaning...");
            LoadUtility.TurnsByLine = CleanAllFiles(JsonExtractor.Docs, JsonExtractor.Tagsets, XmlExtractor.Docs, XmlExtractor.Tagsets, CsvExtractor.Docs, TxtExtractor.Docs);
            lt.text = "All files cleaned.";
            LoadUtility.Progress += 1;
                LoadingManager.UpdatePercentage(LoadUtility.Progress, LoadUtility.ProgressSteps);

            //NAMES
            yield return Control.Instance.StartCoroutine(WebTextLoad("Names/" + PlayerPrefs.GetString("lang") + "MaleNames.txt"));
            LoadUtility.Nomi = LoadUtility.LoadedText.ToString().Split('\n').ToList();
            yield return Control.Instance.StartCoroutine(WebTextLoad("Names/" + PlayerPrefs.GetString("lang") + "FemaleNames.txt"));
            LoadUtility.Nomif = LoadUtility.LoadedText.ToString().Split('\n').ToList();
            lt.text = "Names loaded";
                LoadUtility.Progress += 1;
                LoadingManager.UpdatePercentage(LoadUtility.Progress, LoadUtility.ProgressSteps);

            //SETUP
            lt.text = "Processed! Spawning...";
            WebDebug.Print("Processed! Spawning...");
            //LoadUtility.AllLoaded = true;
            lt.text = "All loaded";
            WebDebug.Print("All loaded");
        }
        public static IEnumerator WebTextLoad(string file)
        {
            string path = Path.Combine(StreamingAssetsPath, file);

            WebDebug.Print("Starting the web request...");
            UnityWebRequest myWr = UnityWebRequest.Get(path);         
            WebDebug.Print(file + " loading...");
            yield return myWr.SendWebRequest();
            if (!myWr.isNetworkError && !myWr.isHttpError)
            {
                WebDebug.Print(file + " loaded successfully from: " + path);
                LoadUtility.LoadedText = myWr.downloadHandler.text;
            }
            else
            {
                WebDebug.Print("Could not load" + file);
                WebDebug.Print(myWr.error);
                LoadUtility.LoadedText = "";
            }
        }

        public static IEnumerator DiskLoadAll() //I use static variables because it is simpler with coroutines
        {
            //Load xml and json files
            TextMeshPro lt = GameObject.FindGameObjectWithTag("LoadingText").GetComponent<TextMeshPro>();
            lt.text = "Loading resources...";
            yield return Control.Instance.StartCoroutine(JsonExtractor.LoadJsonsOverFrames());
            LoadUtility.Progress += 1;
            LoadingManager.UpdatePercentage(LoadUtility.Progress, LoadUtility.ProgressSteps);
            lt.text = "Jsons loaded. Loading Xmls...";
            //

            yield return Control.Instance.StartCoroutine(XmlExtractor.LoadXmlsOverFrames());
            LoadUtility.Progress += 1;
            LoadingManager.UpdatePercentage(LoadUtility.Progress, LoadUtility.ProgressSteps);
            lt.text = "Xmls loaded. Loading Csvs...";
            //

            yield return Control.Instance.StartCoroutine(CsvExtractor.LoadCsvsOverFrames());
            LoadUtility.Progress += 1;
            LoadingManager.UpdatePercentage(LoadUtility.Progress, LoadUtility.ProgressSteps);
            lt.text = "Csvs loaded. Loading Txts...";
            //

            yield return Control.Instance.StartCoroutine(TxtExtractor.LoadTxtsOverFrames());
            LoadUtility.Progress += 1;
            LoadingManager.UpdatePercentage(LoadUtility.Progress, LoadUtility.ProgressSteps);
            lt.text = "Txts loaded. Cleaning...";
            //

            //Load tags
            List<string> jTagsets = JsonExtractor.LoadJsonTags();
            List<string> xTagsets = XmlExtractor.LoadXmlTags();
            List<string> cTagsets = CsvExtractor.LoadCsvTags();

            annSO.turns.Clear();
            annSO.roles.Clear();
            annSO.lines.Clear();
            annSO.annotations.Clear();
            //Clean and aggregate files
            LoadUtility.TurnsByLine = new List<string>();
            try
            {
                LoadUtility.TurnsByLine = CleanAllFiles(JsonExtractor.Docs, jTagsets, XmlExtractor.Docs, xTagsets, CsvExtractor.Docs, TxtExtractor.Docs);
            }
            catch (Exception e)
            {
                WebDebug.Print(e.Message);
            }
            LoadUtility.Progress += 1;
            LoadingManager.UpdatePercentage(LoadUtility.Progress, LoadUtility.ProgressSteps);
            lt.text = "Cleaned!";

            //Load conversation names
            LoadUtility.Nomi = File.ReadAllLines(LoadUtility.CorporaPath + "/Names/" + PlayerPrefs.GetString("lang") + "MaleNames.txt").ToList();
            WebDebug.Print("Male names loaded");
            LoadUtility.Nomif = File.ReadAllLines(LoadUtility.CorporaPath + "/Names/" + PlayerPrefs.GetString("lang") + "FemaleNames.txt").ToList();
            WebDebug.Print("Female names loaded");

                //LoadUtility.AllLoaded = true;
                WebDebug.Print("All loaded successfully!");

        }

        public static List<string> CleanAllFiles(List<string> jsonTexts, List<string> jTagsets, List<XmlDocument> xmlTexts, List<string> xTagsets, List<string> csvTexts, List<string> txtTexts)
        {
            //Delcare lists of cleaned lines
            List<string> cleanJsons = new List<string>();
            List<string> cleanXmls = new List<string>();
            List<string> cleanCsvs = new List<string>();
            List<string> cleanTxts = new List<string>();
            //TxtTexts are always clean
            List<string> texts = new List<string>();
            //Clean jsons
            List<string> partialAnnLines;
            if (jsonTexts.Count > 0)
            {
                List<string> turns;
                WebDebug.Print("Cleaning jsons...");
                for (int i = 0; i < jsonTexts.Count; i++)
                {
                    turns = JsonExtractor.LineExtractor(jsonTexts[i], jTagsets[i]);
                    cleanJsons.AddRange(turns);
                    annSO.roles.AddRange(GetRoles(turns));
                    annSO.lines.AddRange(GetLines(turns));
                    if (JsonExtractor.AnnotatedDocs.Count > 0)
                    {
                        if (JsonExtractor.AnnotatedDocs[i] != "empty")
                        {
                            WebDebug.Print("Retrieving json gold list...");
                            ReconstructGoldList(JsonExtractor.AnnotatedDocs[i], turns);
                            WebDebug.Print("Retrieved!");
                        }
                        else
                        {
                            for (int w = 0; w < turns.Count; w++)
                            {
                                annSO.annotations.Add("");
                            }
                        }
                    }
                }
                texts.AddRange(cleanJsons);
                WebDebug.Print("Jsons cleaned!");
            }

            //Clean xmls
            if (xmlTexts.Count > 0)
            {
                List<string> turns;
                WebDebug.Print("Cleaning xmls...");
                for (int i = 0; i < xmlTexts.Count; i++)
                {
                    turns = XmlExtractor.LineExtractor(xmlTexts[i], xTagsets[i]);
                    cleanXmls.AddRange(turns);
                    annSO.roles.AddRange(GetRoles(turns));
                    annSO.lines.AddRange(GetLines(turns));
                    if (XmlExtractor.AnnotatedDocs.Count > 0)
                    {
                        if (XmlExtractor.AnnotatedDocs[i] != "empty")
                        {
                            WebDebug.Print("Retrieving xml gold list...");
                            ReconstructGoldList(XmlExtractor.AnnotatedDocs[i], turns);
                            WebDebug.Print("Retrieved!");
                        }
                        else
                        {
                            for (int w = 0; w < turns.Count; w++)
                            {
                                annSO.annotations.Add("");
                            }
                        }
                    }
                }
                texts.AddRange(cleanXmls);
                WebDebug.Print("Xmls cleaned!");
            }

            if (csvTexts.Count > 0)
            {
                List<string> turns;
                WebDebug.Print("Cleaning csvs...");
                for (int i = 0; i < csvTexts.Count; i++)
                {
                    turns = CsvExtractor.LineExtractor(csvTexts[i]);
                    cleanCsvs.AddRange(turns);
                    annSO.roles.AddRange(GetRoles(turns));
                    annSO.lines.AddRange(GetLines(turns));
                    if (CsvExtractor.AnnotatedDocs.Count > 0)
                    {
                        if (CsvExtractor.AnnotatedDocs[i] != "empty")
                        {
                            WebDebug.Print("Retrieving csv gold list...");
                            ReconstructGoldList(CsvExtractor.AnnotatedDocs[i], turns);
                            WebDebug.Print("Retrieved!");
                        }
                        else
                        {
                            for (int w = 0; w < turns.Count; w++)
                            {
                                annSO.annotations.Add("");
                            }
                        }
                    }
                }
                texts.AddRange(cleanCsvs);
                WebDebug.Print("Csvs cleaned!");
            }

            if (txtTexts.Count > 0)
            {
                List<string> turns;
                WebDebug.Print("Cleaning txts...");
                TxtExtractor.mean = new List<int>();
                for (int i = 0; i < txtTexts.Count; i++)
                {
                    turns = TxtExtractor.LineExtractor(txtTexts[i]);
                    annSO.roles.AddRange(GetRoles(turns));
                    annSO.lines.AddRange(GetLines(turns));
                    cleanTxts.AddRange(turns);
                    if (TxtExtractor.AnnotatedDocs.Count > 0)
                    {
                        if (TxtExtractor.AnnotatedDocs[i] != "empty")
                        {
                            WebDebug.Print("Retrieving txt gold list...");
                            ReconstructGoldList(TxtExtractor.AnnotatedDocs[i], turns);
                            WebDebug.Print("Retrieved!");
                        }
                        else
                        {
                            for (int w = 0; w < turns.Count; w++)
                            {
                                annSO.annotations.Add("");
                            }
                        }
                    }
                }
                texts.AddRange(cleanTxts);
                WebDebug.Print("Txts cleaned!");
            }
            WebDebug.Print("Returning texts...");
            //Return final texts block
            return texts;
        }
        public static List<string> GetRoles(List<string> lines)
        {
            List<string> newList = new List<string>();
            foreach (string line in lines)
            {
                newList.Add(line.Split(' ')[0]);
            }
            return newList;
        }
        public static List<string> GetLines(List<string> lines)
        {
            string role;
            List<string> newList = new List<string>();
            foreach (string line in lines)
            {
                role = line.Split(' ')[0];
                if (line.Length > role.Length)
                    newList.Add(line.Substring(role.Length + 1));
                else
                    newList.Add("");
            }
            return newList;
        }

        public static void ReconstructGoldList(string rawGolds, List<string> lines)
        {
            List<string> partialAnnLines;
            partialAnnLines = Regex.Split(rawGolds, "\r\n|\r|\n").ToList();
            string id;
            string annotation;
            int offset = 0;
            int coveredLength = 0;
            for (int j = 0; j < partialAnnLines.Count;)
            {
                id = partialAnnLines[j].Split(' ')[0];
                annotation = partialAnnLines[j].Substring(id.Length + 1);

                if (int.Parse(id) == j + offset)
                {
                    annSO.annotations.Add(annotation);
                    coveredLength += 1;
                    j += 1;
                }
                else
                {
                    annSO.annotations.Add("");
                    coveredLength += 1;
                    offset += 1;
                    if (offset >= lines.Count)
                        break;
                }
            }
            int diff = lines.Count - coveredLength;
            if (diff > 0)
            {
                for (int w = 0; w < diff; w++)
                {
                    annSO.annotations.Add("");
                }
            }
        }
    }
}