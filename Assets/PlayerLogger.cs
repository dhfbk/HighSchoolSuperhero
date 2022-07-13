using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;
using System.Xml.Serialization;
using System;
using System.Threading;
using System.Globalization;
using System.IO;
using DataUtilities;
using UnityEngine.Networking;

[Serializable]
public class PlayerLog
{
    public string id;
    public float CustomizationTime;
    public float GameTime;
    public float GraffitiTime;
    public float DialogueTime;

    public int NumberOfGraffitiActivated;
    public int NumberOfAnnotatedGraffiti;
    public int NumberOfObservedGraffiti;
    ////Within
    //public List<float> GraffitiTimePerToken;
    //public float AvgTimePerGraffitiNormalized;

    ///Between
    //public float AvgTimePassedBetweenGraffiti { get => Average(TimePassedBetweenGraffiti); }
    //public List<float> TimePassedBetweenGraffiti;

    public int NumberOfDialoguesActivated;
    public int NumberOfAnnotatedSentences;
    public int NumberOfObservedSentences;

    //public List<float> SentenceTimes;
    //public List<float> SentenceAnnotationPercentages;
    //public List<float> SentenceTimePerToken;
    //public float AvgSentenceAnnotationPercentage { get => Average(SentenceAnnotationPercentages); }
    //public float AvgTimePerSentence { get => Average(SentenceTimes); }
    //public float AvgTimePassedBetweenSentences;
    //public List<float> TimePassedBetweenDialogues;
    //public float AvgTimePassedBetweenDialogues { get => Average(TimePassedBetweenDialogues); }

    public PlayerLog()
    {
        //PlayerLogger.playerLog = this;
        //GraffitiTimePerToken = new List<float>();
        //TimePassedBetweenGraffiti = new List<float>();
        //TimePassedBetweenDialogues = new List<float>();
        //SentenceTimes = new List<float>();
        //SentenceTimePerToken = new List<float>();
    }

    private float Average(List<float> list)
    {
        float sum = list.Sum();
        return sum / list.Count;
    }
}

public class PlayerLogger : MonoBehaviour
{
    //SW
    public Stopwatch gasw;
    public Stopwatch dasw;
    public Stopwatch sasw;
    public static Stopwatch casw;
    public Stopwatch gametimesw;
    public PlayerLog playerLog;

    private void Start()
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-GB", false);
        gasw = new Stopwatch();
        dasw = new Stopwatch();
        sasw = new Stopwatch();
        casw = new Stopwatch();
        gametimesw = new Stopwatch();
    }

    //Annotation
    public void StartGameTimeSW()
    {
        gametimesw.Start();
    }
    public void StopGameTimeSW()
    {
        gametimesw.Stop();
    }
    public void StartGraffitiAnnotationSW()
    {
        gasw.Reset();
        gasw.Start();
    }
    public void StopGraffitiAnnotationSW()
    {
        gasw.Stop();
        playerLog.NumberOfAnnotatedGraffiti += 1;
        playerLog.GraffitiTime += gasw.ElapsedMilliseconds / 1000f;
    }

    public float GetGraffitiAnnotationTime()
    {
        return gasw.ElapsedMilliseconds / 1000f;
    }

    public void StartDialogueAnnotationSW()
    {
        dasw.Reset();
        dasw.Start();
    }
    public void StopDialogueAnnotationSW()
    {
        playerLog.NumberOfDialoguesActivated += 1;
        playerLog.DialogueTime += dasw.ElapsedMilliseconds / 1000f;
        dasw.Stop();
    }
    public float GetDialogueAnnotationTime()
    {
        return dasw.ElapsedMilliseconds / 1000f;
    }

    public void StartSentenceAnnotationSW()
    {
        sasw.Reset();
        sasw.Start();
    }

    public float StopSentenceAnnotationSW()
    {
        sasw.Stop();
        return sasw.ElapsedMilliseconds / 1000f;
    }

    //public static void StartCustomizationSW()
    //{
    //    casw.Reset();
    //    casw.Start();
    //    print("started");
    //}

    //public static float StopCustomizationSW()
    //{
    //    casw.Stop();
    //    playerLog.CustomizationTime = casw.ElapsedMilliseconds / 1000f;
    //    return casw.ElapsedMilliseconds / 1000f;
    //    print(playerLog.CustomizationTime);
    //}

    private void Update()
    {
        //playerLog.GameTime = gametimesw.ElapsedMilliseconds / 1000f;
    }

    public static float CalculateTimePerToken(int tokenCount, float time)
    {
        float timePerToken = time / tokenCount;
        return timePerToken;
    }
    public static float ThinkCloudQualityControl(AnnotationData anndata)
    {
        List<int> totalMarked = new List<int>();
        totalMarked.AddRange(anndata.annotations.Where(x => x == 1));
        return PlayerLogger.AnnotationRate(totalMarked.Count, anndata.tokens.Count);
    }

    public static float AnnotationRate(int marked, int total)
    {
        float percentage = (float)marked / total;
        return percentage;
    }

    private void OnApplicationQuit()
    {
        //SaveManager.Save(playerLog, "playerLog" + " " + GetComponent<Player>().id, Application.streamingAssetsPath);
        //PlayerLogSql playerLogSql = new PlayerLogSql(GetComponent<Player>().id, playerLog);
        //SaveManager.Save(playerLogSql, "playerLogSql" + " " + GetComponent<Player>().id, Application.streamingAssetsPath);
    }

    //public static IEnumerator WebLog(Player agent, PlayerLogSql playerLog)
    //{
    //    if (playerLog.id == null)
    //    {
    //        NotificationUtility.ShowString(agent, "Not every variable set!");
    //    }
    //    string path = Path.Combine(Application.streamingAssetsPath);

    //    //Get php url
    //    string url;
    //    if (LoadUtility.Web)
    //    {
    //        yield return Control.Instance.StartCoroutine(LoadUtility.WebTextLoad("Url.txt"));
    //        if (LoadUtility.LoadedText != null)
    //        {
    //            url = LoadUtility.LoadedText.ToString();
    //        }
    //        else
    //        {
    //            url = "http://localhost/dev/post.php";
    //        }
    //    }
    //    else
    //    {
    //        //url = "http://localhost/dev/post.php";
    //        url = File.ReadAllLines(Path.Combine(Application.streamingAssetsPath, "Url.txt"))[0].Trim();
    //    }

    //    WWWForm form = new WWWForm();
    //    try
    //    {
    //        form.AddField("UserID", playerLog.id);

    //        //Idle data
    //        form.AddField("NumberOfAnnotatedGraffiti", playerLog.NumberOfAnnotatedGraffiti);
    //        form.AddField("NumberOfDialoguesActivated", playerLog.NumberOfDialoguesActivated);
    //        form.AddField("TimePassedBetweenDialogues", playerLog.TimePassedBetweenDialogues);
    //        form.AddField("TimePassedBetweenGraffiti", playerLog.TimePassedBetweenGraffiti);
    //    }
    //    catch (Exception e)
    //    {
    //        NotificationUtility.ShowString(agent, e.Message);
    //    }

    //    using (UnityWebRequest www = UnityWebRequest.Post(url, form))
    //    {
    //        print("Posting...");
    //        yield return www.SendWebRequest();

    //        if (www.error != null)
    //        {
    //            NotificationUtility.ShowString(agent, www.error);
    //        }
    //        else
    //        {
    //            NotificationUtility.ShowString(agent, "Logged!");
    //        }
    //    }
    //}
}
