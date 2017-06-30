using UnityEngine;
using System.Collections;
using TMPro;

public class LoginandAutoLogoin : StateMachineBehaviour {

    GameObject text1, text2, text3;
    S1String stringpool;
    public void Awake()
    {
        stringpool = new S1String();
    }
	// OnStateEnter is called when a transition start and the state machine begin to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
      
        text1 = GameObject.Find("AutoLoginText1");
        text2 = GameObject.Find("AutoLoginText2");
        text3 = GameObject.Find("AutoLoginText3");
        text1.GetComponent<TextMeshPro>().text = stringpool.autotext1;
        text2.GetComponent<TextMeshPro>().text = stringpool.autotext2;
        text3.GetComponent<TextMeshPro>().text = "    SUCCESS"+"\nID : "+ConnectFunction.F.playerInfo.ID+"\nPSD : "+ConnectFunction.F.playerInfo.PSD;
        
	}

	// OnStateUpdate is called at each Update frame between OnStateEnter and OnStateExit callback
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition end and the state machine end to evaluate this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        text1 = GameObject.Find("AutoLoginText1");
        text2 = GameObject.Find("AutoLoginText2");
        text3 = GameObject.Find("AutoLoginText3");
        text1.GetComponent<TextMeshPro>().text = "";
        text2.GetComponent<TextMeshPro>().text = "";
        text3.GetComponent<TextMeshPro>().text = "";
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
