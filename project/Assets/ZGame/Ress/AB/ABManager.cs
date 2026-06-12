
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ZGame.Event;
using ZGame.Ress.AB.Holder;
using ZGame.Ress.Info;
using ZGame.UGUIExtention;


namespace ZGame.Ress.AB
{
    public class ABManager : Singleton<ABManager>
    {
        bool isInit = false;

        public ABManager()
        {
            if (!isInit)
            {
                Debug.Log("load common ab");
                AB.Load("common", ABType.Null, (objs) =>
                {

                    int count = objs.Length;
                    for (int i = 0; i < count; i++)
                    {
                        if (objs[i].name == "ShaderVariants")
                        {
                            Debug.Log("ShaderVariants WarmUp");
                            (objs[i] as ShaderVariantCollection).WarmUp();//WarmUp() all variants,in case of stuck while to complie variant at run time
                        }
                    }

                    EventDispatcher.Instance.AddListener(EventID.OnABResLoaded, onABResLoaded);
                    EventDispatcher.Instance.AddListener(EventID.OnRootCompInfoHolderObjDestroy, onRootCompObjDestroy);
                    EventDispatcher.Instance.AddListener(EventID.OnCompInfoHolderChildObjDestroy, onCompInfoHolderChildObjDestroy);

                    EventDispatcher.Instance.AddListener(EventID.OnDynamicCompInfoHolderObjInstantiate, onDynamicCompInfoHolderObjInstantiate);
                    Debug.Log("ABManger init finished");
                    isInit = true;
                }, true, true);// load common ab,we can not call ab.Unload(false) , otherwise shader will miss on mobile/PC devices!so set keep AB status is true.
            }
        }


        public void RemoveCachedRes(ABType abType, Res res)
        {
            ABCache.Instance.RemoveRes(res);
        }
        public void RemoveCachedRes(ABType abType, GameObject obj)
        {
            ABCache.Instance.RemoveRes(obj, abType);
        }
        public T GetCachedRes<T>(string name) where T : Res
        {
            return ABCache.Instance.GetRes<T>(name);
        }


        private void onDynamicCompInfoHolderObjInstantiate(string evtId, object[] paras)
        {
            GameObject obj = paras[0] as GameObject;
            DynamicCompInfoHolder dynamicHolder = obj.GetComponent<DynamicCompInfoHolder>();
            if (dynamicHolder != null)
            {
                this.fillReferenceOfDynamicCompInfoHolder(obj);

            }


            //TODO:一般不会直接对带有RootCompInfoHolder的prefab直接instantiate
            //////RootCompInfoHolder rootCompInfoHolder = obj.GetComponent<RootCompInfoHolder>();
            //////if (rootCompInfoHolder != null)
            //////{
            //////    this.FillReferenceOfRootCompInfoHolder(obj, true);
            //////    this.AddDestroyNotice(obj);
            //////}

        }


        private void onCompInfoHolderChildObjDestroy(string evtId, object[] paras)
        {
            GameObject obj = paras[0] as GameObject;
            removeSpriteReference(obj.transform);
            removeTextureReference(obj.transform);
            removeMatReference(obj.transform);
            removeMeshReference(obj.transform);
            removeAnimatorControllerReference(obj.transform);
            removeObjectReference(obj.transform);
        }

        void removeSpriteReference(Transform trans)
        {
            //TODO:loop check cost too much.Need change!
            var spriteCacheList = new List<SpriteRes>(ABCache.Instance.spriteCacheDic.Values);
            SpriteRes sr;
            for (int j = spriteCacheList.Count - 1; j >= 0; j--)
            {
                sr = spriteCacheList[j];
                //////if (sr.ContainRefTrs(trans))
                //////{
                sr.RemoveRefTrs(trans);
                //////}
            }

        }
        void removeTextureReference(Transform trans)
        {
            //TODO:loop check cost too much.Need change!  
            var textureCacheList = new List<TextureRes>(ABCache.Instance.textureCacheDic.Values);
            TextureRes tr;
            for (int j = textureCacheList.Count - 1; j >= 0; j--)
            {
                tr = textureCacheList[j];
                //////if (tr.ContainRefTrs(trans))
                //////{
                tr.RemoveRefTrs(trans);
                //////}
            }

        }
        void removeMatReference(Transform trans)
        {
            //TODO:loop check cost too much.Need change!  
            var matCacheList = new List<MatRes>(ABCache.Instance.matCacheDic.Values);
            MatRes mr;
            for (int j = matCacheList.Count - 1; j >= 0; j--)
            {
                mr = matCacheList[j];
                //////if (mr.ContainRefTrs(trans))
                //////{
                mr.RemoveRefTrs(trans);
                //////}
            }
        }

