using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Character.Controller2D
{
    public class CharacterAnimationCallback : StateMachineBehaviour
    {
        internal Action DyingAction { get; set; }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            DyingAction.Invoke();
        }
    }
}