using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro; 
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using ZGame.RessEditor;
using Path = System.IO.Path;

public class ArtFastTool
{
    [MenuItem("GameObject/ZGame/美术工具/替换自己和所有子物体的Unity默认材质球为基础材质球")]
    static void SetDefaultMaterial()
    {
        bool isURP = false;
        if (QualitySettings.renderPipeline != null && QualitySettings.renderPipeline.ToString().Contains("URP"))
        {
            isURP = true;
        }
        string defaultUIMatPath = "Assets/ZGame/Art/Shader/UGUI/My-UI-Default.mat";

        string defaultSpriteMatPath = "Assets/ZGame/Art/Shader/Sprite/My-Sprites-Default.mat";

        string defaultParticleSystemMatPath = isURP ? "Assets/ZGame/Art/Shader/URP/My-ParticlesUnlit.mat" : "Assets/ZGame/Art/Shader/BuildInRP/BuildIn/My-Particles-Standard-Unlit.mat";

        //string defaultStandardMatPath = "Assets/ZGame/Art/Shader/BuildIn/My-Standard.mat";
        //string defaultStandardSpecularMatPath = "Assets/ZGame/Art/Shader/BuildIn/My-StandardSpecular.mat";

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
                EditorUtility.SetDirty(selectObj);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            var rawImage = objs[i].GetComponent<RawImage>();
            if (rawImage != null && rawImage.material.name == "Default UI Material")
            {
                rawImage.material = AssetDatabase.LoadAssetAtPath(defaultUIMatPath, typeof(Material)) as Material;
                Debug.Log($"reset {rawImage.transform.GetHierarchy()}'s RawImage material");
                EditorUtility.SetDirty(selectObj);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            var text = objs[i].GetComponent<Text>();
            if (text != null && text.material.name == "Default UI Material")
            {
                text.material = AssetDatabase.LoadAssetAtPath(defaultUIMatPath, typeof(Material)) as Material;
                Debug.Log($"reset {text.transform.GetHierarchy()}'s Text material");
                EditorUtility.SetDirty(selectObj);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            var sr = objs[i].GetComponent<SpriteRenderer>();
            if (sr != null && sr.sharedMaterial.name == "Sprites-Default")
            {
                sr.sharedMaterial = AssetDatabase.LoadAssetAtPath(defaultSpriteMatPath, typeof(Material)) as Material;

                Debug.Log($"reset {sr.transform.GetHierarchy()}'s SpriteRenderer material");

                EditorUtility.SetDirty(selectObj);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }


            var pr = objs[i].GetComponent<ParticleSystemRenderer>();
            if (pr != null && (pr.sharedMaterial != null && pr.sharedMaterial.name == "Default-ParticleSystem"))
            {
                pr.sharedMaterial = AssetDatabase.LoadAssetAtPath(defaultParticleSystemMatPath, typeof(Material)) as Material;
                Debug.Log($"reset {pr.transform.GetHierarchy()}'s ParticleSystemRenderer material");
                //EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                EditorUtility.SetDirty(selectObj);
                AssetDatabase.SaveAssets();
            }
            if (pr != null && (pr.sharedMaterial == null))
            {
                pr.sharedMaterial = AssetDatabase.LoadAssetAtPath(defaultParticleSystemMatPath, typeof(Material)) as Material;
                Debug.Log($"  {pr.transform.GetHierarchy()}'s ParticleSystemRenderer material is null ,set it to default");
                //EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                EditorUtility.SetDirty(selectObj);
                AssetDatabase.SaveAssets();
            }


            //see ShaderReplacement.cs
            ////////TODO：MeshRenderer和SkinedMeshRenderer 应该可以合并起来，通过父类Renderer来处理
            ////////MeshRenderer
            var mr = objs[i].GetComponent<MeshRenderer>();
            if (mr != null)
            {
                var count = mr.sharedMaterials.Length;
                if (count > 0)
                {
                    for (int j = 0; j < count; j++)
                    {
                        var tmpMat = mr.sharedMaterials[j];
                        if (tmpMat != null)
                        {
                            if (tmpMat.shader.name == "Standard")
                            {
                                tmpMat.shader = Shader.Find("My/Standard");
                                Debug.Log($"reset {mr.transform.GetHierarchy()}'s MeshRenderer material's shader");
                                EditorUtility.SetDirty(selectObj);
                            }
                            if (tmpMat.shader.name == "Standard (Specular setup)")
                            {
                                tmpMat.shader = Shader.Find("My/Standard (Specular setup)");
                                Debug.Log($"reset {mr.transform.GetHierarchy()}'s MeshRenderer material's shader");
                                EditorUtility.SetDirty(selectObj);
                            }
                        }
                    }

                }

                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }



            ////////SkinnedMeshRenderer
            var smr = objs[i].GetComponent<SkinnedMeshRenderer>();
            if (smr != null)
            {
                var count = smr.sharedMaterials.Length;
                if (count > 0)
                {
                    for (int j = 0; j < count; j++)
                    {
                        var tmpMat = smr.sharedMaterials[j];
                        if (tmpMat != null)
                        {
                            if (tmpMat.shader.name == "Standard")
                            {
                                tmpMat.shader = Shader.Find("My/Standard");
                                Debug.Log($"reset {smr.transform.GetHierarchy()}'s MeshRenderer material's shader");

                                EditorUtility.SetDirty(selectObj);

                            }
                            if (tmpMat.shader.name == "Standard (Specular setup)")
                            {
                                tmpMat.shader = Shader.Find("My/Standard (Specular setup)");
                                Debug.Log($"reset {smr.transform.GetHierarchy()}'s MeshRenderer material's shader");

                                EditorUtility.SetDirty(selectObj);
                            }
                        }
                    }

                }

                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }


        }
    }

    [MenuItem("GameObject/ZGame/美术工具/设置自己和所有子物体使用默认图片(如果为空)")]
    static void SetUIDefaultTex()
    {
        string defaultSpritePath = "Assets/ZGame/Art/Texture/DefaultSprite.png";
        string defaultTexturePath = "Assets/ZGame/Art/Texture/DefaultTexture.png";

        var selectObj = Selection.activeGameObject;
        var objs = selectObj.GetComponentsInChildren(typeof(Transform), true);
        for (int i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];
            //Image
            var img = obj.GetComponent<Image>();
            if (img != null && (img.sprite == null || (BuildConfig.buildInImageSpriteNames.Contains(img.sprite.name))))
            {
                var s = AssetDatabase.LoadAssetAtPath(defaultSpritePath, typeof(Sprite)) as UnityEngine.Sprite;
                if (s == null)
                {
                    Debug.LogError("error, s is null");
                }

                img.sprite = s;
                Debug.Log($"set Image with default sprite,path:{img.transform.GetHierarchy()}");
                EditorUtility.SetDirty(img);
            }

            //rawImage
            var rawImg = obj.GetComponent<RawImage>();
            if (rawImg != null && (rawImg.texture == null || BuildConfig.buildInImageSpriteNames.Contains(rawImg.texture.name) || rawImg.texture.name == "DefaultSprite"))
            {
                var tex = AssetDatabase.LoadAssetAtPath(defaultTexturePath, typeof(Texture2D)) as UnityEngine.Texture2D;
                if (tex == null)
                {
                    Debug.LogError("error load tex:" + defaultTexturePath);
                }
                else
                {
                    rawImg.texture = tex;
                    Debug.Log("set RawImage with default texture,path:" + rawImg.transform.GetHierarchy());
                    EditorUtility.SetDirty(rawImg);
                }
            }
        }
    }


