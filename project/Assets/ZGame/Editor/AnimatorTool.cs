using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AnimatorTool : Editor
{
    [MenuItem("GameObject/AnimatorTool/获得当前状态")]
    public static void GetCurAnimatorState()
    {
        int idleId = Animator.StringToHash("Idle");
        int walkId = Animator.StringToHash("Walk");
        int runId = Animator.StringToHash("Run");
        int jumpId = Animator.StringToHash("Jump");

        GameObject root = Selection.activeObject as GameObject;
        var animator = root.GetComponent<Animator>();
        if (animator != null)
        {
            //var clipInfos = animator.GetCurrentAnimatorClipInfo(0);//获得当前层所有的clip片段

            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            var hashCode = stateInfo.shortNameHash;
            if (hashCode == idleId)
            {
                Debug.Log("cur animator clip is idle");
            }
            else if (hashCode == walkId)
            {
                Debug.Log("cur animator clip is walk");
            }
            else if (hashCode == runId)
            {
                Debug.Log("cur animator clip is run");
            }
            else if (hashCode == jumpId)
            {
                Debug.Log("cur animator clip is jump");
            }
            else
            {
                Debug.Log("cur animator clip unknown!");
            }
        }
    }
}
