using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq; 
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class TextureCheckTool
{


    [MenuItem("工具/检测/图片/mipmap")]
    public static void CheckMipMap()
    {
        string[] guidsTexture = AssetDatabase.FindAssets("t:Texture", new[] { "Assets" });
        string[] guidsSprite = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets" });
        List<string> guidList = new List<string>();
        guidList.AddRange(guidsTexture);
        guidList.AddRange(guidsSprite);

        for (int i = 0; i < guidList.Count; i++)
        {
            string filePath = AssetDatabase.GUIDToAssetPath(guidList[i]);
            //Debug.Log("----->" + filePath);
            System.Type t = AssetDatabase.GetMainAssetTypeAtPath(filePath);//对于TextureType为Sprite(2D and UI)类型的图，获得的类型也是Texture2D

            if (t == typeof(UnityEngine.Texture2D))
            {
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
                //mipmapCount=1则未开启，mipmapCount>1则开启了Mip Maps
                //Debug.LogError(filePath + ",  mipmapCount:" + tex.mipmapCount);
                //mipmapCount is always 1 or more.
                if (tex.mipmapCount > 1)
                {
                    Debug.Log(filePath + "开启了Mip Maps，请关闭。tex.mipmapCount：" + tex.mipmapCount);
                }


            }
        }

        Debug.Log("检测完毕！");
    }


    [MenuItem("工具/检测/图片/Size")]
    public static void CheckSize()
    {
        string[] guidsTexture = AssetDatabase.FindAssets("t:Texture", new[] { "Assets" });
        string[] guidsSprite = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets" });
        List<string> guidList = new List<string>();
        guidList.AddRange(guidsTexture);
        guidList.AddRange(guidsSprite);

        for (int i = 0; i < guidList.Count; i++)
        {
            string filePath = AssetDatabase.GUIDToAssetPath(guidList[i]);
            //Debug.Log("----->" + filePath);
            System.Type t = AssetDatabase.GetMainAssetTypeAtPath(filePath);//对于TextureType为Sprite(2D and UI)类型的图，获得的类型也是Texture2D

            if (t == typeof(UnityEngine.Texture2D))
            {
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);


                if (tex.width >= 1024 || tex.height >= 1024)
                {
                    if (tex.width >= 2048 || tex.height >= 2048)
                    {
                        Debug.LogError(filePath + "，尺寸太大，width:" + tex.width + ", height:" + tex.height);
                    }
                    else
                    {
                        Debug.LogWarning(filePath + "，尺寸过大，width:" + tex.width + ", height:" + tex.height);
                    }
                }
            }
        }

        Debug.Log("检测完毕！");
    }


    /// <summary>
    /// 检测图片资源
    /// 遍历目标目录下的预制件，查找预制件引用到的图片，检测图片
    /// 尺寸>=512或者开了mipmap的图片需要提示用户
    /// </summary>
    [MenuItem("工具/检测/CheckTexture")]
    public static void CheckTexture()
    {
        List<string> mipMapErrorList = new List<string>();
        List<string> sizeErrorList = new List<string>();


        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        int length = guids.Length;
        //Debug.Log("prefab count:" + length);
        for (int i = 0; i < length; i++)
        {
            string filePath = AssetDatabase.GUIDToAssetPath(guids[i]);
            if (!filePath.Contains("Effects") || filePath.Contains("NOT_USED"))
            {
                continue;
            }

            //检查该prefab的依赖项
            string[] dependencies = AssetDatabase.GetDependencies(filePath);
            if (dependencies == null || dependencies.Length == 0)
            {
                continue;
            }

            for (int j = 0; j < dependencies.Length; j++)
            {
                string path = dependencies[j];
                //AssetDatabase.LoadAssetAtPath(dependencies[j],)
                //Debug.Log("----->" + path);
                System.Type t = AssetDatabase.GetMainAssetTypeAtPath(path);//对于TextureType为Sprite(2D and UI)类型的图，获得的类型也是Texture2D

                if (t == typeof(UnityEngine.Texture2D))
                {
                    Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    //有时候会出现判断mip maps和预想中的不一致的情况，这种一般是图片导入有问题
                    //我遇到的情况是图片没有转成纹理格式，还是保持原始状态，因此mipmap的判断会出错
                    //解决方法：对图片右键，弹出菜单选择reimport即可
                    if (tex.mipmapCount != 0)
                    {
                        if (!mipMapErrorList.Contains(path))
                        {
                            Debug.Log(path + "开启了Mip Maps，请关闭");
                            mipMapErrorList.Add(path);
                        }
                    }

                    if (tex.width >= 512 || tex.height >= 512)
                    {
                        if (!sizeErrorList.Contains(path))
                        {
                            if (tex.width >= 1024 || tex.height >= 1024)
                            {
                                Debug.LogError(path + "，尺寸太大，width:" + tex.width + ", height:" + tex.height);
                            }
                            else
                            {
                                Debug.LogWarning(path + "，尺寸过大，width:" + tex.width + ", height:" + tex.height);
                            }

                            sizeErrorList.Add(path);
                        }
                    }

                }
            }
        }

        Debug.Log("检测完毕！");
    }

    [MenuItem("工具/检测/CheckSameRes")]
    public static void CheckSameRes()
    {
        string path = Application.dataPath;


        Debug.Log(" --->check 图片");//*.png|*.jpg|*.bmp|*.jpeg|*.tga 
        logSameFileMsg(Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".bmp") || s.EndsWith(".jpeg") || s.EndsWith(".tga")).ToArray());
        Debug.Log(" --->check 预制件");
        logSameFileMsg(Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories));
        Debug.Log(" --->check 材质球");
        logSameFileMsg(Directory.GetFiles(path, "*.mat", SearchOption.AllDirectories));

        Debug.Log(" --->check fbx/FBX");
        logSameFileMsg(Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".fbx") || s.EndsWith(".FBX")).ToArray());
        Debug.Log(" --->check mesh");
        logSameFileMsg(Directory.GetFiles(path, "*.mesh", SearchOption.AllDirectories));
        Debug.Log(" --->check anim");//动画片段
        logSameFileMsg(Directory.GetFiles(path, "*.anim", SearchOption.AllDirectories));
        Debug.Log(" --->check controller");
        logSameFileMsg(Directory.GetFiles(path, "*.controller", SearchOption.AllDirectories));

        Debug.LogError("检查完毕");
    }

    static void logSameFileMsg(string[] filePaths)
    {
        Dictionary<string, List<string>> resultDic = new Dictionary<string, List<string>>();
        foreach (var path in filePaths)
        {
            if (path.Contains("temp_for_prefab") || path.Contains("TMP_OUTPUT") || path.Contains("Editor") || path.Contains("LogViewer") || path.Contains("AniInstancing") || path.Contains("SoftMask") || path.Contains("Samples~") || path.Contains("ExampleScenes"))
                continue;

            string filename = Path.GetFileNameWithoutExtension(path);
            if (resultDic.ContainsKey(filename))
            {
                if (resultDic[filename].Count > 0)
                {
                    resultDic[filename].Add(path);
                }
            }
            else
            {
                resultDic[filename] = new List<string>();
                resultDic[filename].Add(path);
            }
        }


        //log duplicated files
        foreach (var item in resultDic)
        {
            List<string> list = item.Value;
            if (list.Count >= 2)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Debug.Log(list[i].Substring(list[i].IndexOf("Assets")));
                }
            }
        }
    }
}
