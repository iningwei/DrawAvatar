using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RuntimeDistanceCalculateExtention : EditorWindow
{
    GameObject obj1;
    GameObject obj2;

    [MenuItem("工具/距离测算", false, 14)]
    static void DistanceCalculate()
    {
        RuntimeDistanceCalculateExtention visualizeTool = EditorWindow.GetWindow(typeof(RuntimeDistanceCalculateExtention)) as RuntimeDistanceCalculateExtention;
    }

    float dis = 0f;
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("物体 1:");
        obj1 = EditorGUILayout.ObjectField(obj1, typeof(GameObject), true) as GameObject;

        GUILayout.Label("物体 2:");
        obj2 = EditorGUILayout.ObjectField(obj2, typeof(GameObject), true) as GameObject;
        GUILayout.EndHorizontal();

        if (GUILayout.Button("计算"))
        {
            if (obj1 != null && obj2 != null)
            {
                dis = Vector3.Distance(obj1.transform.position, obj2.transform.position);
            }
        }

        GUILayout.Label("距离：" + dis);

    }

}
