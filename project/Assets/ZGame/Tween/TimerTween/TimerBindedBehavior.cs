using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.TimerTween;


//绑定到MonoHabavior目标上的Timer，当目标消失，需要回收Timer
public class TimerBindedBehavior : MonoBehaviour
{
    Timer timer;
    public long timerId;

    public void BindTimer(Timer timer, long timerId)
    {
        this.timer = timer;
        this.timerId = timerId;
    }
    public void FreeTimer()
    {
        TimerTween.Cancel(this.timer, this.timerId);
    }
}
