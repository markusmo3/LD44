using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetBehaviour : AIMachineBehaviour {

    public override void OnAIStateUpdate() {
        var target = controller.GetTarget();
        controller.navAgent.destination = target.transform.position.to2D();
    }

    public override void OnAIStateExit() {
        controller.navAgent.ResetPath();
    }
}