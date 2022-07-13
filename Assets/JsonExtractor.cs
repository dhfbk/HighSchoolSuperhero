using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Diagnostics;
using System.Text;

namespace DataUtilities
{
    public class TagList
    {
        public List<string> taglist = new List<string>();
    }
    public class JsonExtractor : MonoBehaviour
    {
        public List<string> fileLines;
        public static string manifest;
        public string loadedtext;

        public static bool Loaded { get; set; }
        public static List<string> JsonLines { get; set; }
        public static List<string> Docs { get; set; }
        public static List<string> Tagsets { get; set; }
        public static List<string> AnnotatedDocs { get; set; }
        public static List<string> LoadJsons()
        {
            List<string> texts = new List<string>();
            foreach (string file in Directory.GetFiles(LoadUtility.StreamingAssetsPath + "/Corpora/JSONCORPORA", "*.json"))
            {
                if (!file.Contains("tag"))
                {
                    texts.Add(File.ReadAllText(file));
                }
            }
            return texts;
        }

        public static IEnumerator LoadJsonsOverFrames()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<string> texts = new List<string>();
            List<string> annTexts = new List<string>();

            byte[] buffer = new byte[LoadUtility.BufferSize];
            string stringToLoad;
            string annFileName;
            WebDebug.Print("Started json extraction");
            foreach (string file in Directory.GetFiles(LoadUtility.StreamingAssetsPath + "/Corpora/JSONCORPORA", "*.json"))
            {
                string filename;
                if (file.Contains('\\')) //Windows
                    filename = file.Substring(file.LastIndexOf('\\') + 1).Split('.')[0];
                else //Mac
                    filename = file.Substring(file.LastIndexOf('/') + 1).Split('.')[0];

                annFileName = LoadUtility.StreamingAssetsPath + "/Corpora/Annotated/JSON/" +  filename + ".txt";
                using (MemoryStream jsonStream = new MemoryStream())
                {
                    using (FileStream fileStream = File.OpenRead(file))
                    {
                        while (true)
                        {
                            int numBytesRead = fileStream.Read(buffer, 0, buffer.Length);
                            if (numBytesRead == 0)
                            {
                                break;
                            }
                            jsonStream.Write(buffer, 0, numBytesRead);
                            //print("Elapsed:" + sw.Elapsed + " " + Encoding.UTF8.GetString(jsonStream.ToArray()));
                            if (sw.ElapsedMilliseconds > LoadUtility.AllowedMilliseconds)
                            {
                                sw.Reset();
                                yield return null;
                                sw.Start();
                            }
                        }
                    }
                    yield return null;
                    stringToLoad = Encoding.UTF8.GetString(jsonStream.ToArray());
                    texts.Add(stringToLoad);
                }
                if (File.Exists(annFileName))
                    annTexts.Add(File.ReadAllText(annFileName));
                else
                    annTexts.Add("empty");
            }
            Docs = texts;
            AnnotatedDocs = annTexts;
        }

        public static List<string> LoadJsonTags()
        {
            List<string> tagsets = new List<string>();
            foreach (string file in Directory.GetFiles(LoadUtility.StreamingAssetsPath + "/Corpora/JSONCORPORA", "*.txt"))
            {
                if (file.Contains("jtag"))
                {
                    tagsets.Add(File.ReadAllText(file));
                }
            }
            return tagsets;
        }

        public static List<string> LineExtractor(string text, string tagset)
        {
            //Clean line characters
            text = Regex.Replace(text, "\r\n|\r|\n", "");

            List<string> textSplit = Regex.Split(text, "},{").ToList();

            List<string> tagList = Regex.Split(tagset, ",").ToList();

            List<string> lines = new List<string>();
            for (int i = 0; i < tagList.Count; i++)
            {
                foreach (string entry in textSplit)
                {
                    if (entry.Contains(tagList[i]))
                    {
                        string newstr = Regex.Split(entry, tagList[i])[1];
                        newstr = newstr.Substring(3);
                        newstr = Regex.Split(newstr, "\",\"")[0];
                        newstr = tagList[i] + " " + newstr;
                        if (!lines.Contains(newstr))
                        {
                            lines.Add(newstr);
                        }
                    }
                }
            }
            return lines;
        }
        public static string ElementExtractor(string text, string tag)
        {
            int startIndex = text.IndexOf(tag)+tag.Length;
            string substr = text.Substring(startIndex);

            startIndex = substr.IndexOf("\"");
            substr = substr.Substring(startIndex+1);
            
            startIndex = substr.IndexOf("\"");
            substr = substr.Substring(startIndex+1);

            int endIndex = substr.IndexOf("\",");
            string content = substr.Substring(0, endIndex);
            return content;
        }

        public static IEnumerator LineExtractorOverFrames(string text, string tagset)
        {
            Stopwatch sw = new Stopwatch();
            //Clean line characters
            text = Regex.Replace(text, "\r|\n|\r\n", "");

            List<string> textSplit = Regex.Split(text, "},{").ToList();

            List<string> tagList = Regex.Split(tagset, ",").ToList();

            List<string> lines = new List<string>();
            for (int i = 0; i < tagList.Count; i++)
            {
                foreach (string entry in textSplit)
                {
                    if (entry.Contains(tagList[i]))
                    {
                        string newstr = Regex.Split(entry, tagList[i])[1];
                        newstr = newstr.Substring(3);
                        newstr = Regex.Split(newstr, "\",\"")[0];
                        newstr = tagList[i] + " " + newstr;
                        if (!lines.Contains(newstr))
                        {
                            lines.Add(newstr);
                        }
                    }
                    if (sw.ElapsedMilliseconds > LoadUtility.AllowedMilliseconds)
                    {
                        sw.Reset();
                        yield return null;
                        sw.Start();
                    }
                }
            }
            JsonLines = lines;
        }
    }
}