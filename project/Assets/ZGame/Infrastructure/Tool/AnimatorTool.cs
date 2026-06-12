using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame
{
    public class AnimatorTool
    {
        public static bool IsAnimatorPlayingTargetState(Animator animator, int layerIndex, string stateName)
        {
            if (animator != null && animator.enabled)
            {
                var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
                if (stateInfo.IsName(stateName))
                {
                    return true;
                }
            }

            return false;
        }
    }
}