using UnityEngine;
using System.Collections.Generic;

public class Buttonfriends : ButtonBase {
    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)) //左鍵 聊天
        {
            if (state && !chatController.chatingCollection.ContainsKey(this.uid))
            {
                Dictionary<byte, object> packet = new Dictionary<byte, object> 
                {
                    {(byte)0,ConnectFunction.F.playerInfo.UID},
                    {(byte)1,DataManager.datapool.name},
                    {(byte)2,this.uid},
                    {(byte)3,this.name},
                };
                ConnectFunction.F.Deliver((byte)7, packet);
            }
        }

    }


    public int uid;
    public string name;
    public bool state;
    //GameObject log;
    //S3Log logscript;
    public void Awake()
    {
     //   log = GameObject.Find("Log");
     //   logscript = log.GetComponent<S3Log>();
        Color32 offline = new Color32(75, 48, 48, 255);
        SetTextColor(offline);
    }
    
    
    void Update()
    {
        if (state)
        {
            

        }

    }
    public bool isSet()
    {
        if (uid.ToString().Length > 0 && name.Length > 0)
            return true;
        else
            return false;
    }
    public void Setuid(int uid)
    {
        this.uid = uid;
    }
    public void Setname(string name) 
    {
        this.name = name;
    }
    public void Setstate(bool b)
    {
        this.state = b;
        if (b)
        {
            Color32 online = new Color32(255, 255, 255, 255);
            SetTextColor(online);
        }
        else
        {
            Color32 offline = new Color32(75, 48, 48, 255);
            SetTextColor(offline);
        }
    }
    public int Getuid()
    {
        return this.uid;
    }
    public string Getname()
    {
        return this.name;
    }
    public bool Getstate()
    {
        return this.state;
    }
    public void Destroythis()
    {
        Destroy(this.gameObject);
    }
}
