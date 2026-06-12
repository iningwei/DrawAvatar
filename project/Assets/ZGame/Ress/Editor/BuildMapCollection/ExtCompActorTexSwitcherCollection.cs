using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB;
using ZGame.Ress.Info;

//////public class ExtCompActorTexSwitcherCollection : IRefResCollection
//////{
//////    public List<CompInfo> GetCompInfo(GameObject obj)
//////    {
//////        List<CompInfo> compInfos = new List<CompInfo>();

//////        var actorTexSwitcherChilds = new List<ActorTexSwitcher>();
//////        obj.GetComponentsInChildren<ActorTexSwitcher>(true, actorTexSwitcherChilds);

//////        for (int i = 0; i < actorTexSwitcherChilds.Count; i++)
//////        {
//////            ActorTexSwitcher switcher = actorTexSwitcherChilds[i];

//////            if (switcher.actorTexMsg.renderers == null || switcher.actorTexMsg.blueCompTexs.Length == 0 ||
//////                switcher.actorTexMsg.redCompTexs.Length == 0
//////                || switcher.actorTexMsg.blueCompTexs.Length !=
//////                switcher.actorTexMsg.redCompTexs.Length)//异常情况。                                                              
//////            {
//////                Debug.LogWarning("check actorTexSwitcher:" + switcher.transform.GetHierarchy());
//////                continue;
//////            }

//////            TextureInfo[] blueTextureInfos = new TextureInfo[switcher.actorTexMsg.blueCompTexs.Length];
//////            for (int j = 0; j < switcher.actorTexMsg.blueCompTexs.Length; j++)
//////            {
//////                //compInfos add element 
//////                string texName = switcher.actorTexMsg.blueCompTexs[j].name;
//////                TextureInfo texInfo = new TextureInfo(texName, "");
//////                blueTextureInfos[j] = texInfo;
//////            }

//////            TextureInfo[] redTextureInfos = new TextureInfo[switcher.actorTexMsg.redCompTexs.Length];
//////            for (int j = 0; j < switcher.actorTexMsg.redCompTexs.Length; j++)
//////            {
//////                //compInfos add element 
//////                string texName = switcher.actorTexMsg.redCompTexs[j].name;
//////                TextureInfo texInfo = new TextureInfo(texName, "");
//////                redTextureInfos[j] = texInfo;
//////            }

//////            //-------------------------->
//////            TextureInfo[] blueStar1TextureInfos = new TextureInfo[switcher.actorTexMsg.blueCompStar1Texs.Length];
//////            for (int j = 0; j < switcher.actorTexMsg.blueCompStar1Texs.Length; j++)
//////            {
//////                //compInfos add element 
//////                string texName = switcher.actorTexMsg.blueCompStar1Texs[j].name;
//////                TextureInfo texInfo = new TextureInfo(texName, "");
//////                blueStar1TextureInfos[j] = texInfo;
//////            }

//////            TextureInfo[] redStar1TextureInfos = new TextureInfo[switcher.actorTexMsg.redCompStar1Texs.Length];
//////            for (int j = 0; j < switcher.actorTexMsg.redCompStar1Texs.Length; j++)
//////            {
//////                //compInfos add element 
//////                string texName = switcher.actorTexMsg.redCompStar1Texs[j].name;
//////                TextureInfo texInfo = new TextureInfo(texName, "");
//////                redStar1TextureInfos[j] = texInfo;
//////            }


//////            TextureInfo[] blueStar2TextureInfos = new TextureInfo[switcher.actorTexMsg.blueCompStar2Texs.Length];
//////            for (int j = 0; j < switcher.actorTexMsg.blueCompStar2Texs.Length; j++)
//////            {
//////                //compInfos add element 
//////                string texName = switcher.actorTexMsg.blueCompStar2Texs[j].name;
//////                TextureInfo texInfo = new TextureInfo(texName, "");
//////                blueStar2TextureInfos[j] = texInfo;
//////            }

//////            TextureInfo[] redStar2TextureInfos = new TextureInfo[switcher.actorTexMsg.redCompStar2Texs.Length];
//////            for (int j = 0; j < switcher.actorTexMsg.redCompStar2Texs.Length; j++)
//////            {
//////                //compInfos add element 
//////                string texName = switcher.actorTexMsg.redCompStar2Texs[j].name;
//////                TextureInfo texInfo = new TextureInfo(texName, "");
//////                redStar2TextureInfos[j] = texInfo;
//////            }


//////            TextureInfo[] blueStar3TextureInfos = new TextureInfo[switcher.actorTexMsg.blueCompStar3Texs.Length];
//////            for (int j = 0; j < switcher.actorTexMsg.blueCompStar3Texs.Length; j++)
//////            {
//////                //compInfos add element 
//////                string texName = switcher.actorTexMsg.blueCompStar3Texs[j].name;
//////                TextureInfo texInfo = new TextureInfo(texName, "");
//////                blueStar3TextureInfos[j] = texInfo;
//////            }

