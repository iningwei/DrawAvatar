using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.TimerTween
{
    class TimerManager : SingletonMonoBehaviour<TimerManager>
    {
        public Queue<Timer> cachedTimer = new Queue<Timer>();

        public List<Timer> timers = new List<Timer>();


        public Timer GetFreeTimer(out long id)
        {
            Timer timer = null;
            if (cachedTimer.Count > 0)
            {
                timer = cachedTimer.Dequeue();
            }
            else
            {
                timer = new Timer();
            }
            id = IdAssginer.GetID(IdAssginer.IdType.Timer);
            timer.SetId(id);//set new id
            return timer;
        }

        void recycleTimer(Timer timer)
        {
            timer.Recycle();
            this.cachedTimer.Enqueue(timer);
            //Debug.Log("cachedTimer count:" + cachedTimer.Count);
        }

        public void RegisterTimer(Timer timer)
        {
            this.timers.Add(timer);
        }

        //////public void CancelAllTimers()
        //////{
        //////    foreach (Timer timer in this.timers)
        //////    {
        //////        timer.CancelSelf();
        //////        this.recycleTimer(timer);
        //////    }
        //////    foreach (Timer timer in this.timersToAdd)
        //////    {
        //////        this.recycleTimer(timer);
        //////    }
        //////    this.timers = new List<Timer>();
        //////    this.timersToAdd = new List<Timer>();
        //////}

        //////public void PauseAllTimers()
        //////{
        //////    foreach (Timer timer in timers)
        //////    {
        //////        timer.Pause();
        //////    }
        //////}

        //////public void ResumeAllTimers()
        //////{
        //////    foreach (Timer timer in timers)
        //////    {
        //////        timer.Resume();
        //////    }
        //////}

        public void CancelTimer(Timer timer, long id)
        {
            bool flag = false;
            if (this.timers.Contains(timer) && timer.GetId() == id)
            {
                this.timers.Remove(timer);
                flag = true;
            }
            if (flag)
            {
                //timer.Cancel();
                this.recycleTimer(timer);
            }
        }

        public void CancelAndExeculteCompleteCallbackTimer(Timer timer, long id)
        {
            bool flag = false;
            if (this.timers.Contains(timer) && timer.GetId() == id)
            {
                this.timers.Remove(timer);
                flag = true;
            }
            if (flag)
            {
                //timer.CancelAndExecuteCompleteCallback();
                timer.ExecuteCompleteCallback();
                this.recycleTimer(timer);
            }
        }

        public Timer GetTimer(int id)
        {

            foreach (Timer timer in this.timers)
            {
                if (timer.GetId() == id)
                {
                    return timer;
                }
            }
            return null;
        }

        private void Update()
        {
            this.updateAllTimers();
        }
        private void FixedUpdate()
        {
            this.fixedUpdateTimers();
        }
        private void LateUpdate()
        {
            this.lateUpdateTimers();
        }

        int timerCount = 0;
        Timer tmpTimer;
        void updateAllTimers()
        {
            timerCount = timers.Count;
            if (timerCount > 0)
            {
                int i = timerCount - 1;
                try
                {
                    for (; i >= 0; i--)
                    {
                        if (i < timers.Count)
                        {
                            tmpTimer = this.timers[i];
                            if (tmpTimer != null)
                            {
                                tmpTimer.Update();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    Debug.LogError("error:" + ex.ToString() + ",timerCount: " + timerCount + ", cur i:" + i);
                }
            }
        }

        void fixedUpdateTimers()
        {
            if (timers.Count > 0)
            {
                try
                {
                    foreach (Timer timer in timers)
                    {
                        try
                        {
                            if (timer.IsNeedDoRealFixedUpdate)
                            {
                                timer.FixedUpdate();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("fixedUpdate timer ex:" + ex.ToString() + ", we remove it! please check,Id:" + timer.GetId() + ",  Tag:" + timer.Tag);
                            //////timer.CancelSelf();
                            //////this.recycleTimer(timer);
                        }
                    }
                }
                catch (Exception ex1)
                {
                    Debug.LogError("fixedUpdate timer ex1:" + ex1.ToString());
                }
            }
        }


        void lateUpdateTimers()
        {
            if (timers.Count > 0)
            {
                try
                {
                    foreach (Timer timer in timers)
                    {
                        try
                        {
                            if (timer.IsNeedDoRealLateUpdate)
                            {
                                timer.LateUpdate();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("lateUpdate timer ex:" + ex.ToString() + ", we remove it! please check,Id:" + timer.GetId() + ",  Tag:" + timer.Tag);
                            //////timer.CancelSelf();
                            //////this.recycleTimer(timer);
                        }
                    }
                }
                catch (Exception ex1)
                {
                    Debug.LogError("lateUpdate timer ex1:" + ex1.ToString());
                }
            }
        }
    }
}
