using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class SetDefaultMaterial : MonoBehaviour
{
    [MenuItem("GameObject/Extentions/SetMaterial-With-Self-Default")]
    static void SetUGUIImgDefaultMaterial()
    {
        string defaultUIMatPath = "Assets/ZGame/Art/Shader/BuildIn/My-UI-Default.mat";
        string defaultSpriteMatPath = "Assets/ZGame/Art/Shader/BuildIn/My-Sprites-Default.mat";
        string defaultParticleSystemMatPath = "Assets/ZGame/Art/Shader/BuildIn/My-Particles-Standard-Unlit.mat";

        //////string defaultStandardMatPath = "Assets/ZGame/Art/Shader/BuildIn/My-Standard.mat";
        //////string defaultStandardSpecularMatPath = "Assets/ZGame/Art/Shader/BuildIn/My-StandardSpecular.mat";

        var selectObj = Selection.activeGameObject;
        var objs = selectObj.GetComponentsInChildren(typeof(Transform), true);
        for (int i = 0; i < objs.Length; i++)
        {
            var image = objs[i].GetComponent<Image>();
            if (image != null &&
                image.material.name == "Default UI Material")
            {
                image.material = AssetDatabase.LoadAssetAtPath(defaultUIMatPath, typeof(Material)) as Material;
                Debug.Log($"reset {image.transform.GetHierarchy()}'s Image material");
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            var text = objs[i].GetComponent<Text>();
            if (text != null && text.material.name == "Default UI Material")
            {
                text.material = AssetDatabase.LoadAssetAtPath(defaultUIMatPath, typeof(Material)) as Material;
                Debug.Log($"reset {image.transform.GetHierarchy()}'s Text material");

                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            var sr = objs[i].GetComponent<SpriteRenderer>();
            if (sr != null && sr.sharedMaterial.name == "Sprites-Default")
            {
                sr.sharedMaterial = AssetDatabase.LoadAssetAtPath(defaultSpriteMatPath, typeof(Material)) as Material;

                Debug.Log($"reset {image.transform.GetHierarchy()}'s SpriteRenderer material");
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }


            var pr = objs[i].GetComponent<ParticleSystemRenderer>();
            //TODO:当pr enable的时候，若pr.sharedMaterial若为null，则弹出报错信息
            //或者提供工具检测出这种情况
            if (pr != null && pr.sharedMaterial != null && pr.sharedMaterial.name == "Default-ParticleSystem")
            {
                pr.sharedMaterial = AssetDatabase.LoadAssetAtPath(defaultParticleSystemMatPath, typeof(Material)) as Material;
                Debug.Log($"reset {image.transform.GetHierarchy()}'s ParticleSystemRenderer material");
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }


            //see ShaderReplacement.cs
            ////////TODO：MeshRenderer和SkinedMeshRenderer 应该可以合并起来，通过父类Renderer来处理
            ////////MeshRenderer
            //////var mr = objs[i].GetComponent<MeshRenderer>();
            //////if (mr != null)
            //////{
            //////    var count = mr.sharedMaterials.Length;
            //////    if (count > 0)
            //////    {
            //////        for (int j = 0; j < count; j++)
            //////        {
            //////            var tmpMat = mr.sharedMaterials[j];
            //////            if (tmpMat != null)
            //////            {
            //////                if (tmpMat.shader.name == "Standard")
            //////                {
            //////                    tmpMat.shader = Shader.Find("My/Standard");
            //////                    Debug.Log($"reset {mr.transform.GetHierarchy()}'s MeshRenderer material's shader");

            //////                }
            //////                if (tmpMat.shader.name == "Standard (Specular setup)")
            //////                {
            //////                    tmpMat.shader = Shader.Find("My/Standard (Specular setup)");
            //////                    Debug.Log($"reset {mr.transform.GetHierarchy()}'s MeshRenderer material's shader");
            //////                }
            //////            }
            //////        }

            //////    }

            //////    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            //////}



            ////////SkinnedMeshRenderer
            //////var smr = objs[i].GetComponent<SkinnedMeshRenderer>();
            //////if (smr != null)
            //////{
            //////    var count = smr.sharedMaterials.Length;
            //////    if (count > 0)
            //////    {
            //////        for (int j = 0; j < count; j++)
            //////        {
            //////            var tmpMat = smr.sharedMaterials[j];
            //////            if (tmpMat != null)
            //////            {
            //////                if (tmpMat.shader.name == "Standard")
            //////                {
            //////                    tmpMat.shader = Shader.Find("My/Standard");
            //////                    Debug.Log($"reset {smr.transform.GetHierarchy()}'s MeshRenderer material's shader");

            //////                }
            //////                if (tmpMat.shader.name == "Standard (Specular setup)")
            //////                {
            //////                    tmpMat.shader = Shader.Find("My/Standard (Specular setup)");
            //////                    Debug.Log($"reset {smr.transform.GetHierarchy()}'s MeshRenderer material's shader");
            //////                }
            //////            }
            //////        }

            //////    }

            //////    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            //////}


        }
    }
}