//////            TextureInfo[] redStar3TextureInfos = new TextureInfo[switcher.actorTexMsg.redCompStar3Texs.Length];
//////            for (int j = 0; j < switcher.actorTexMsg.redCompStar3Texs.Length; j++)
//////            {
//////                //compInfos add element 
//////                string texName = switcher.actorTexMsg.redCompStar3Texs[j].name;
//////                TextureInfo texInfo = new TextureInfo(texName, "");
//////                redStar3TextureInfos[j] = texInfo;
//////            }
//////            //<-------------------------


//////            CompInfo buildInCompImageInfo = new ExtCompActorTexSwitcherInfo(
//////                    switcher.transform,
//////                    blueTextureInfos, redTextureInfos,blueStar1TextureInfos,redStar1TextureInfos,blueStar2TextureInfos,redStar2TextureInfos,blueStar3TextureInfos,redStar3TextureInfos);
//////            compInfos.Add(buildInCompImageInfo);

//////        }
//////        return compInfos;
//////    }

//////    public List<AssetBundleBuild> GetResMap(GameObject obj)
//////    {
//////        List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();

//////        var actorTexSwitchers = new List<ActorTexSwitcher>();
//////        obj.GetComponentsInChildren<ActorTexSwitcher>(true, actorTexSwitchers);
//////        Debug.Log(obj.name + "'s ActorTexSwitcher count:" + actorTexSwitchers.Count);


//////        for (int i = 0; i < actorTexSwitchers.Count; i++)
//////        {
//////            ActorTexSwitcher switcher = actorTexSwitchers[i];

//////            if (switcher.actorTexMsg.renderers == null || switcher.actorTexMsg.blueCompTexs.Length == 0 ||
//////     switcher.actorTexMsg.redCompTexs.Length == 0
//////     || switcher.actorTexMsg.blueCompTexs.Length !=
//////     switcher.actorTexMsg.redCompTexs.Length)//异常情况。                                                            
//////            {
//////                Debug.LogWarning("warning,ActorTexSwitcher:" + switcher.transform.GetHierarchy() + "  sprites not set");
//////                continue;
//////            }

            
//////            for (int j = 0; j < switcher.actorTexMsg.blueCompTexs.Length; j++)
//////            {

//////                string texName = switcher.actorTexMsg.blueCompTexs[j].name;
//////                //buildMap add element
//////                string texPath = AssetDatabase.GetAssetPath(switcher.actorTexMsg.blueCompTexs[j]);
//////                if (texPath.Contains("Assets/UI"))
//////                {
//////                    Debug.LogError("do not use tex in Assets/UI, path:" + switcher.transform.GetHierarchy());
//////                    continue;
//////                }

//////                AssetBundleBuild build = new AssetBundleBuild();
//////                string preFix = ABTypeUtil.GetPreFix(ABType.Texture);
//////                build.assetBundleName = preFix + texName.ToLower() + IOTools.abSuffix;
//////                build.assetNames = new string[] { texPath };
//////                buildMap.Add(build);
//////            }

//////            for (int j = 0; j < switcher.actorTexMsg.redCompTexs.Length; j++)
//////            {

//////                string texName = switcher.actorTexMsg.redCompTexs[j].name;
//////                //buildMap add element
//////                string texPath = AssetDatabase.GetAssetPath(switcher.actorTexMsg.redCompTexs[j]);
//////                if (texPath.Contains("Assets/UI"))
//////                {
//////                    Debug.LogError("do not use tex in Assets/UI, path:" + switcher.transform.GetHierarchy());
//////                    continue;
//////                }

//////                AssetBundleBuild build = new AssetBundleBuild();
//////                string preFix = ABTypeUtil.GetPreFix(ABType.Texture);
//////                build.assetBundleName = preFix + texName.ToLower() + IOTools.abSuffix;
//////                build.assetNames = new string[] { texPath };
//////                buildMap.Add(build);
//////            }

//////            //--------------------------->
//////            for (int j = 0; j < switcher.actorTexMsg.redCompStar1Texs.Length; j++)
//////            {

//////                string texName = switcher.actorTexMsg.redCompStar1Texs[j].name;
//////                //buildMap add element
//////                string texPath = AssetDatabase.GetAssetPath(switcher.actorTexMsg.redCompStar1Texs[j]);
//////                if (texPath.Contains("Assets/UI"))
//////                {
//////                    Debug.LogError("do not use tex in Assets/UI, path:" + switcher.transform.GetHierarchy());
//////                    continue;
//////                }

