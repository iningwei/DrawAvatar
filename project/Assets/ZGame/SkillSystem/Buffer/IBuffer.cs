using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.FSM;

namespace ZGame.SkillSystem.Buffer
{
    public interface IBuffer
    {

        void OnRelease(Entity entity);

        void OnStop();

        void Update(float dt);
    }
}