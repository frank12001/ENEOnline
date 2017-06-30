using UnityEngine;
using System.Collections;
using TMPro;

public class ResetState : StateMachineBehaviour {

    S1String thisstring;
    void Awake()
    {
        thisstring = new S1String();
    }
	// OnStateEnter is called when a transition start and the state machine begin to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetInteger("state", 0);
        GameObject g1 = GameObject.Find("Text1");
        GameObject g2 = GameObject.Find("Text2");
        GameObject g3 = GameObject.Find("Text3");
        TextMeshPro text1 = g1.GetComponent<TextMeshPro>();
        TextMeshPro text2 = g2.GetComponent<TextMeshPro>();
        TextMeshPro text3 = g3.GetComponent<TextMeshPro>();
        text1.text = thisstring.state1;
        text2.text = thisstring.state2;
        text3.text = thisstring.state3;
	}

	// OnStateUpdate is called at each Update frame between OnStateEnter and OnStateExit callback
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition end and the state machine end to evaluate this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        GameObject g1 = GameObject.Find("Text1");
        GameObject g2 = GameObject.Find("Text2");
        GameObject g3 = GameObject.Find("Text3");
        TextMeshPro text1 = g1.GetComponent<TextMeshPro>();
        TextMeshPro text2 = g2.GetComponent<TextMeshPro>();
        TextMeshPro text3 = g3.GetComponent<TextMeshPro>();
        text1.text = "";
        text2.text = "";
        text3.text = "";
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
