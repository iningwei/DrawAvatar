using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.FSM
{
    public class FSMSystem
    {

        List<FSMState> states = null;

        public FSMState CurState
        {
            get;
            set;
        }

        public int CurStateType
        {
            get;
            set;
        }
        public FSMSystem()
        {

            states = new List<FSMState>();
        }

        public void AddState(FSMState state, params object[] paras)
        {
            if (state == null)
            {
                Debug.LogError("AddState:null is not allowed");
            }

            if (this.states.Count == 0)
            {
                this.states.Add(state);
                this.CurState = state;
                this.CurStateType = state. StateType;
                this.CurState.DoAfterEntering(paras);
                return;
            }

            for (int i = 0; i < this.states.Count; i++)
            {
                if (this.states[i]. StateType == state. StateType)
                {
                    Debug.LogError("already exist,can not add state:" + state. StateType.ToString());
                    return;
                }
            }
            this.states.Add(state);
        }

        public void DeleteState(FSMState state)
        {
            if (state == null)
            {
                Debug.LogError("DeleteState:null is not allowed");
            }
            if (state.StateType == StateType.None)
            {
                Debug.LogError("error,delete state None is not allowed");
                return;
            }
            for (int i = 0; i < this.states.Count; i++)
            {
                if (this.states[i].StateType == state.StateType)
                {
                    this.states.Remove(this.states[i]);
                    return;
                }
            }
            Debug.LogError("DeleteState:" + state.ToString() + " failed,it is not exist in state list");
        }

        public void DeleteAllState()
        {
            this.states.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trans">TransitionType</param>
        /// <param name="paras"></param>
        public void PerformTransition(int trans, params object[] paras)
        {
            if (trans == TransitionType.None)
            {
                Debug.LogError("error,Transition None is not allowed for PerformTransition()");
                return;
            }

            int stateType = this.CurState.GetOutputState(trans);
            if (stateType == StateType.None)
            {
                return;
            }

            for (int i = 0; i < this.states.Count; i++)
            {
                if (this.states[i].StateType == stateType)
                {
                    this.CurState.DoBeforeLeaving();
                    this.CurState = this.states[i];
                    this.CurState.DoBeforeEntering(paras);
                    this.CurState.DoAfterEntering(paras);
                    this.CurStateType = stateType;
                    return;
                }
            }
            Debug.LogError("state:" + CurStateType.ToString() + " do not contain transition:" + trans.ToString());
        }


        public void Update(float dt)
        {
            this.CurState.Reason(dt);
            this.CurState.Act(dt);
        }
    }
}