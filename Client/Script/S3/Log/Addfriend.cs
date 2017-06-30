using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Addfriend : InputLogBase {

    GameObject log;
    S3String stringpool;
    void Awake()
    {
        stringpool = new S3String("Addfriend");
        log = GameObject.Find("Log");
    }
	// Use this for initialization
	void Start () {
        SetTopic(stringpool.InputLogtext1);
        SetTextButton1(stringpool.InputLogtext2);
        SetTextButton2(stringpool.InputLogtext3);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void Button1Onclick()
    {
        if (GetTextInput().ToString() == DataManager.datapool.name) //如果名子等於自己  的處理
        {
            this.log.GetComponent<S3Log>().SetText(stringpool.Addfriendtext4);
        }
        else
        {
            if (DataManager.datapool.FriendsList.Count > 20) //如果超過好友名單最大長度  的處理
            {
                this.log.GetComponent<S3Log>().SetText(stringpool.Addfriendtext3);
            }
            else
            {
                if (!DataManager.datapool.FriendsList.ContainsValue(GetTextInput())) //判斷這個暱稱有沒有在好友名單內
                {
                    Dictionary<byte, object> b = new Dictionary<byte, object>
                 {
                     {(byte)0,ConnectFunction.F.playerInfo.UID},
                     {(byte)1,DataManager.datapool.name},
                     {(byte)2,GetTextInput().ToString()},
                 };
                    //Debug.Log(b[0] + "///" + b[1] + "///" + b[2]);
                    ConnectFunction.F.Deliver((byte)6, b);
                    Destroy(this.gameObject);
                }
                else
                {
                    this.log.GetComponent<S3Log>().SetText(stringpool.Addfriendtext2);
                    //以後再加程式碼 ， 處理 已經有這位好友
                    //S3String Addfriendtext1 = Has been friend
                }
            }
        }
         
    }
    public void Button2Onclick()
    {
        Destroy(this.gameObject);
    }
}
