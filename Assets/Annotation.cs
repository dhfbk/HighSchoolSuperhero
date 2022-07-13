using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System;
using UnityEngine.Networking;
using DataUtilities;
using System.IO;
using System.Linq;
using TMPro;
public class AnnotationException : Exception
{
    public AnnotationException() { }
    public AnnotationException(string message) : base(message) { }
}
[Serializable]
public class MultiTypeTokens
{
    public List<string> tokens;

    public MultiTypeTokens()
    {
        tokens = new List<string>();
    }
}
//[Serializable]
//public class MultiTypeTokensString
//{
//    public List<string> tokens;

//    public MultiTypeTokensString()
//    {
//        tokens = new List<string>();
//    }
//}
[Serializable]
public class AnnotationData
{
    public int id;
    public List<string> tokens;
    public List<string> newTokens;
    public List<int> annotations;
    public float timePerToken;
    public string task;
    public string gold;
    public AnnotationData() 
    {
        id = 0;
        tokens = new List<string>();
        annotations = new List<int>();
        newTokens = new List<string>();
    }
    public AnnotationData(int id, List<string> tokens, List<int> annotations, float timePerToken, string task, List<string> newTokens = null, string gold ="")
    {
        this.id = id;
        this.tokens = tokens;
        this.annotations = annotations;
        this.timePerToken = timePerToken;
        this.task = task;
        this.gold = gold;

        if (newTokens == null)
            this.newTokens = new List<string>();
    }
    public AnnotationData(int id, string tokens, string annotations, float timePerToken=0, string task="", string newTokens = "", string gold="")
    {
        this.id = id;
        this.tokens = WordByWord.RegexTokenizer(tokens);
        if (annotations != "")
        {
            List<string> temp = annotations.Split(',').ToList();
            this.annotations = temp.Select(x => Convert.ToInt32(x)).ToList();
        }
        else
        {
            int[] values = new int[this.tokens.Count];
            this.annotations = values.ToList();
        }
        this.timePerToken = timePerToken;
        this.task = task;
        if (!String.IsNullOrEmpty(newTokens))
            this.newTokens = WordByWord.RegexTokenizer(newTokens);
        else
            this.newTokens = new List<string>();
        this.gold = gold;
    }

    public void CleanSpaces()
    {
        List<string> templist = new List<string>();
        foreach(string s in this.tokens)
        {
            if (s[s.Length - 1] == ' ')
                templist.Add(s.Remove(s.Length - 1));
            else
                templist.Add(s);
        }
        //this.tokens.Clear();
        this.tokens = templist;
    }
}

[Serializable]
public class SqlAnnotatedSentence
{
    public string id;
    public string tokens;
    public string newTokens;
    public string annotations;
    public string timePerToken;
    public string task;
    public string gold;
    public string json;
    public MultiTypeTokens multiTypeTokens;

    public SqlAnnotatedSentence() 
    {
        this.id = "0";
        this.tokens = "frase di graffiti di prova";
        this.newTokens = "";
        this.timePerToken = "0.25";
        this.task = "gr";
        this.multiTypeTokens = new MultiTypeTokens();
    }
    public SqlAnnotatedSentence(int id, List<string> tokens, List<int> annotations, float timePerToken, string task, List<string> newTokens=null, string gold="")
    {
        this.id = id.ToString();
        this.tokens = string.Join(",", tokens);
        if (newTokens != null)
            this.newTokens = string.Join(",", newTokens);
        else
            this.newTokens = "";
        this.annotations = string.Join(",", annotations);
        this.timePerToken = timePerToken.ToString();
        this.task = task;
        this.gold = gold;
        multiTypeTokens = new MultiTypeTokens();
        if (task.Contains("D"))
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] != newTokens[i])
                {
                    if (string.IsNullOrEmpty(newTokens[i]) || newTokens[i] == " ")
                    {
                        multiTypeTokens.tokens.Add(Annotation.trueLabel);
                    }
                    else
                    {
                        multiTypeTokens.tokens.Add(newTokens[i]);
                    }
                }
                else
                {
                    multiTypeTokens.tokens.Add(Annotation.falseLabel);
                }
            }
        }
        else
        {
            foreach (int ann in annotations)
            {
                if (ann == 1)
                    multiTypeTokens.tokens.Add(Annotation.trueLabel);
                else
                    multiTypeTokens.tokens.Add(Annotation.falseLabel);
            }
        }
    }
    public SqlAnnotatedSentence(AnnotationData anndata)
    {
        id = anndata.id.ToString();
        tokens = string.Join(",", anndata.tokens);
        if (anndata.newTokens != null)
            newTokens = string.Join(",", anndata.newTokens);
        else
            newTokens = "";
        annotations = string.Join(",", anndata.annotations);
        timePerToken = anndata.timePerToken.ToString();
        task = anndata.task;
        gold = anndata.gold;

        multiTypeTokens = new MultiTypeTokens();

        if (anndata.task.Contains("D"))
        {
            for (int i = 0; i < anndata.tokens.Count; i++)
            {
                if (anndata.tokens[i] != anndata.newTokens[i])
                {
                    if (string.IsNullOrEmpty(anndata.newTokens[i]) || anndata.newTokens[i] == " ")
                    {
                        multiTypeTokens.tokens.Add(Annotation.trueLabel);
                    }
                    else
                    {
                        multiTypeTokens.tokens.Add(anndata.newTokens[i]);
                    }
                }
                else
                {
                    multiTypeTokens.tokens.Add(Annotation.falseLabel);
                }
            }
        }
        else
        {
            foreach (int ann in anndata.annotations)
            {
                if (ann == 1)
                    multiTypeTokens.tokens.Add(Annotation.trueLabel);
                else
                    multiTypeTokens.tokens.Add(Annotation.falseLabel);
            }
        }
    }
}

