using UnityEngine;
using System.Collections.Generic;

public class GroupModeBase : MonoBehaviour {

    /// <summary>
    /// 在使用時都要用 base 關鍵字 先呼叫一次原本的Awake
    /// </summary>
    public void Awake()
    {
        players = new Baseplayer[5];
    }
    /// <summary>
    /// 在使用時都要用 base 關鍵字 先呼叫一次原本的start
    /// </summary>
    public void Start()
    {
        playerposition[0] = new Vector3(0.0f, 1.67f, -4.53f);
        playerposition[1] = new Vector3(1.7f, 1.67f, -4.53f);
        playerposition[2] = new Vector3(-1.7f, 1.67f, -4.53f);
        playerposition[3] = new Vector3(3.4f, 1.67f, -4.53f);
        playerposition[4] = new Vector3(-3.4f, 1.67f, -4.53f);
    }

    public GameObject Inputlog;
    public GameObject Starttimer;
    public GameObject button;
    /// <summary>
    /// 在使用時都要用 base 關鍵字 先呼叫一次原本的update
    /// </summary>
    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return) && !Inputlog.activeSelf)
        {
            Inputlog.SetActive(true);
            Inputlog.GetComponent<InputLogGroupmode>().Base = this;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < players.Length; i++)
            {
                
                if(players[i]!=null)
                {
                    if (i == 0)
                        Debug.Log("目前的隊員有");
                    Debug.Log("Number = " + i + " uid = " + players[i].uid + "  name " + players[i].name);
                }
            }
        }
    }

    /// <summary>
    /// 5位玩家固定的點
    /// </summary>
    public Vector3[] playerposition = new Vector3[5];
    /// <summary>
    /// players[0]一定式captain。注意可能會有[2]=null ,[3] !=null .的情況
    /// </summary>
    protected Baseplayer[] players;

    public void GruopButtonOnClick(int index)
    {
        switch (index)
        {
            #region bt1 onclick (start)
            case 1:
                //start button
                Debug.Log("start b");
                if (TriangleButton.GroupMode && !TriangleButton.GroupModePlayer && !TriangleButton.GroupQueue)
                //當我是隊長時 //還在組隊沒排隊時  //按下後進入排隊
                {
                    if (players.Length < 2) //當只有自己時不給按start
                    {
                        //以後增加些log
                        return;
                    }
                    //跟所有隊友說要進入排隊了
                    Dictionary<byte, object> tdic = GetUidDic((byte)0, false);
                    Dictionary<byte, object> group = GetGroupDic(true);                  
                    Dictionary<byte, object> packet = new Dictionary<byte, object>
                    {
                        {(byte)0,4}, //switch code
                        {(byte)1,tdic},
                        {(byte)2,group},
                    };
                    
                    ConnectFunction.F.Deliver((byte)21, packet);

                    //開始倒數
                    Starttimer.SetActive(true);
                    TriangleButton.GroupQueue = true;
                    button.SetActive(false);
                }
                break;
            #endregion
            #region bt2 onclick (stop)
            case 2:
                //stop button
                Debug.Log("stop b");
                //還缺兩個狀態，進排隊時隊長和隊員
                if (TriangleButton.GroupMode && !TriangleButton.GroupModePlayer&&!TriangleButton.GroupQueue) 
                    //當我是隊長時 //還在組隊沒排隊時 //按下後stop時
                {
                    if (this.GetPlayerAccounts() > 1)
                    {
                        //解散隊伍
                        Dictionary<byte, object> tDic = this.GetUidDic((byte)0, false);
                        Dictionary<byte, object> packet = new Dictionary<byte, object>
                        {
                            {(byte)0,3},
                            {(byte)1,1},
                            {(byte)2,tDic},
                        };
                        ConnectFunction.F.Deliver((byte)21, packet);
                    }
                    this.gameObject.SetActive(false);
                }
                else if (TriangleButton.GroupMode && !TriangleButton.GroupModePlayer && TriangleButton.GroupQueue)
                    //如果我是隊長，又在排對時
                {
                    //向Server 請求 1. 取消排隊 2.發給隊友說排隊取消
                    Dictionary<byte, object> tdic = GetUidDic((byte)0, false);
                    Dictionary<byte, object> group = GetGroupDic(true);
                    Dictionary<byte, object> packet = new Dictionary<byte, object>
                    {
                        {(byte)0,5}, //switch code
                        {(byte)1,tdic},
                        {(byte)2,group},
                    };

                    ConnectFunction.F.Deliver((byte)21, packet);

                    //開始倒數
                    Starttimer.SetActive(false);
                    TriangleButton.GroupQueue = false;
                    button.SetActive(true);

                }
                else if (TriangleButton.GroupModePlayer && !TriangleButton.GroupMode && !TriangleButton.GroupQueue) //當我是隊員時
                {
                    //發訊息給隊長說我要離開
                    Dictionary<byte, object> packet = new Dictionary<byte, object>
                    {
                        {(byte)0,3}, //switch code 1 
                        {(byte)1,2}, //switch code 2 //用來區分(captain||隊員) 
                        {(byte)2,players[0].uid}, // captain uid= Tuid
                    };
                    ConnectFunction.F.Deliver((byte)21, packet);
                }
                break;
            #endregion
        }
    }
    
    #region Group 操做 Contain,Add,Get,GetAccount,replace,remove,GetUidDic,GetNumber

    /// <summary>
    /// 回傳有沒有在隊伍中
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public bool ContainPlayer(int uid)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if(players!=null)
            {
                if (players[i].uid == uid)
                    return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 將此玩家加入Group中
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="name"></param>
    /// <returns>有沒有加入成功</returns>
    public bool AddPlayer(int uid, string name)
    {
        //增加玩家到Array中
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                if (players[i].uid == uid || i == players.Length - 1) //有相同的uid或人滿了
                    return false;
            }
            else
            {
                players[i] = new Baseplayer(uid, name);
                return true;
            }

        }
        return false;
    }
    /// <summary>
    ///回傳在房裡的玩家
    /// </summary>
    /// <returns>Baseplayer[]</returns>
    public Baseplayer[] GetGroup()
    {
        return this.players;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>回傳現在有多少玩家在房裡</returns>
    public byte GetPlayerAccounts()
    {
        byte account = 0;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
                account++;
        }
        return account;
    }
    /// <summary>
    /// 用一個Baseplayer整個取代原本的
    /// </summary>
    /// <param name="group">注意長度一定要為5</param>
    /// <returns>success?</returns>
    public bool ReplaceGroup(Baseplayer[] group)
    {
        try
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = group[i];
            }

        }
        catch
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// 從隊伍中移除
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public bool Remove(int uid)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                if (players[i].uid == uid)
                {
                    players[i] = null;
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// 取得一個包含所有隊員uid的dic
    /// </summary>
    /// <param name="dicstartindex">產出的dic，第一個值得key</param>
    /// <param name="hascaptain">須不需要把隊長的uid加進去</param>
    /// <returns>一個包含所有隊員uid的dic</returns>
    public Dictionary<byte, object> GetUidDic(byte dicstartindex, bool hascaptain)
    {
        Dictionary<byte, object> packet = new Dictionary<byte, object>();
        int captainv = 0;
        if (!hascaptain)
            captainv = 1;
        for (int i = 0 + captainv; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                packet.Add(dicstartindex, players[i].uid);
                dicstartindex++;
            }
        }
        return packet;
    }
    /// <summary>
    /// 回傳一個包含隊伍的Dic
    /// </summary>
    /// <param name="hascaptain">有沒有包含隊長</param>
    /// <returns>不會把null的加進去第一層，第二層Dic [0] = uid,[1] = name </returns>
    public Dictionary<byte, object> GetGroupDic(bool hascaptain)
    {
        Dictionary<byte, object> packet = new Dictionary<byte, object>();
        int captainv = 0;
        if (!hascaptain)
            captainv = 1;
        for (int i = 0 + captainv; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                Dictionary<byte,object> d = new Dictionary<byte,object>();
                d.Add((byte)0,players[i].uid);
                d.Add((byte)1,players[i].name);
                packet.Add((byte)i, d);
            }
        }
        return packet;
    }
    /// <summary>
    /// 用uid取得在Baseplayer中得編號
    /// </summary>
    /// <param name="uid"></param>
    /// <returns>回傳-1代表不在房裡</returns>
    public sbyte GetNumber(int uid)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].uid == uid)
                return (sbyte)i;
        }
        return (sbyte)(players.Length+1);
    }
    #endregion
}
