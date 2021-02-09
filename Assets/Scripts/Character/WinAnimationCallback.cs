using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinAnimationCallback : StateMachineBehaviour
{
    internal Action WinAction { get; set; }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        WinAction.Invoke();
    }
}
