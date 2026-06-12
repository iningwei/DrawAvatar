using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress;
using ZGame.Window;

namespace ZGame.Window
{
    public class Node : UIRoot
    {
        public Action<Node> onDestroy;

        public Holder holder;
        Transform parentTran;

        public Node(GameObject obj, Holder holder, params object[] paras) : base(obj, holder, paras)
        { 
            this.holder = holder;
            this.parentTran = holder.obj.transform;
        }


        public override void Destroy()
        {
            base.Destroy();
            onDestroy?.Invoke(this);
        }
    }
}