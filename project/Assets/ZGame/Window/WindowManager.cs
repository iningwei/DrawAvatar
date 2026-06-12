using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZGame.Ress;
using ZGame.Ress.AB;

namespace ZGame.Window
{
    public class WindowManager : SingletonMonoBehaviour<WindowManager>
    {
        Transform rootCanvasTran = null;
        Canvas rootCanvas = null;
        Camera rootUICamera = null;
        Canvas root3DCanvas = null;
        Dictionary<string, WindowInfo> _windowInfos = null;
        Dictionary<string, WindowInfo> windowInfos
        {
            get
            {
                if (_windowInfos == null)
                {
                    _windowInfos = new Dictionary<string, WindowInfo>();
                    WindowRegister.Register();
                }
                return _windowInfos;
            }
        }


        public Dictionary<string, Window> allWindowDic = new Dictionary<string, Window>();


        public void Init(Transform launcherNode)
        {
            if (rootCanvasTran == null)
            {
                rootCanvasTran = launcherNode.Find("Canvas").transform;
            }

            if (rootCanvas == null)
            {
                if (rootCanvasTran != null)
                {
                    rootCanvas = rootCanvasTran.GetComponent<Canvas>();
                }
            }
            if (root3DCanvas == null)
            {
                root3DCanvas = launcherNode.Find("Canvas3D")?.GetComponent<Canvas>();
            }
            if (rootUICamera == null)
            {
                rootUICamera = launcherNode.Find("UICamera").GetComponent<Camera>();
            }
        }



        List<KeyValuePair<string, Window>> allWindowList = new List<KeyValuePair<string, Window>>();
        private void Update()
        {
            if (allWindowList.Count != allWindowDic.Count)
            {
                allWindowList.Clear();
                allWindowList.AddRange(allWindowDic);
            }

            for (int i = allWindowList.Count - 1; i >= 0; i--)
            {
                if (allWindowList[i].Value != null)
                {
                    allWindowList[i].Value.Update(Time.deltaTime);
                }
            }
        }
        private void FixedUpdate()
        {
            if (allWindowDic.Count > 0)
            {
                foreach (var item in allWindowDic)
                {
                    item.Value.FixedUpdate();
                }
            }
        }

        private void LateUpdate()
        {
            if (allWindowDic.Count > 0)
            {
                foreach (var item in allWindowDic)
                {
                    item.Value.LateUpdate();
                }
            }
        }


        public void RegisterWindowType(string name, string scriptName, string resName)
        {
            windowInfos.TryGetValue(name, out WindowInfo info);
            if (info == null)
            {
                info = new WindowInfo(scriptName, resName);
                windowInfos[name] = info;
            }
            else
            {
                Debug.LogError("error,already registed C# WindowType:" + scriptName);
            }
        }


        public Window GetWindow(string windowName)
        {
            this.allWindowDic.TryGetValue(windowName, out Window target);
            return target;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        Window getExistWindow(string windowName, out WindowInfo info)
        {
            windowInfos.TryGetValue(windowName, out info);
            if (info == null)
            {
                Debug.LogError("error, " + windowName + " is not registed");
                return null;
            }

            if (allWindowDic.ContainsKey(windowName))
            {
                Debug.LogError("window: " + windowName + ", has already opened!");
                return allWindowDic[windowName];
            }

            return null;
        }

        public void SendWindowMessage(string windowName, int msgId, params object[] datas)
        {
            var window = GetWindow(windowName);
            if (window != null)
            {
                window.HandleMessage(msgId, datas);
            }
            else
            {
                Debug.LogWarning("sendWindowMsg to an unexist window:" + windowName);
            }
        }

        public void CloseAllWindow()//you can not close neverClose window by CloseAllWindow.But can close it by Close function
        {
            List<Window> windows = new List<Window>(this.allWindowDic.Values);
            for (int i = windows.Count - 1; i >= 0; i--)
            {
                if (windows[i].neverClose)
                {
                    continue;
                }
                CloseWindow(windows[i].name);
            }
        }


        public void ShowWindow(string name, string layerName, bool isExclusive, bool neverClose, bool sync, Action<GameObject> onWindowShowed, params object[] paras)
        {
            if (string.IsNullOrEmpty(layerName))
            {
                Debug.LogError("error, layerName is null");
            }

            WindowInfo info;
            Window window = getExistWindow(name, out info);
            if (window == null)
            {
                if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.AssetBundle)
                {
                    ABManager.Instance.LoadWindow(info.resName, (obj) =>
                    {
                        window = createWindow(name, obj as GameObject, layerName, isExclusive, neverClose, paras);
                        allWindowDic.Add(name, window);
                        showWindow(window, onWindowShowed);
                    },
                    sync
                     );
                }
                else if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Resources)
                {
                    var obj = ResourcesManager.Instance.LoadObj("Window/" + info.resName);
                    window = createWindow(name, obj, layerName, isExclusive, neverClose, paras);
                    allWindowDic.Add(name, window);
                    showWindow(window, onWindowShowed);
                }
                else if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Addressable)
                {
                    AAManager.Instance.LoadWindow(info.resName, (obj) =>
                    {
                        window = createWindow(name, obj as GameObject, layerName, isExclusive, neverClose, paras);
                        allWindowDic.Add(name, window);
                        showWindow(window, onWindowShowed);
                    }
                    );
                }
                else
                {
                    Debug.LogError("unsupported resLoadType:" + ConfigUtility.Data.ResLoadType);
                }
            }
            else
            {
                if (window.IsActiveSelf() == false)//处于隐藏层
                {
                    showWindow(window, onWindowShowed);
                }
                else
                {
                    Debug.LogError("window " + name + " already showed");
                }
            }
        }


        private void showWindow(Window window, Action<GameObject> onWindowShowed)
        {
            //set parent layer
            GameObject windowObj = window.obj;
            windowObj.transform.SetParent(WindowLayer.GetTran(window.windowLayer));

            window.Show();
            onWindowShowed?.Invoke(windowObj);
        }


        private Window createWindow(string windowName, GameObject uiObj, string layerName, bool isExclusive, bool neverClose, params object[] paras)
        {
            Type t = Type.GetType(windowName);
            Window target = Activator.CreateInstance(t, new object[] { uiObj, windowName, layerName, isExclusive, neverClose, paras }) as Window;
            return target;
        }


        public void CloseWindow(string windowName)
        {
            Window window = GetWindow(windowName);
            if (window != null)
            {
                CloseWindow(window);
            }
        }


        public void CloseWindow(Window window)
        {
            allWindowDic.Remove(window.name);
            window.Destroy();
        }


        public void HideWindow(Window window)
        {
            //set parent layer hidden
            GameObject windowObj = window.obj;
            windowObj.transform.SetParent(WindowLayer.GetTran(WindowLayer.Hidden));
            windowObj.SetActive(false);
        }


        public Window HideWindow(string windowName)
        {
            Window window = GetWindow(windowName);
            if (window != null)
            {
                this.HideWindow(window);
            }
            return window;
        }
        public bool IsWindowOpen(string windowName)
        {
            allWindowDic.TryGetValue(windowName, out Window target);
            if (target != null)
            {
                if (target.obj.activeSelf)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public Transform GetRootCanvasTran()
        {
            return this.rootCanvasTran;
        }
        public Canvas GetRootCanvas()
        {
            return this.rootCanvas;
        }
        public Canvas GetRoot3DCanvas()
        {
            return this.root3DCanvas;
        }

        public Camera GetRootUICamera()
        {
            return this.rootUICamera;
        }
    }
}