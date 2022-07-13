using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;

public class WordByWord : MonoBehaviour
{
    public string text;
    public GameObject canvas;
    public int i;
    public bool write;
    public float t;
    public GameObject precWord;
    public int precLetters;
    public bool calltext;
    public string path;
    public List<GameObject> tokens;
    private GameObject tokenPrefab;

    public string battuta, messaggio, sentence;
    public DialogueManagerScriptableObject dialogo;
    public List<string> files;
    //Provvisoria
    public GameObject bully, msgbox, lastLine;

    public List<GameObject> lines;
    public int wordsPerLine;

    //new system
    public string file;
    public List<string> roles;
    public string aut;

    void Start()
    {
        wordsPerLine = 5;
    }

    public void CallText(string author)
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        List<string> words;
        if (API.sentence != null)
            words = RegexTokenizer(API.sentence);
        else
            words = API.currentSentence.tokens.ToList();

        tokenPrefab = Resources.Load<GameObject>("ThinkToken");

        for (int i = 0; i < words.Count; i++)
        {
            if (i % wordsPerLine == 0)
            { 
                GameObject line = Instantiate(Resources.Load<GameObject>("Line"));
                lines.Add(line);
                line.transform.SetParent(player.cameraInterface.thinkCloud.transform);
                line.transform.localPosition = Vector3.zero;
                    //Shader glitches. Offset z by a small amount.
                line.transform.localPosition -= new Vector3(0, 0, 0.1f);
                line.transform.rotation = Camera.main.transform.rotation;
                line.transform.localScale = Vector3.one;
                lastLine = line;
            }
            GameObject tok = Instantiate(tokenPrefab);
            tokens.Add(tok);
            tok.transform.SetParent(lastLine.transform);
            tok.transform.localScale *= 1.0f;
            tok.transform.localPosition = Vector3.zero;
            tok.transform.rotation = Camera.main.transform.rotation;
            TextMeshPro tmpro = tok.GetComponent<TextMeshPro>();
                tmpro.text = $"{words[i]}";
            calltext = false;
            LayoutRebuilder.ForceRebuildLayoutImmediate(player.cameraInterface.thinkCloud.GetComponent<RectTransform>());
            tok.GetComponent<BoxCollider>().center = new Vector3(tok.GetComponent<RectTransform>().sizeDelta.x / 2, 0, 0);
            tok.GetComponent<BoxCollider>().size = new Vector3(tok.GetComponent<RectTransform>().sizeDelta.x, 1, 1);
            tok.GetComponent<Fluctuate>().StartFluctuate();
        }
        aut = author;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GameObject.Find("NameCloud").GetComponent<RectTransform>());
        player.cameraInterface.nameCloud.transform.GetChild(0).GetComponent<TextMeshPro>().text = aut;

        player.cameraInterface.thinkCloud.GetComponent<ContentSizeFitter>().enabled = false;
        player.cameraInterface.thinkCloud.GetComponent<ContentSizeFitter>().enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(player.cameraInterface.thinkCloud.GetComponent<RectTransform>());
    }

    public static List<string> RegexTokenizer(string message)
    {
        List<string> tokenizedString = Regex.Split(message, "([a-zA-ZÀ-ÿ0-9_*]+[^a-zA-ZÀ-ÿ0-9_*]+)").ToList();
        tokenizedString.RemoveAll(x => x == "");
        return tokenizedString;
    }

    public static List<string> Tokenizer(string message)
    {
        bool specialBegun = false;
        char[] seps = new char[] { ' ', '.', ',', '\'' };
        string constructingToken = "";
        List<string> tokenizedString = new List<string>();
        for (int i = 0; i < message.Length; i++)
        {
            if (i == message.Length-1)
            {
                constructingToken += message[i];
                tokenizedString.Add(constructingToken);
                return tokenizedString;
            }
            else
            {
                if (specialBegun)
                {
                    if (!seps.Contains(message[i]))
                    {
                        tokenizedString.Add(constructingToken);
                        constructingToken = "";
                        constructingToken += message[i];
                        specialBegun = false;
                    }
                    else
                    {
                        constructingToken += message[i];
                    }
                }
                else
                {
                    if (seps.Contains(message[i]))
                    {
                        specialBegun = true;
                        constructingToken += message[i];
                    }
                    else
                    {
                        constructingToken += message[i];
                    }
                }
            }
        }
        return tokenizedString;
    }

    /*
        public void Center(bool sub)
        {
            //Puts the pivot of the group of words in the center of the screen
            float begin = tokens[0].GetComponent<RectTransform>().anchoredPosition.x;
            float end = tokens[tokens.Count - 1].GetComponent<RectTransform>().anchoredPosition.x + tokens[tokens.Count - 1].GetComponent<RectTransform>().sizeDelta.x;
            float length = end - begin;
            // calcolare spazio rimanente
            float space = canvas.GetComponent<RectTransform>().sizeDelta.x - length;
            float margin = space / 2;
            // spsotare tutto a sinistra o destra in modo che i margini siano entrambi margine/2
            tokens[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(margin, -225);
            for (int i = 1; i < tokens.Count; i++)
            {
                tokens[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(tokens[i - 1].GetComponent<RectTransform>().anchoredPosition.x + tokens[i - 1].GetComponent<RectTransform>().sizeDelta.x + 2, -225);
                tokens[i].transform.localPosition = new Vector3(tokens[i].transform.localPosition.x, tokens[i].transform.localPosition.y, 0);
                Canvas.ForceUpdateCanvases();

            }
            GetComponent<Substitute>().update = false;

            //Draw line
            GetComponent<LineRenderer>().SetWidth(0.3F, 0.05F);
            // Set the number of vertex fo the Line Renderer
            GetComponent<LineRenderer>().SetVertexCount(2);
            GetComponent<LineRenderer>().SetPosition(0, tokens[0].transform.position);
            GameObject source = GameObject.Find(aut.gameObject.name);
            GetComponent<LineRenderer>().SetPosition(1, new Vector3(source.transform.position.x, source.transform.position.y + 0.5F, source.transform.position.z));
        }*/
}







