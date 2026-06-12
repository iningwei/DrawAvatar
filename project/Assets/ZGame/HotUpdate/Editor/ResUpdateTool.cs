using UnityEditor;
using UnityEngine;
using ZGame;

public class ResUpdateTool : EditorWindow
{
    static BuildTargetGroup buildTargetGroup;
    static string macros;
    [MenuItem("HotUpdate/游戏热更/整理热更代码和资源")]
    public static void GenUpdateResList()
    {
        ResUpdateTool resTool = EditorWindow.GetWindow(typeof(ResUpdateTool)) as ResUpdateTool;
        buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        macros = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("version:" + ConfigUtility.Data.AppVersion);
        EditorGUILayout.LabelField("resVersion:" + ConfigUtility.Data.ResVersion);
        EditorGUILayout.LabelField("channelId:" + ConfigUtility.Data.GameChannelId);
        GUILayout.Space(10);
        EditorGUILayout.LabelField("macros:" + macros);
        GUILayout.Space(10);


        if (GUILayout.Button("打热更代码并整理热更资源"))
        {
            new HotResCollector().Build();
            this.Close();
        }
        if (GUILayout.Button("整理热更资源"))
        {
            new HotResCollector().ProcessHotUpdateRes();
            this.Close();
        }
        if (GUILayout.Button("Cancel"))
        {
            this.Close();
        }
    }
}
