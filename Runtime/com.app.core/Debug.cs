using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Reflection;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Diagnostics;

namespace Core
{
    public static class Debug
    {
        public static bool autoClear = true;
        private static readonly MethodInfo clearConsole;

        static Debug()
        {
            if (!Application.isEditor || !autoClear) return;
            clearConsole = Type.GetType("UnityEditor.LogEntries,UnityEditor.dll").GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);            
        }
        public static void OutputLogTo(string path, string log)
        {
            File.WriteAllText(path, log);
        }
        public static void ClearLog()
        {
            if (Application.isEditor) clearConsole.Invoke(null, null);
        }
        [Conditional("ENABLE_LOG")]
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(ChangeMessageStyle(message,Color.white));
        }
        [Conditional("ENABLE_LOG")]
        public static void ServicesData(object message)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Length = 0;
            foreach (PropertyInfo p in message.GetType().GetProperties())
            {
                stringBuilder.Append(p.Name);
                stringBuilder.Append("=");
                stringBuilder.Append(p.GetValue(message));
                stringBuilder.Append(",");
            }
            UnityEngine.Debug.Log(stringBuilder.ToString());

        }
        [Conditional("ENABLE_LOG")]
        public static void Log(object message, Color color)
        {
            UnityEngine.Debug.Log(ChangeMessageStyle(message, color));
        }
        [Conditional("ENABLE_LOG")]
        public static void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(ChangeMessageStyle(message, Color.white));
        }

        [Conditional("ENABLE_LOG")]
        public static void LogWarning(object message, Color color)
        {
            UnityEngine.Debug.LogWarning(ChangeMessageStyle(message, color));
        }
        [Conditional("ENABLE_LOG")]
        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError(ChangeMessageStyle(message, Color.white));
        }
        [Conditional("ENABLE_LOG")]
        public static void LogException(Exception ex)
        {
            UnityEngine.Debug.LogException(ex);
        }
        /// <summary>
        /// 不会被屏蔽的日志输出
        /// </summary>
        /// <param name="message"></param>
        public static void LogFail(object message)
        {
            UnityEngine.Debug.LogError(ChangeMessageStyle(message, Color.white));
        }
        [Conditional("ENABLE_LOG")]
        public static void LogError(object message, Color color)
        {
            UnityEngine.Debug.LogError(ChangeMessageStyle(message, color));
        }
        [Conditional("ENABLE_LOG")]
        public static void Log<T>(IList<T> enumerable)
        {
            Log(enumerable, element => element);
        }
        [Conditional("ENABLE_LOG")]
        public static void LogWarningEnumerable<T>(IList<T> enumerable)
        {
            LogWarningEnumerable(enumerable, element => element);
        }
        [Conditional("ENABLE_LOG")]
        public static void LogErrorEnumerable<T>(IList<T> enumerable)
        {
            LogErrorEnumerable(enumerable, element => element);
        }
        [Conditional("ENABLE_LOG")]
        public static void Log<T>(IList<T> enumerable, Func<T, object> logFunc)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Length = 0;
            foreach (var element in enumerable)
            {
                stringBuilder.Append(logFunc(element));
                stringBuilder.Append(" , ");
            }
            if (stringBuilder.Length == 0) return;
            stringBuilder.Length -= 3;
            UnityEngine.Debug.Log(stringBuilder.ToString());
        }
        [Conditional("ENABLE_LOG")]
        public static void LogWarningEnumerable<T>(IList<T> enumerable, Func<T, object> logFunc)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Length = 0;
            foreach (var element in enumerable)
            {
                stringBuilder.Append(logFunc(element));
                stringBuilder.Append(" , ");
            }
            if (stringBuilder.Length == 0) return;
            stringBuilder.Length -= 3;
            UnityEngine.Debug.LogWarning(stringBuilder.ToString());
        }
        [Conditional("ENABLE_LOG")]
        public static void LogErrorEnumerable<T>(IList<T> enumerable, Func<T, object> logFunc)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Length = 0;
            foreach (var element in enumerable)
            {
                stringBuilder.Append(logFunc(element));
                stringBuilder.Append(" , ");
            }
            if (stringBuilder.Length == 0) return;
            stringBuilder.Length -= 3;
            UnityEngine.Debug.LogError(stringBuilder.ToString());
        }
        [Conditional("ENABLE_LOG")]
        public static void IsNull<T>(T obj, string message = null) where T : class
        {
            if (!Application.isEditor) return;
            if (message == null) message = "断言失败,该对象不为null!";
            Assert.IsNull(obj, message);
        }
        [Conditional("ENABLE_LOG")]
        public static void IsNotNull<T>(T obj, string message = null) where T : class
        {
            if (!Application.isEditor) return;
            if (message == null) message = "断言失败,该对象为null!";
            Assert.IsNotNull(obj, message);
        }
        [Conditional("ENABLE_LOG")]
        public static void IsTrue(bool value, string message = null)
        {
            if (!Application.isEditor) return;
            if (message == null) message = "断言失败,该值为false!";
            Assert.IsTrue(value, message);
        }
        [Conditional("ENABLE_LOG")]
        public static void IsFalse(bool value, string message = null)
        {
            if (!Application.isEditor) return;
            if (message == null) message = "断言失败,该值为true!";
            Assert.IsFalse(value, message);
        }
        [Conditional("ENABLE_LOG")]
        public static void AreEqual<T>(T objA, T objB, string message = null)
        {
            if (!Application.isEditor) return;
            if (message == null) message = $"断言失败,对象 {objA} 和对象 {objB} 不相等!";
            Assert.AreEqual(objA, objB, message);
        }
        [Conditional("ENABLE_LOG")]
        public static void AreNotEqual<T>(T objA, T objB, string message = null)
        {
            if (!Application.isEditor) return;
            if (message == null) message = $"断言失败,对象 {objA} 和对象 {objB} 相等!";
            Assert.AreNotEqual(objA, objB, message);
        }
        [Conditional("ENABLE_LOG")]
        public static void AreApproximatelyEqual(float actual, float expected, string message = null)
        {
            if (!Application.isEditor) return;
            if (message == null) message = $"断言失败,实际值为 {actual} ,期望值为 {expected} ,两者不近似!";
            Assert.AreApproximatelyEqual(expected, actual, message);
        }
        [Conditional("ENABLE_LOG")]
        public static void AreNotApproximatelyEqual(float actual, float expected, string message = null)
        {
            if (!Application.isEditor) return;
            if (message == null) message = $"断言失败,实际值为 {actual} ,期望值为 {expected} ,两者近似!";
            Assert.AreNotApproximatelyEqual(expected, actual, message);
        }

        static StringBuilder stringBuilder = new StringBuilder();
        private static string ChangeMessageStyle(object message, Color color)
        {
            lock (stringBuilder)
            {
                stringBuilder.Length = 0;
                if (!Application.isEditor)
                {
                    stringBuilder.Append('[');
                    stringBuilder.Append(DateTime.Now.ToString("hh:mm:ss.fff"));
                    stringBuilder.Append(']');
                }
                else
                {
                    stringBuilder.Append('[');
                    stringBuilder.Append(DateTime.Now.Millisecond);
                    stringBuilder.Append(']');
                }
                if (Application.isEditor)
                {
                    stringBuilder.Append("<color=#");
                    stringBuilder.Append(ColorUtility.ToHtmlStringRGBA(color));
                    stringBuilder.Append('>');
                }

                stringBuilder.Append(message);

                if (Application.isEditor)
                    stringBuilder.Append("</color>");

                return stringBuilder.ToString();
            }
        }
    }
}