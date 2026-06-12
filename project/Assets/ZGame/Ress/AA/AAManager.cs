
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using ZGame.Ress;
using ZGame.Ress.AB;
using System.Threading.Tasks;
using UnityEngine.Video;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;


public class AAManager : Singleton<AAManager>
{
    Dictionary<string, AudioClip> audioDic = new Dictionary<string, AudioClip>();
    Dictionary<string, VideoClip> videoDic = new Dictionary<string, VideoClip>();
    Dictionary<string, Scene> sceneDic = new Dictionary<string, Scene>();
    Dictionary<string, GameObject> windowDic = new Dictionary<string, GameObject>();
    Dictionary<string, GameObject> othterPrefabDic = new Dictionary<string, GameObject>();
    Dictionary<string, Texture2D> texture2DDic = new Dictionary<string, Texture2D>();

    public void LoadBytesData(string name, Action<byte[]> callback)
    {
        var op = Addressables.LoadAssetAsync<TextAsset>(name + ".bytes");
        op.Completed += (handle) =>
        {
            var datas = handle.Result.bytes;
            callback?.Invoke(datas);
        };
    }

    //1，注意 WEBGL 中不支持 WaitForCompletion()
    //2，回调，Update中。也不可间接调用到WaitForCompletion
    public void LoadBytesDataSync(string name, Action<byte[]> callback)
    {
        var op = Addressables.LoadAssetAsync<TextAsset>(name + ".bytes");

        op.WaitForCompletion();//阻塞等待加载完成，实现同步效果
        if (op.Status == AsyncOperationStatus.Succeeded)
        {
            var datas = op.Result.bytes;
            callback?.Invoke(datas);
        }
        Addressables.Release(op);//释放句柄。TODO：有什么用？？？
    }

