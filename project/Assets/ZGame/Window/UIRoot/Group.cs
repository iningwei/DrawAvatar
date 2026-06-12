using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Window;

namespace ZGame.Window
{
    //Holder是动态布局，内部动态删减节点
    //Group用于静态布局，内部无动态删减子节点。
    //Group对应的obj上必须挂载继承自ComponentGroup的组件
    public class Group : UIRoot
    {

        public Group(GameObject obj, UIRoot parentUIRoot, params object[] paras) : base(obj, parentUIRoot, paras)
        {
        }
        public override void Init(params object[] paras)
        {
            base.Init(paras);
        }
    }
}