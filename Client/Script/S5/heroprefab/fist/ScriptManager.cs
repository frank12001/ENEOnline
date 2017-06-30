using UnityEngine;
using System.Collections;

public class ScriptManager : MonoBehaviour {
   

    private cameraController cameracontroller;
    private SenderMotion senderMotion;
    private AnimController animcontroller;
    private MouseLook mouseLook;
    private AttackGenerator attackGenerator;
  //  private PlayerbehaviorReceiver playerbehaivorReceiver;
    private Sender sender;

    public void Awake(){
        
        cameracontroller = this.gameObject.GetComponent<cameraController>();
        senderMotion = this.gameObject.GetComponent<SenderMotion>();
        animcontroller = this.gameObject.GetComponent<AnimController>();
        mouseLook = this.gameObject.GetComponent<MouseLook>();
        attackGenerator = this.gameObject.GetComponent<AttackGenerator>();
       // playerbehaivorReceiver = this.gameObject.GetComponent<PlayerbehaviorReceiver>();
        sender = this.gameObject.GetComponent<Sender>();

        //Destroy(mouseLook);
        //int playerid = int.Parse(this.gameObject.name);
        int playerid = 1;
        //在下面的開關Script中，考慮要不要直接把Script Destroy 掉
        if (playerGroup.playersGroup[playerid].uid == (int)ConnectFunction.F.playerInfo.UID)
        {

      //    playerbehaivorReceiver.enabled = false;
            cameracontroller.AssignCamera();
        }
        else
        {
            cameracontroller.enabled = false;
            senderMotion.enabled = false;
            animcontroller.enabled = false;
            mouseLook.enabled = false;
            attackGenerator.enabled = false;
            sender.enabled = false;
        }
    }
	// Use this for initialization
	void Start () {

        
 

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
