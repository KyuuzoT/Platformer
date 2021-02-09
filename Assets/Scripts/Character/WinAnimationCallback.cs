using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Platformer.Character.Controller2D
{
    public class WinAnimationCallback : StateMachineBehaviour
    {
        internal Action WinAction { get; set; }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            WinAction.Invoke();
        }
    }
}

