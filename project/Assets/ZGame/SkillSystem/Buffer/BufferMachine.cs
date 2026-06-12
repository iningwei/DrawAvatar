using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.SkillSystem.Buffer
{
    public class BufferMachine : Singleton<BufferMachine>
    {
        public IBuffer Spwan(BufferID id)
        {
            switch (id)
            {
                //////case BufferID.TemporaryStop:
                //////    return new TemporaryStop(id);
                //////case BufferID.ADSpeedUp:
                //////    return new ADSpeedUp(id);
                //////case BufferID.MouseClickSpeedUp:
                //////    return new MouseClickSpeedUp(id);
                //////case BufferID.SpeedLevelChange:
                //////    return new SpeedLevelChange(id);
                //////case BufferID.CarDash:
                //////    return new CarDash(id);
            }
            return null;
        }
    }
}