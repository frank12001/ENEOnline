using UnityEngine;
using System.Collections;

public class anim25 : StateMachineBehaviour {

    GameObject mainCamera;
    public float x,y,z,t;
    Vector3 now,goal1;
    float d,move;
    public float down;
    void Awake() {
        mainCamera = GameObject.Find("Main Camera");
    } 
	// OnStateEnter is called when a transition start and the state machine begin to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        goal1 = new Vector3(x, y, z);
        now = mainCamera.transform.position;
        d = Mathf.Sqrt((goal1.x - now.x)*(goal1.x - now.x) + (goal1.y - now.y)*(goal1.y - now.y) + (goal1.z - now.z)*(goal1.z - now.z));
        move = d / t;
	}

	// OnStateUpdate is called at each Update frame between OnStateEnter and OnStateExit callback
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (mainCamera.transform.position.y > 0.823)
        {
            Vector3 v = Vector3.Normalize(new Vector3((goal1.x - now.x), (goal1.y - now.y), (goal1.z - now.z)));
            mainCamera.transform.Translate(v * move);
        }
        else
        {
            if (mainCamera.GetComponent<Camera>().fieldOfView >= 1 + down)
            {
                mainCamera.GetComponent<Camera>().fieldOfView = mainCamera.GetComponent<Camera>().fieldOfView - down;
            }
            else
            {
                Application.LoadLevel("S2");
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
