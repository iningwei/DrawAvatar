using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.FSM;
using ZGame.SkillSystem.Buffer;

namespace ZGame.SkillSystem.Buffer
{
    public class Buffer
    {
        public BufferID id;
        public Entity entity;
        public bool isInited = false;
        public float totalTime;

        public Buffer(BufferID id)
        {
            this.id = id;
        }
    }
}