        void removeMeshReference(Transform trans)
        {
            var meshCacheList = new List<MeshRes>(ABCache.Instance.meshCacheDic.Values);
            MeshRes mr;
            for (int j = meshCacheList.Count - 1; j >= 0; j--)
            {
                mr = meshCacheList[j];
                //////if (mr.ContainRefTrs(trans))
                //////{
                mr.RemoveRefTrs(trans);
                //////}
            }
        }
        void removeObjectReference(Transform trans)
        {
            var objectCacheList = new List<ObjectRes>(ABCache.Instance.objectCacheDic.Values);
            ObjectRes objectRes;
            for (int i = objectCacheList.Count - 1; i >= 0; i--)
            {
                objectRes = objectCacheList[i];
                objectRes.RemoveRefTrs(trans);
            }
        }
        void removeAnimatorControllerReference(Transform trans)
        {
            var acCacheList = new List<AnimatorControllerRes>(ABCache.Instance.animatorControllerCacheDic.Values);
            AnimatorControllerRes acRes;
            for (int i = acCacheList.Count - 1; i >= 0; i--)
            {
                acRes = acCacheList[i];
                acRes.RemoveRefTrs(trans);
            }
        }

        void onRootCompObjDestroy(string evtId, object[] paras)
        {
            GameObject obj = paras[0] as GameObject;
            RootCompInfoHolder holder = obj.GetComponent<RootCompInfoHolder>();
            if (holder != null)
            {
                ABType abType = holder.abType;
                RemoveCachedRes(abType, obj);
            }
        }

        public void AddRootCompInfoHolderDestroyNotice(GameObject rootObj)
        {
            rootObj.GetOrAddComponent<OnRootCompInfoHolderObjDestroyNotice>();
        }
        private void onABResLoaded(string evtId, object[] paras)
        {
            Res res = paras[0] as Res;

            bool sync = true;
            if (paras.Length >= 2 && paras[1] != null)
            {
                sync = bool.Parse(paras[1].ToString());
            }

            if (res is PrefabRes)
            {
                GameObject obj = res.resObj as GameObject;
                FillReferenceOfRootCompInfoHolder(obj, sync);
                this.AddRootCompInfoHolderDestroyNotice(obj);
                res.AddRefTrs(obj.transform);
            }
            ABCache.Instance.AddRes(res);

        }


        public void LoadWindow(string name, Action<UnityEngine.Object> callback, bool sync)
        {
            ABPrefab.Load(name, ABType.Window, callback, sync);
        }


        public void LoadScene(string name, LoadSceneMode loadSceneMode, Action<UnityEngine.Object> sceneLoadedCallback, Action lightmapAttachedCallback, bool sync)
        {
            ABScene.Load(name, loadSceneMode, sceneLoadedCallback, lightmapAttachedCallback, sync);
        }

        public void UnloadScene(string name, Action callback)
        {
            ABScene.Unload(name, callback);
        }

        public void LoadLargeScene(string name, LoadSceneMode loadSceneMode, Vector3 initPos, Action<UnityEngine.Object> sceneLoadedCallback, Action initChunksLoadedCallback, bool sync)
        {
            ABLargeScene.Load(name, loadSceneMode, initPos, sceneLoadedCallback, initChunksLoadedCallback, sync);
        }

        public void UnloadLargeScene(string name, Action callback)
        {
            ABLargeScene.Unload(name, callback);
        }


        public void LoadEffect(string name, Action<UnityEngine.Object> callback, bool sync)
        {
            ABPrefab.Load(name, ABType.Effect, callback, sync);
        }

        public void LoadOtherPrefab(string name, Action<UnityEngine.Object> callback, bool sync)
        {
            ABPrefab.Load(name, ABType.OtherPrefab, callback, sync);
        }