/*
    public void Clean()
    {
        //clean
        path = Path.Combine(Application.dataPath, "corp.xml");
        XmlDocument tok = new XmlDocument();
        tok.Load(path);
        XmlNodeList list = tok.GetElementsByTagName("token");
        int n;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].InnerText.Contains("&#") ||
                list[i].InnerText == "-" ||
                list[i].InnerText == "/" ||
                list[i].InnerText == ":" ||
                list[i].InnerText == "," ||
                int.TryParse(list[i].InnerText, out n) && (list[i + 1].InnerText == "/" || list[i + 1].InnerText == "," || list[i + 1].InnerText == ":" || list[i - 1].InnerText == ":" || list[i - 1].InnerText.Contains("Bullo") || list[i - 1].InnerText.Contains("Vittima"))
                || list[i].InnerText.Any(Char.IsDigit) && sentence != list[i].Attributes["sentence"].Value
                )
            {
            }
            else
            {
                tokens.Add(list[i].InnerText);
            }
            sentence = list[i].Attributes["sentence"].Value;
        }
    }

    public void Detokenize()
    {
        //Creo XML dei turni
        XmlDocument xmlTurns = new XmlDocument();
        XmlNode root = xmlTurns.CreateElement("turns");
        xmlTurns.AppendChild(root);

        XmlNode turnNode;
        XmlAttribute number;
        XmlAttribute role;

        xmlTurns.Save("turns.xml");

        int i3 = 1;
        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens[i].Contains("Bullo") || tokens[i].Contains("Vittima"))
            {
                turnNode = xmlTurns.CreateElement("turn");
                number = xmlTurns.CreateAttribute("number");
                number.Value = (i3 - 1).ToString();
                turnNode.Attributes.Append(number);
                role = xmlTurns.CreateAttribute("role");
                role.Value = name;
                turnNode.Attributes.Append(role);
                turnNode.InnerText = battuta;
                root.AppendChild(turnNode);

                xmlTurns.Save("turns.xml");
                i3++;
                battuta = "";
                name = tokens[i];
            }
            else
            {
                battuta = battuta + " " + tokens[i];
            }
        }
        //Seleziono messaggio di prova
        messaggio = xmlTurns.SelectSingleNode("/turns/turn[@number='122']").InnerText;
        xmlTurns.RemoveChild(xmlTurns.SelectSingleNode("/turns/turn[@number='0']"));
        xmlTurns.Save("turns.xml");
    }
}
*/