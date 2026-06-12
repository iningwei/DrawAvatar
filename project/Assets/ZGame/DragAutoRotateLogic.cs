using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.TimerTween;

public class DragAutoRotateLogic : MonoBehaviour
{
    List<AutoRotate> autoRotates = new List<AutoRotate>();
    Drag dragListerner;

    bool flag = false;
    public void Init(AutoRotate autoRotate, Drag dragListerner)
    {
        this.autoRotates.Add(autoRotate);
        this.dragListerner = dragListerner;

        this.dragListerner.onDragBegin += this.onDragBegin;
        this.dragListerner.onDrag += this.onDrag;
        this.dragListerner.onDragEnd += this.onDragEnd;
    }
    public void Add(AutoRotate autoRotate)
    {
        this.autoRotates.Add(autoRotate);
    }
    private void onDragEnd()
    {
        this.triggerRotate();
        flag = false;
    }

    Timer triggerRotateTimer;
    long triggerRotateId;
    private void triggerRotate()
    {
        TimerTween.Cancel(this.triggerRotateTimer, triggerRotateId);

        this.triggerRotateTimer = TimerTween.Delay(3, () =>
        {
            for (int i = 0; i < autoRotates.Count; i++)
            {
                autoRotates[i].StartRotate();
            }
        });
        this.triggerRotateTimer.Start(out triggerRotateId);
    }

    private void onDrag(Vector2 delta)
    {
        if (flag)
        {
            for (int i = 0; i < autoRotates.Count; i++)
            {
                autoRotates[i].transform.RotateAround(this.autoRotates[i].transform.position, Vector3.up, -delta.x * Time.deltaTime * 20);
            }
        }
    }

    private void onDragBegin()
    {
        for (int i = 0; i < autoRotates.Count; i++)
        {
            autoRotates[i].StopRotate();
        }
        TimerTween.Cancel(this.triggerRotateTimer, triggerRotateId);
        flag = true;
    }

    private void OnDestroy()
    {
        TimerTween.Cancel(this.triggerRotateTimer, triggerRotateId);
        this.dragListerner.onDragBegin = null;
        this.dragListerner.onDrag = null;
        this.dragListerner.onDragEnd = null;
    }
}
