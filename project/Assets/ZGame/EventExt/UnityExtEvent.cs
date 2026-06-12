using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.EventExt
{

    //Unity中button的事件机制
    //比如对于.AddListener(funcA)，重复添加funcA,那么funcA会被执行多次
    //.RemoveListener(funcA)方法则会移除所有添加的funcA

    public class UnityExtEvent
    {
        Action<object> listener;
        public UnityExtEvent()
        {
            { }
        }

        public void AddListener(Action<object> call)
        {
            if (this.listener != null)
            {
                //if (  !this.listener.GetInvocationList().Contains(call))
                //{
                //    this.listener += call;
                //}

                //为了和unity的button事件机制一致。
                //暂时不再限制相同的call
                this.listener += call;
            }
            else
            {
                this.listener = call;
            }
        }

        public void RemoveListener(Action<object> call)
        {
            //if (this.listener != null && this.listener.GetInvocationList().Contains(call))
            //{
            //    this.listener -= call;
            //}

            if (this.listener != null)
            {
                //if (this.listener.GetInvocationList().Contains(call))
                //{
                //    this.listener -= call;
                //}

                //和Unity中button机制一致，remove的时候会移除所有的相同的call
                //移除所有相同的的call
                var invos = this.listener.GetInvocationList();
                for (int i = invos.Length - 1; i >= 0; i--)
                {
                    if (invos[i].Equals(call))
                    {
                        this.listener -= call;
                    }
                }

            }
        }

        public void RemoveAllListeners()
        {
            this.listener = null;
        }


        public void Invoke(object pram)
        {
            this.listener?.Invoke(pram);
        }
    }
}