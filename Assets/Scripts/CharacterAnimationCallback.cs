using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationCallback : StateMachineBehaviour
{
    internal Action DyingAction { get; set; }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        DyingAction.Invoke();
    }
}
