using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Animations;

public class HierarchyLayoutExtention : EditorWindow
{
    string sequenceChildNamePrefix = "";
    int sequenceChildStartIndex = 0;
    GameObject sequenceChildParent = null;

    GameObject arrangeChildParent = null;
    Axis targetAxis = Axis.X;
    float arrangeGap = 10f;

    GameObject s3_arrangeChildParent = null;
    int s3_columnCount = 3;
    float s3_xGap;
    float s3_yGap;


    [MenuItem("工具/界面排版布局", false, 13)]
    static void HierarchyLayout()
    {
        HierarchyLayoutExtention visualizeTool = EditorWindow.GetWindow(typeof(HierarchyLayoutExtention)) as HierarchyLayoutExtention;
    }


    private void OnGUI()
    {
        //有序重命名子物体
        GUILayout.Label("有序重命名子物体", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("父物体:");
        sequenceChildParent = EditorGUILayout.ObjectField(sequenceChildParent, typeof(GameObject), true) as GameObject;
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("子物体前缀:");
        sequenceChildNamePrefix = EditorGUILayout.TextField(sequenceChildNamePrefix);
        GUILayout.Label("起始索引编号:");
        sequenceChildStartIndex = EditorGUILayout.IntField(sequenceChildStartIndex);
        if (GUILayout.Button("重命名"))
        {
            var childCount = sequenceChildParent.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                sequenceChildParent.transform.GetChild(i).name = sequenceChildNamePrefix + (i + sequenceChildStartIndex).ToString();
            }

            Debug.Log("重命名完毕！");
        }
        GUILayout.EndHorizontal();



        //子物体以第1个物体为依准，z方向上按照正反方向依次等间距排列
        GUILayout.Space(20f);
        GUILayout.Label("以第1个子物体为准，z方向上按照正反方向依次等间距排列子物体", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("父物体:");
        arrangeChildParent = EditorGUILayout.ObjectField(arrangeChildParent, typeof(GameObject), true) as GameObject;
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("目标方向:");
        targetAxis = (UnityEngine.Animations.Axis)EditorGUILayout.EnumPopup(targetAxis);
        GUILayout.Label("间距：");
        arrangeGap = EditorGUILayout.FloatField(arrangeGap);

        if (GUILayout.Button("排列"))
        {
            var childs = (arrangeChildParent as GameObject).GetDirectChilds();
            if (childs != null && childs.Count > 1)
            {
                Transform refTran = childs[0].transform;
                Vector3 originPos = refTran.localPosition;
                for (int i = 1; i < childs.Count; i++)
                {
                    if (targetAxis == Axis.Z)
                    {
                        if (i % 2 == 1)
                        {
                            childs[i].transform.SetPosZAccordRefTran(refTran, originPos.z + (i / 2 + 1) * arrangeGap, Space.Self);
                        }
                        else
                        {
                            childs[i].transform.SetPosZAccordRefTran(refTran, originPos.z - (i / 2) * arrangeGap, Space.Self);
                        }
                    }
                    else if (targetAxis == Axis.X)
                    {
                        if (i % 2 == 1)
                        {
                            childs[i].transform.SetPosXAccordRefTran(refTran, originPos.x + (i / 2 + 1) * arrangeGap, Space.Self);
                        }
                        else
                        {
                            childs[i].transform.SetPosXAccordRefTran(refTran, originPos.x - (i / 2) * arrangeGap, Space.Self);
                        }
                    }
                    else if (targetAxis == Axis.Y)
                    {
                        if (i % 2 == 1)
                        {
                            childs[i].transform.SetPosYAccordRefTran(refTran, originPos.y + (i / 2 + 1) * arrangeGap, Space.Self);
                        }
                        else
                        {
                            childs[i].transform.SetPosYAccordRefTran(refTran, originPos.y - (i / 2) * arrangeGap, Space.Self);
                        }
                    }

                }
            }

            Debug.Log("排列完毕！");
        }
        GUILayout.EndHorizontal();




        //子物体以第1个物体为依准，以x方向为准，并向右；y方向向下。按照一定间距排列
        GUILayout.Space(20f);
        GUILayout.Label("子物体以第1个物体为依准，以x方向为准，并向右；y方向向下。按照一定间距排列", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("父物体:");
        s3_arrangeChildParent = EditorGUILayout.ObjectField(s3_arrangeChildParent, typeof(GameObject), true) as GameObject;
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("列数:");
        s3_columnCount = EditorGUILayout.IntField(s3_columnCount);
        GUILayout.Label("x间距：");
        s3_xGap = EditorGUILayout.FloatField(s3_xGap);
        GUILayout.Label("y间距：");
        s3_yGap = EditorGUILayout.FloatField(s3_yGap);

        if (GUILayout.Button("排列"))
        {

            Transform refTran = s3_arrangeChildParent.transform.GetChild(0);
            Vector3 originPos = refTran.localPosition;
            var childCount = s3_arrangeChildParent.transform.childCount;

            for (int i = 1; i < childCount; i++)
            {
                float xPos = originPos.x + i % s3_columnCount * s3_xGap;
                float yPos = originPos.y - ((int)(i / s3_columnCount)) * s3_yGap;
                s3_arrangeChildParent.transform.GetChild(i).transform.SetPosXY(xPos, yPos, Space.Self);
            }


            Debug.Log("排列完毕！");
        }
        GUILayout.EndHorizontal();
    }
}
