using UnityEngine;
using System.Collections;
using TMPro;

public class Registeridle : StateMachineBehaviour {

    GameObject g6;
    GameObject g4;
    GameObject gtextregister1;
    TextMeshPro text6;
    TextMeshPro text4;
    TextMeshPro textregister1;
    public S1String thisstring;

    //處理抓Gameobject的問題，在別的地方抓
    public delegate void Registering(bool appear);
    static public event Registering register;

    public void Awake()
    {
        thisstring = new S1String();
        g6 = GameObject.Find("Text6");
        g4 = GameObject.Find("Text4");
        gtextregister1 = GameObject.Find("Textregister1");
//        text6 = g6.GetComponent<TextMeshPro>();
 //       text4 = g4.GetComponent<TextMeshPro>();
 //      textregister1 = gtextregister1.GetComponent<TextMeshPro>();
    }


	// OnStateEnter is called when a transition start and the state machine begin to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        text6 = g6.GetComponent<TextMeshPro>();
        text4 = g4.GetComponent<TextMeshPro>();
        textregister1 = gtextregister1.GetComponent<TextMeshPro>();
        text6.text = thisstring.register1;
        text4.text = thisstring.idle1;
	}

	// OnStateUpdate is called at each Update frame between OnStateEnter and OnStateExit callback
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (ConnectFunction.F.playerInfo.isConnect)
        {
            text6.text = thisstring.register2;
            textregister1.text = thisstring.register3;
            register(true);
        }
        
	}

	// OnStateExit is called when a transition end and the state machine end to evaluate this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        text6.text = "";
        text4.text = "";
        textregister1.text = "";
        register(false);
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
