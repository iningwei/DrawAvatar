using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.AB;
using ZGame.Ress;
using AYellowpaper.SerializedCollections;
using System;

namespace ZGame.Ress.AB
{
    public class ABCache : SingletonMonoBehaviour<ABCache>
    {
        public SerializedDictionary<string, SpriteRes> spriteCacheDic = new SerializedDictionary<string, SpriteRes>();

        public SerializedDictionary<string, TextureRes> textureCacheDic = new SerializedDictionary<string, TextureRes>();
        public SerializedDictionary<string, MatRes> matCacheDic = new SerializedDictionary<string, MatRes>();
        public SerializedDictionary<string, AudioRes> audioCacheDic = new SerializedDictionary<string, AudioRes>();
        public SerializedDictionary<string, MeshRes> meshCacheDic = new SerializedDictionary<string, MeshRes>();
        public SerializedDictionary<string, AnimatorControllerRes> animatorControllerCacheDic = new SerializedDictionary<string, AnimatorControllerRes>();
        public SerializedDictionary<string, AnimationClipRes> animationClipCacheDic = new SerializedDictionary<string, AnimationClipRes>();
        public SerializedDictionary<string, ObjectRes> objectCacheDic = new SerializedDictionary<string, ObjectRes>();



        public SerializedDictionary<string, WindowRes> windowCacheDic = new SerializedDictionary<string, WindowRes>();
        public SerializedDictionary<string, EffectRes> effectCacheDic = new SerializedDictionary<string, EffectRes>();
        public SerializedDictionary<string, OtherPrefabRes> otherPrefabCacheDic = new SerializedDictionary<string, OtherPrefabRes>();


        public void RemoveRes(Res res)
        {
            if (res is SpriteRes)
            {
                spriteCacheDic.Remove(res.resName);
            }
            else if (res is TextureRes)
            {
                textureCacheDic.Remove(res.resName);
            }
            else if (res is MatRes)
            {
                matCacheDic.Remove(res.resName);
            }
            else if (res is AudioRes)
            {
                audioCacheDic.Remove(res.resName);
            }
            else if (res is MeshRes)
            {
                meshCacheDic.Remove(res.resName);
            }
            else if (res is AnimatorControllerRes)
            {
                animatorControllerCacheDic.Remove(res.resName);
            }
            else if (res is AnimationClipRes)
            {
                animationClipCacheDic.Remove(res.resName);
            }
            else if (res is ObjectRes)
            {
                objectCacheDic.Remove(res.resName);
            }

            else if (res is WindowRes)
            {
                windowCacheDic.Remove(res.resName);
            }
            else if (res is EffectRes)
            {
                effectCacheDic.Remove(res.resName);
            }
            else if (res is OtherPrefabRes)
            {
                otherPrefabCacheDic.Remove(res.resName);
            }
            else if (res is AnimationClipRes)
            {
                //本项目常驻，不处理
                //TODO:
            }
        }

        public void RemoveRes(GameObject targetObj, ABType abType)
        {
            string targetKey = "";
            switch (abType)
            {
                case ABType.Effect:
                    foreach (var item in effectCacheDic)
                    {
                        if (item.Value.resObj == targetObj)
                        {
                            targetKey = item.Key;
                            break;
                        }
                    }
                    if (targetKey != "")
                    {
                        effectCacheDic.Remove(targetKey);
                    }
                    break;
                case ABType.Window:

                    foreach (var item in windowCacheDic)
                    {
                        if (item.Value.resObj == targetObj)
                        {
                            targetKey = item.Key;
                            break;
                        }
                    }
                    if (targetKey != "")
                    {
                        windowCacheDic.Remove(targetKey);
                    }
                    break;
                case ABType.OtherPrefab:
                    foreach (var item in otherPrefabCacheDic)
                    {
                        if (item.Value.resObj == targetObj)
                        {
                            targetKey = item.Key;
                            break;
                        }
                    }
                    if (targetKey != "")
                    {
                        otherPrefabCacheDic.Remove(targetKey);
                    }
                    break;
                default:
                    Debug.Log("not supported yet, abType:" + abType);
                    break;

            }
        }

