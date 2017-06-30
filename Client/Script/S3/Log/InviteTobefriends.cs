using UnityEngine;
using System.Collections.Generic;

public class InviteTobefriends : chooseLogBase {

    public int Iuid;
    public string Iname;
    void Awake()
    {
    }
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
    //OnClick----
    public void Button1Onclick()  //Suid,Iuid,是否
    {
        //處理邀請回傳 (同意)
        Debug.Log("agree"+Iuid+"///"+Iname);
        Dictionary<byte,object> packet = new Dictionary<byte,object>()
        {
            {(byte)0,ConnectFunction.F.playerInfo.UID},
            {(byte)1,Iuid},
            {(byte)2,true},
            {(byte)3,DataManager.datapool.name.ToString()},
        };
        DataManager.datapool.AddFriends(Iuid, Iname, true);
        
        ConnectFunction.F.Deliver((byte)8, packet);
        ButtonSpark.Spark(true);
        GameObject ComplexQButton = GameObject.Find("ComplexQButton");
        ComplexQButton.GetComponent<MeshCollider>().enabled = true;
        Destroy(this.gameObject);
    }
    public void Button2Onclick()
    {
        //處理邀請回傳(不同意)
        Debug.Log("disagree");
        Dictionary<byte, object> d = new Dictionary<byte, object>()
        {
            {(byte)0,ConnectFunction.F.playerInfo.UID},
            {(byte)1,Iuid},
            {(byte)2,false},
            {(byte)3,DataManager.datapool.name.ToString()},
        };
        ConnectFunction.F.Deliver((byte)8, d);
        GameObject ComplexQButton = GameObject.Find("ComplexQButton");
        ComplexQButton.GetComponent<MeshCollider>().enabled = true;
        Destroy(this.gameObject);
    }
    //----
}
