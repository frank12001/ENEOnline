using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mainchat : MonoBehaviour {

    content content;
    public chat1 chat1;
    public chat2 chat2;
    private int UID;
    private string name;
    public GameObject gchat1,gchat2;
    chatController chatcontroller;
    /// <summary>
    /// 儲存當 chat2 的Active為false時 別人傳過來的字串
    /// </summary>
    public List<string> chatstorage;

    void Awake()
    {
        content = this.gameObject.GetComponentInChildren<content>();
        chat1 = this.gameObject.GetComponentInChildren<chat1>();
        chat2 = this.gameObject.GetComponentInChildren<chat2>();
        chatcontroller = GameObject.Find("Main Camera").GetComponent<chatController>();
    }
	// Use this for initialization
	void Start () {
        gchat1 = chat1.Getchat1();
        gchat2 = chat2.Getchat2();
        gchat2.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	if (Input.GetKeyDown(KeyCode.Return)&&gchat2.activeSelf)
        {
        //設定自己這邊的chat
        string inputchat = chat2.GetInputField();
            content.Setchatcontent(inputchat,false);
            chat2.ClearInputField();
            chat2.Focus();
        //設定別人那邊的chat
        //send 聊天string Suid Sname Tuid Tname 
            if (inputchat.Length > 0)
            {
                Dictionary<byte, object> packet = new Dictionary<byte, object>
                {
                    {(byte)0,ConnectFunction.F.playerInfo.UID},
                    {(byte)1,DataManager.datapool.name},
                    {(byte)2,this.UID},
                    {(byte)3,this.name},
                    {(byte)4,inputchat},
                };
                ConnectFunction.F.Deliver((byte)10, packet);
            }
        }
	}
    
    public void controllchat(int code)
    {
        switch (code)
        {
            case 1: //chatimage2 被按 
                gchat1.SetActive(true);
                gchat2.SetActive(false);
                break;
            case 2: //chatimage1 被按
                chat1Spark(false);   //chat1 Spark stop
                gchat1.SetActive(false);
                gchat2.SetActive(true);
                chatcontroller.chooseOther(UID);
                if (chatstorage.Count > 0)
                {
                    foreach (string chatstring in chatstorage)
                    {
                        content.Setchatcontent(chatstring, true);
                    }
                    chatstorage.Clear();
                }
                break;
            case 3: //chatimage1 被右建按(關閉)
                chatController.chatingCollection.Remove(this.UID);
                Destroy(this.gameObject);
                break;

        }
    }
    public int whoActive()
    {
        if (gchat1.activeSelf)
            return 1;
        else
            return 2;
    }
    public void SetUid(int uid)
    {
        this.UID = uid;
    }
    public int GetUid()
    {
        return this.UID;
    }
    public void SetName(string name)
    {
        this.name = name;
    }
    public string GetName()
    {
        return this.name;
    }

    public void Setchat(string chatString)
    {
        content.Setchatcontent(chatString,true);
    }

    public void chat1Spark(bool start)
    {
        chat1.Spark(start);
    }
}
