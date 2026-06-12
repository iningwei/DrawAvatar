using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.FSM
{
    public class FSMState
    {
        //to specify whehter this state is finished.
        public bool IsStateFinished = true;


        /// <summary>
        /// key为TransitionType，value为StateType
        /// </summary>
        Dictionary<int, int> map = new Dictionary<int, int>();

        public int StateType
        {
            get;
            set;
        }

        public FSMState()
        {
            InitTransitions();
        }

        public virtual void InitTransitions()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trans">TransitionType</param>
        /// <param name="state">StateType</param>
        public void AddTransition(int trans, int state)
        {
            if (trans == TransitionType.None)
            {
                DebugExt.LogE("error, not allow add TransitionType None");
                return;
            }
            if (state == ZGame.FSM.StateType.None)
            {
                DebugExt.LogE("error, not allow add StateType None");
                return;
            }
            if (this.map.ContainsKey(trans))
            {
                DebugExt.LogE("error,already exist transition:" + trans);
                return;
            }
            this.map.Add(trans, state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trans">TransitionType</param>
        public void DeleteTransition(int trans)
        {
            if (trans == TransitionType.None)
            {
                DebugExt.LogE("DeleteTransition None is not allowed");
                return;
            }

            if (this.map.ContainsKey(trans))
            {
                this.map.Remove(trans);
                return;
            }

            DebugExt.LogE($"error,Transition:{trans.ToString()} not exist");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="trans">TransitionType</param>
        /// <returns>StateType</returns>
        public int GetOutputState(int trans)
        {
            if (this.map.ContainsKey(trans))
            {
                return this.map[trans];
            }
            DebugExt.Log($"error,Transition:{trans.ToString()} not exist in state:" + GetType().ToString());
            return ZGame.FSM.StateType.None;
        }


        public virtual void DoBeforeEntering(params object[] paras)
        {

        }
        public virtual void DoAfterEntering(params object[] paras)
        {
            this.IsStateFinished = false;
        }

        public virtual void DoBeforeLeaving()
        {
            this.RemoveEventListeners();
        }

        public virtual void AddEventListeners()
        {

        }

        public virtual void RemoveEventListeners()
        {

        }

        public virtual void Reason(float dt, params object[] paras)
        {

        }

        public virtual void Act(float dt, params object[] paras)
        {

        }

        public virtual void Destroy()
        {

        }
    }
}