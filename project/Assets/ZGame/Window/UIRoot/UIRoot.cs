using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ZGame;
using ZGame.Event;
using ZGame.Ress;

namespace ZGame.Window
{
    public class UIRoot
    {
        public long id;
        public GameObject obj;
        public UIRoot parentUIRoot;
        public UIRoot(GameObject obj, UIRoot parentUIRoot, params object[] paras)
        {
            this.id = IdAssginer.GetID(IdAssginer.IdType.UIRoot);
            this.obj = obj;
            this.parentUIRoot = parentUIRoot;
            AutoLinkUI(this);
            Init(paras);
            AddEventListener();
            this.FillTextContent();
            if (parentUIRoot != null)
            {
                parentUIRoot.AddSubUIRoot(this);
            }
        }

        Dictionary<long, UIRoot> subUIRoots = new Dictionary<long, UIRoot>();
        public void AddSubUIRoot(UIRoot uiRoot)
        {
            if (subUIRoots.ContainsKey(uiRoot.id) == false)
            {
                subUIRoots.Add(uiRoot.id, uiRoot);
            }
        }
        public void RemoveSubUIRoot(UIRoot uiRoot)
        {
            if (subUIRoots.ContainsKey(uiRoot.id))
            {
                subUIRoots.Remove(uiRoot.id);
                uiRoot.Destroy();
            }
        }

        public void AutoLinkUI(UIRoot uiRoot)
        {
            if (this.obj.GetComponent<UIRootTag>() == null)
            {
                Debug.LogError(this.obj.transform.GetHierarchy() + " have no UIRootTag attached");
                return;
            }
            Type trueType = uiRoot.GetType();

            FieldInfo[] fields = trueType.GetFields();//get all public fields
            foreach (var field in fields)
            {
                if (!field.Name.StartsWith("ui_"))
                {
                    continue;
                }
                if (field.FieldType == typeof(GameObject))
                {
                    Component tmpCom = this.obj.transform.DFS<Transform>(
                        field.Name.Replace("ui_", ""),
                        (tran) =>
                    {
                        if (tran != this.obj.transform && tran.GetComponent<UIRootTag>())
                        {
                            return false;
                        }
                        return true;
                    });

                    if (tmpCom != null)
                    {
                        field.SetValue(uiRoot, tmpCom.gameObject);
                    }
                    else
                    {
                        Debug.LogError("can not find:" + field.Name + " in " + uiRoot.obj.name);
                    }
                }
                else
                {
                    Component tmpCom = this.obj.transform.DFS(field.FieldType,
                       field.Name.Replace("ui_", ""),
                       (tran) =>
                       {
                           if (tran != this.obj.transform && tran.GetComponent<UIRootTag>())
                           {
                               return false;
                           }
                           return true;
                       });

                    if (tmpCom != null)
                    {
                        field.SetValue(uiRoot, tmpCom);
                    }
                    else
                    {
                        Debug.LogError("can not find:" + field.Name + " in " + uiRoot.obj.name);
                    }
                }
            }
        }


        public virtual void Update(float dt)
        {
            foreach (var sub in subUIRoots)
            {
                try
                {
                    if (sub.Value.obj.activeInHierarchy)
                    {
                        sub.Value.Update(dt);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("type:" + sub.Value.GetType().ToString() + ", error:" + ex.ToString());
                }
            }
        }

        public virtual void FixedUpdate()
        {
            foreach (var sub in subUIRoots)
            {
                if (sub.Value.obj.activeInHierarchy)
                {
                    sub.Value.FixedUpdate();
                }

            }
        }
        public virtual void LateUpdate()
        {
            foreach (var sub in subUIRoots)
            {
                if (sub.Value.obj.activeInHierarchy)
                {
                    sub.Value.LateUpdate();
                }
            }
        }
        public virtual void Init(params object[] paras)
        {

        }
        public virtual void Show(params object[] paras)
        {
            this.obj.SetActive(true);
        }
        public virtual void Hide()
        {
            this.obj.SetActive(false);
        }

        public virtual void AddEventListener()
        {
            EventDispatcher.Instance.AddListener(EventID.OnLanguageCodeChange, this.onLanguageCodeChange);
        }

        private void onLanguageCodeChange(string evtId, object[] paras)
        {
            this.FillTextContent();
        }
        public virtual void FillTextContent()
        {

        }

        public virtual void RemoveEventListener()
        {
            EventDispatcher.Instance.RemoveListener(EventID.OnLanguageCodeChange, this.onLanguageCodeChange);
        }
        public virtual void Destroy()
        {
            foreach (var sub in subUIRoots)
            {
                sub.Value.Destroy();
            }
            subUIRoots.Clear();


            RemoveEventListener();
            if (this is Window)
            {
                if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.AssetBundle)
                {
                    GameObjectHelper.DestroyImmediate(this.obj);
                }
                else
                {
                    GameObject.DestroyImmediate(this.obj);
                }
            }
        }

        public bool IsActiveSelf()
        {
            return this.obj.activeSelf;
        }
    }
}
