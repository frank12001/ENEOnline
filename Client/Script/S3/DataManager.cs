using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataManager : MonoBehaviour {

    static public Data datapool; //玩家基本資料
    public GameObject ButtonPrefab; //Button的預置物件
    static public Buttonfriends[] ButtonCol; //實體Button 的存放空間
    void Awake()
    {
        datapool = new Data();
    }
	// Use this for initialization
	void Start () {
        ConnectFunction.F.getGameinfo += getGameinfo;
        Dictionary<byte,object> d = new Dictionary<byte,object>{ {(byte)0,ConnectFunction.F.playerInfo.UID},{(byte)1,GameOverInfo.fromWhichScene} } ;
        ConnectFunction.F.Deliver((byte)5, d);
	}
    public void OnDestroy()
    {
        ConnectFunction.F.getGameinfo -= getGameinfo;
    }

    #region myfunction
    //create button Entity
    static public Buttonfriends[] CreateButtonEntity(Dictionary<int, string> friendslist,Dictionary<int,bool> state,GameObject prefab)
    {
        Buttonfriends[] buttonlist = new Buttonfriends[friendslist.Count];
        int i =0;
        foreach (KeyValuePair<int, string> item in friendslist)
        {
            GameObject g = (GameObject)Instantiate(prefab);
            //---計算位置
            float x=1,y=1,xd=1.2f,yd=0.4f;
            int number = i + 1;
            if (number <= 4) { x = 1; }
            else if (number > 4 && number <= 8) { x = -1; }
            else if (number > 8 && number <= 12) { x = 2; }
            else if (number > 12 && number <= 16) { x = -2; } else if (number > 16 && number <= 20) { x = 3; }
            if (number % 4 == 1){ y = 0;}
            else if (number % 4 == 2){ y = 1; }
            else if (number % 4 == 3){ y = 2; }
            else if (number % 4 == 0){ y = 3; }
            Vector3 v = new Vector3(g.transform.position.x+(x*xd),g.transform.position.y+(y*yd),g.transform.position.z);
            g.transform.position = v;
            //---
            bool nowstate = false;
            state.TryGetValue(item.Key, out nowstate);
            buttonlist[i] = g.GetComponent<Buttonfriends>();
            buttonlist[i].Setuid(item.Key);
            buttonlist[i].Setname(item.Value);
            buttonlist[i].SetText(item.Value);
            buttonlist[i].Setstate(nowstate);
            //橋button的位置
            i++;
        }
        return buttonlist;
    }
    static public void RemoveEntityButton(Buttonfriends[] ButtonColthis)
    {
        if (ButtonColthis.Length > 0)
        {
            for (int i = 0; i < ButtonColthis.Length; i++)
            {
                ButtonColthis[i].Destroythis();
            }
        }
    }
    #endregion
    #region mydelegate
    public void getGameinfo(short returncode,Dictionary<byte,object> d) //取得 start.Gameinfo 裡的資料 和 friendslist 存入  datapool
    {
        if (returncode==(byte)0)
        {
            foreach (KeyValuePair<byte, object> item in (Dictionary<byte,object>)d[1])
            {
                switch (item.Key)
                {
                    case 0:
                        ConnectFunction.F.playerInfo.UID = int.Parse(item.Value.ToString());
                        break;
                    case 1:
                        datapool.name = item.Value.ToString();
                        break;
                    case 2:
                        datapool.level = int.Parse(item.Value.ToString());
                        break;
                    case 3:
                        datapool.energy = int.Parse(item.Value.ToString());
                        break;
                    case 4:
                        datapool.laststate = int.Parse(item.Value.ToString());
                        break;
                    case 5:
                        datapool.Property = item.Value.ToString();
                        break;
                    case 6:
                        datapool.herostring = item.Value.ToString();
                        ConnectFunction.F.gamingInfo.herostring = item.Value.ToString();
                        Debug.Log("Hero String = " + ConnectFunction.F.gamingInfo.herostring);
                        break;
                }
            }
            Dictionary<int, string> s = new Dictionary<int, string>();
            foreach(KeyValuePair<int,object> item in (Dictionary<int,object>)d[3])
            {
                s.Add(item.Key, item.Value.ToString());
            }
            Dictionary<int,bool> state = new Dictionary<int,bool>();
            foreach(KeyValuePair<int,bool> item in (Dictionary<int,bool>)d[2])
            {
                state.Add(item.Key,item.Value);
            }
            datapool.InsertInfriendslist(s,state); //這裡要傳入friendslist 和他的 state
        }
        ButtonCol = CreateButtonEntity(datapool.FriendsList,datapool.FriendsState, this.ButtonPrefab);
    }
    #endregion
}
