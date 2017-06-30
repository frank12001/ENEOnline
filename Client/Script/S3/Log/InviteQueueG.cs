using UnityEngine;
using System.Collections.Generic;

public class InviteQueueG : chooseLogBase {

    public int Suid;
    public string Sname;
    //OnClick----
    public void Button1Onclick()  //Suid,Iuid,是否
    {
        //處理邀請回傳 (同意)
        Debug.Log("agree" + Suid + "///" + Sname);
        Dictionary<byte, object> packet = new Dictionary<byte, object>()
        {
            {(byte)0,0}, //switch code 1
            {(byte)1,1}, //switch code 2
            {(byte)2,Suid},
            {(byte)3,ConnectFunction.F.playerInfo.UID},
            {(byte)4,DataManager.datapool.name.ToString()},
        };

        ConnectFunction.F.Deliver((byte)21, packet);
        ButtonSpark.Spark(true);
        GameObject ComplexQButton = GameObject.Find("ComplexQButton");
        ComplexQButton.GetComponent<MeshCollider>().enabled = true;
        Destroy(this.gameObject);
    }
    public void Button2Onclick()
    {
        //處理邀請(不同意)       
        Destroy(this.gameObject);
    }
    //----
}
