using System;

namespace ZGame.TimerTween
{
    public class TimerTween
    {
        public static Timer ValueByInterval(float from, float to, float duration, float interval, Action<float> updateCallback, Action complteCallback)
        {
            float lastTime = 0;
            float gap = to - from;
            float offset = gap / duration;
            int tickedCount = 0;
            Timer timer = TimerManager.Instance.GetFreeTimer(out long id);
            timer.SetDuration(duration).SetOnComplete(() =>
            {
                //UnityEngine.Debug.Log("tickedCount:" + tickedCount);
                lastTime = 0;
                tickedCount = 0;
                if (complteCallback != null)
                {
                    complteCallback();
                }
                Cancel(timer, id);
            }).SetOnUpdate((t) =>
            {
                //UnityEngine.Debug.Log("  t:" + t);
                if (Math.Abs(t - lastTime) >= interval)
                {
                    tickedCount++;
                    lastTime = t;
                    float v = from + t * duration * offset;
                    //UnityEngine.Debug.Log("v:" + v + ", t:" + t + ", duration:" + duration + ", offset:" + offset + ", from:" + from);
                    if (updateCallback != null)
                    {
                        updateCallback(v);
                    }
                }
            }).SetLoop(1);

            TimerManager.Instance.RegisterTimer(timer);
            return timer;
        }

        public static Timer Value(float from, float to, float duration, Action<float> updateCallback, Action complteCallback)
        {
            float gap = to - from;
            float offset = gap / duration;
            int tickedCount = 0;
            //t由于要进行ease变换，故限制了范围为[0,1]
            Timer timer = TimerManager.Instance.GetFreeTimer(out long id);
            timer.SetDuration(duration).SetOnComplete(() =>
            {
                tickedCount = 0;
                if (complteCallback != null)
                {
                    complteCallback();
                }
                Cancel(timer, id);
            }).SetOnUpdate((t) =>
            {
                tickedCount++;
                float v = from + t * duration * offset;
                //UnityEngine.Debug.Log("v:" + v + ", t:" + t + ", duration:" + duration + ", offset:" + offset + ", from:" + from);
                if (updateCallback != null)
                {
                    updateCallback(v);
                }
            }).SetLoop(1);
            TimerManager.Instance.RegisterTimer(timer);
            return timer;
        }

        public static Timer Delay(float delayTime, Action call)
        {
            Timer timer = TimerManager.Instance.GetFreeTimer(out long id);

            timer.SetDuration(delayTime).SetOnComplete(() =>
            {
                call?.Invoke();
                Cancel(timer, id);
            }).SetLoop(1);

            TimerManager.Instance.RegisterTimer(timer);

            return timer;
        }

        public static Timer Repeat(float interval, Func<bool> repeatCallback, bool callOnAwake)
        {
            Timer timer = TimerManager.Instance.GetFreeTimer(out long id);
            timer.SetDuration(interval).SetLoop(0).SetOnComplete(() =>
            {
                if (repeatCallback != null)
                {
                    bool r = repeatCallback();
                    if (!r)
                    {
                        Cancel(timer, id);
                    }
                }
            });
            TimerManager.Instance.RegisterTimer(timer);

            if (callOnAwake)
            {
                if (repeatCallback != null)
                {
                    bool r = repeatCallback();
                    if (!r)
                    {
                        Cancel(timer, id);
                    }
                }
            }

            return timer;
        }

        public static Timer RealUpdate(Action<float> realUpdateCallback)
        {
            Timer timer = TimerManager.Instance.GetFreeTimer(out long id);
            timer.SetOnRealUpdate(realUpdateCallback);

            TimerManager.Instance.RegisterTimer(timer);
            return timer;
        }

        public static Timer RealFixedUpdate(Action<float> realFixedUpdateCallback)
        {
            Timer timer = TimerManager.Instance.GetFreeTimer(out long id);
            timer.SetOnRealFixedUpdate(realFixedUpdateCallback).SetLoop(0);

            TimerManager.Instance.RegisterTimer(timer);
            return timer;
        }

        public static Timer RealLateUpdate(Action<float> realLateUpdateCallback)
        {
            Timer timer = TimerManager.Instance.GetFreeTimer(out long id);
            timer.SetOnRealLateUpdate(realLateUpdateCallback).SetLoop(0);

            TimerManager.Instance.RegisterTimer(timer);
            return timer;
        }

        public static Timer RepeatCount(float interval, int count, Func<bool> repeatCallback, bool callOnawake)
        {
            if (count < 0)
            {
                UnityEngine.Debug.LogError("error, count<0");
                return null;
            }


            Timer timer = TimerManager.Instance.GetFreeTimer(out long id);
            timer.SetDuration(interval);
            timer.SetLoop(count);
            timer.SetOnComplete(() =>
            {
                if (repeatCallback != null)
                {
                    bool r = repeatCallback();
                    if (!r)
                    {
                        Cancel(timer, id);
                    }
                }
            });

            TimerManager.Instance.RegisterTimer(timer);
            if (callOnawake)
            {
                count--;
                if (repeatCallback != null)
                {
                    bool r = repeatCallback();
                    if (!r)
                    {
                        Cancel(timer, id);
                    }
                }
            }

            return timer;
        }

        public static void Cancel(Timer timer, long id)
        {
            if (timer != null)
            {
                TimerManager.Instance.CancelTimer(timer, id);
            }
        }

        public static void CancelAndExeculteCompleteCallbackTimer(Timer timer, long id)
        {
            if (timer != null)
            {
                TimerManager.Instance.CancelAndExeculteCompleteCallbackTimer(timer, id);
            }
        }

        public static void Pause(Timer timer)
        {
            timer?.Pause();
        }

        public static void Resume(Timer timer)
        {
            timer?.Resume();
        }
    }
}
