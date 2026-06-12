using System.Collections;
using System.Collections.Generic; 
using UnityEditor;
using UnityEngine;

public class ShaderReplacement : ScriptableWizard
{
    //////public string oldShaderName;
    //////public string newShaderName;

    //////[MenuItem("ZGame/Shader/Shader替换")]
    //////static void ShaderReplacementWizard()
    //////{
    //////    ScriptableWizard.DisplayWizard("Shader替换", typeof(ShaderReplacement), "Confirm", "Cancel");
    //////}

    //////private void OnWizardCreate()
    //////{
    //////    this.doReplace();
    //////    Close();
    //////}
    //////private void OnWizardOtherButton()
    //////{
    //////    Close();
    //////}

    //////void doReplace()
    //////{
    //////    var mats = Resources.FindObjectsOfTypeAll<Material>();
    //////    foreach (var mat in mats)
    //////    {
    //////        if (mat.shader.name == oldShaderName)
    //////        {
    //////            mat.shader = Shader.Find(newShaderName);
    //////        }
    //////    }
    //////}
    ///

    static Dictionary<string, string> shaderDic;

    static void InitShaderDic()
    {
        shaderDic = new Dictionary<string, string>();
        shaderDic.Add("Mobile/Diffuse", "My/Mobile/Diffuse");
        shaderDic.Add("Mobile/Bumped Diffuse", "My/Mobile/Bumped Diffuse");

        shaderDic.Add("Legacy Shaders/Diffuse", "My/Legacy Shaders/Diffuse");
        shaderDic.Add("Legacy Shaders/Bumped Diffuse", "My/Legacy Shaders/Bumped Diffuse");

        shaderDic.Add("Legacy Shaders/Particles/Additive", "My/Legacy Shaders/Particles/Additive");


        shaderDic.Add("Legacy Shaders/Transparent/Diffuse", "My/Legacy Shaders/Transparent/Diffuse");
        shaderDic.Add("Legacy Shaders/Transparent/Bumped Diffuse", "My/Legacy Shaders/Transparent/Bumped Diffuse");
        shaderDic.Add("Legacy Shaders/Particles/Alpha Blended", "My/Legacy Shaders/Particles/Alpha Blended");
        shaderDic.Add("Legacy Shaders/Particles/Alpha Blended Premultiply", "My/Legacy Shaders/Particles/Alpha Blended Premultiply");
        shaderDic.Add("Particles/Standard Unlit", "My/Particles/Standard Unlit");

        shaderDic.Add("Particles/Standard Surface", "My/Particles/Standard Surface");

        shaderDic.Add("Standard", "My/Standard");
        shaderDic.Add("Standard (Specular setup)", "My/Standard (Specular setup)");



        //---------------->URP
        shaderDic.Add("Universal Render Pipeline/Lit", "My/Universal Render Pipeline/Lit");
        shaderDic.Add("Universal Render Pipeline/Particles/Unlit", "My/Universal Render Pipeline/Particles/Unlit");
        shaderDic.Add("Universal Render Pipeline/Simple Lit", "My/Universal Render Pipeline/Simple Lit");

    }


    [MenuItem("工具/Shader/一键替换所有材质球的Shader")]
    static void BuildInShaderReplace()
    {
        InitShaderDic();


        //var mats = Resources.FindObjectsOfTypeAll<Material>();//can not get all typed targets//See this:https://forum.unity.com/threads/resources-findobjectsoftypeall-doesnt-find-everything.101054/
        var matGuids = AssetDatabase.FindAssets("t:Material");

        foreach (var g in matGuids)
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(g));

            if (shaderDic.ContainsKey(mat.shader.name))
            {
                Debug.Log($"替换shader, mat:{mat.name}, oldShader:{mat.shader.name},newShader:{shaderDic[mat.shader.name]}");
                mat.shader = Shader.Find(shaderDic[mat.shader.name]);
            }
        }
        Debug.LogError("替换完成！");
    }

    [MenuItem("工具/Shader/一键替换目标物体和其子物体的Shader")]
    static void ReplaceTargetRootShader()
    {
        InitShaderDic();
        //GameObject root = Selection.activeObject as GameObject;
        var objects = Selection.objects;
        for (int i = 0; i < objects.Length; i++)
        {
            var obj = objects[i] as GameObject;
            if (obj != null)
            {
                replaceTargetShader(obj);
            }
        }
    }

    static void replaceTargetShader(GameObject root)
    {
        if (root != null)
        {
            Transform[] all = root.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < all.Length; i++)
            {
                Transform tmp = all[i];
                Renderer render = tmp.GetComponent<Renderer>();
                if (render != null)
                {
                    Material[] mats = render.sharedMaterials;
                    if (mats != null && mats.Length > 0)
                    {
                        foreach (var mat in mats)
                        {
                            if (mat == null)
                            {
                                Debug.LogWarning("mat   is null: " + render.transform.GetHierarchy());
                                continue;
                            }
                            if (mat.shader != null)
                            {
                                if (shaderDic.ContainsKey(mat.shader.name))
                                {
                                    Debug.Log($"replace shader, mat:{mat.name}, oldShader:{mat.shader.name},newShader:{shaderDic[mat.shader.name]},tran:{render.transform.GetHierarchy()}");
                                    mat.shader = Shader.Find(shaderDic[mat.shader.name]);

                                }
                            }
                            else
                            {
                                Debug.LogError("mat shader is null: " + render.transform.GetHierarchy());
                            }
                        }
                    }
                }
            }
        }

        Debug.LogError(root.name + " 替换完成！");
    }
}