        public void AddRes(Res res)
        {
            if (res is SpriteRes)
            {
                if (this.spriteCacheDic.ContainsKey(res.resName) == false)
                {
                    this.spriteCacheDic[res.resName] = res as SpriteRes;
                }
            }
            else if (res is TextureRes)
            {
                if (this.textureCacheDic.ContainsKey(res.resName) == false)
                {
                    TextureRes textureRes = res as TextureRes;
                    this.textureCacheDic[res.resName] = textureRes;
                }
            }
            else if (res is MatRes)
            {
                if (this.matCacheDic.ContainsKey(res.resName) == false)
                {
                    this.matCacheDic[res.resName] = res as MatRes;
                }
            }
            else if (res is AudioRes)
            {
                if (this.audioCacheDic.ContainsKey(res.resName) == false)
                {
                    this.audioCacheDic[res.resName] = res as AudioRes;
                }
            }
            else if (res is MeshRes)
            {
                if (this.meshCacheDic.ContainsKey(res.resName) == false)
                {
                    this.meshCacheDic[res.resName] = res as MeshRes;
                }
            }
            else if (res is AnimatorControllerRes)
            {
                if (this.animatorControllerCacheDic.ContainsKey(res.resName) == false)
                {
                    this.animatorControllerCacheDic[res.resName] = res as AnimatorControllerRes;
                }
            }
            else if (res is AnimationClipRes)
            {
                if (this.animationClipCacheDic.ContainsKey(res.resName) == false)
                {
                    this.animationClipCacheDic[res.resName] = res as AnimationClipRes;
                }
            }
            else if (res is ObjectRes)
            {
                if (this.objectCacheDic.ContainsKey(res.resName) == false)
                {
                    this.objectCacheDic[res.resName] = res as ObjectRes;
                }
            }
            else if (res is PrefabRes)
            {
                if (res is WindowRes)
                {
                    if (this.windowCacheDic.ContainsKey(res.resName) == false)
                    {
                        this.windowCacheDic[res.resName] = res as WindowRes;
                    }
                }
                else if (res is OtherPrefabRes)
                {
                    if (this.otherPrefabCacheDic.ContainsKey(res.resName) == false)
                    {
                        this.otherPrefabCacheDic[res.resName] = res as OtherPrefabRes;
                    }
                }
                else if (res is EffectRes)
                {
                    if (this.effectCacheDic.ContainsKey(res.resName) == false)
                    {
                        this.effectCacheDic[res.resName] = res as EffectRes;
                    }
                }
            }

        }

        public T GetRes<T>(string name) where T : Res
        {
            Type type = typeof(T);
            if (type.Equals(typeof(SpriteRes)))
            {
                if (this.spriteCacheDic.ContainsKey(name))
                {
                    return (T)(object)this.spriteCacheDic[name];
                }
            }
            else if (type.Equals(typeof(TextureRes)))
            {
                if (this.textureCacheDic.ContainsKey(name))
                {
                    return (T)(object)this.textureCacheDic[name];
                }
            }
            else if (type.Equals(typeof(MatRes)))
            {
                if (this.matCacheDic.ContainsKey(name))
                {
                    return (T)(object)this.matCacheDic[name];
                }
            }
            else if (type.Equals(typeof(AudioRes)))
            {
                if (this.audioCacheDic.ContainsKey(name))
                {
                    return (T)(object)this.audioCacheDic[name];
                }
            }
            else if (type.Equals(typeof(MeshRes)))
            {
                if (this.meshCacheDic.ContainsKey(name))
                {
                    return (T)(object)this.meshCacheDic[name];
                }
            }
            else if (type.Equals(typeof(AnimatorControllerRes)))
            {
                if (this.animatorControllerCacheDic.ContainsKey(name))
                {
                    return (T)(object)this.animatorControllerCacheDic[name];
                }
            }
            else if (type.Equals(typeof(AnimationClipRes)))
            {
                if (this.animationClipCacheDic.ContainsKey(name))
                {
                    return (T)(object)this.animationClipCacheDic[name];

                }
            }
            else if (type.Equals(typeof(ObjectRes)))
            {
                if (this.objectCacheDic.ContainsKey(name))
                {
                    return (T)(object)this.objectCacheDic[name];
                }
            }
            else
            {
                Debug.LogError("error get res:" + name + ", T:" + type.ToString());
            }
            //////其它类型不走缓存，有必要自己通过对象池来实现

            return null;
        }
    }
}