using System;
using System.Diagnostics;
using UnityEngine;

public static class DLog
{
    public static readonly Color COLOR_ERROR = new Color(1f, 0.5f, 0.2f);
    public static bool enabled
    {
        get
        {
            return false;
        }
    }

    [Conditional("ENABLE_LOG")]
    public static void Log(string str, string msg)
    {
    }

    [Conditional("ENABLE_LOG")]
    public static void Log(string str)
    {
        UnityEngine.Debug.Log(str);
    }


    public static void LogWarning(string str)
    {

    }

    public static void LogWarning(string category, string str)
    {
    }

    public static void LogError(string str)
    {

    }

    public static void LogError(string category, string str)
    {

    }

    public static void LogException(Exception exc)
    {
        UnityEngine.Debug.LogException(exc);
    }
}
