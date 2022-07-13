using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Linq;

public class QualityTools : MonoBehaviour
{
    public static Stopwatch sw;
    public static TimeSpan ts;
    public static void StartTimer()
    {
        sw = new Stopwatch();
        sw.Start();
    }

    public static void StopTimer()
    {
        ts = sw.Elapsed;
        sw.Stop();
    }

    public static string GetElapsed()
    {
        return ts.ToString();
    }

    public static float ThinkCloudQualityControl(AnnotationData anndata) 
    {
        List<int> totalMarked = new List<int>();
        totalMarked.AddRange(anndata.annotations.Where(x => x == 1));
        return QualityTools.AnnotationRate(totalMarked.Count, Control.Instance.byWord.tokens.Count);
    }

    public static float AnnotationRate(int marked, int total)
    {
        float percentage = marked / total * 0.00f;
        return percentage;
    }
}
