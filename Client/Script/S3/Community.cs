using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Community : MonoBehaviour { //專門拿來放委派的

    public GameObject Log;
    public GameObject InviteLog;
    S3String stringpool;
    public void Awake()
    {
        stringpool = new S3String("Community");
    }
	// Use this for initialization
	void Start () {
        ConnectFunction.F.addfriend1 += this.addfriend1;
        ConnectFunction.F.addfriend2 += this.addfriend2;
        ConnectFunction.F.addfriend3 += this.addfriend3;
        ConnectFunction.F.removefriend += this.removefriend;
        ConnectFunction.F.removefriendevent += this.removefriendevent;
	}
    public void OnDestroy()
    {
        ConnectFunction.F.addfriend1 -= this.addfriend1;
        ConnectFunction.F.addfriend2 -= this.addfriend2;
        ConnectFunction.F.addfriend3 -= this.addfriend3;
        ConnectFunction.F.removefriend -= this.removefriend;
        ConnectFunction.F.removefriendevent -= this.removefriendevent;
    }
	
	// Update is called once per frame
	void Update () {

    }
    #region delegate
       #region  新增好友的回傳處理
    public void addfriend1(short returncode)
    {
        LogBase log =Instantiate(Log).GetComponent<LogBase>();
        switch (returncode)
        {
            case 0:
                log.SetText(stringpool.Communitytext1);
                //Debug.Log("發送成功");
                break;
            case 1:
                log.SetText(stringpool.Communitytext2);
                log.SetColor(Color.red);
                //Debug.Log("該玩家不存在");
                break;
            case 2:
                log.SetText(stringpool.Communitytext3);
                log.SetColor(Color.red);
                //Debug.Log("該玩家不在線上");
                break;
        }
    }
    public void addfriend2(Dictionary<byte, object> d)
    {
        GameObject g = Instantiate(InviteLog);
        InviteTobefriends c = g.GetComponent<InviteTobefriends>();
        string s = d[3].ToString() + stringpool.Communitytext4 + "\n" + stringpool.Communitytext5;
        c.SetTopic(s);
        c.Iname = d[3].ToString();
        c.Iuid =int.Parse(d[2].ToString());
        c.SetTextButton1(stringpool.Communitytext6);
        c.SetTextButton2(stringpool.Communitytext7);

        GameObject ComplexQButton = GameObject.Find("ComplexQButton");
        ComplexQButton.GetComponent<MeshCollider>().enabled = false;

    }
    public GameObject ButtonPrefab;
    public void addfriend3(Dictionary<byte, object> d)
    {
          //處理 好友答覆 
        //1. 顯示答覆
        //2.如果答應的話 加入 Data 
        int Suid =int.Parse(d[1].ToString());
        string Sname = d[2].ToString();
        bool agree =bool.Parse(d[3].ToString());
        if(agree)
        {
            DataManager.datapool.AddFriends(Suid, Sname, true);
            LogBase log = Instantiate(Log).GetComponent<LogBase>();
            log.SetText(Sname + " " + stringpool.Communitytext9);
            log.SetColor(Color.red);
            ButtonSpark.Spark(true);
        }
        else
        {
            LogBase log =Instantiate(Log).GetComponent<LogBase>();
            log.SetText(Sname+" "+stringpool.Communitytext8);
            log.SetColor(Color.red);
        }

    }
       #endregion
       #region 刪除好友回傳處理
       //寫這裡 刪除回傳處理 "如果是發送者的話" 執行這裡 
       public void removefriend(short returncode, Dictionary<byte, object> packet)
       {
           switch (returncode)
           {
               case 0:
                   Dictionary<int, string> newfriendslist = new Dictionary<int, string>();
                   int? uid = ConnectFunction.F.playerInfo.UID;
                   foreach(KeyValuePair<int,object> item in (Dictionary<int,object>)packet[3])
                   {
                       if(!item.Key.Equals(uid))
                          newfriendslist.Add(item.Key, item.Value.ToString());
                   }
                   
                   Dictionary<int,bool> state = new Dictionary<int,bool>();
                   foreach(KeyValuePair<int,bool> item in (Dictionary<int,bool>)packet[2])
                   {
                       if (!item.Key.Equals(uid))
                           state.Add(item.Key, item.Value);
                   }
                   DataManager.datapool.InsertInfriendslist(newfriendslist, state);
                  /* foreach (KeyValuePair<int,bool> item in state) 測試用
                   {
                       Debug.Log(item.Key + "///" + item.Value);
                   }
                   * */
                   LogBase log = Instantiate(Log).GetComponent<LogBase>();
                   log.SetText(stringpool.Communitytext9);
                   break;
           }
       }
       //如果是被刪除的人，又剛好在線上的話，用這裡接收回傳
       public void removefriendevent(Dictionary<byte, object> packet)
       {
           Dictionary<int, string> newfriendslist = new Dictionary<int, string>();
           int? uid = ConnectFunction.F.playerInfo.UID;
           foreach (KeyValuePair<int, object> item in (Dictionary<int, object>)packet[3])
           {
               if (!item.Key.Equals(uid))
                   newfriendslist.Add(item.Key, item.Value.ToString());
           }

           Dictionary<int, bool> state = new Dictionary<int, bool>();
           foreach (KeyValuePair<int, bool> item in (Dictionary<int, bool>)packet[2])
           {
               if (!item.Key.Equals(uid))
                   state.Add(item.Key, item.Value);
           }
           DataManager.datapool.InsertInfriendslist(newfriendslist, state);
         /*  foreach (KeyValuePair<int,bool> item in state) //測試用
            {
                Debug.Log(item.Key + "///" + item.Value);
            }
          */
       }
       #endregion
    #endregion
}
