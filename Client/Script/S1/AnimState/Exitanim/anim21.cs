using UnityEngine;
using System.Collections;

public class anim21 : StateMachineBehaviour {
    public void Awake()
    {
        plane2 = GameObject.Find("Plane2");
    }

    GameObject plane2;
    float timer;
    bool key=false;
    
	// OnStateEnter is called when a transition start and the state machine begin to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        plane2.SetActive(false);
        timer = 0.0f;
        key = true;
	}

	// OnStateUpdate is called at each Update frame between OnStateEnter and OnStateExit callback
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (key)
        {
            timer = timer + Time.deltaTime;
            if (timer >= 1.5f)
            {
                plane2.SetActive(true);
                key = false;
            }
            
        }
	}

	// OnStateExit is called when a transition end and the state machine end to evaluate this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        plane2.SetActive(true);
	}

	// Callback for processing animation movements for modifying root motion. Called rigth after Animator.OnAnimatorMove().
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// Callback for setting up animation IK (inverse kinematics). Called right after Animator.OnAnimatorIK().
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