public class JSONSentence
{
    public string json;
    public JSONSentence(SqlAnnotatedSentence annsent)
    {
        json = JsonUtility.ToJson(annsent);
    }
}

[Serializable]
public class AnnotatedDoc
{
    public List<AnnotationData> anndata;
    public AnnotatedDoc() { anndata = new List<AnnotationData>(); }
}
public class Annotation : MonoBehaviour {

    public bool modified;
    public string memtoken;
    public int ID;
    public static string trueLabel;
    public static string falseLabel;

    private void Start()
    {
    }

    public static void ValidateAnnotatedSentence(SqlAnnotatedSentence sqlsent)
    {
        if (String.IsNullOrEmpty(sqlsent.id))
            throw new AnnotationException("Sentence ID is missing or wrong format");
        if (String.IsNullOrEmpty(sqlsent.tokens))
            throw new AnnotationException("Tokens are missing or wrong format");
        if (String.IsNullOrEmpty(sqlsent.annotations))
            throw new AnnotationException("Annotations are missing or wrong format");
    }

    public static AnnotationData AnnotateModifiedText(int id, List<GameObject> tokens, float timePerToken, string task)
    {
        AnnotationData anndata = new AnnotationData();
        anndata.id = id;
        char[] charsToTrim = { ' ', '\u00A0' };
        foreach (GameObject tok in tokens)
        {
            if (tok.GetComponent<Annotation>().modified)
            {
                string s = tok.GetComponent<Annotation>().memtoken;


                //if (s[s.Length - 1] == ' ')
                //    anndata.tokens.Add(s.Remove(s.Length - 1));
                //else
                //anndata.tokens.Add(s);
                anndata.tokens.Add(s.Trim(charsToTrim));
                anndata.annotations.Add(1); //Add 1 to vector if marked

                string newt = tok.GetComponent<TextMeshPro>().text;
                if (newt.Length > 0)
                {
                    if (newt[newt.Length - 1] == ' ')
                        anndata.newTokens.Add(newt.Remove(newt.Length - 1));
                    else
                        anndata.newTokens.Add(newt);
                }
                else
                {
                    anndata.newTokens.Add("");
                }
            }
            else
            {
                string s = tok.GetComponent<TextMeshPro>().text;
                //if (s[s.Length - 1] == ' ')
                //{
                //    anndata.tokens.Add(s.Remove(s.Length - 1));
                //    anndata.newTokens.Add(s.Remove(s.Length - 1));
                //}
                //else
                //{
                //    anndata.tokens.Add(s);
                //    anndata.newTokens.Add(s);
                //}
                anndata.tokens.Add(s.Trim(charsToTrim));
                anndata.newTokens.Add(s.Trim(charsToTrim));
                anndata.annotations.Add(0); //Add 0 to vector if not marked
            }
        }
        anndata.timePerToken = timePerToken;
        anndata.task = task;
        return anndata;
    }

    public static List<string> GetNewTokens(List<GameObject> tokens)
    {
        List<string> messageTokens = new List<string>();
        foreach (GameObject tok in tokens)
        {
            messageTokens.Add(tok.GetComponent<TextMeshPro>().text);
        }
        return messageTokens;
    }

    public static float SilverCompare(AnnotationData currentSentence)
    {
        return 0;
    }

    public static float GoldCompare(AnnotationData current, AnnotationData old)
    {
        return 0;
    }


    //public static IEnumerator Annotate(Player agent, MonoBehaviour mb, string taskType = "R", Variant variant)
    //{
    //    if (variant == Variant.D)
    //    {
    //        float agreement = 0;
    //        List<GameObject> tokens = MessageUtility.FindTokens();
    //        float timePerToken = PlayerLogger.CalculateTimePerToken(tokens.Count, agent.playerLogger.StopSentenceAnnotationSW());
    //        AnnotationData anndata = Annotation.AnnotateModifiedText(DialogueInstancer.uniqueLineIndex, tokens, timePerToken, Player.condition + taskType);

