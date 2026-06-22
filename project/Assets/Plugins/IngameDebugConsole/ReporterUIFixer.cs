using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//遇到了场景切换后，日志插件图片被引擎底层清除而导致丢失的问题。（通过游戏内运行时的inspector和hierarchy插件，发现图片节点的图在场景切换后消失）
//暂时找不到具体被清除的原因，通过该脚本 复原
public class ReporterUIFixer : MonoBehaviour
{
    private Image[] images;
    private string[] originalSprites; // 缓存原始 Sprite名字
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        CacheImagesAndSprites();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void CacheImagesAndSprites()
    {
        images = GetComponentsInChildren<Image>(true);
        originalSprites = new string[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            originalSprites[i] = images[i].sprite.name;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 检查和修复 Image
        for (int i = 0; i < images.Length; i++)
        {
            var image = images[i];
            // 恢复 Sprite
            if (image.sprite == null)
            {
                if (originalSprites[i] != null)
                {
                    var s = Resources.Load<Sprite>("IngameDebugConsole/" + originalSprites[i]);
                    if (s == null)
                    {
                        Debug.LogError("error load:" + "IngameDebugConsole/" + originalSprites[i]);
                    }
                    image.sprite = s;
                }
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}