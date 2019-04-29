using UnityEngine;

public abstract class AIMachineBehaviour : AIMachineBehaviour<AIController> {
}

public abstract class AIMachineBehaviour<TC> : StateMachineBehaviour
    where TC: AIController {

    protected Animator animator;
    protected TC controller;

    private bool initted;
    private bool shouldContinue;

    public void init(Animator animator, TC controller) {
        this.animator = animator;
        this.controller = controller;
        init();
        initted = true;
    }

    public virtual void init() {
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (initted && !shouldContinue) {
            OnAIStateUpdate();
        }
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (initted) {
            OnAIStateEnter();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (initted) {
            shouldContinue = false;
            OnAIStateExit();
        }
    }

    public virtual void OnAIStateUpdate() {
    }

    public virtual void OnAIStateEnter() {}
    public virtual void OnAIStateExit() {}

    public void doContinue() {
        shouldContinue = true;
        animator.SetTrigger("continue");
    }
}