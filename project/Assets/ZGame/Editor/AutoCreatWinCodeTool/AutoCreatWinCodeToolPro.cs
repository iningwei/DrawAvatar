using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System;
using ZGame.RessEditor;

public class AutoCreatWinCodeToolPro : EditorWindow
{
    private GameObject targetWindowPrefab;
    private Vector2 scrollPos;
    private string hierarchyOutput = ""; // Store the text output

    string windowName;
    [MenuItem("Tools/AutoCreatWinCodeToolPro")]
    public static void ShowWindow()
    {
        GetWindow<AutoCreatWinCodeToolPro>("Window Code Tool Pro");
    }

    private void OnGUI()
    {
        // 拖拽区域
        GUILayout.Label("Drag and Drop Window Prefab:", EditorStyles.boldLabel);
        var dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drop Window Prefab Here");
        HandleDragAndDrop(dropArea);

        if (targetWindowPrefab != null)
        {
            GUILayout.Label($"Selected Prefab: {targetWindowPrefab.name}", EditorStyles.boldLabel);
            windowName = targetWindowPrefab.name;
            if (windowName.EndsWith("Window"))
            {
                if (GUILayout.Button("Analyze and Build Hierarchy"))
                {
                    BuildHierarchyText();
                }

                // 显示文本输出
                if (!string.IsNullOrEmpty(hierarchyOutput))
                {
                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                    GUILayout.Label(hierarchyOutput, EditorStyles.wordWrappedLabel);
                    EditorGUILayout.EndScrollView();

                    if (GUILayout.Button("Output code!"))
                    {
                        outputCode();
                    }
                }
            }
        }
    }

