using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using DataUtilities;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

public class TxtExtractor : MonoBehaviour
{
    public static List<int> mean;
    public static List<string> Docs { get; set; }
    public static List<string> Tagsets { get; set; }
    public static List<string> AnnotatedDocs { get; set; }
    public static IEnumerator LoadTxtsOverFrames()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        List<string> texts = new List<string>();
        List<string> annTexts = new List<string>();

        byte[] buffer = new byte[LoadUtility.BufferSize];
        string stringToLoad;
        string annFileName;
        foreach (string file in Directory.GetFiles(LoadUtility.StreamingAssetsPath + "/Corpora/TXTCORPORA", "*.txt"))
        {
            string filename;
            if (file.Contains('\\')) //Windows
                filename = file.Substring(file.LastIndexOf('\\') + 1).Split('.')[0];
            else //Mac
                filename = file.Substring(file.LastIndexOf('/') + 1).Split('.')[0];

            annFileName = LoadUtility.StreamingAssetsPath + "/Corpora/Annotated/TXT/" + filename + ".txt";
            if (!file.Contains("tag"))
            {
                using (MemoryStream txtStream = new MemoryStream())
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
                            txtStream.Write(buffer, 0, numBytesRead);
                            //print("Elapsed:" + sw.Elapsed + " " + Encoding.UTF8.GetString(txtStream.ToArray()));
                            if (sw.ElapsedMilliseconds > LoadUtility.AllowedMilliseconds)
                            {
                                sw.Reset();
                                yield return null;
                                sw.Start();
                            }
                        }
                    }
                    yield return null;
                    stringToLoad = Encoding.UTF8.GetString(txtStream.ToArray());
                    //stringToLoad = Regex.Replace(stringToLoad, @"\p{C}+", string.Empty);
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

    public static List<string> LineExtractor(string text)
    {
        List<string> lines = new List<string>();
        lines = Regex.Split(text, "\r\n|\r|\n").ToList();
        foreach (string line in lines)
        {
            mean.Add(System.Text.UTF8Encoding.Unicode.GetByteCount(line));
        }
        return lines;
    }
}
