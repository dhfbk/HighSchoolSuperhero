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
    public float annotationTime;

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
public class TinyAnnotationData
{
    public int id;
    public float timePerToken;
    public string type;
}

[Serializable]
public class AnnotationData
{
    public int id;
    public List<string> tokens;
    public List<string> newTokens;
    public List<int> annotations;
    public List<int> goldOffensiveTokens;
    public float time;
    public float timePerToken;
    public string task;
    public string gold;
    public int goldLabel; //0 = not gold, 1 = offensive, 2 = not offensive

    public AnnotationData() 
    {
        id = 0;
        tokens = new List<string>();
        annotations = new List<int>();
        newTokens = new List<string>();
        goldOffensiveTokens = new List<int>();
    }
    public AnnotationData(int id, List<string> tokens, List<int> annotations, float timePerToken, string task, int goldLabel, List<int> goldOffensiveTokens, List<string> newTokens = null, string gold ="", float time = 0)
    {
        this.id = id;
        this.tokens = tokens;
        this.annotations = annotations;
        this.time = time;
        this.timePerToken = timePerToken;
        this.task = task;
        this.gold = gold;
        this.goldLabel = goldLabel;
        this.goldOffensiveTokens = goldOffensiveTokens;

        if (newTokens == null)
            this.newTokens = new List<string>();


    }
    public AnnotationData(int id, string tokens, string annotations, int goldLabel, List<int> goldOffensiveTokens, float timePerToken=0, string task="", string newTokens = "", string gold="", float time = 0)
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
        this.time = time;
        this.timePerToken = timePerToken;
        this.task = task;
        if (!String.IsNullOrEmpty(newTokens))
            this.newTokens = WordByWord.RegexTokenizer(newTokens);
        else
            this.newTokens = new List<string>();
        this.gold = gold;
        this.goldLabel = goldLabel;

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
    public string time;
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

    //Constructor from raw data
    public SqlAnnotatedSentence(int id, List<string> tokens, List<int> annotations, float timePerToken, string task, List<string> newTokens=null, string gold="", float time=0)
    {
        this.id = id.ToString();
        this.tokens = string.Join(",", tokens);
        if (newTokens != null)
            this.newTokens = string.Join(",", newTokens);
        else
            this.newTokens = "";
        this.annotations = string.Join(",", annotations);
        this.time = time.ToString();
        this.timePerToken = timePerToken.ToString();
        this.task = task;
        this.gold = gold;

        multiTypeTokens = new MultiTypeTokens();
        if (task.Contains("D")) //if dialogues
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
            multiTypeTokens.annotationTime = time;
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
            multiTypeTokens.annotationTime = time;
        }
    }

    //Constructor from object
    public SqlAnnotatedSentence(AnnotationData anndata)
    {
        id = anndata.id.ToString();
        tokens = string.Join(",", anndata.tokens);
        if (anndata.newTokens != null)
            newTokens = string.Join(",", anndata.newTokens);
        else
            newTokens = "";
        annotations = string.Join(",", anndata.annotations);
        time = anndata.time.ToString();
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
            multiTypeTokens.annotationTime = anndata.time;
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
            multiTypeTokens.annotationTime = anndata.time;
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

    public static AnnotationData AnnotateModifiedText(int id, List<GameObject> tokens, float timePerToken, string task, float time = 0)
    {
        AnnotationData anndata = new AnnotationData();
        anndata.id = id;
        anndata.time = time;
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

    public static float CalculateAccuracy(AnnotationData anndata)
    {

        int userLabel = anndata.annotations.Contains(1) ? 1 : 2;

        List<int> offensiveIndeces = new List<int>();
        for (int i = 0; i < anndata.annotations.Count; i++)
        {
            if (anndata.annotations[i] == 1)
                offensiveIndeces.Add(i);
        }
        print("goldlabel = " + anndata.goldLabel);


        if (anndata.goldLabel != 0)
        {
            if (userLabel == anndata.goldLabel)
            {
                if (anndata.goldOffensiveTokens == null || anndata.goldOffensiveTokens.Count == 0)
                {
                    print("No gold TOKEN information");
                    return -1;
                }
                //Check agreement span
                foreach (int i in offensiveIndeces)
                    print("offensive index: " + i);

                List<int> intersect = anndata.goldOffensiveTokens.Intersect(offensiveIndeces).ToList();

                int p = anndata.goldOffensiveTokens.Count;
                int n = anndata.annotations.Count - p;

                int tp = intersect.Count;
                int fn = p - tp;
                int fp = offensiveIndeces.Count - tp;
                int tn = n - fp;

                float prec = (float)tp / (tp + fp);
                float rec = (float)tp / (tp + fn);

                float acc = ((float)(tp + tn) / (tp + tn + fp + fn));
                float f1 = (2 * prec * rec) / (prec + rec);

                if (rec == 0)
                    f1 = 0;
                print(f1);
                print(acc);
                return f1;
            }
            else
            {
                return 0;
            }

        }
        else //if goldLabel == 0
        {
            print("No gold information found for this sentence");
            return -1f; //if no gold is available, multiplier is default
        }
    }

}
