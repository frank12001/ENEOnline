using UnityEngine;
using System.Collections.Generic;

public class Queuedelegate : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ConnectFunction.F.queuevent += queuevent;
        ConnectFunction.F.roomevent += roomevent;
        ConnectFunction.F.groupqueuevent += groupqueuevent;
	}

    void OnDestroy()
    {
        ConnectFunction.F.queuevent -= queuevent;
        ConnectFunction.F.roomevent -= roomevent;
        ConnectFunction.F.groupqueuevent += groupqueuevent;
    }

    #region singleQ delegate
    public void queuevent(short returncode, Dictionary<byte, object> packet)
    {
        if (returncode.Equals(1))
        {
            //成功加入排隊，正帶等候中
            Debug.Log("queue success");
        }
    }
    public void roomevent(Dictionary<byte, object> packet)
    {
        int playerNumber=0;
        foreach (KeyValuePair<byte, object> playerInfo in packet)
        {
            Baseplayer bp = new Baseplayer(-1, "");

            foreach (KeyValuePair<byte, object> b in (Dictionary<byte, object>)playerInfo.Value)
            {
                if (b.Key==0)
                    bp.uid = int.Parse(b.Value.ToString());
                if (b.Key==1)
                    bp.name = b.Value.ToString();
                if (b.Key == 2)
                    bp.playId = byte.Parse(b.Value.ToString());
                if (b.Key == 3)
                    bp.teamNumber = byte.Parse(b.Value.ToString());
            }
            
            playerGroup.playersGroup.Add(playerNumber,bp);
            playerNumber++;
        }

        if (playerGroup.playersGroup.Count == 10)
        {
            playerGroup.DestributeplayerGroupToOneAndTwo();
            Application.LoadLevel("S4");
        }
        else
        {
            //這邊擺錯誤處理方法
            //會進入這表示room裡面沒有10人，出現傳輸錯誤
        }
        /* 檢測playerGroup裡的值
        foreach (KeyValuePair<int,Baseplayer> i in playerGroup.playersGroup)
        {
            Debug.Log(i.Key + "//" + i.Value.uid + "//" + i.Value.name);
        }
         * */
    }

    #endregion

    public GameObject InviteQueueGObj;
    public GameObject GroupMode,GroupModePlayer;
    public GameObject Starttimer;
    public GameObject OutputChat;
    public GameObject startgamtimer; //in case 6
    #region complexQ event
    public void groupqueuevent( Dictionary<byte, object> packet)
    {
        byte code =byte.Parse( packet[0].ToString());
        switch (code)
        {
            case 0: //收到團進邀請的處理//receive invite from captain
                //加入邀請
                if (!TriangleButton.GroupModePlayer&&!TriangleButton.GroupMode&&!TriangleButton.SingleQueue) 
                    //不是隊長 也不是隊員 也沒在做單排 才對此邀請做出反應
                {
                    GameObject g = Instantiate(InviteQueueGObj);
                    InviteQueueG c = g.GetComponent<InviteQueueG>();
                    string s = packet[3].ToString() + " invite you play together";
                    c.SetTopic(s);
                    c.Sname = packet[3].ToString();
                    c.Suid = int.Parse(packet[2].ToString());
                    c.SetTextButton1("Yes");
                    c.SetTextButton2("No");

                    GameObject ComplexQButton = GameObject.Find("ComplexQButton");
                    ComplexQButton.GetComponent<MeshCollider>().enabled = false;              
                }
                break;
            case 1: //收到有人要進來時的處理 captain
                Debug.Log("in case 1");
                if (TriangleButton.GroupMode) //如果還在GroupMode在執行，不然不處理
                {
                    GroupModeMainScript groupmode = GroupMode.GetComponent<GroupModeMainScript>();
                    var Suid =int.Parse( packet[1].ToString());
                    var Sname = packet[2].ToString();
                    if (groupmode.AddPlayer(Suid, Sname))
                    {
                        //加入成功
                        //告訴Suid她加入成功了
                        //把Baseplayer[] players 傳過去
                        Baseplayer[] players = groupmode.GetGroup();
                        Dictionary<byte, object> tuidlist2 = groupmode.GetUidDic((byte)0, false);
                        Dictionary<byte, object> playersdic = groupmode.GetGroupDic(true);
                        Dictionary<byte, object> packet2 = new Dictionary<byte, object> 
                        {
                            {(byte)0,2},    //case code in 21
                            {(byte)1,tuidlist2}, //目標dic
                            {(byte)2,playersdic},
                        };
                        //packet從key = 2 開始是房裡的成員 Baseplayer[key-2] ，所以key=2為captain
                        ConnectFunction.F.Deliver((byte)21, packet2);
                    }
                    else
                    {
                        //加入失敗
                        //告訴Suid它失敗了
                    }
                   
                }
                break;
            case 2: //加入成功後，接收captain回傳的資訊
                    //接收packet格式 0 = switch code
                    //               1 = Dicionary<playerNumber,(dic)Baseplayer> => Baseplayer[] (0,1);
                if (!TriangleButton.GroupMode&&!TriangleButton.GroupModePlayer) //現在沒進甚麼mode
                {
                    Baseplayer[] players2 = new Baseplayer[5];
                    Dictionary<byte, object> players = (Dictionary<byte, object>)packet[1];
                    foreach (KeyValuePair<byte, object> player in players)
                    {
                        Dictionary<byte,object> playerdic = (Dictionary<byte,object>)player.Value;
                        players2[player.Key] = new Baseplayer(int.Parse(playerdic[0].ToString()), playerdic[1].ToString());
                    }
                    //叫出介面
                    GroupModePlayer.SetActive(true);
                    GroupModePlayer groupmodeplayer = GroupModePlayer.GetComponent<GroupModePlayer>();
                    groupmodeplayer.ReplaceGroup(players2);
                }
                else if (TriangleButton.GroupModePlayer) //現在處於組隊中(隊員)
                {
                    Dictionary<byte,object> playerdic = (Dictionary<byte,object>)packet[1];
                    Dictionary<byte, object> captain = (Dictionary<byte,object>)playerdic[0];
                    GroupModePlayer groupmodeplayer = GroupModePlayer.GetComponent<GroupModePlayer>();
                    Baseplayer[] players = groupmodeplayer.GetGroup();
                    if (int.Parse(captain[0].ToString()) == players[0].uid) //檢察現在在的房間是不是和這個隊長同房
                    {
                        //已經在Group房中的人，藉由這個packet增減隊員
                        Baseplayer[] newgroup = new Baseplayer[5];
                        foreach (KeyValuePair<byte, object> player in playerdic)
                        {
                            Dictionary<byte, object> member = (Dictionary<byte, object>)player.Value;
                            newgroup[player.Key] = new Baseplayer(int.Parse(member[0].ToString()), member[0].ToString());
                        }
                        groupmodeplayer.ReplaceGroup(newgroup);
                    }
                }
                else
                {
                    //呼叫待會寫的leave功能，向captain發送離開訊息
                }
                break;
            case 3: //接收離開訊息
                byte code3 = byte.Parse(packet[1].ToString());

                if (code3 == 1) //隊長傳過來要解散隊伍
                {
                    if (TriangleButton.GroupModePlayer)
                    {
                        GroupModePlayer.SetActive(false);
                    }
                }
                else if (code3 == 2) //隊員傳過來說要離開
                {
                    if (TriangleButton.GroupMode) //當我是隊長時，有人要離開時
                    {
                        var ruid = int.Parse(packet[2].ToString());
                        GroupModeMainScript groupmode = GroupMode.GetComponent<GroupModeMainScript>();
                        if (groupmode.Remove(ruid)) //刪除成功
                        {
                            //在取得Group並傳給在房間的人
                            Dictionary<byte, object> newgroup = groupmode.GetGroupDic(true);
                            Dictionary<byte, object> tuidDic = groupmode.GetUidDic((byte)0, false);
                            Dictionary<byte, object> packet312 = new Dictionary<byte, object>
                            {
                                {(byte)0,3},
                                {(byte)1,3},
                                {(byte)2,newgroup},
                                {(byte)3,tuidDic},
                            };

                            ConnectFunction.F.Deliver((byte)21, packet312);
                            //發給剛剛發過來的人
                            Dictionary<byte, object> repacket = new Dictionary<byte, object> 
                            {
                                {(byte)0,3},
                                {(byte)1,4},
                                {(byte)2,ruid},
                            };
                            ConnectFunction.F.Deliver((byte)21, repacket);
                        }
                    }
                }
                else if (code3 == 3) 
                    //對員發送之後先等，等到隊長那邊改玩在接收
                    //避免出現時間差的問題
                {
                        GroupModePlayer.SetActive(false);             
                }
                else if (code3 == 4)
                {
                    GroupModePlayer.SetActive(false);
                }
                break;
            case 4:
                //已進入排隊
                //隊長和隊員
                Starttimer.SetActive(true);
                if (TriangleButton.GroupMode&&!TriangleButton.GroupModePlayer)
                {
                    GroupModeMainScript groupmode = GroupMode.GetComponent<GroupModeMainScript>();
                    groupmode.button1.SetActive(false);
                }
                else if (!TriangleButton.GroupMode && TriangleButton.GroupModePlayer)
                {                   
                    //GroupModePlayer groupmodeplayer = GroupModePlayer.GetComponent<GroupModePlayer>();
                    //groupmodeplayer.button2.SetActive(false);
                }
                
                break;
            case 5: //如果是對員時，接到這個訊息代表隊長要進入排隊了
                if (TriangleButton.GroupModePlayer && !TriangleButton.GroupQueue)
                {
                    GroupModePlayer groupmodeplayer = GroupModePlayer.GetComponent<GroupModePlayer>();
                    groupmodeplayer.button2.SetActive(false);
                    //開始倒數
                    startgamtimer.SetActive(true);
                    TriangleButton.GroupQueue = true;
                } 
                else if (TriangleButton.GroupModePlayer && TriangleButton.GroupQueue)
                {
                    //我是隊員，且我在排隊//
                    GroupModePlayer groupmodeplayer = GroupModePlayer.GetComponent<GroupModePlayer>();
                    groupmodeplayer.button2.SetActive(true);
                    //開始倒數
                    startgamtimer.SetActive(false);
                    TriangleButton.GroupQueue = false;
                }
                break;
            case 6:
                if (TriangleButton.GroupMode || TriangleButton.GroupModePlayer)
                {
                    //get chat string and output
                    string chat = packet[2].ToString();
                    byte playernumber = byte.Parse(packet[1].ToString());
                    //出現chat string 在指定playernumber人的位置上
                    //在GroupModeBase中有存每位玩家的位置，用Baseplayer[index]，的index索引
                    GroupModeBase groupbase;
                    if (TriangleButton.GroupMode)
                        groupbase = GroupMode.GetComponent<GroupModeBase>();
                    else
                        groupbase = GroupModePlayer.GetComponent<GroupModeBase>();
                    Debug.Log("收到訊息，要印出來");
                    GameObject output = Instantiate(OutputChat, groupbase.playerposition[playernumber], OutputChat.transform.rotation) as GameObject;
                    output.GetComponent<OutputChat>().SetText("");
                    output.GetComponent<OutputChat>().SetText(chat);
                }
                break;
        }
    }
    #endregion
}
