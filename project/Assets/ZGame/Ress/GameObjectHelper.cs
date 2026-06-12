
using System;
using UnityEngine;
using ZGame.Event;
using ZGame.Ress.AB;
using ZGame.Ress.AB.Holder;
using ZGame.TimerTween;

namespace ZGame.Ress
{
    public class GameObjectHelper
    {
        public static GameObject Instantiate(GameObject target)
        {
            if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.AssetBundle)
            {
                if (target.GetComponent<DynamicCompInfoHolder>() == null)
                {
                    Debug.LogError("The instantiate target have no component DynamicCompInfoHolder attached:" + target.GetHierarchy());
                    return null;
                }
                GameObject resultObj = GameObject.Instantiate(target);

                EventDispatcher.Instance.DispatchEvent(EventID.OnDynamicCompInfoHolderObjInstantiate, resultObj);

                return resultObj;
            }
            else
            {
                GameObject resultObj = GameObject.Instantiate(target);
                return resultObj;
            }
        }


        public static void Destroy(GameObject target, float time)
        {
            if (target == null)
            {
                return;
            }
            if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.AssetBundle)
            {
                ZGame.TimerTween.TimerTween.Delay(time, () =>
                {
                    if (target)
                    {
                        if (target.GetComponent<RootCompInfoHolder>() != null)
                        {
                            EventDispatcher.Instance.DispatchEvent(EventID.OnRootCompInfoHolderObjDestroy, target);
                        }

                        //通知所有物体要删除咯
                        NoticeAllChildTranWillBeDestroy(target);
                        UnityEngine.Object.Destroy(target);
                    }
                }).Start();
            }
            else
            {
                UnityEngine.Object.Destroy(target, time);
            }
        }



        public static void DestroyImmediate(GameObject target)
        {
            if (target == null)
            {
                return;
            }
            if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.AssetBundle)
            {
                if (target.GetComponent<RootCompInfoHolder>() != null)
                {
                    EventDispatcher.Instance.DispatchEvent(EventID.OnRootCompInfoHolderObjDestroy, target);
                }

                NoticeAllChildTranWillBeDestroy(target);
                UnityEngine.Object.DestroyImmediate(target);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(target);
            }

        }
        public static void NoticeAllChildTranWillBeDestroy(GameObject target)
        {
            var childs = target.GetComponentsInChildren<Transform>(true);

            try
            {
                for (int i = 0; i < childs.Length; i++)
                {
                    EventDispatcher.Instance.DispatchEvent(EventID.OnCompInfoHolderChildObjDestroy, childs[i].gameObject);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("error, ex:" + ex.ToString());
            }

        }
    }
}