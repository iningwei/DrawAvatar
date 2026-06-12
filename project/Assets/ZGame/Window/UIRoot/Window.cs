using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ZGame.cc;
using ZGame.Ress;

namespace ZGame.Window
{
    public class Window : UIRoot
    {
        public string name;
        public string windowLayer;
        public bool neverClose = false;

        //是否独占(打开时是否隐藏对应层上的其它窗体)
        public bool isExclusive { get; protected set; } = true;



        public Window(GameObject obj, string windowName, string windowLayer, bool isExclusive, bool neverClose, params object[] paras) : base(obj, null, paras)
        { 
            this.name = windowName;
            this.windowLayer = windowLayer;
            this.isExclusive = isExclusive;
            this.neverClose = neverClose;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="datas"> </param>
        public override void Show(params object[] paras)
        {
            base.Show(paras);
            //Debug.Log("show window:" + name); 
            var rt = this.obj.GetComponent<RectTransform>();
            //rt.offsetMin = Vector2.zero;
            //rt.offsetMax = Vector2.zero;
            //rt.localPosition = Vector3.zero;


            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition3D = Vector3.zero;

            rt.localScale = Vector3.one;


            if (this.obj.GetComponent<WindowUniversalSafeAreaAdaptive>() == null)
            {
                this.obj.AddComponent<WindowUniversalSafeAreaAdaptive>();
            }
        }

        public virtual void HandleMessage(int msgId, params object[] paras)
        {
        }

        public void Close()
        {
            WindowManager.Instance.CloseWindow(this);
        }
        public override void Hide()
        {
            WindowManager.Instance.HideWindow(this);
        }

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}