using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Character.Controller2D
{
    public class CharacterAnimationCallback : StateMachineBehaviour
    {
        internal Action DyingAction { get; set; }
        internal Action WinAction { get; set; }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log($"Die: {DyingAction}, Win: {WinAction}");
            if (stateInfo.IsName("Die"))
            {
                DyingAction.Invoke();
            }

            if(stateInfo.IsName("Win"))
            {
                WinAction.Invoke();
            }
        }
    }
}