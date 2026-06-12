using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Event;

public class PreviewData
{
    public string uid;
    public long stamp;//精确到秒
    public PreviewData(string uid, long stamp)
    {
        this.uid = uid;
        this.stamp = stamp;
    }
}
public class QueryPreviewManager : SingletonMonoBehaviour<QueryPreviewManager>
{
    Stack<PreviewData> queryStack = new Stack<PreviewData>();

    string curPreviewUid;
    public void AddPreviewData(string uid)
    {
        if (queryStack.Count > 0)
        {
            if (queryStack.Peek().uid == uid || curPreviewUid == uid)
            {
                return;
            }
        }

        queryStack.Push(new PreviewData(uid, TimeTool.GetNowSecondStamp()));
    }

    float checkDuration = 0f;
    private void Update()
    {
        checkDuration -= Time.deltaTime;
        if (checkDuration < 0f)
        {
            var result = this.tryDoPreview();
            if (result)
            {
                this.checkDuration = 3.5f;
            }
        }
    }

    private bool tryDoPreview()
    {
        if (queryStack.Count > 0)
        {
            var data = queryStack.Pop();
            if (TimeTool.GetNowSecondStamp() - data.stamp > 30)//超时
            {
                queryStack.Clear();
                this.curPreviewUid = "";
                return false;
            }
            else
            {
                this.curPreviewUid = data.uid;
                if (GameLogic.Instance.daddyManager.GetDaddy(this.curPreviewUid) == null)//有可能各种原因，已不在
                {
                    this.curPreviewUid = "";
                    return false;
                }
                else
                {
                    EventDispatcher.Instance.DispatchEvent(EventID.OnSelfPetQuery, data.uid);
                    return true;
                }
            }
        }
        else
        {
            this.curPreviewUid = "";
            return false;
        }
    }
}
