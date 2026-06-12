using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class QuotaCoroutine : MonoBehaviour
{
    // 每帧的额度时间，全局共享
    static float frameQuotaSec = 0.001f;

    static LinkedList<IEnumerator> s_tasks = new LinkedList<IEnumerator>();

    // Use this for initialization
    void Start()
    {
        StartQuotaCoroutine(Task(1, 100));
    }

    // Update is called once per frame
    void Update()
    {
        ScheduleTask();
    }

    void StartQuotaCoroutine(IEnumerator task)
    {
        s_tasks.AddLast(task);
    }

    static void ScheduleTask()
    {
        float timeStart = Time.realtimeSinceStartup;
        while (s_tasks.Count > 0)
        {
            var t = s_tasks.First.Value;
            bool taskFinish = false;
            while (Time.realtimeSinceStartup - timeStart < frameQuotaSec)
            {
                // 执行任务的一步, 后续没步骤就是任务完成
                Profiler.BeginSample(string.Format("QuotaTaskStep, f:{0}", Time.frameCount));
                taskFinish = !t.MoveNext();
                Profiler.EndSample();

                if (taskFinish)
                {
                    s_tasks.RemoveFirst();
                    break;
                }
            }

            // 任务没结束执行到这里就是没时间额度了
            if (!taskFinish)
                return;
        }
    }

    IEnumerator Task(int taskId, int stepCount)
    {
        int i = 0;
        while (i < stepCount)
        {
            Debug.LogFormat("taskId:{0},step:{1}, frame:{2}", taskId, i, Time.frameCount);
            i++;
            yield return null;
        }
    }
}