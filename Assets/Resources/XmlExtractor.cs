using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Diagnostics;
using System.Text;

namespace DataUtilities
{
    public class XmlExtractor : MonoBehaviour, IExtractor
    {
        public static bool Loaded { get; set; }
        public static List<XmlDocument> Docs { get; set; }
        public static List<string> Tagsets { get; set; }
        public static List<string> AnnotatedDocs { get; set; }
        public static string manifest;
        public static List<XmlDocument> LoadXmls()
        {
            List<XmlDocument> texts = new List<XmlDocument>();

            foreach (string file in Directory.GetFiles(LoadUtility.StreamingAssetsPath + "/Corpora/XMLCORPORA", "*.xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                if (!file.Contains("tag"))
                {
                    //textFiles.Add(file);
                    texts.Add(doc);
                }
            }
            return texts;
        }
        public static IEnumerator LoadXmlsOverFrames()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<XmlDocument> texts = new List<XmlDocument>();
            List<string> annTexts = new List<string>();

            byte[] buffer = new byte[LoadUtility.BufferSize];
            string stringToLoad;
            string annFileName;

            foreach (string file in Directory.GetFiles(LoadUtility.StreamingAssetsPath + "/Corpora/XMLCORPORA", "*.xml"))
            {
                string filename;
                if (file.Contains('\\')) //Windows
                    filename = file.Substring(file.LastIndexOf('\\') + 1).Split('.')[0];
                else //Mac
                    filename = file.Substring(file.LastIndexOf('/') + 1).Split('.')[0];

                annFileName = LoadUtility.StreamingAssetsPath + "/Corpora/Annotated/XML/" + filename + ".txt";
                using (MemoryStream xmlStream = new MemoryStream())
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
                            xmlStream.Write(buffer, 0, numBytesRead);
                            //print("Elapsed:" + sw.Elapsed + " " + Encoding.UTF8.GetString(xmlStream.ToArray()));
                            if (sw.ElapsedMilliseconds > LoadUtility.AllowedMilliseconds)
                            {
                                sw.Reset();
                                yield return null;
                                sw.Start();
                            }
                        }
                    }
                    yield return null;
                    stringToLoad = Encoding.UTF8.GetString(xmlStream.ToArray());
                    XmlDocument xmlToLoad = new XmlDocument();
                    xmlToLoad.LoadXml(stringToLoad);
                    texts.Add(xmlToLoad);
                }
                if (File.Exists(annFileName))
                    annTexts.Add(File.ReadAllText(annFileName));
                else
                    annTexts.Add("empty");
            }
            Docs = texts;
            AnnotatedDocs = annTexts;
        }
        public static List<string> LoadXmlTags()
        {
            List<string> tagsets = new List<string>();
            foreach (string file in Directory.GetFiles(LoadUtility.StreamingAssetsPath + "/Corpora/XMLCORPORA", "*.txt"))
            {
                if (file.Contains("xtag")) //Xml tag files only
                {
                    //textFiles.Add(file);
                    tagsets.Add(File.ReadAllText(file));
                }
            }
            return tagsets;
        }

        public static List<string> LineExtractor(XmlDocument doc, string tagset)
        {

            List<string> lines = new List<string>();

            string parentTagPointer = "Parent-";
            XmlNodeList nodeList;

            if (tagset.Contains(parentTagPointer)) //Use the tag innertext as participant name. Collection of lines inside a parent node, in the form of <participant>Name</>; <turnText>Text</>...
            {
                string parentTag = Regex.Split(tagset, ",").ToList()[0].Substring(parentTagPointer.Length);
                string nameTag = Regex.Split(tagset, ",").ToList()[1];
                string textTag = Regex.Split(tagset, ",").ToList()[2];

                nodeList = doc.GetElementsByTagName(parentTag);
                foreach (XmlNode node in nodeList)
                {
                    string line = node.SelectSingleNode(nameTag).InnerText + " " + node.SelectSingleNode(textTag).InnerText;
                    lines.Add(line);
                }
            }
            else //Use the tag name as participant name
            {
                List<string> tagList = Regex.Split(tagset, ",").ToList();
                for (int i = 0; i < tagList.Count; i++)
                {
                    nodeList = doc.GetElementsByTagName(tagList[i]); //raw collection of lines in the form of <participantName>Text</>

                    foreach (XmlNode node in nodeList)
                    {
                        string line = tagList[i] + " " + node.InnerText; //create "Tag [space] Text" line
                        lines.Add(line);
                    }
                }
            }
            return lines;
        }

        public static IEnumerator LineExtractorOverFrames(XmlDocument doc, string tagset)
        {
            Stopwatch sw = new Stopwatch();
            List<string> lines = new List<string>();

            string parentTagPointer = "Parent-";
            XmlNodeList nodeList;

            if (tagset.Contains(parentTagPointer)) //collection of lines inside a parent node, in the form of <participant>Name</>; <turnText>Text</>...
            {
                string parentTag = Regex.Split(tagset, ",").ToList()[0].Substring(parentTagPointer.Length);
                string nameTag = Regex.Split(tagset, ",").ToList()[1];
                string textTag = Regex.Split(tagset, ",").ToList()[2];

                nodeList = doc.GetElementsByTagName(parentTag);
                foreach (XmlNode node in nodeList)
                {
                    string line = node.SelectSingleNode(nameTag).InnerText + " " + node.SelectSingleNode(textTag).InnerText;
                    lines.Add(line);
                    if (sw.ElapsedMilliseconds > LoadUtility.AllowedMilliseconds)
                    {
                        sw.Reset();
                        yield return null;
                        sw.Start();
                    }
                }
            }
            else
            {
                List<string> tagList = Regex.Split(tagset, ",").ToList();
                for (int i = 0; i < tagList.Count; i++)
                {
                    nodeList = doc.GetElementsByTagName(tagList[i]); //raw collection of lines in the form of <participantName>Text</>

                    foreach (XmlNode node in nodeList)
                    {
                        string line = tagList[i] + " " + node.InnerText; //create "Tag [space] Text" line
                        lines.Add(line);
                        if (sw.ElapsedMilliseconds > LoadUtility.AllowedMilliseconds)
                        {
                            sw.Reset();
                            yield return null;
                            sw.Start();
                        }
                    }
                }
            }
        }
    }
}