    //        string goldann = "";
    //        WWWForm form = new WWWForm();
    //        form.AddField("ID", anndata.id);
    //        using (UnityWebRequest www = UnityWebRequest.Post(API.urls.getGoldDialogues, form))
    //        {
    //            yield return www.SendWebRequest();

    //            if (www.error == null && !www.downloadHandler.text.Contains("errore"))
    //                goldann = www.downloadHandler.text;
    //        }

    //        int annotated = anndata.annotations.Contains(1) ? 0 : 1;

    //        if (!String.IsNullOrEmpty(goldann))
    //        {
    //            //AnnotationData goldSentence = new AnnotationData(iLine, sqlSentence, goldann);
    //            //agreement = Annotation.GoldCompare(anndata, goldSentence);
    //            anndata.gold = goldann;
    //            if (int.Parse(goldann) == annotated)
    //                agreement = 1;
    //            else
    //                agreement = 0;
    //        }
    //        else
    //        {
    //            anndata.gold = "1";
    //        }

    //        int points = 5 + (int)(10 * agreement);

    //        if (timePerToken > 0.25f)
    //        {
    //            points *= Player.pointMultiplier;
    //            PointSystem.AddPoints(mb, agent, points);
    //            mb.GetComponent<SpawnCrystals>().Spawn(agent);
    //        }
    //        SafetyBar.AddSafety((20 + 20 * agreement) * Player.pointMultiplier);
    //        agent.TotalAnnotatedDialogues += 1;

    //        //Save
    //        API.PostAnnotation(agent, anndata);

    //        DialogueInstancer.IncrementIndex();

    //        API.PostSave(agent, false);
    //    }
    //    else
    //    {
    //        List<int> occludedTokens = CalculateOcclusion(tokens);
    //        float timePerToken = PlayerLogger.CalculateTimePerToken(currentAnnSent.tokens.Count, agent.playerLogger.GetGraffitiAnnotationTime());
    //        string tasktype = Restriction ? "GR" : "G";
    //        tasktype = Player.condition + tasktype;
    //        AnnotationData anndata = new AnnotationData(currentAnnSent.id, currentAnnSent.tokens, occludedTokens, timePerToken, tasktype);
    //        anndata.CleanSpaces();
    //        string goldann = "";
    //        float agreement = 0;
    //        //if (currentAnnSent.annotations.Count > 0) //if gold annotation data was found in LoadUtility.annSO at currentAnnSent index
    //        //    agreement = Annotation.GoldCompare(anndata, currentAnnSent);
    //        //else
    //        //    agreement = Annotation.SilverCompare(currentAnnSent);
    //        WWWForm form = new WWWForm();
    //        form.AddField("ID", anndata.id);
    //        using (UnityWebRequest www = UnityWebRequest.Post(API.urls.getGoldGraffiti, form))
    //        {
    //            yield return www.SendWebRequest();

    //            if (www.error == null && !www.downloadHandler.text.Contains("errore"))
    //                goldann = www.downloadHandler.text;
    //        }

    //        int annotated = anndata.annotations.Contains(1) ? 0 : 1;

    //        if (!String.IsNullOrEmpty(goldann))
    //        {
    //            //AnnotationData goldSentence = new AnnotationData(iLine, sqlSentence, goldann);
    //            //agreement = Annotation.GoldCompare(anndata, goldSentence);
    //            anndata.gold = goldann;

    //            if (int.Parse(goldann) == annotated)
    //                agreement = 1;
    //            else
    //                agreement = 0;
    //        }
    //        else
    //        {
    //            anndata.gold = "1";
    //        }

    //        if (exclamationIcon)
    //        {
    //            DestroyImmediate(exclamationIcon);
    //            questSatisfied = true;
    //        }

    //        if (!agent.gameState.annotatedGraffitiIndeces.Contains(currentAnnSent.id))
    //            agent.gameState.annotatedGraffitiIndeces.Add(currentAnnSent.id);

    //        API.PostAnnotation(agent, anndata);


    //        if (!pointsAlreadyGiven)
    //        {
    //            GetComponent<SpawnCrystals>().Spawn(agent);
    //            int points = 5 + (int)(15 * agreement);
    //            points *= Player.pointMultiplier;
    //            Agent.TotalAnnotatedGraffiti += 1;
    //            PointSystem.AddPoints(this, agent, points);
    //            SafetyBar.AddSafety((20 + 20 * agreement) * Player.pointMultiplier);
    //            if (Player.condition == Condition.W3D)
    //                pointsAlreadyGiven = true;
    //        }

    //        API.PostSave(agent, false);
    //    }
    //}

}