    [MenuItem("GameObject/ZGame/美术工具/设置自己和所有子物体的TextMeshPro字体为默认字体或其它在用字体")]
    static void SetTextMeshProWithDefaultFontAsset()
    {
        string defaultFontAssetPath = BuildConfig.defaultTMPTextFontPath;
        var selectObj = Selection.activeGameObject;
        var objs = selectObj.GetComponentsInChildren(typeof(Transform), true);
        for (int i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];
            //TextMeshPro UGUI
            var tm = obj.GetComponent<TextMeshProUGUI>();

            if (tm != null && (tm.font == null || BuildConfig.fontPathList.Exists(a => a.Contains(tm.font.name)) == false))
            {
                TMP_FontAsset s = AssetDatabase.LoadAssetAtPath(defaultFontAssetPath, typeof(TMP_FontAsset)) as TMP_FontAsset;
                if (s == null)
                {
                    Debug.LogError("error, s is null");
                }

                tm.font = s;
                Debug.Log($"set TextMeshProUGUI with default Font,path:{tm.transform.GetHierarchy()}");
                EditorUtility.SetDirty(tm);
            }


            //TextMeshPro InputField
            var tm_input = obj.GetComponent<TMP_InputField>();
            if (tm_input != null)
            {
                if ((tm_input.fontAsset == null || BuildConfig.fontPathList.Exists(a => a.Contains(tm_input.fontAsset.name)) == false))
                {
                    TMP_FontAsset s = AssetDatabase.LoadAssetAtPath(defaultFontAssetPath, typeof(TMP_FontAsset)) as TMP_FontAsset;
                    if (s == null)
                    {
                        Debug.LogError("error, s is null");
                    }

                    tm_input.fontAsset = s;
                    Debug.Log($"set TMP_InputField with default Font,path:{tm_input.transform.GetHierarchy()}");
                    EditorUtility.SetDirty(tm_input);
                }
            }

        }
    }

    [MenuItem("GameObject/ZGame/美术工具/一键更新字体组件")]
    static void UpdateTextComponent()
    {
        var selectObj = Selection.activeGameObject;
        var objs = selectObj.GetComponentsInChildren(typeof(Transform), true);
        for (int i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];
            var txtCom = obj.GetComponent<Text>();
            if (txtCom != null)
            {

                string text = txtCom.text;
                int size = txtCom.fontSize;
                float lineSpace = txtCom.lineSpacing;
                Color c = txtCom.color;
                FontStyle fs = txtCom.fontStyle;
                HorizontalWrapMode hMode = txtCom.horizontalOverflow;
                VerticalWrapMode vMode = txtCom.verticalOverflow;
                TextAnchor ta = txtCom.alignment;
                bool fit = txtCom.resizeTextForBestFit;
                int min = txtCom.resizeTextMinSize;
                int max = txtCom.resizeTextMaxSize;

                Object.DestroyImmediate(txtCom);
                Object.DestroyImmediate(obj.GetComponent<CanvasRenderer>());
                TextMeshProUGUI tmp = obj.gameObject.AddComponent<TextMeshProUGUI>();
                tmp.text = text;
                tmp.fontSize = size;
                tmp.lineSpacing = lineSpace;
                tmp.color = c;
                tmp.fontStyle = fs == FontStyle.BoldAndItalic ? TMPro.FontStyles.Bold : (TMPro.FontStyles)fs;
                tmp.enableWordWrapping = hMode == HorizontalWrapMode.Wrap ? true : false;
                //tmp.overflowMode = vMode == VerticalWrapMode.Overflow ? TextOverflowModes.Overflow : TextOverflowModes.Truncate;
                if (vMode == VerticalWrapMode.Truncate)
                {
                    tmp.overflowMode = hMode == HorizontalWrapMode.Wrap ? TextOverflowModes.Truncate : TextOverflowModes.Overflow;
                }
                else
                {
                    tmp.overflowMode = TextOverflowModes.Overflow;
                }
                List<TextAlignmentOptions> lst = new List<TextAlignmentOptions>()
                {
                    TextAlignmentOptions.TopLeft, TextAlignmentOptions.Top, TextAlignmentOptions.TopRight,
                    TextAlignmentOptions.MidlineLeft, TextAlignmentOptions.Center, TextAlignmentOptions.MidlineRight,
                    TextAlignmentOptions.BottomLeft, TextAlignmentOptions.Bottom, TextAlignmentOptions.BaselineRight
                };
                tmp.alignment = lst[(int)ta];
                tmp.enableAutoSizing = fit;
                if (fit)
                {
                    tmp.fontSizeMin = min;
                    tmp.fontSizeMax = max;
                }
                Debug.Log($"update Text Component To TextMeshProUGUI ,path:{tmp.transform.GetHierarchy()}");
            }
        }
        SetTextMeshProWithDefaultFontAsset();
    }


    //TODO:改变字体后，预制件并没有变化，无法apply，需要手动改个参数才能apply.
    [MenuItem("GameObject/ZGame/美术工具/替换Legacy字体为项目默认字体")]
    static void SetLegacyTextWithDefaultFont()
    {
        string defaultTextFontPath = BuildConfig.defaultTextFontPath;

        var selectObj = Selection.activeGameObject;
        var objs = selectObj.GetComponentsInChildren(typeof(Transform), true);
        for (int i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];
            var txtCom = obj.GetComponent<Text>();
            if (txtCom != null)
            {
                if (txtCom.font == null || txtCom.font.name == "Arial" || txtCom.font.name != BuildConfig.defaultTextFontName)
                {
                    txtCom.font = AssetDatabase.LoadAssetAtPath(defaultTextFontPath, typeof(Font)) as Font;
                    Debug.Log("set " + txtCom.transform.GetHierarchy() + " with default font");
                    EditorUtility.SetDirty(selectObj);
                    //PrefabUtility.RecordPrefabInstancePropertyModifications(selectObj);

                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                    AssetDatabase.SaveAssets();
                }
            }
        }

    }

    [MenuItem("Assets/ZGame/拼界面-一键替换散图为图集资源")]
    [MenuItem("GameObject/拼界面-一键替换散图为图集资源")]
    static void ReplaceSeperateUIWithSprite()
    {
        //获得ArtResources/Sprite目录下的所有图集信息
        Dictionary<string, List<Sprite>> spriteDic = new Dictionary<string, List<Sprite>>();

        string folder = Application.dataPath + "/ArtResources/Sprite";

        DirectoryInfo dInfo = new DirectoryInfo(folder);
        // 获取 文件夹以及子文件夹中所有文件
        FileInfo[] fileInfoArr = dInfo.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < fileInfoArr.Length; ++i)
        {
            string fullName = fileInfoArr[i].FullName;
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fullName);//文件名不带后缀 
            string assetPath = fullName.Substring(fullName.IndexOf("Assets"));
            if (assetPath.EndsWith(".meta"))
            {
                continue;
            }

            var sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath);//获得图集下的所有精灵
            List<Sprite> spriteList = new List<Sprite>();
            for (int j = 0; j < sprites.Length; j++)//图集自己的图片(Texture2D类型)也位于sprites中，需要过滤掉
            {
                if (sprites[j] is UnityEngine.Texture2D)
                {
                    Debug.Log($"index {j}, {sprites[j].name} is texture2D ,continue");
                    continue;
                }
                spriteList.Add(sprites[j] as UnityEngine.Sprite);
            }
            spriteDic.Add(fileNameWithoutExt, spriteList);
        }


        var selectObj = Selection.activeGameObject;
        var imageTrans = selectObj.GetComponentsInChildren<Image>(true);
        if (imageTrans != null && imageTrans.Length > 0)
        {
            for (int i = 0; i < imageTrans.Length; i++)
            {
                var sprite = imageTrans[i].sprite;
                if (sprite != null)
                {
                    string path = AssetDatabase.GetAssetPath(sprite);
                    if (path.Contains("Assets/UI"))
                    {
                        var s = getSpriteByChildSpriteName(sprite.name, spriteDic);
                        imageTrans[i].sprite = s;
                        Debug.Log("set sprite from atlas, " + imageTrans[i].transform.GetHierarchy());

                        EditorUtility.SetDirty(imageTrans[i].transform);
                    }
                }
            }
        }


    }

    /// <summary>
    /// 找到第一个包含目标精灵名的图集
    /// </summary>
    /// <param name="childSpriteName"></param>
    /// <returns></returns>
    static Sprite getSpriteByChildSpriteName(string childSpriteName, Dictionary<string, List<Sprite>> targetAtlasDic)
    {
        foreach (var item in targetAtlasDic)
        {
            var allSprites = item.Value;

            for (int i = 0; i < allSprites.Count; i++)
            {
                if (allSprites[i].name == childSpriteName)
                {
                    return allSprites[i];
                }
            }
        }

        Debug.LogError("no atlas has sprite:" + childSpriteName);
        return null;
    }

    [MenuItem("GameObject/ZGame/美术工具/拼图一键快乐")]
    static void OneKeyHandleArt()
    {
        SetDefaultMaterial();
        SetUIDefaultTex();
        if (BuildConfig.textType == TextType.TMP)
        {
            SetTextMeshProWithDefaultFontAsset();
        }
        else if (BuildConfig.textType == TextType.Legacy)
        {

            SetLegacyTextWithDefaultFont();
        }
        else
        {
            Debug.LogError("TODO:");
        }
    }
}
