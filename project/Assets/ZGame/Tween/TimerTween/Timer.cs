using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace ZGame.TimerTween
{
    public class TimerIdPair
    {
        public Timer timer;
        public long id;
        public TimerIdPair(Timer timer, long id)
        {
            this.timer = timer;
            this.id = id;
        }
    }



    [Serializable]
    public class Timer
    {
        private Action onComplete;
        private Action<float> onUpdate;
        private Action<float> onRealUpdate;
        private Action<float> onRealFixedUpdate;
        private Action<float> onRealLateUpdate;
        private bool isNeedDoRealFixedUpdate = false;
        private bool isNeedDoRealLateUpdate = false;

        private float? timeElapsedBeforePause;
        private float? timeElapsedBeforeCancel;
        private MonoBehaviour autoDestroyOwner;
        private bool hasAutoDestroyOwner;
        private float startTime;
        private float lastUpdateTime;
        public long loopedCount;//Already played count
        private Func<float, float> easeFunc = EaseTool.Get(Ease.Linear);



        private bool isOwnerDestroyed
        {
            get { return this.hasAutoDestroyOwner && this.autoDestroyOwner == null; }
        }


        public float Duration { get; private set; }

        public long Loop { get; private set; }

        public bool IsCompleted { get; private set; }

        public bool UseRealTime { get; private set; }
        //unique id,you can get timer or cancel timer by it  //TODO:改成private
        private long id = -1;


        //string tag, it is used for you to rectify timer
        [SerializeField, SetProperty("Tag")]
        private string tag = "";


        public ITimerBindedForMB bindedTarget;

        public string Tag
        {
            get
            {
                return tag;
            }
            private set
            {
                tag = value;
            }
        }

        public bool IsPaused
        {
            get { return this.timeElapsedBeforePause.HasValue; }
        }

        public bool IsCancelled
        {
            get { return this.timeElapsedBeforeCancel.HasValue; }
        }

        public bool IsDone
        {
            get { return this.IsCompleted || this.IsCancelled || this.isOwnerDestroyed; }
        }




        private float getWorldTime()
        {
            return this.UseRealTime ? Time.realtimeSinceStartup : Time.time;
        }
        private float getFireTime()
        {
            return this.startTime + this.Duration;
        }
        private float getTimeDelta()
        {
            return this.getWorldTime() - this.lastUpdateTime;
        }

        public float getTimeElapsed()
        {
            if (this.IsCompleted || this.getWorldTime() >= this.getFireTime())
            {
                return this.Duration;
            }
            return this.timeElapsedBeforeCancel ??
                this.timeElapsedBeforePause ??
                this.getWorldTime() - this.startTime;
        }


        public Timer()
        {

        }

        [Obsolete("Do not use this ctor, it can not set unique id,Now we use TimerManager to generate or reuse timer with unique id")]
        public Timer(float duration, Action onComplete = null, Action<float> onUpdate = null, int loop = 1, bool useRealTime = false, MonoBehaviour autoDestroyOwner = null)
        {
            this.Duration = duration;
            this.onComplete = onComplete;
            this.onUpdate = onUpdate;
            this.Loop = loop;
            loopedCount = 0;
            this.UseRealTime = useRealTime;
            this.autoDestroyOwner = autoDestroyOwner;
            this.hasAutoDestroyOwner = autoDestroyOwner != null;
        }



        bool startFlag = false;
        public void Start()
        {
            this.startTime = this.getWorldTime();
            this.lastUpdateTime = this.startTime;
            startFlag = true;
        }
        public void Start(out long refId)
        {
            refId = this.GetId();
            this.Start();
        }
        public void Recycle()
        {
            this.startFlag = false;
            this.onComplete = null;
            this.onUpdate = null;
            this.onRealUpdate = null;
            this.onRealFixedUpdate = null;
            this.onRealLateUpdate = null;

            this.Loop = 1;
            this.UseRealTime = false;
            this.autoDestroyOwner = null;
            this.hasAutoDestroyOwner = false;
            this.isNeedDoRealFixedUpdate = false;
            this.isNeedDoRealLateUpdate = false;

            this.SetId(-1);
            this.loopedCount = 0;
            if (String.IsNullOrEmpty(this.Tag) == false)
            {
                this.Tag = "";
            }
            this.timeElapsedBeforePause = null;
            this.timeElapsedBeforeCancel = null;
            this.IsCompleted = false;
        }

        public Timer SetOnComplete(Action onComplete)
        {
            if (this.onComplete != null)
            {
                Debug.LogWarning("Timer, id:" + this.GetId() + ", tag:" + (String.IsNullOrEmpty(this.tag) == false ? this.tag : "") + " , already has onComplete function,you will override it");
            }
            this.onComplete = onComplete;
            return this;
        }
        public Timer SetOnUpdate(Action<float> onUpdate)
        {
            if (this.onUpdate != null)
            {
                Debug.LogWarning("Timer already has onUpdate function,you will override it");
            }
            this.onUpdate = onUpdate;
            return this;
        }
        public Timer SetOnRealUpdate(Action<float> onRealUpdate)
        {
            if (this.onRealUpdate != null)
            {
                Debug.LogWarning("Timer already has onRealUpdate function,you will override it");
            }
            this.onRealUpdate = onRealUpdate;
            return this;
        }


        public Timer SetOnRealFixedUpdate(Action<float> onRealFixedUpdate)
        {
            if (this.onRealFixedUpdate != null)
            {
                Debug.LogWarning("Timer already has onRealFixedUpdate function,you will override it");
            }
            this.onRealFixedUpdate = onRealFixedUpdate;
            this.isNeedDoRealFixedUpdate = true;
            return this;
        }

        public Timer SetOnRealLateUpdate(Action<float> onRealLateUpdate)
        {
            if (this.onRealLateUpdate != null)
            {
                Debug.LogWarning("Timer already has onRealLateUpdate function,you will override it");
            }
            this.onRealLateUpdate = onRealLateUpdate;
            this.isNeedDoRealLateUpdate = true;
            return this;
        }


        public bool IsNeedDoRealFixedUpdate
        {
            get
            {
                return this.isNeedDoRealFixedUpdate;
            }
        }

        public bool IsNeedDoRealLateUpdate
        {
            get
            {
                return this.isNeedDoRealLateUpdate;
            }
        }
        /// <summary>
        /// </summary>
        /// <param name="loop">
        /// 0 means loop forever
        /// >0 means loop play specific times
        /// <0 is invalid input
        /// </param>
        /// <returns></returns>
        public Timer SetLoop(int loop)
        {
            if (loop < 0)
            {
                Debug.LogError("loop can not less than 0, we force set it to 1");
                loop = 1;
            }
            this.Loop = loop;
            return this;
        }
        public Timer SetDuration(float duration)
        {
            if (duration < 0)
            {
                Debug.LogError("can not set duration less than 0,we force set it to 0");
                duration = 0;
            }
            this.Duration = duration;
            return this;
        }

        public Timer SetUseRealTime(bool useRealTime)
        {
            this.UseRealTime = useRealTime;
            return this;
        }
        public Timer SetOwner(MonoBehaviour owner)
        {
            this.autoDestroyOwner = owner;
            this.hasAutoDestroyOwner = true;
            return this;
        }
        public Timer SetEase(Ease ease)
        {
            easeFunc = EaseTool.Get(ease);
            return this;
        }
        public Timer SetTag(string tag)
        {
            this.Tag = tag;
            return this;
        }
        public Timer SetId(long id)
        {
            this.id = id;
            return this;
        }
        public long GetId()
        {
            return this.id;
        }


        public void FixedUpdate()
        {
            if (this.startFlag == false)
            {
                return;
            }
            if (this.IsDone)
            {
                return;
            }
            if (this.IsCancelled)
            {
                return;
            }
            if (this.IsNeedDoRealFixedUpdate && this.onRealFixedUpdate != null)
            {
                this.onRealFixedUpdate(Time.fixedDeltaTime);
            }
        }

        public void LateUpdate()
        {
            if (this.startFlag == false)
            {
                return;
            }
            if (this.IsDone)
            {
                return;
            }
            if (this.IsCancelled)
            {
                return;
            }
            if (this.IsNeedDoRealLateUpdate && this.onRealLateUpdate != null)
            {
                this.onRealLateUpdate(Time.deltaTime);
            }
        }
        public void Update()
        {
            if (this.startFlag == false)
            {
                return;
            }
            if (this.IsDone)
            {
                return;
            }
            if (this.IsCancelled)
            {
                return;
            }
            if (this.IsPaused)
            {
                this.startTime += this.getTimeDelta();
                this.lastUpdateTime = this.getWorldTime();
                return;
            }
            if (this.onRealUpdate != null)
            {
                this.onRealUpdate(Time.deltaTime);
                return;
            }

            this.lastUpdateTime = this.getWorldTime();
            if (this.onUpdate != null)
            {
                this.onUpdate(this.easeFunc(this.getTimeElapsed() / this.Duration));
            }
            if (this.getWorldTime() >= this.getFireTime())
            {
                loopedCount++;
                if (Loop != 1)
                {
                    if (Loop == 0)
                    {
                        this.startTime = this.getWorldTime();
                    }
                    else
                    {
                        if (loopedCount < Loop)
                        {
                            this.startTime = this.getWorldTime();
                        }
                        else
                        {
                            this.IsCompleted = true;
                        }
                    }
                }
                else
                {
                    this.IsCompleted = true;
                }

                this.onComplete?.Invoke();
            }
        }



        public void ExecuteCompleteCallback()
        {
            this.onComplete?.Invoke();
        }

        [Obsolete("Do not use it!use TimerTween.CancelAndExeculteCompleteCallbackTimer")]
        public void CancelAndExecuteCompleteCallback()
        {
            this.onComplete?.Invoke();
            this.Cancel();
        }

        [Obsolete("Do not use it!use TimerTween.Cancel")]
        public void Cancel()
        {
            if (this.IsDone)
            {
                return;
            }
            this.timeElapsedBeforeCancel = this.getTimeElapsed();
            this.timeElapsedBeforePause = null;
        }
        public void Pause()
        {
            if (this.IsPaused || this.IsDone)
            {
                return;
            }
            this.timeElapsedBeforePause = this.getTimeElapsed();
        }

        public void Resume()
        {
            if (!this.IsPaused || this.IsDone)
            {
                return;
            }
            this.timeElapsedBeforePause = null;
        }

        public float GetTimeRemaining()
        {
            return this.Duration - this.getTimeElapsed();
        }

        public float GetRatioComplete()
        {
            return this.getTimeElapsed() / this.Duration;
        }
        public float GetRatioRemaining()
        {
            return this.GetTimeRemaining() / this.Duration;
        }


    }
}
