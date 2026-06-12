using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ZGame.RessEditor;


//buildin项目转换到urp后一些快捷处理
public class BuildInStandard2URPTool : Editor
{
    [MenuItem("工具/BuildInStandard2URPTool/BuildIn项目材质球shader由MyStandard转换为Standard")]
    [MenuItem("Assets/ZGame/BuildInStandard2URPTool/BuildIn项目材质球shader由MyStandard转换为Standard")]
    static void ReplaceMyStandard2Standard()
    {
        UnityEngine.Object[] assets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets) as UnityEngine.Object[];
        for (int i = 0; i < assets.Length; i++)
        {
            var path = AssetDatabase.GetAssetOrScenePath(assets[i]);
            if (path.EndsWith(".mat"))
            {
                var mat=assets[i] as Material;
                if (mat.shader.name=="My/Standard")
                {
                    mat.shader = Shader.Find("Standard");
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    Debug.Log("replace " + mat.name + "'s shader from My/Standard to Standard");
                }
                if (mat.shader.name== "My/Standard (Specular setup)")
                {
                    mat.shader= Shader.Find("Standard (Specular setup)");
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    Debug.Log("replace " + mat.name + "'s shader from My/Standard (Specular setup) to Standard (Specular setup)");
                }
            }
        }
    }
}