//////                AssetBundleBuild build = new AssetBundleBuild();
//////                string preFix = ABTypeUtil.GetPreFix(ABType.Texture);
//////                build.assetBundleName = preFix + texName.ToLower() + IOTools.abSuffix;
//////                build.assetNames = new string[] { texPath };
//////                buildMap.Add(build);
//////            }
//////            for (int j = 0; j < switcher.actorTexMsg.redCompStar2Texs.Length; j++)
//////            {

//////                string texName = switcher.actorTexMsg.redCompStar2Texs[j].name;
//////                //buildMap add element
//////                string texPath = AssetDatabase.GetAssetPath(switcher.actorTexMsg.redCompStar2Texs[j]);
//////                if (texPath.Contains("Assets/UI"))
//////                {
//////                    Debug.LogError("do not use tex in Assets/UI, path:" + switcher.transform.GetHierarchy());
//////                    continue;
//////                }

//////                AssetBundleBuild build = new AssetBundleBuild();
//////                string preFix = ABTypeUtil.GetPreFix(ABType.Texture);
//////                build.assetBundleName = preFix + texName.ToLower() + IOTools.abSuffix;
//////                build.assetNames = new string[] { texPath };
//////                buildMap.Add(build);
//////            }
//////            for (int j = 0; j < switcher.actorTexMsg.redCompStar3Texs.Length; j++)
//////            {

//////                string texName = switcher.actorTexMsg.redCompStar3Texs[j].name;
//////                //buildMap add element
//////                string texPath = AssetDatabase.GetAssetPath(switcher.actorTexMsg.redCompStar3Texs[j]);
//////                if (texPath.Contains("Assets/UI"))
//////                {
//////                    Debug.LogError("do not use tex in Assets/UI, path:" + switcher.transform.GetHierarchy());
//////                    continue;
//////                }

//////                AssetBundleBuild build = new AssetBundleBuild();
//////                string preFix = ABTypeUtil.GetPreFix(ABType.Texture);
//////                build.assetBundleName = preFix + texName.ToLower() + IOTools.abSuffix;
//////                build.assetNames = new string[] { texPath };
//////                buildMap.Add(build);
//////            }

//////            for (int j = 0; j < switcher.actorTexMsg.blueCompStar1Texs.Length; j++)
//////            {

//////                string texName = switcher.actorTexMsg.blueCompStar1Texs[j].name;
//////                //buildMap add element
//////                string texPath = AssetDatabase.GetAssetPath(switcher.actorTexMsg.blueCompStar1Texs[j]);
//////                if (texPath.Contains("Assets/UI"))
//////                {
//////                    Debug.LogError("do not use tex in Assets/UI, path:" + switcher.transform.GetHierarchy());
//////                    continue;
//////                }

//////                AssetBundleBuild build = new AssetBundleBuild();
//////                string preFix = ABTypeUtil.GetPreFix(ABType.Texture);
//////                build.assetBundleName = preFix + texName.ToLower() + IOTools.abSuffix;
//////                build.assetNames = new string[] { texPath };
//////                buildMap.Add(build);
//////            }
//////            for (int j = 0; j < switcher.actorTexMsg.blueCompStar2Texs.Length; j++)
//////            {

//////                string texName = switcher.actorTexMsg.blueCompStar2Texs[j].name;
//////                //buildMap add element
//////                string texPath = AssetDatabase.GetAssetPath(switcher.actorTexMsg.blueCompStar2Texs[j]);
//////                if (texPath.Contains("Assets/UI"))
//////                {
//////                    Debug.LogError("do not use tex in Assets/UI, path:" + switcher.transform.GetHierarchy());
//////                    continue;
//////                }

//////                AssetBundleBuild build = new AssetBundleBuild();
//////                string preFix = ABTypeUtil.GetPreFix(ABType.Texture);
//////                build.assetBundleName = preFix + texName.ToLower() + IOTools.abSuffix;
//////                build.assetNames = new string[] { texPath };
//////                buildMap.Add(build);
//////            }
//////            for (int j = 0; j < switcher.actorTexMsg.blueCompStar3Texs.Length; j++)
//////            {

//////                string texName = switcher.actorTexMsg.blueCompStar3Texs[j].name;
//////                //buildMap add element
//////                string texPath = AssetDatabase.GetAssetPath(switcher.actorTexMsg.blueCompStar3Texs[j]);
//////                if (texPath.Contains("Assets/UI"))
//////                {
//////                    Debug.LogError("do not use tex in Assets/UI, path:" + switcher.transform.GetHierarchy());
//////                    continue;
//////                }

//////                AssetBundleBuild build = new AssetBundleBuild();
//////                string preFix = ABTypeUtil.GetPreFix(ABType.Texture);
//////                build.assetBundleName = preFix + texName.ToLower() + IOTools.abSuffix;
//////                build.assetNames = new string[] { texPath };
//////                buildMap.Add(build);
//////            }
//////        }

//////        return buildMap;
//////    }
//////}
