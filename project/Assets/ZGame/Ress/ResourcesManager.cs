using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ResourcesManager : Singleton<ResourcesManager>
{
    //TODO:
    //后续处理依赖等
    //后续 GameObjectHelper的处理
    public GameObject LoadObj(string path)
    {
        GameObject p = Resources.Load(path) as GameObject;
        var g = GameObject.Instantiate(p);
        return g;
    }

    public void LoadLoadObjAsync(string path, Action<GameObject> onLoaded)
    {
        ResourceRequest rr = Resources.LoadAsync(path);
        rr.completed += (ao) =>
        {
            if (ao.isDone)
            {
                GameObject p = (rr.asset) as GameObject;
                GameObject g = GameObject.Instantiate(p);
                if (onLoaded != null)
                {
                    onLoaded(g);
                }
            }
        };
    }

    public Sprite LoadSpriteFromSpriteAtlas(string atlasFolder, string spriteName)
    {
        SpriteAtlas atlas = Resources.Load<SpriteAtlas>(atlasFolder);
        Sprite target = null;
        if (atlas != null)
        {
            target = atlas.GetSprite(spriteName);
        }

        if (target == null)
        {
            Debug.LogError($"error get sprite {spriteName} from {atlasFolder}");
        }
        return target;
    }
}
