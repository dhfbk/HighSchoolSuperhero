using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using DataUtilities;
using System.Text;

public interface IExtractor
{
}
public class CsvExtractor : MonoBehaviour, IExtractor
{
    public static List<string> CsvLines { get; set; }
    public static List<string> Docs { get; set; }
    public static List<string> Tagsets { get; set; }
    public static List<string> AnnotatedDocs { get; set; }
    public static bool Loaded { get; set; }

    public static List<string> LoadCsvs()
    {
        List<string> texts = new List<string>();
        foreach (string file in Directory.GetFiles(LoadUtility.StreamingAssetsPath + "/Corpora/CSVCORPORA", "*.csv"))
        {
            if (!file.Contains("tag"))
            {
                texts.Add(File.ReadAllText(file));
            }
        }

        Control.Instance.StartCoroutine(LineExtractorOverFrames(texts[0]));
        return texts;
    }
    public static List<string> LoadCsvTags()
    {
        List<string> tagsets = new List<string>();
        foreach (string file in Directory.GetFiles(LoadUtility.StreamingAssetsPath + "/Corpora/CSVCORPORA", "*.txt"))
        {
            if (file.Contains("ctag")) //Xml tag files only
            {
                //textFiles.Add(file);
                tagsets.Add(File.ReadAllText(file));
            }
        }
        return tagsets;
    }

    public static IEnumerator LoadCsvsOverFrames()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        List<string> texts = new List<string>();
        List<string> annTexts = new List<string>();

        byte[] buffer = new byte[LoadUtility.BufferSize];
        string stringToLoad;
        string annFileName;
        foreach (string file in Directory.GetFiles(LoadUtility.StreamingAssetsPath + "/Corpora/CSVCORPORA", "*.csv"))
        {
            string filename;
            if (file.Contains('\\')) //Windows
                filename = file.Substring(file.LastIndexOf('\\') + 1).Split('.')[0];
            else //Mac
                filename = file.Substring(file.LastIndexOf('/') + 1).Split('.')[0];

            annFileName = LoadUtility.StreamingAssetsPath + "/Corpora/Annotated/CSV/" + filename + ".txt";
            if (!file.Contains("tag"))
            {
                using (MemoryStream csvStream = new MemoryStream())
                {
                    using (FileStream fileStream = File.OpenRead(file))
                    {
                        while (true)
                        {
                            int numBytesRead = fileStream.Read(buffer, 0, buffer.Length); //buffer content, offset, maximum buffer size
                            if (numBytesRead == 0)
                            {
                                break;
                            }
                            csvStream.Write(buffer, 0, numBytesRead);
                            //print("Elapsed:" + sw.Elapsed + " " + Encoding.UTF8.GetString(csvStream.ToArray()));
                            if (sw.ElapsedMilliseconds > LoadUtility.AllowedMilliseconds)
                            {
                                sw.Reset();
                                yield return null;
                                sw.Start();
                            }
                        }
                    }
                    yield return null;
                    stringToLoad = Encoding.UTF8.GetString(csvStream.ToArray());
                    texts.Add(stringToLoad);
                }
                if (File.Exists(annFileName))
                    annTexts.Add(File.ReadAllText(annFileName));
                else
                    annTexts.Add("empty");
            }
        }
        Docs = texts;
        AnnotatedDocs = annTexts;

    }

    public static List<string> LineExtractor(string text, int authorColumn = 1, int textColumn = 2)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new Exception("String is null or empty");
        }

        List<string> csvLines = text.Split('\n').ToList();
        List<string> lines = new List<string>();
        foreach (string line in csvLines)
        {
            string[] values = line.Split(';');
            if (values.Length > 2 && !String.IsNullOrEmpty(values[authorColumn]) && !String.IsNullOrEmpty(values[textColumn]))
            {
                string newline = values[authorColumn] + " " + values[textColumn];
                lines.Add(newline);
            }
        }
        return lines;
    }

    public static IEnumerator LineExtractorOverFrames(string text, int authorColumn = 1, int textColumn = 2)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new Exception("String is null or empty");
        }

        Stopwatch sw = new Stopwatch();
        sw.Start();
        List<string> csvLines = text.Split('\n').ToList();
        //Drop useless/empty values
        List<string> columns = csvLines[0].Split(';').ToList();
        for (int i = 0; i < columns.Count; i++)
        {
            if (string.IsNullOrEmpty(columns[i]))
            {
                columns.Remove(columns[i]);
            }
        }
        int numberOfColumns = columns.Count;

        //Extract name and text line
        List<string> lines = new List<string>();
        foreach (string line in csvLines)
        {
            List<string> values = line.Split(';').ToList();

            //Check every value of line and drop unused columns
            for (int i = 0; i < numberOfColumns; i++)
            {
                if (string.IsNullOrEmpty(values[i]))
                {
                    values.Remove(columns[i]);
                }
            }

            //Create dialogueable lines
            string newline = string.Join(" ", values[authorColumn], values[textColumn]);
            lines.Add(newline);
            if (sw.ElapsedMilliseconds > LoadUtility.AllowedMilliseconds)
            {
                sw.Reset();
                yield return null;
                sw.Start();
            }
        }

        CsvExtractor.CsvLines = lines;
        CsvExtractor.Loaded = true;
    }
}
