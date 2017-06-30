using UnityEngine;
using System.Collections.Generic;

public class PlayerbehaviorReceiver : MonoBehaviour {

    byte playerid;
    public void playerBehaviorReceiver(Dictionary<byte, object> packet)
    {
        Debug.Log("有傳過來了"+packet[1]);
        
        //測試內容值
       /* foreach (KeyValuePair<byte, object> value in packet)
        {
            Debug.Log("Key = "+value.Key+"  值 = "+value.Value);
        }
        * */
        if ((byte)packet[1] == playerid)
        {
            Vector3 pos = new Vector3((float)packet[2],(float)packet[3],(float)packet[4]);
            Quaternion rot = new Quaternion((float)packet[5], (float)packet[6], (float)packet[7], (float)packet[8]);
            int animParameter = (int)packet[9];

            this.gameObject.transform.position = pos;
            this.gameObject.transform.rotation = rot;
            this.gameObject.GetComponent<Animator>().SetInteger("state", animParameter);
        }
        
    }

	// Use this for initialization
	void Start () {
        playerid = byte.Parse(this.gameObject.name);
        ConnectFunction.F.playerBehaviorReceiver += this.playerBehaviorReceiver;
        
	}
    void OnDestroy(){
        ConnectFunction.F.playerBehaviorReceiver -= this.playerBehaviorReceiver;
    }
	
}
