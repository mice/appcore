using Pool;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DataFactoryInspector : MonoBehaviour
{
    public bool Check;
}


#if UNITY_EDITOR
[CustomEditor(typeof(DataFactoryInspector), true)]
public class DataFactoryInspectorEditor : Editor
{
    /// <summary>
    /// 大于该阈值才显示在面板上
    /// </summary>
    public int showThreshold = 5;
    public override void OnInspectorGUI()
    {
        DataFactoryInspector component = base.target as DataFactoryInspector;
        component.Check = GUILayout.Toggle(component.Check, "Check");
        if (!component.Check)
        {
            return;
        }

        var dictField = typeof(DataFactory).GetField("m_ClassObjectDic", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        var dict = dictField.GetValue(null) as Dictionary<int, Queue<object>>;
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("C#类名");
        GUILayout.Label("池中数量", GUILayout.Width(50));
        GUILayout.Label("常驻数量", GUILayout.Width(50));
        GUILayout.EndHorizontal();

        if (component != null)
        {
            foreach (var item in dict.Values)
            {
                if (item.Count < showThreshold)
                    continue;
                GUILayout.BeginHorizontal("box");
                string class_name = item.Peek().GetType().Name;
                GUILayout.Label(class_name);
                GUILayout.Label(item.Count.ToString(), GUILayout.Width(50));
                //DataFactory.classObjectCount.TryGetValue(item.Peek().GetType().GetHashCode(), out byte resideCount);
                //GUILayout.Label(resideCount.ToString(), GUILayout.Width(50));
                GUILayout.EndHorizontal();
            }
        }

        serializedObject.ApplyModifiedProperties();
        //重绘，刷新数据
        Repaint();
    }
}
#endif