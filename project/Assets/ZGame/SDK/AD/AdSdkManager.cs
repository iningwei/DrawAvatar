using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame;

namespace ZGame.SDK
{
    public class AdSdkManager
    {
        static AdSdkBase instance = null;

        public static AdSdkBase Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject adSdkObj = new GameObject();
                    adSdkObj.name = "adSdkObj";

#if UNITY_EDITOR
                    instance = adSdkObj.AddComponent<EditorAdSdkManager>();
#else
#if !SDK
                       
                        instance = adSdkObj.AddComponent<EditorAdSdkManager>();
#else
#if UNITY_ANDROID
                    DebugExt.Log("use ironSource ad sdk--->");
                     //TODO:这里千万不要instance=new IronSourceAdSdkManager()
                                        instance = adSdkObj.AddComponent<IronSourceAdSdkManager>(); 
#elif UNITY_IOS
                                   instance = adSdkObj.AddComponent<EditorAdSdkManager>();     
#endif
#endif
#endif
                }


                return instance;
            }

        }
    }
}