        public void ResetEditorShader(Transform tran, Material mat, string shaderName)
        {
#if UNITY_EDITOR
            if (mat != null)
            {
                var shader = Shader.Find(shaderName);
                if (shader != null)
                {
                    mat.shader = shader;

                    //bug hack处理
                    //在较低版本的Unity中，如果是standard shader且使用了Transparent或Fade或Cutout的渲染模式，通过AB的方式加载模型，那么在编辑器模式下RenderQueue可能值出错，造成渲染顺序上的错误
                    //2021.3.8f1中对于renderQueue值不匹配的已经自动设置了默认值，相关代码参见 StandardShaderGUI.cs(https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/StandardShaderGUI.cs)
                    //唯一恼火的是在客户端会打印出很多重置RenderQueue的日志(StandardShaderGUI.cs line 410)。因此本项目集成了官方的StardardShaderGUI.cs，并注释了相关打印信息

                    if (shaderName == "My/Standard" || shaderName == "My/Standard (Specular setup)")
                    {
                        if (mat.IsKeywordEnabled("_ALPHAPREMULTIPLY_ON"))
                        {
                            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                        }
                        if (mat.IsKeywordEnabled("_ALPHABLEND_ON"))
                        {
                            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                        }
                        if (mat.IsKeywordEnabled("_ALPHATEST_ON"))
                        {
                            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                        }
                    }
                }
                else
                {
                    DebugExt.LogE("can not find shader:" + shaderName);
                }
            }
#endif
        }

        void fillReferenceOfDynamicCompInfoHolder(GameObject target)
        {
            DynamicCompInfoHolder holder = target.GetComponent<DynamicCompInfoHolder>();
            if (holder == null)
            {
                DebugExt.LogE("no dynamicCompInfoHolder attached:" + target.GetHierarchy());
                return;
            }

            //----------------->BuildIn Image
            if (holder.buildInCompImageInfos != null && holder.buildInCompImageInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompImageInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.refSprites != null && item.refSprites.Count > 0)
                    {
                        SpriteRes spriteRes = this.GetCachedRes<SpriteRes>(item.refSprites[0].atlasName);
                        if (spriteRes != null)
                        {
                            spriteRes.AddRefTrs(childTarget);
                        }
                        else
                        {
                            Debug.LogError("error, please check Image");
                        }
                    }
                    if (item.matIndex != -1)
                    {
                        MatRes matRes = this.GetCachedRes<MatRes>(item.matName);
                        if (matRes != null)
                        {
                            matRes.AddRefTrs(childTarget);
                        }
                        else
                        {
                            Debug.LogError("error, please check mat");
                        }
                    }
                }
            }

