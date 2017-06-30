using UnityEngine;
using System.Collections;
using TMPro;
public class Loginidle : StateMachineBehaviour {
    //處理抓Gameobject的問題，在別的地方抓
    public delegate void Logining(bool appear);
    static public event Logining logining;


    //使用的字串
    public S1String thisstring;
    public void Awake()
    {
        thisstring = new S1String();
    }

	// OnStateEnter is called when a transition start and the state machine begin to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        GameObject g4 = GameObject.Find("Text4");
        GameObject g5 = GameObject.Find("Text5");
        TextMeshPro text4 = g4.GetComponent<TextMeshPro>();
        TextMeshPro text5 = g5.GetComponent<TextMeshPro>();
        text4.text = thisstring.idle1;
        text5.text = thisstring.idle2;
	}

	// OnStateUpdate is called at each Update frame between OnStateEnter and OnStateExit callback
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (ConnectFunction.F.playerInfo.isConnect)
        {          
            logining(true);
        }
        else
        {
            logining(false);
        }
        
	}

	// OnStateExit is called when a transition end and the state machine end to evaluate this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        GameObject g4 = GameObject.Find("Text4");
        GameObject g5 = GameObject.Find("Text5");
        TextMeshPro text4 = g4.GetComponent<TextMeshPro>();
        TextMeshPro text5 = g5.GetComponent<TextMeshPro>();
        text4.text = "";
        text5.text = "";
        logining(false);
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
