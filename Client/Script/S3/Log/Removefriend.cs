using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Removefriend : InputLogBase{

    S3String stringpool;
    GameObject log;
    MeshCollider Addfriendcollider;
    void Awake()
    {
        stringpool = new S3String("Removefriend");
        Addfriendcollider = GameObject.Find("ButtonAddfriend").GetComponent<MeshCollider>();
        log = GameObject.Find("Log");
    }
    // Use this for initialization
    void Start()
    {
        Addfriendcollider.enabled = false;
        SetTopic(stringpool.Removefriendtext1);
        SetTextButton1(stringpool.Removefriendtext2);
        SetTextButton2(stringpool.Removefriendtext3);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Button1Onclick()
    {
        string friendname = GetTextInput().ToString();
        if (friendname == DataManager.datapool.name) //如果這個名稱是自己
        {
            this.log.GetComponent<S3Log>().SetText(stringpool.Removefriendtext4);
        }
        else
        {
            if (!DataManager.datapool.FriendsList.ContainsValue(friendname))
            {
                this.log.GetComponent<S3Log>().SetText(stringpool.Removefriendtext5);
            }
            else
            {
                //Debug.Log(DataManager.datapool.GetfriendsNumberInfriendslist(friendname));
                Dictionary<byte, object> packet = new Dictionary<byte, object> 
                {
                    {(byte)0,ConnectFunction.F.playerInfo.UID},
                    {(byte)1,DataManager.datapool.GetUidbyNameInfriendslist(friendname)},
                    {(byte)2,friendname},
                };
                ConnectFunction.F.Deliver((byte)9, packet);
                ButtonSpark.Spark(true);
            }
        }
        Addfriendcollider.enabled = true;
        Destroy(this.gameObject);
    }
    public void Button2Onclick()
    {
        
        Addfriendcollider.enabled = true;
        Destroy(this.gameObject);
    }
}
