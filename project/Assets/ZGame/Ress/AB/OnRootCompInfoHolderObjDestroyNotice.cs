using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using ZGame.Event;
using ZGame.Ress.AB.Holder;

namespace ZGame.Ress.AB
{
    public class OnRootCompInfoHolderObjDestroyNotice : MonoBehaviour
    {
        bool noticed = false;

  

        private void OnDestroy()//If a object never actived, the OnDestroy will not call while call Object.Destroy or Scene changed.If an object once actived , even it is not active while destorying,this OnDestroy method will still be called. ref:https://www.jianshu.com/p/0d6878b0ef66
        {
            this.Notice();
        }


        public void Notice()
        {
            if (noticed)
            {
                return;
            }
            noticed = true;
            if (this.GetComponent<RootCompInfoHolder>() == null)
            {
                Debug.LogError(this.transform.GetHierarchy() + " has no RootCompInfoHolder please check!");
            }
            else
            {
                EventDispatcher.Instance.DispatchEvent(EventID.OnRootCompInfoHolderObjDestroy, this.gameObject);

                //获得所有子物体，并通知
                GameObjectHelper.NoticeAllChildTranWillBeDestroy(this.gameObject);
            }

        }
    }
}
