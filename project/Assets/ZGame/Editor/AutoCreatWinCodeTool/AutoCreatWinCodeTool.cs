using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ZGame.RessEditor
{
    public class SurfixComponentData
    {
        public string surfix;
        public string componentName;
        public int level;//级别
        public SurfixComponentData(string surfix, string comName, int level)
        {
            this.surfix = surfix;
            this.componentName = comName;
            this.level = level;
        }
    }
    public class AutoCreatWinCodeTool : EditorWindow
    {

        UnityEngine.GameObject windowPrefabObj;









        [MenuItem("工具/强制编译")]
        static void ForcedToCompile()
        {
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }

        [MenuItem("工具/自动创建窗体脚本并绑定常用逻辑")]
        static void PrefabChanger()
        {
            EditorWindow.GetWindow(typeof(AutoCreatWinCodeTool));
        }
        private void OnGUI()
        {
            windowPrefabObj = EditorGUILayout.ObjectField("窗口预制体：", windowPrefabObj, typeof(UnityEngine.GameObject), false) as GameObject;
            if (windowPrefabObj == null)
                return;
            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(windowPrefabObj))
            {
                Debug.LogError("该物体不是预制件,请选择预制件;");
                return;
            }
            if (!windowPrefabObj.name.EndsWith("Window"))
            {
                Debug.LogError("名称应以Window结束");
                return;
            }

            if (GUILayout.Button("Do"))
            {
                string windowName = windowPrefabObj.name;


                Debug.LogError("TODO");
            }
        }



        private string retriveParentRootTag(Transform child, GameObject rootObj)
        {
            while (child.parent != null)
            {
                if (child.parent.GetComponent<UIRootTag>() != null)
                {
                    return child.parent.name;
                }
                else
                {
                    child = child.parent;
                    if (child == rootObj.transform)
                    {
                        return "";
                    }
                }
            }
            return "";
        }




        //////private void outputNodeCode(string nodeName)
        //////{
        //////    Debug.Log("begin output NodeCode:" + nodeName);

        //////    string codeStr = "";
        //////    codeStr += "using TMPro;\r\n";
        //////    codeStr += "using UnityEngine;\r\n";
        //////    codeStr += "using UnityEngine.UI;\r\n";
        //////    codeStr += "using ZGame.Window;\r\n";
        //////    codeStr += "using ZGame.Event;\r\n";


        //////    codeStr += "public class " + nodeName + " : Node\r\n";
        //////    codeStr += "{\r\n";

        //////    //属性
        //////    List<Transform> usedTrans = uiRootRefDic[nodeName];
        //////    for (int i = 0; i < usedTrans.Count; i++)
        //////    {
        //////        Transform tmpTran = usedTrans[i];
        //////        if (extractFieldStr(tmpTran, out string fieldStr))
        //////        {
        //////            codeStr += fieldStr;
        //////        }
        //////    }

        //////    //构造函数
        //////    codeStr += "    public " + nodeName + "(GameObject obj, Holder holder, params object[] paras) : base(obj, holder, paras)\r\n";
        //////    codeStr += "    {\r\n";
        //////    codeStr += "    }\r\n";
        //////    //Init
        //////    codeStr += $"    public override void Init(params object[] paras)\r\n";
        //////    codeStr += "    {\r\n";
        //////    codeStr += $"        base.Init(paras);\r\n";
        //////    codeStr += "    }\r\n";

        //////    //Show
        //////    codeStr += "     public override void Show(params object[] paras)\r\n";
        //////    codeStr += "    {\r\n";
        //////    codeStr += "        base.Show(paras);\r\n";
        //////    codeStr += "    }\r\n";

        //////    //AddEventListener
        //////    codeStr += "     public override void AddEventListener()\r\n";
        //////    codeStr += "    {\r\n";
        //////    codeStr += "        base.AddEventListener();\r\n";
        //////    for (int i = 0; i < usedTrans.Count; i++)
        //////    {
        //////        Transform tmpTran = usedTrans[i];

        //////        if (tmpTran.name.EndsWith("Btn"))
        //////        {
        //////            codeStr += $"        this.ui_{tmpTran.name}.onClick.AddListener(this.on{tmpTran.name}Clicked);\r\n";
        //////        }
        //////    }
        //////    codeStr += "    }\r\n";

        //////    //按钮点击事件
        //////    for (int i = 0; i < usedTrans.Count; i++)
        //////    {
        //////        Transform tmpTran = usedTrans[i];

        //////        if (tmpTran.name.EndsWith("Btn"))
        //////        {
        //////            codeStr += $"    private void on{tmpTran.name}Clicked()\r\n";
        //////            codeStr += "    {\r\n";
        //////            codeStr += "    }\r\n";
        //////        }
        //////    }

        //////    //RemoveEventListener
        //////    codeStr += "     public override void RemoveEventListener()\r\n";
        //////    codeStr += "    {\r\n";
        //////    codeStr += "        base.RemoveEventListener();\r\n";
        //////    for (int i = 0; i < usedTrans.Count; i++)
        //////    {
        //////        Transform tmpTran = usedTrans[i];

        //////        if (tmpTran.name.EndsWith("Btn"))
        //////        {
        //////            codeStr += $"        this.ui_{tmpTran.name}.onClick.RemoveAllListeners();\r\n";
        //////        }
        //////    }
        //////    codeStr += "    }\r\n";


        //////    //大括号
        //////    codeStr += "}";

        //////    //打印脚本内容
        //////    Debug.Log("脚本:" + nodeName + ".cs, 内容:" + codeStr);
        //////    IOTools.WriteString(saveFolder + $"/{nodeName}.cs", codeStr);
        //////}

        //////private void outputHolderCode(string holderName)
        //////{
        //////    string codeStr = "";
        //////    codeStr += "using TMPro;\r\n";
        //////    codeStr += "using UnityEngine;\r\n";
        //////    codeStr += "using UnityEngine.UI;\r\n";
        //////    codeStr += "using ZGame.Window;\r\n";
        //////    codeStr += "using ZGame.Event;\r\n";
        //////    codeStr += "using System.Collections.Generic;\r\n";

        //////    codeStr += "public class " + holderName + " : Holder\r\n";
        //////    codeStr += "{\r\n";

        //////    //属性
        //////    List<Transform> usedTrans = uiRootRefDic[holderName];
        //////    for (int i = 0; i < usedTrans.Count; i++)
        //////    {
        //////        Transform tmpTran = usedTrans[i];
        //////        if (extractFieldStr(tmpTran, out string fieldStr))
        //////        {
        //////            codeStr += fieldStr;
        //////        }
        //////    }

        //////    //构造函数
        //////    codeStr += "    public " + holderName + "(GameObject holderObj, Window window, List<GameObject> nodeItemObjs, params object[] paras) : base(holderObj, window, nodeItemObjs, paras)\r\n";
        //////    codeStr += "    {\r\n";
        //////    codeStr += "    }\r\n";
        //////    //Init
        //////    codeStr += $"    public override void Init(params object[] paras)\r\n";
        //////    codeStr += "    {\r\n";
        //////    codeStr += $"        base.Init(paras);\r\n";
        //////    codeStr += "    }\r\n";

        //////    //Show
        //////    codeStr += "     public override void Show(params object[] paras)\r\n";
        //////    codeStr += "    {\r\n";
        //////    codeStr += "        base.Show(paras);\r\n";
        //////    codeStr += "    }\r\n";
        //////    //FillItems
        //////    codeStr += "     public override void FillItems()\r\n";
        //////    codeStr += "    {\r\n";
        //////    codeStr += "        base.FillItems();\r\n";
        //////    codeStr += "        //TODO:\r\n";
        //////    codeStr += "    }\r\n";



        //////    //AddEventListener
        //////    codeStr += "     public override void AddEventListener()\r\n";
        //////    codeStr += "    {\r\n";
        //////    codeStr += "        base.AddEventListener();\r\n";
        //////    for (int i = 0; i < usedTrans.Count; i++)
        //////    {
        //////        Transform tmpTran = usedTrans[i];

        //////        if (tmpTran.name.EndsWith("Btn"))
        //////        {
        //////            codeStr += $"        this.ui_{tmpTran.name}.onClick.AddListener(this.on{tmpTran.name}Clicked);\r\n";
        //////        }
        //////    }
        //////    codeStr += "    }\r\n";

        //////    //按钮点击事件
        //////    for (int i = 0; i < usedTrans.Count; i++)
        //////    {
        //////        Transform tmpTran = usedTrans[i];

        //////        if (tmpTran.name.EndsWith("Btn"))
        //////        {
        //////            codeStr += $"    private void on{tmpTran.name}Clicked()\r\n";
        //////            codeStr += "    {\r\n";
        //////            codeStr += "    }\r\n";
        //////        }
        //////    }

        //////    //RemoveEventListener
        //////    codeStr += "     public override void RemoveEventListener()\r\n";
        //////    codeStr += "    {\r\n";
        //////    codeStr += "        base.RemoveEventListener();\r\n";
        //////    for (int i = 0; i < usedTrans.Count; i++)
        //////    {
        //////        Transform tmpTran = usedTrans[i];

        //////        if (tmpTran.name.EndsWith("Btn"))
        //////        {
        //////            codeStr += $"        this.ui_{tmpTran.name}.onClick.RemoveAllListeners();\r\n";
        //////        }
        //////    }
        //////    codeStr += "    }\r\n";


        //////    //大括号
        //////    codeStr += "}";

        //////    //打印脚本内容
        //////    Debug.Log("脚本:" + holderName + ".cs, 内容:" + codeStr);
        //////    IOTools.WriteString(saveFolder + $"/{holderName}.cs", codeStr);
        //////}

        //////private void outputAreaCode(string areaName)
        //////{
        //////    string codeStr = "";
        //////    codeStr += "using TMPro;\r\n";
        //////    codeStr += "using UnityEngine;\r\n";
        //////    codeStr += "using UnityEngine.UI;\r\n";
        //////    codeStr += "using ZGame.Window;\r\n";
        //////    codeStr += "using ZGame.Event;\r\n";

        //////    codeStr += "public class " + areaName + " : Area\r\n";
        //////    codeStr += "{\r\n";

        //////    //属性
        //////    List<Transform> usedTrans = uiRootRefDic[areaName];
        //////    for (int i = 0; i < usedTrans.Count; i++)
        //////    {
        //////        Transform tmpTran = usedTrans[i];

        //////        if (extractFieldStr(tmpTran, out string fieldStr))
        //////        {
        //////            codeStr += fieldStr;
        //////        }
        //////    }


        //////    //holders 
        //////    foreach (var item in holders)
        //////    {
        //////        string[] keySepes = item.Key.ToString().Split(keySeperateStr);
        //////        if (keySepes[0] == areaName)
        //////        {
        //////            codeStr += $"    {keySepes[1]} {keySepes[1].FirstCharToLower()};\r\n";
        //////        }
        //////    }

        //////    //构造函数
        //////    codeStr += "    public " + areaName + "(GameObject obj, Window window, bool initVisible, params object[] paras) : base(obj, window, initVisible, paras)\r\n";
        //////    codeStr += "    {\r\n";
        //////    codeStr += "    }\r\n";
        //////    //Init
        //////    codeStr += $"    public override void Init(params object[] paras)\r\n";
        //////    codeStr += "    {\r\n";
        //////    codeStr += $"        base.Init(paras);\r\n";
        //////    foreach (var item in holders)
        //////    {
        //////        string[] keySepes = item.Key.ToString().Split(keySeperateStr);
        //////        if (keySepes[0] == areaName)
        //////        {
        //////            //////var nodeNames = item.Value.GetComponent<HolderTag>().nodeTypeObjKVs;
        //////            string nodeItemObjsParaStr = "new List<GameObject>() {";
        //////            //////for (int i = 0; i < nodeNames.Count; i++)
        //////            //////{
        //////            //////    nodeItemObjsParaStr += " ui_" + nodeNames[i] + ".gameObject,";
        //////            //////}
        //////            nodeItemObjsParaStr = nodeItemObjsParaStr.TrimEnd(',') + " }";


        //////            codeStr += $"        {keySepes[1].FirstCharToLower()} = new {keySepes[1]}(ui_{keySepes[1]}.gameObject, this.parentWindow,  {nodeItemObjsParaStr});\r\n";
        //////        }
        //////    }
        //////    codeStr += "    }\r\n";

        //////    //Show
        //////    codeStr += "     public override void Show(params object[] paras)\r\n";
        //////    codeStr += "    {\r\n";
        //////    codeStr += "        base.Show(paras);\r\n";
        //////    codeStr += "    }\r\n";
        //////    //AddEventListener
        //////    codeStr += "     public override void AddEventListener()\r\n";
        //////    codeStr += "    {\r\n";
        //////    codeStr += "        base.AddEventListener();\r\n";
        //////    for (int i = 0; i < usedTrans.Count; i++)
        //////    {
        //////        Transform tmpTran = usedTrans[i];

        //////        if (tmpTran.name.EndsWith("Btn"))
        //////        {
        //////            codeStr += $"        this.ui_{tmpTran.name}.onClick.AddListener(this.on{tmpTran.name}Clicked);\r\n";
        //////        }
        //////    }
        //////    codeStr += "    }\r\n";

        //////    //按钮点击事件
        //////    for (int i = 0; i < usedTrans.Count; i++)
        //////    {
        //////        Transform tmpTran = usedTrans[i];

        //////        if (tmpTran.name.EndsWith("Btn"))
        //////        {
        //////            codeStr += $"    private void on{tmpTran.name}Clicked()\r\n";
        //////            codeStr += "    {\r\n";
        //////            codeStr += "    }\r\n";
        //////        }
        //////    }

        //////    //RemoveEventListener
        //////    codeStr += "     public override void RemoveEventListener()\r\n";
        //////    codeStr += "    {\r\n";
        //////    codeStr += "        base.RemoveEventListener();\r\n";
        //////    for (int i = 0; i < usedTrans.Count; i++)
        //////    {
        //////        Transform tmpTran = usedTrans[i];

        //////        if (tmpTran.name.EndsWith("Btn"))
        //////        {
        //////            codeStr += $"        this.ui_{tmpTran.name}.onClick.RemoveAllListeners();\r\n";
        //////        }
        //////    }
        //////    codeStr += "    }\r\n";


        //////    //大括号
        //////    codeStr += "}";

        //////    //打印脚本内容
        //////    Debug.Log("脚本:" + areaName + ".cs, 内容:" + codeStr);
        //////    IOTools.WriteString(saveFolder + $"/{areaName}.cs", codeStr);
        //////}





    }
}