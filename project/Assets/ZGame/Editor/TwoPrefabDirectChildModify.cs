using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TwoPrefabDirectChildModify : EditorWindow
{
    private GameObject referenceA;      // 参照物体A
    private GameObject targetB;         // 待修改物体B
    private string childNamesInput = "attackPoint_0,hitPoint_0,hudPoint,attackEffectPoint_0"; // 输入的子节点名称，用逗号分隔

    [MenuItem("Tools/双物体子节点同步工具 (TwoPrefabDirectChildModify)")]
    public static void ShowWindow()
    {
        GetWindow<TwoPrefabDirectChildModify>("子节点同步工具").Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("双物体直接子节点同步工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 拖拽参照物体A
        referenceA = EditorGUILayout.ObjectField("参照物体 A (源)", referenceA, typeof(GameObject), true) as GameObject;

        // 拖拽待修改物体B
        targetB = EditorGUILayout.ObjectField("待修改物体 B (目标)", targetB, typeof(GameObject), true) as GameObject;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("需要同步的直接子节点名称（用英文逗号分隔）", EditorStyles.boldLabel);
        childNamesInput = EditorGUILayout.TextArea(childNamesInput, GUILayout.Height(60));

        EditorGUILayout.HelpBox("示例：LeftWing, RightWing, Head, Tail\n只处理直接子节点（第一层）", MessageType.Info);

        EditorGUILayout.Space();
        if (GUILayout.Button("执行同步", GUILayout.Height(40)))
        {
            ExecuteSync();
        }
    }

    private void ExecuteSync()
    {
        if (referenceA == null || targetB == null)
        {
            EditorUtility.DisplayDialog("错误", "请同时指定参照物体A和待修改物体B！", "确定");
            return;
        }

        if (string.IsNullOrWhiteSpace(childNamesInput))
        {
            EditorUtility.DisplayDialog("错误", "请输入至少一个子节点名称！", "确定");
            return;
        }

        // 处理输入的名称列表
        string[] names = childNamesInput.Split(',');
        List<string> nameList = new List<string>();

        foreach (var n in names)
        {
            string trim = n.Trim();
            if (!string.IsNullOrEmpty(trim))
                nameList.Add(trim);
        }

        if (nameList.Count == 0)
        {
            EditorUtility.DisplayDialog("错误", "请输入有效的子节点名称！", "确定");
            return;
        }

        int successCount = 0;
        int createCount = 0;
        int errorCount = 0;

        Undo.RecordObject(targetB, "Sync Direct Children Transform");

        foreach (string childName in nameList)
        {
            // 在A中查找直接子节点
            Transform childInA = referenceA.transform.Find(childName);
            if (childInA == null)
            {
                Debug.LogError($"【错误】参照物体A中不存在直接子节点：{childName}，请先在A中补充该节点。");
                errorCount++;
                continue;
            }

            // 在B中查找直接子节点
            Transform childInB = targetB.transform.Find(childName);

            if (childInB != null)
            {
                // 情况1：都存在 → 同步Transform
                childInB.localPosition = childInA.localPosition;
                childInB.localRotation = childInA.localRotation;
                childInB.localScale = childInA.localScale;

                successCount++;
                Debug.Log($"已同步子节点：{childName}");
            }
            else
            {
                // 情况2：A有，B没有 → 在B中创建
                GameObject newChild = new GameObject(childName);
                newChild.transform.SetParent(targetB.transform, false);

                newChild.transform.localPosition = childInA.localPosition;
                newChild.transform.localRotation = childInA.localRotation;
                newChild.transform.localScale = childInA.localScale;

                createCount++;
                Debug.Log($"已在B中创建子节点：{childName} 并同步Transform");
            }
        }

        EditorUtility.SetDirty(targetB);

        // 完成提示
        string message = $"同步完成！\n" +
                        $"成功同步：{successCount} 个\n" +
                        $"新建子节点：{createCount} 个\n" +
                        $"错误（A中缺失）：{errorCount} 个";

        EditorUtility.DisplayDialog("完成", message, "确定");
    }
}