using UnityEngine;
using System.Collections;

public class anim22 : StateMachineBehaviour {

    GameObject plane3,bug,light;
    
    void Awake()
    {
        plane3 = GameObject.Find("Plane3");
        bug = GameObject.Find("Bug");
        light = GameObject.Find("Point light1");
    }
	// OnStateEnter is called when a transition start and the state machine begin to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        light.transform.position = new Vector3(0.07f, 2.16f, -7.24f);
        bug.transform.position = new Vector3(-0.033f, 0.473f,-6.699f);
        plane3.SetActive(false);
	}

	// OnStateUpdate is called at each Update frame between OnStateEnter and OnStateExit callback
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition end and the state machine end to evaluate this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
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