    private void HandleDragAndDrop(Rect dropArea)
    {
        var evt = Event.current;
        if (evt.type == EventType.DragUpdated && dropArea.Contains(evt.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }
        else if (evt.type == EventType.DragPerform && dropArea.Contains(evt.mousePosition))
        {
            DragAndDrop.AcceptDrag();
            foreach (var obj in DragAndDrop.objectReferences)
            {
                if (obj is GameObject go)
                {
                    targetWindowPrefab = go;
                    hierarchyOutput = ""; // Reset output when new prefab is dropped
                    break;
                }
            }
        }
    }

    private void BuildHierarchyText()
    {
        if (targetWindowPrefab == null) return;

        var rootUIRoot = targetWindowPrefab.GetComponent<UIRootTag>();
        if (rootUIRoot == null)
        {
            Debug.LogWarning("Selected prefab does not have UIRootTag component!");
            hierarchyOutput = "Error: Selected prefab does not have UIRootTag component!";
            return;
        }

        StringBuilder output = new StringBuilder();
        output.AppendLine($"{targetWindowPrefab.name} (Window)");
        BuildHierarchyRecursive(rootUIRoot, output, 1);
        hierarchyOutput = output.ToString();

        Debug.LogError("hierarchyOutput as follows(single click to see detail):");
        Debug.LogError(hierarchyOutput);
    }

    private void BuildHierarchyRecursive(UIRootTag uiRoot, StringBuilder output, int depth)
    {
        if (uiRoot == null || uiRoot.directChildUIRoots == null) return;

        UIRootTag childUIRoot;
        for (int x = 0; x < uiRoot.directChildUIRoots.Count; x++)
        {
            childUIRoot = uiRoot.directChildUIRoots[x];
            if (childUIRoot == null) continue;

            string nodeType = GetTranType(childUIRoot.gameObject);
            string indent = extractIndent(depth);
            output.AppendLine($"{indent}{childUIRoot.gameObject.name} ({nodeType})");

            // 如果是 Holder，添加 NodeTypeObjPair 信息
            var holderTag = childUIRoot.GetComponent<HolderTag>();
            if (holderTag != null && holderTag.nodeTypeObjPairs != null)
            {
                foreach (var pair in holderTag.nodeTypeObjPairs)
                {
                    if (pair != null && pair.obj != null)
                    {
                        string indentNode = extractIndent(depth + 1);
                        output.AppendLine($"{indentNode}{pair.obj.name} (Node)");
                        UIRootTag nodeUIRootTag = pair.obj.GetComponent<UIRootTag>();
                        BuildHierarchyRecursive(nodeUIRootTag, output, depth + 1 + 1);
                    }
                }
            }

            BuildHierarchyRecursive(childUIRoot, output, depth + 1);
        }
    }

    private string extractIndent(int depth)
    {
        string indent = "|";
        for (int i = 0; i < depth; i++)
        {
            if (i == depth - 1)
            {
                if (depth == 1)
                {
                    indent = indent + "── ";
                }
                else
                {
                    indent = indent + "|── ";
                }
            }
            else
            {
                indent = indent + "        ";
            }
        }
        return indent;
    }

    private string GetTranType(GameObject go)
    {
        if (go.name.EndsWith("Window")) return "Window";
        if (go.name.EndsWith("Area")) return "Area";
        if (go.name.EndsWith("Holder")) return "Holder";
        if (go.name.EndsWith("Node")) return "Node";
        return "Unknown";
    }




    #region 代码输出
    string saveFolder;
    Dictionary<string, List<Transform>> uiRootRefDic = new Dictionary<string, List<Transform>>();

    Dictionary<string, SurfixComponentData> surfixComponentMap = new Dictionary<string, SurfixComponentData>() {
            {"Txt",new SurfixComponentData("Txt","TextMeshProUGUI" ,1) } ,
            {"Btn",new SurfixComponentData("Btn","Button"  ,1) } ,
            {"Input",new SurfixComponentData("Input","TMP_InputField"  ,1) } ,
            {"Img",new SurfixComponentData("Img","Image" ,1)   } ,
            {"RawImg",new SurfixComponentData("RawImg","RawImage" ,2)   } ,
            {"Tran",new SurfixComponentData("Tran","Transform",1 )  } ,
            {"Obj",new SurfixComponentData("Obj","GameObject" ,1) } ,
            {"Area",new SurfixComponentData("Area","Transform",1 )  } ,
            {"Node",new SurfixComponentData("Node","Transform" ,1 )  } ,
            {"Holder",new SurfixComponentData("Holder","Transform" ,1)  } ,
        };
    void outputCode()
    {
        saveFolder = Application.dataPath + "/../WindowAutoScripts/" + windowName;
        uiRootRefDic.Clear();
        var rootTags = targetWindowPrefab.GetComponentsInChildren<UIRootTag>(true);
        for (int i = 0; i < rootTags.Length; i++)
        {
            this.processUIRootTag(rootTags[i].transform);
        }
        //////打印
        foreach (var item in uiRootRefDic)
        {
            Debug.Log("----->" + item.Key);
            for (int i = 0; i < item.Value.Count; i++)
            {
                Debug.Log(item.Value[i].name);
            }
        }
        for (int i = 0; i < rootTags.Length; i++)
        {
            string classType = this.GetTranType(rootTags[i].gameObject);
            this.outputUIRootCode(rootTags[i].gameObject, classType);
        }
    }

    private void outputUIRootCode(GameObject obj, string classType)
    {
        string objName = obj.name;

        string codeStr = "";
        codeStr += "using TMPro;\r\n";
        codeStr += "using UnityEngine;\r\n";
        codeStr += "using UnityEngine.UI;\r\n";
        codeStr += "using ZGame.Window;\r\n";
        codeStr += "using ZGame.Event;\r\n";
        codeStr += "using System.Collections.Generic;\r\n";

        codeStr += $"public class {objName} : {classType}\r\n";
        codeStr += "{\r\n";

        //属性
        List<Transform> usedTrans = uiRootRefDic[objName];
        for (int i = 0; i < usedTrans.Count; i++)
        {
            Transform tmpTran = usedTrans[i];
            if (extractFieldStr(tmpTran, out string fieldStr))
            {
                codeStr += fieldStr;
            }
        }

        //DirectChildUIRoots
        var childUIRoots = obj.GetComponent<UIRootTag>().directChildUIRoots;
        foreach (var item in childUIRoots)
        {
            codeStr += $"    {item.name} {item.name.FirstCharToLower()};\r\n";
        }

        //ctor
        if (classType == "Window")
        {
            codeStr += "    public " + objName + "(GameObject obj, string windowName, string layerName, bool isExclusive, bool neverClose, params object[] paras) : base(obj, windowName, layerName, isExclusive, neverClose, paras)\r\n";
        }
        else if (classType == "Area")
        {
            codeStr += "    public " + objName + "(GameObject obj, UIRoot parentUIRoot, params object[] paras) : base(obj, parentUIRoot, paras)\r\n";
        }
        else if (classType == "Holder")
        {
            codeStr += "    public " + objName + "(GameObject holderObj, UIRoot parentUIRoot, List<KeyValuePair<System.Type, GameObject>> nodeTypeObjKVList, params object[] paras) : base(holderObj, parentUIRoot, nodeTypeObjKVList, paras)\r\n";
        }
        else if (classType == "Node")
        {
            codeStr += "    public " + objName + "(GameObject obj, Holder holder, params object[] paras) : base(obj, holder, paras)\r\n";
        }
        codeStr += "    {\r\n";
        codeStr += "    }\r\n";
        //Init
        codeStr += $"    public override void Init(params object[] paras)\r\n";
        codeStr += "    {\r\n";
        codeStr += $"        base.Init(paras);\r\n";
        foreach (var item in childUIRoots)
        {
            if (item.name.EndsWith("Area"))
            {
                codeStr += $"        {item.name.FirstCharToLower()} = new {item.name}(this.ui_{item.name}.gameObject, this);\r\n";
            }
            else if (item.name.EndsWith("Holder"))
            {
                var holderTag = item.GetComponent<HolderTag>();
                var pairs = holderTag.nodeTypeObjPairs;
                string nodeTypeObjKVsStr = "new List<KeyValuePair<System.Type, GameObject>>() {";
                for (int i = 0; i < pairs.Count; i++)
                {
                    nodeTypeObjKVsStr += $" new KeyValuePair<System.Type, GameObject>(typeof({pairs[i].typeName}), this.ui_{pairs[i].typeName}.gameObject),";
                }
                nodeTypeObjKVsStr = nodeTypeObjKVsStr.TrimEnd(',') + " }";

                codeStr += $"        {item.name.FirstCharToLower()} = new {item.name}(this.ui_{item.name}.gameObject, this, {nodeTypeObjKVsStr});\r\n";
            }
        }

        codeStr += "    }\r\n";

        //Show
        codeStr += "    public override void Show(params object[] paras)\r\n";
        codeStr += "    {\r\n";
        codeStr += "        base.Show(paras);\r\n";
        codeStr += "    }\r\n";
        //AddEventListener
        codeStr += "    public override void AddEventListener()\r\n";
        codeStr += "    {\r\n";
        codeStr += "        base.AddEventListener();\r\n";
        for (int i = 0; i < usedTrans.Count; i++)
        {
            Transform tmpTran = usedTrans[i];

            if (tmpTran.name.EndsWith("Btn"))
            {
                codeStr += $"        this.ui_{tmpTran.name}.onClick.AddListener(this.on{tmpTran.name}Clicked);\r\n";
            }
        }
        codeStr += "    }\r\n";

        //按钮点击事件
        for (int i = 0; i < usedTrans.Count; i++)
        {
            Transform tmpTran = usedTrans[i];

            if (tmpTran.name.EndsWith("Btn"))
            {
                codeStr += $"    private void on{tmpTran.name}Clicked()\r\n";
                codeStr += "    {\r\n";
                codeStr += "    }\r\n";
            }
        }

        //RemoveEventListener
        codeStr += "    public override void RemoveEventListener()\r\n";
        codeStr += "    {\r\n";
        codeStr += "        base.RemoveEventListener();\r\n";
        for (int i = 0; i < usedTrans.Count; i++)
        {
            Transform tmpTran = usedTrans[i];

            if (tmpTran.name.EndsWith("Btn"))
            {
                codeStr += $"        this.ui_{tmpTran.name}.onClick.RemoveAllListeners();\r\n";
            }
        }
        codeStr += "    }\r\n";
        //大括号
        codeStr += "}";

        //打印脚本内容
        Debug.Log("脚本:" + objName + ".cs, 内容:" + codeStr);
        IOTools.WriteString(saveFolder + $"/{objName}.cs", codeStr);
    }
    bool extractFieldStr(Transform tmpTran, out string fieldStr)
    {
        fieldStr = "";
        List<SurfixComponentData> datas = new List<SurfixComponentData>();
        foreach (var item in surfixComponentMap)
        {
            if (tmpTran.name.EndsWith(item.Key))
            {
                datas.Add(item.Value);
            }
        }

        if (datas.Count > 0)
        {
            SurfixComponentData targetData = datas[0];
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i].level > targetData.level)
                {
                    targetData = datas[i];
                }
            }
            fieldStr = $"    public {targetData.componentName} ui_" + tmpTran.name + ";\r\n";
            return true;
        }

        return false;
    }
    void processUIRootTag(Transform rootTagTran)
    {
        Debug.Log($"processUIRootTag ---------------> {rootTagTran.name}");
        uiRootRefDic[rootTagTran.name] = new List<Transform>();


        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(rootTagTran.transform);

        while (queue.Count > 0)
        {
            Transform current = queue.Dequeue();

            if (isNameSuit(current) && current != rootTagTran)
            {
                uiRootRefDic[rootTagTran.name].Add(current);
                Debug.Log(rootTagTran.name + " add:" + current.name);
            }

            if (current == rootTagTran || current.GetComponent<UIRootTag>() == null)
            {
                for (int i = 0; i < current.childCount; i++)
                {
                    Transform child = current.GetChild(i);

                    queue.Enqueue(child);
                }
            }
        }

        //排序
        if (uiRootRefDic[rootTagTran.name].Count > 0)
        {
            var list = uiRootRefDic[rootTagTran.name];
            list.Sort((a, b) =>
            {
                bool aIsSpecial = a.name.EndsWith("Node") || a.name.EndsWith("Area") || a.name.EndsWith("Holder");
                bool bIsSpecial = b.name.EndsWith("Node") || b.name.EndsWith("Area") || b.name.EndsWith("Holder");

                if (aIsSpecial && !bIsSpecial) return 1; // a goes after b
                if (!aIsSpecial && bIsSpecial) return -1; // a goes before b
                return 0; // maintain original order
            });
        }

    }
    bool isNameSuit(Transform trans)
    {
        if (trans.name.EndsWith("Txt") ||
      trans.name.EndsWith("Btn") ||
      trans.name.EndsWith("Input") ||
      trans.name.EndsWith("Img") ||
      trans.name.EndsWith("Tran") ||
      trans.name.EndsWith("Obj") ||
      (trans.name.EndsWith("Area") && trans.name != "Text Area") ||//去除InputField下的子物体Text Area的干扰
      trans.name.EndsWith("Node") ||
      trans.name.EndsWith("Holder"))
        {
            return true;
        }
        return false;
    }

    #endregion
}

