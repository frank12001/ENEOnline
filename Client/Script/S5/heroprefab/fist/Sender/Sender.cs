using UnityEngine;
using System.Collections.Generic;

public class Sender : MonoBehaviour {
    

    private Animator animator;
    private Vector3 pos;
    private Quaternion rot;

    private Dictionary<byte, object> packet = new Dictionary<byte, object>();
	// Use this for initialization
	void Start () {
        animator = this.GetComponent<Animator>();

	}
	
	// Update is called once per frame
	void Update () {


          //判斷packet.Count<1的意思為現在是不是第一次執行
           pos = this.gameObject.transform.position;
           rot = this.gameObject.transform.rotation;
           if (packet.Count <10)
           {
               try
               {
                   packet.Add(0, (int)ConnectFunction.F.playerInfo.UID);
                   packet.Add(1, byte.Parse(this.gameObject.name));//byte.Parse(this.gameObject.name)
                   packet.Add(2, pos.x);
                   packet.Add(3, pos.y);
                   packet.Add(4, pos.z);
                   packet.Add(5, rot.x);
                   packet.Add(6, rot.y);
                   packet.Add(7, rot.z);
                   packet.Add(8, rot.w);
                   packet.Add(9, animator.GetInteger("state"));
                   ConnectFunction.F.Deliver((byte)17, this.packet);
                   Debug.Log("已發出1");
               }
               catch
               {
                   packet.Clear();
                  // Debug.Log("進到catch");
               }
           }
           else if(packet.Count==10)
           {
                   //由於packet[0],packet[1] 不會更動，所以不用改
                   packet[2] = pos.x;
                   packet[3] = pos.y;
                   packet[4] = pos.z;
                   packet[5] = rot.x;
                   packet[6] = rot.y;
                   packet[7] = rot.z;
                   packet[8] = rot.w;
                   packet[9] = animator.GetInteger("state");
                   ConnectFunction.F.Deliver((byte)17, this.packet);
                  // Debug.Log("已發出2");
               
           }
           
    }

}