            //-------------->BuildIn RawImage
            if (holder.buildInCompRawImageInfos != null && holder.buildInCompRawImageInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompRawImageInfos)
                {
                    Transform childTarget = item.tran;
                    if (!string.IsNullOrEmpty(item.texName))
                    {
                        TextureRes texRes = this.GetCachedRes<TextureRes>(item.texName);
                        if (texRes != null)
                        {
                            texRes.AddRefTrs(childTarget);
                        }
                        else
                        {
                            Debug.LogError("error, please check Image");
                        }
                    }
                    if (item.matIndex != -1)
                    {
                        MatRes matRes = this.GetCachedRes<MatRes>(item.matName);
                        if (matRes != null)
                        {
                            matRes.AddRefTrs(childTarget);
                        }
                        else
                        {
                            Debug.LogError("error, please check mat");
                        }
                    }
                }
            }




            //-------------->BuildIn SpriteRenderer
            if (holder.buildInCompSpriteRendererInfos != null && holder.buildInCompSpriteRendererInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompSpriteRendererInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.refSprites != null && item.refSprites.Count > 0)
                    {
                        SpriteRes spriteRes = this.GetCachedRes<SpriteRes>(item.refSprites[0].atlasName);
                        if (spriteRes != null)
                        {
                            spriteRes.AddRefTrs(childTarget);
                        }
                        else
                        {
                            Debug.LogError("error, please check SpriteRenderer");
                        }
                    }
                    if (item.matIndex != -1)
                    {
                        MatRes matRes = this.GetCachedRes<MatRes>(item.matName);
                        if (matRes != null)
                        {
                            matRes.AddRefTrs(childTarget);
                        }
                        else
                        {
                            Debug.LogError("error, please check Material");
                        }
                    }
                }
            }

            //-------------->BuildIn Renderer
            if (holder.buildInCompRendererInfos != null && holder.buildInCompRendererInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompRendererInfos)
                {
                    Transform childTarget = item.tran;
                    for (int k = 0; k < item.refTextures.Count; k++)
                    {
                        TextureRes texRes = this.GetCachedRes<TextureRes>(item.refTextures[k].texName);
                        if (texRes != null)
                        {
                            texRes.AddRefTrs(childTarget);
                        }
                        else
                        {
                            Debug.LogError("error, please check, no imgRes with texName:" + item.refTextures[k].texName);
                        }
                    }
                }
            }

            //-------------->Ext ImageSequence
            if (holder.extCompImageSequenceInfos != null && holder.extCompImageSequenceInfos.Count > 0)
            {
                foreach (var item in holder.extCompImageSequenceInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.refSprites != null && item.refSprites.Count > 0)
                    {
                        for (int i = 0; i < item.refSprites.Count; i++)
                        {
                            SpriteRes spriteRes = this.GetCachedRes<SpriteRes>(item.refSprites[i].atlasName);
                            if (spriteRes != null)
                            {
                                spriteRes.AddRefTrs(childTarget);
                            }
                            else
                            {
                                DebugExt.LogE("error, please check ImageSequence");
                            }
                        }
                    }

                }
            }

            //-------------->Ext SpriteSequence
            if (holder.extCompSpriteRendererSequenceInfos != null && holder.extCompSpriteRendererSequenceInfos.Count > 0)
            {
                foreach (var item in holder.extCompSpriteRendererSequenceInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.refSprites != null && item.refSprites.Count > 0)
                    {
                        for (int i = 0; i < item.refSprites.Count; i++)
                        {
                            SpriteRes spriteRes = this.GetCachedRes<SpriteRes>(item.refSprites[i].atlasName);
                            if (spriteRes != null)
                            {
                                spriteRes.AddRefTrs(childTarget);
                            }
                            else
                            {
                                DebugExt.LogE("error, please check SpriteSequence");
                            }
                        }
                    }
                }
            }
            //-------------->Ext SwapSprite
            if (holder.extCompSwapSpriteInfos != null && holder.extCompSwapSpriteInfos.Count > 0)
            {

                foreach (var item in holder.extCompSwapSpriteInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.refSprites != null && item.refSprites.Count > 0)
                    {
                        for (int i = 0; i < item.refSprites.Count; i++)
                        {
                            SpriteRes spriteRes = this.GetCachedRes<SpriteRes>(item.refSprites[i].atlasName);
                            if (spriteRes != null)
                            {
                                spriteRes.AddRefTrs(childTarget);
                            }
                            else
                            {
                                DebugExt.LogE("error, please check SwapSprite");
                            }
                        }
                    }
                }
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="sync">是否同步</param>
        public void FillReferenceOfRootCompInfoHolder(GameObject target, bool sync = true)
        {
            RootCompInfoHolder holder = target.GetComponent<RootCompInfoHolder>();
            if (holder == null || holder.finishedSet)
            {
                return;
            }

            //TODO:改成通过反射的方式调用
            //----------------->BuildIn Image
            if (holder.buildInCompImageInfos != null && holder.buildInCompImageInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompImageInfos)
                {
                    item.FillCompElement(sync);
                }
            }
            //-------------->BuildIn RawImage
            if (holder.buildInCompRawImageInfos != null && holder.buildInCompRawImageInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompRawImageInfos)
                {
                    item.FillCompElement(sync);
                }
            }

            //-------------->BuildIn SpriteRenderer
            if (holder.buildInCompSpriteRendererInfos != null && holder.buildInCompSpriteRendererInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompSpriteRendererInfos)
                {
                    item.FillCompElement(sync);
                }
            }
            //-------------->BuildIn Text
            if (holder.buildInCompTextInfos != null && holder.buildInCompTextInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompTextInfos)
                {
                    item.FillCompElement(sync);
                }
            }
            //-------->BuildIn TextMeshProUGUI
            if (holder.buildInCompTextMeshProUGUIInfos != null && holder.buildInCompTextMeshProUGUIInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompTextMeshProUGUIInfos)
                {
                    item.FillCompElement(sync);
                }
            }

            //-------------->BuildIn Renderer
            if (holder.buildInCompRendererInfos != null && holder.buildInCompRendererInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompRendererInfos)
                {
                    //////if (sync)
                    //////{
                    //////    if (item.meshName != "")
                    //////    {
                    //////        Mesh mesh = null;
                    //////        LoadMesh(item.meshName, (res) =>
                    //////        {
                    //////            mesh = res.GetResAsset<Mesh>();
                    //////            if (item.concreteCompRenderer is MeshRenderer)
                    //////            {
                    //////                item.tran.GetComponent<MeshFilter>().sharedMesh = mesh;

                    //////                ShaderMeshAnimator sma = item.tran.GetComponent<ShaderMeshAnimator>();
                    //////                if (sma != null)
                    //////                {
                    //////                    sma.baseMesh = mesh;
                    //////                }
                    //////            }
                    //////            else if (item.concreteCompRenderer is SkinnedMeshRenderer)
                    //////            {
                    //////                (item.concreteCompRenderer as SkinnedMeshRenderer).sharedMesh = mesh;
                    //////            }
                    //////            else if (item.concreteCompRenderer is ParticleSystemRenderer)
                    //////            {
                    //////                (item.concreteCompRenderer as ParticleSystemRenderer).mesh = mesh;
                    //////            }

                    //////            //add mesh ref 
                    //////            MeshRes meshRes = this.GetCachedRes<MeshRes>(item.meshName);
                    //////            meshRes.AddRefTrs(item.tran);

                    //////        }, sync);
                    //////    }

                    //////    if (item.matIndex != -1)
                    //////    {
                    //////        Material renderMat = null;
                    //////        LoadMat(item.matName, (matRes) =>
                    //////        {
                    //////            renderMat = matRes.GetResAsset<Material>();
                    //////            if (item.concreteCompRenderer is ParticleSystemRenderer)//particleSystemRenderer需要特殊处理
                    //////            {
                    //////                item.concreteCompRenderer.sharedMaterial = renderMat;
                    //////            }
                    //////            else
                    //////            {
                    //////                Material[] sharedMaterialsCopy = item.concreteCompRenderer.sharedMaterials;
                    //////                sharedMaterialsCopy[item.matIndex] = renderMat;
                    //////                item.concreteCompRenderer.sharedMaterials = sharedMaterialsCopy;
                    //////            }
                    //////        });
                    //////        if (renderMat != null)
                    //////        {
                    //////            Transform childTarget = item.tran;
                    //////            //add mat ref 
                    //////            MatRes matRes = this.GetCachedRes<MatRes>(item.matName) as MatRes;
                    //////            matRes.AddRefTrs(childTarget);

                    //////            if (item.refTextures != null && item.refTextures.Count > 0)
                    //////            {
                    //////                for (int k = 0; k < item.refTextures.Count; k++)
                    //////                {
                    //////                    LoadTextureToMat(childTarget, item.refTextures[k].texName, item.refTextures[k].shaderProp, renderMat, sync, null);
                    //////                }
                    //////            }
                    //////            ResetEditorShader(childTarget, renderMat, item.shaderName);
                    //////        }
                    //////    }
                    //////}
                    //////else
                    //////{
                    //////    BuildInCompRenderInfoSyncOperator.Instance.Add(item);
                    //////}


                    item.FillCompElement(sync);
                }
            }

            //-------------->BuildIn MeshCollider
            if (holder.buildInCompMeshColliderInfos != null && holder.buildInCompMeshColliderInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompMeshColliderInfos)
                {
                    item.FillCompElement(sync);
                }
            }

            //--------------->BuildIn AnimatorController
            if (holder.buildInCompAnimatorInfos != null && holder.buildInCompAnimatorInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompAnimatorInfos)
                {
                    item.FillCompElement(sync);
                }
            }

            //-------------->Ext ImageSequence
            if (holder.extCompImageSequenceInfos != null && holder.extCompImageSequenceInfos.Count > 0)
            {
                foreach (var item in holder.extCompImageSequenceInfos)
                {
                    item.FillCompElement(sync);
                }
            }

            //------------->Ext Material Texture Sequence
            if (holder.extCompMaterialTextureSequenceInfos != null && holder.extCompMaterialTextureSequenceInfos.Count > 0)
            {
                foreach (var item in holder.extCompMaterialTextureSequenceInfos)
                {
                    item.FillCompElement(sync);
                }
            }

            //-------------->Ext SpriteRendererSequence
            if (holder.extCompSpriteRendererSequenceInfos != null && holder.extCompSpriteRendererSequenceInfos.Count > 0)
            {
                foreach (var item in holder.extCompSpriteRendererSequenceInfos)
                {
                    item.FillCompElement(sync);
                }
            }
            //-------------->Ext SwapSprite
            if (holder.extCompSwapSpriteInfos != null && holder.extCompSwapSpriteInfos.Count > 0)
            {

                foreach (var item in holder.extCompSwapSpriteInfos)
                {
                    item.FillCompElement(sync);
                }
            }
            //-------------->Ext Switch2Button
            if (holder.extCompSwitch2ButtonInfos != null && holder.extCompSwitch2ButtonInfos.Count > 0)
            {
                foreach (var item in holder.extCompSwitch2ButtonInfos)
                {
                    item.FillCompElement(sync);
                }
            }
            //-------------->Ext ShaderMeshAnimator
            //META NEW TODO:zhouhui
            //////if (holder.extCompShaderMeshAnimationInfos != null && holder.extCompShaderMeshAnimationInfos.Count > 0)
            //////{
            //////    foreach (var item in holder.extCompShaderMeshAnimationInfos)
            //////    {
            //////        item.FillCompElement(sync);
            //////    }
            //////}
            //---------------->Lightmap
            //if (holder.lightmapInfo != null && holder.lightmapInfo.lightmapColors != null && holder.lightmapInfo.lightmapColors.Count > 0 && holder.lightmapInfo.lightmapRenders != null && holder.lightmapInfo.lightmapRenders.Count > 0)
            //{
            //    ABScene.LoadLightmaps(holder.lightmapInfo, sync);
            //}
            //handle at ABScene.cs

            holder.finishedSet = true;
        }




        public void LoadTexture(string texName, Action<Texture> callback, bool sync)
        {
            Texture tex = null;
            var cache = this.GetCachedRes<TextureRes>(texName);
            if (cache == null)
            {
                ABTexture.Load(texName, callback, sync);
            }
            else
            {
                tex = cache.GetResAsset<Texture>();
                callback?.Invoke(tex);
            }
        }




        public void LoadMat(string matName, Action<MatRes> callback)
        {
            loadMat(matName, callback, true);
        }

        private void loadMat(string matName, Action<MatRes> callback, bool sync)
        {
            var cacheRes = this.GetCachedRes<MatRes>(matName);
            if (cacheRes == null)
            {
                ABMaterial.Load(matName, callback, sync);
            }
            else
            {
                callback?.Invoke(cacheRes);
            }
        }
        public void LoadAnimatorController(string acName, Action<AnimatorControllerRes> callback, bool sync)
        {
            var cacheRes = this.GetCachedRes<AnimatorControllerRes>(acName);
            if (cacheRes == null)
            {
                ABAnimatorController.Load(acName, callback, sync);
            }
            else
            {
                callback?.Invoke(cacheRes);
            }
        }

        public void LoadMesh(string meshName, Action<MeshRes> callback, bool sync)
        {
            this.loadMesh(meshName, callback, sync);
        }
        void loadMesh(string meshName, Action<MeshRes> callback, bool sync)
        {
            var cacheRes = this.GetCachedRes<MeshRes>(meshName);
            if (cacheRes == null)
            {
                ABMesh.Load(meshName, callback, sync);
            }
            else
            {
                callback?.Invoke(cacheRes);
            }
        }

        public void LoadSprite(string atlasName, string spriteName, Action<Sprite> callback, bool sync)
        {
            Sprite target = null;
            var cache = this.GetCachedRes<SpriteRes>(atlasName);
            if (cache == null)
            {
                ABSprite.Load(atlasName, spriteName, callback, sync);
            }
            else
            {
                target = cache.GetResAsset<Sprite>(spriteName);
                callback?.Invoke(target);
            }
        }

        public void LoadTextureToMat(Transform targetTrs, string texName, string shaderPropName, Material mat, bool sync, Action callback)
        {
            Action<Texture> handleTextureAfterLoad = (tex) =>
            {
                var oldTex = mat.GetTexture(shaderPropName);
                string oldTexName = "";
                if (oldTex != null)
                {
                    oldTexName = oldTex.name;
                }

                mat.SetTexture(shaderPropName, tex);


                //remove old ref
                if (!string.IsNullOrEmpty(oldTexName) && oldTexName != texName)
                {
                    var oldRes = this.GetCachedRes<TextureRes>(oldTexName);
                    if (oldRes != null)
                    {
                        oldRes.RemoveRefTrs(targetTrs);
                        Debug.Log(oldTexName + " remove ref tran:" + targetTrs.GetHierarchy());
                    }
                }

                //add ref
                if (targetTrs != null)
                {
                    try
                    {
                        TextureRes texRes = this.GetCachedRes<TextureRes>(texName);
                        texRes.AddRefTrs(targetTrs);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"error:{ex.ToString()}, texName:{texName}, tran:{targetTrs.GetHierarchy()}");
                    }
                }


                callback?.Invoke();
            };

            LoadTexture(texName, handleTextureAfterLoad, sync);

        }

        public void LoadTextureToMatTextureSeq(MaterialTextureSequence ts, string texName, int index, bool sync)
        {
            string oldTexName = "";
            Texture oldTex = ts.textures[index];
            if (oldTex != null)
            {
                oldTexName = oldTex.name;
            }


            LoadTexture(texName, (tex) =>
            {
                ts.textures[index] = tex;


                //remove old ref
                if (!string.IsNullOrEmpty(oldTexName) && oldTexName != texName)
                {
                    var oldRes = this.GetCachedRes<TextureRes>(oldTexName);
                    if (oldRes != null)
                    {
                        oldRes.RemoveRefTrs(ts.transform);
                    }
                }

                //add ref
                TextureRes texRes = this.GetCachedRes<TextureRes>(texName);
                texRes.AddRefTrs(ts.transform);

            },
            sync);

        }

        public void LoadTextureToRawImage(RawImage targetRawImage, string texName, bool sync)
        {
            Action<Texture> handleTextureAfterLoad = (tex) =>
            {
                var oldTex = targetRawImage.texture;
                string oldTexName = "";
                if (oldTex != null)
                {
                    oldTexName = oldTex.name;
                }

                targetRawImage.texture = tex;



                //remove old ref
                if (!string.IsNullOrEmpty(oldTexName) && oldTexName != texName)
                {
                    var oldRes = this.GetCachedRes<TextureRes>(oldTexName);
                    if (oldRes != null)
                    {
                        oldRes.RemoveRefTrs(targetRawImage.transform);
                    }
                }

                //add ref 
                TextureRes texRes = this.GetCachedRes<TextureRes>(texName);
                texRes.AddRefTrs(targetRawImage.transform);


            };


            LoadTexture(texName, handleTextureAfterLoad, sync);
        }

        public void LoadSpriteToTarget<T>(T target, string atlasName, string spriteName, bool sync, params object[] objs) where T : UnityEngine.Component
        {
            Action<Sprite> handleSpriteAfterLoad = (s) =>
            {
                Transform trs = target.transform;
                string oldAtlasName = "";

                Type ty = target.GetType();
                if (ty.Equals(typeof(SpriteRenderer)))
                {
                    SpriteRenderer sr = (target as SpriteRenderer);
                    oldAtlasName = sr.sprite != null ? sr.sprite.texture.name : "";
                    sr.sprite = s;
                }
                else if (ty.Equals(typeof(Image)))
                {
                    Image img = (target as Image);
                    oldAtlasName = img.sprite != null ? img.sprite.texture.name : "";
                    img.sprite = s;
                }
                else if (ty.Equals(typeof(SpriteRendererSequence)))
                {
                    SpriteRendererSequence ss = target as SpriteRendererSequence;
                    int index = int.Parse(objs[0].ToString());
                    oldAtlasName = ss.sprites[index] != null ? ss.sprites[index].texture.name : "";
                    ss.sprites[index] = s;
                }
                else if (ty.Equals(typeof(ImageSequence)))
                {
                    ImageSequence imgS = target as ImageSequence;
                    int index = int.Parse(objs[0].ToString());
                    oldAtlasName = imgS.sprites[index] != null ? imgS.sprites[index].texture.name : "";
                    imgS.sprites[index] = s;
                }
                else if (ty.Equals(typeof(SwapSprite)))
                {
                    SwapSprite ss = target as SwapSprite;
                    int index = int.Parse(objs[0].ToString());
                    oldAtlasName = ss.sprites[index] != null ? ss.sprites[index].texture.name : "";
                    ss.sprites[index] = s;

                }
                else if (ty.Equals(typeof(Switch2Button)))
                {
                    Switch2Button s2b = target as Switch2Button;
                    int index = int.Parse(objs[0].ToString());
                    oldAtlasName = s2b.switchSprites[index] != null ? s2b.switchSprites[index].texture.name : "";
                    s2b.switchSprites[index] = s;
                }
                else
                {
                    DebugExt.LogE("type not suit!!");
                }



                //remove old ref
                if (!string.IsNullOrEmpty(oldAtlasName) && oldAtlasName != atlasName)
                {
                    var oldRes = this.GetCachedRes<SpriteRes>(oldAtlasName);
                    if (oldRes != null)
                    {
                        oldRes.RemoveRefTrs(trs);
                    }
                }

                //add ref
                var spriteRes = this.GetCachedRes<SpriteRes>(atlasName);
                spriteRes.AddRefTrs(trs);
            };
            LoadSprite(atlasName, spriteName, handleSpriteAfterLoad, sync);
        }



        public void LoadAudioClip(string name, Action<UnityEngine.AudioClip> callback, bool sync)
        {
            AudioClip clip = null;
            var cache = this.GetCachedRes<AudioRes>(name);
            if (cache == null || cache.resObj == null)
            {
                ABAudio.Load(name, callback, sync);
            }
            else
            {
                clip = cache.GetResAsset<AudioClip>();
                callback?.Invoke(clip);
            }
        }

        public void LoadAnimationClip(string name, Action<AnimationClipRes> callback, bool sync)
        {
            var cache = this.GetCachedRes<AnimationClipRes>(name);
            if (cache == null)
            {
                ABAnimationClip.Load(name, callback, sync);
            }
            else
            {
                callback?.Invoke(cache);
            }
        }


        public void LoadByte(string name, Action<TextAsset> callback, bool sync)
        {
            TextAsset asset = null;
            var cache = this.GetCachedRes<ByteRes>(name);
            if (cache == null || cache.resObj == null)
            {
                ABByte.Load(name, callback, sync);
            }
            else
            {
                asset = cache.GetResAsset<TextAsset>();
                callback?.Invoke(asset);
            }
        }

        public void LoadObject(string name, Action<ObjectRes> callback, bool sync)
        {
            var cacheRes = this.GetCachedRes<ObjectRes>(name);
            if (cacheRes == null || cacheRes.resObj == null)
            {
                ABObject.Load(name, callback, sync);
            }
            else
            {
                callback?.Invoke(cacheRes);
            }
        }

        public void LoadLogic(string name, Action<TextAsset[]> callback)
        {
            ABLogic.LoadAll(name, callback);
        }

        public override void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EventID.OnABResLoaded, onABResLoaded);
            EventDispatcher.Instance.RemoveListener(EventID.OnRootCompInfoHolderObjDestroy, onRootCompObjDestroy);
            EventDispatcher.Instance.RemoveListener(EventID.OnCompInfoHolderChildObjDestroy, onCompInfoHolderChildObjDestroy);

            EventDispatcher.Instance.RemoveListener(EventID.OnDynamicCompInfoHolderObjInstantiate, onDynamicCompInfoHolderObjInstantiate);
        }
    }
}