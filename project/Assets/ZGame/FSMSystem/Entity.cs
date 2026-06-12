using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame;
using ZGame.FSM;
using ZGame.SkillSystem.Buffer;

namespace ZGame.FSM
{
    public class Entity
    {
        public GameObject obj = null;
        public FSMSystem fsm = null;

        public List<IBuffer> bufferList = new List<IBuffer>();
        public Entity(GameObject obj)
        {
            this.obj = obj;
            this.fsm = new FSMSystem( );
            this.InitFSMSystem();
        }

        public virtual void InitFSMSystem()
        {

        }

        public IBuffer AddBuffer(BufferID id)
        {
            if (this.GetBuffer(id) != null)
            {
                Debug.LogError("exist same buffer:" + id.ToString());
                return null;
            }

            var buffer = BufferMachine.Instance.Spwan(id);
            this.bufferList.Add(buffer);
            buffer.OnRelease(this);
            return buffer;
        }

        public bool RemoveBuffer(BufferID id)
        {
            for (int i = 0; i < bufferList.Count; i++)
            {
                if ((bufferList[i] as ZGame.SkillSystem.Buffer.Buffer).id == id)
                {
                    bufferList[i].OnStop();
                    this.bufferList.RemoveAt(i);

                    return true;
                }
            }
            return false;
        }

        public IBuffer GetBuffer(BufferID id)
        {
            for (int i = 0; i < bufferList.Count; i++)
            {
                if ((bufferList[i] as ZGame.SkillSystem.Buffer.Buffer).id == id)
                {
                    return bufferList[i];
                }
            }
            return null;
        }

        public virtual void Update(float dt)
        {
            if (fsm != null)
            {
                this.fsm.Update(dt);
            }

            for (int i = 0; i < this.bufferList.Count; i++)
            {
                this.bufferList[i].Update(Time.deltaTime);
            }
        }

        public virtual void Destroy()
        {
            this.fsm.CurState.Destroy();

            if (obj != null)
            {
                GameObject.Destroy(obj);
            }
        }
    }
}