    public TextAsset LoadBytesDataSync(string name)
    {
        var op = Addressables.LoadAssetAsync<TextAsset>(name + ".bytes");
        var textAsset = op.WaitForCompletion();//webgl not support such method,it will cause an error.
        return textAsset;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="atlasName"></param>
    /// <param name="atlasSurfix">图集后缀，不同图集攻击产生的后缀不一样；传统TexturePacker产生的后缀一般为.png格式；SpriteAtlas的图集后缀为.spriteatlas 或 .spriteatlasv2</param>
    /// <param name="spriteName"></param>
    /// <param name="callback"></param>
    public void LoadSprite(string atlasName, string atlasSurfix, string spriteName, Action<Sprite> callback)
    {
        var op = Addressables.LoadAssetAsync<Sprite>(atlasName + atlasSurfix + "[" + spriteName + "]");
        op.Completed += (handle) =>
        {
            Sprite s = handle.Result;
            if (s)
            {
                callback?.Invoke(s);
            }
        };
    }
    public void LoadTexture2D(string texName, Action<Texture2D> callback)
    {
        if (texture2DDic.ContainsKey(texName))
        {
            var tex = texture2DDic[texName];
            callback?.Invoke(tex);
#if UNITY_EDITOR
            Debug.Log("loadTexture2D " + texName + " from cache");
#endif
        }
        else
        {
            var op = Addressables.LoadAssetAsync<Texture2D>(texName + ".png");
            op.Completed += (handle) =>
            {
                Texture2D tex = handle.Result;
                if (tex)
                {
                    callback?.Invoke(tex);
                    texture2DDic[texName] = tex;
                }
            };
        }
    }
    public void LoadTexture(string texName, Action<Texture> callback)
    {
        var op = Addressables.LoadAssetAsync<Texture>(texName + ".png");
        op.Completed += (handle) =>
        {
            Texture texture = handle.Result;
            if (texture)
            {
                callback?.Invoke(texture);
            }
        };
    }


    public void LoadAllSprite(string atlasName, Action<Sprite[]> callback)
    {
        var op = Addressables.LoadAssetAsync<SpriteAtlas>(atlasName + ".spriteatlas");
        op.Completed += (handle) =>
        {
            Sprite[] ss = new Sprite[handle.Result.spriteCount];
            int count = handle.Result.GetSprites(ss);
            callback?.Invoke(ss);
        };
    }


    public void LoadAudioClip(string name, string extension, Action<UnityEngine.AudioClip> callback)
    {
        if (extension == "")
        {
            extension = "mp3";
        }
        name = name + "." + extension;
        AudioClip clip = null;
        if (audioDic.ContainsKey(name))
        {
            clip = audioDic[name];
            callback?.Invoke(clip);
        }
        else
        {
            Addressables.LoadAssetAsync<AudioClip>(name).Completed += (handle) =>
            {
                var clip = handle.Result;
                if (clip)
                {
                    audioDic[name] = clip;
                    callback?.Invoke(clip);
                }
            };
        }
    }

    public async Task<AudioClip> LoadAudioClip(string name, string extension)
    {
        if (string.IsNullOrEmpty(extension))
        {
            extension = "mp3";
        }
        name = name + "." + extension;
        if (audioDic.ContainsKey(name))
        {
            return audioDic[name];
        }

        AudioClip clip = await Addressables.LoadAssetAsync<AudioClip>(name).Task;
        audioDic[name] = clip;
        return clip;
    }

    public async Task<VideoClip> LoadVideoClip(string name,string extension)
    {
        if (string.IsNullOrEmpty(extension))
        {
            extension="mp4";
        }
        name = name + "." + extension;
        if (videoDic.ContainsKey(name))
        {
            return videoDic[name];
        }

        VideoClip clip = await Addressables.LoadAssetAsync<VideoClip>(name).Task;
        videoDic[name] = clip;

        return clip;
    }


    public void LoadScene(string name, LoadSceneMode loadMode, Action callback)
    {
        Addressables.LoadSceneAsync(name + ".unity", loadMode).Completed += (handle) =>
        {
            //Debug.Log("load scene suc:" + name);
            callback?.Invoke();
        };
    }
    public void LoadWindow(string name, Action<UnityEngine.Object> callback)
    {
        Addressables.LoadAssetAsync<GameObject>(name + ".prefab").Completed += (handle) =>
        {
            Debug.Log("load window suc:" + name);
            var prefabObj = handle.Result;
            GameObject entityObj = GameObject.Instantiate(prefabObj);
            callback?.Invoke(entityObj);
        };
    }

    public void LoadOtherPrefab(string name, Action<UnityEngine.GameObject> callback)
    {
        Addressables.LoadAssetAsync<GameObject>(name + ".prefab").Completed += (handle) =>
        {
            var prefabObj = handle.Result;
            GameObject entityObj = GameObject.Instantiate(prefabObj);
            callback?.Invoke(entityObj);
            callback = null;
        };
    }
    public void LoadMaterial(string name, Action<Material> callback)
    {
        Addressables.LoadAssetAsync<Material>(name + ".mat").Completed += (handle) =>
        {
            var materialObj = handle.Result;
            callback?.Invoke(materialObj);
        };
    }

    public void LoadVideoClip(string name, Action<VideoClip> callback)
    {
        name = name + ".mp4";
        if (videoDic.ContainsKey(name))
        {
            var videoClipObj = videoDic[name];
            callback?.Invoke(videoClipObj);
        }
        else
        {
            Addressables.LoadAssetAsync<VideoClip>(name).Completed += (handle) =>
            {
                var videoClipObj = handle.Result;
                videoDic.Add(name, videoClipObj);
                callback?.Invoke(videoClipObj);
            };
        }
    }

    public async Task<GameObject> LoadOtherPrefab(string name)
    {
        GameObject prefabObj = await Addressables.LoadAssetAsync<GameObject>(name + ".prefab").Task;
        GameObject obj = GameObject.Instantiate(prefabObj);
        return obj;
    }

    public void DestroyRes<T>(string name)
    {
        Type type = typeof(T);
        if (type.Equals(typeof(AudioClip)))
        {
            if (audioDic.ContainsKey(name))
            {
                Addressables.Release(audioDic[name]);
                audioDic.Remove(name);
            }
        }
    }
}
