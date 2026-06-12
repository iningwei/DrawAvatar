using UnityEngine;
using System.Collections;
using UnityEditor;

public class OpenPesistantPath : ScriptableObject
{
    [MenuItem("工具/OpenPersistantPathFolder")]
    static void OpenPersistantPath()
    {
        string path = Application.persistentDataPath;
        System.Diagnostics.Process.Start(path);
    }
}
