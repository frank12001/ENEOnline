using UnityEngine;
using System.Collections;

public class Exit3 : StateMachineBehaviour {

    GameObject Arc,Box;
    bool key = false;
    float timer = 0.0f,f;
    void Awake()
    {
        Arc = GameObject.Find("Arc003");
        Box = GameObject.Find("Box003");
    }
	// OnStateEnter is called when a transition start and the state machine begin to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        key = true;
        timer = 0.0f;
	}

	// OnStateUpdate is called at each Update frame between OnStateEnter and OnStateExit callback
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (key)
        {
            timer = timer + Time.deltaTime;
            if (timer >= 0.13&&Arc.activeSelf)
                Arc.SetActive(false);
            if (timer >= 0.15 && Box.activeSelf)
            {
                Box.SetActive(false);
            }
        }
	}

	// OnStateExit is called when a transition end and the state machine end to evaluate this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// Callback for processing animation movements for modifying root motion. Called rigth after Animator.OnAnimatorMove().
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// Callback for setting up animation IK (inverse kinematics). Called right after Animator.OnAnimatorIK().
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
