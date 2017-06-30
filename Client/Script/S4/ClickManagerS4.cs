using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ClickManagerS4 : MonoBehaviour {

    //當倒數完時，設為true
    private bool Key = false; //能不能進入遊戲的鑰匙
    //檢察是不是每個在Group中的玩家的hero都到齊，10為到期
    private byte checkreadlly = 0;
    //最後檢查到哪步驟
    private byte checkStep = 0;

    #region 倒數計時的處理 Key = true 代表 倒數到0了
    public GameObject TimerOutputer;
    TextMeshPro timerOutputer;
    float timer = 50.0f; //70.0f
    void FixedUpdate(){
        calculateWaitingTime();
    }
    /// <summary>
    /// 計算等待的時間，如果Key=false才進入。當倒數到0時將Key設成true
    /// </summary>
    private void calculateWaitingTime()
    {
        if (!Key)
        {
            timer = timer - Time.deltaTime;
            timerOutputer.text = ((int)timer).ToString();
            if ((int)timer == 0)
            {
                timerOutputer.text = "Waiting Teammate";
                Key = true;
            }
        }
    }
    #endregion

    void Awake(){
        characterdatabase = this.gameObject.GetComponent<characterDatabase>();
        timerOutputer = TimerOutputer.GetComponent<TextMeshPro>();
    }

	// Use this for initialization
	void Start () {
        ConnectFunction.F.heroidReceiver += this.heroidReceiver;
        ConnectFunction.F.finalheroReceiver += this.finalheroReceiver;

        //用來知道自己是哪個底座
        //以後要為自己的底座，做顏色的更改
 
        myNumber = playerGroup.GetPlayerNumber((int)ConnectFunction.F.playerInfo.UID);
        if (myNumber >= 5)
        {
            myNumber = myNumber - 5;
            mychair = "chair" + (myNumber+1).ToString();
        }
        else
            mychair = "chair" + (myNumber+1).ToString();

        initialize.chair[myNumber].gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
	}
    public void OnDestroy() 
    {
        ConnectFunction.F.heroidReceiver -= this.heroidReceiver;
        ConnectFunction.F.finalheroReceiver -= this.finalheroReceiver;
    } //移除委派
    #region 委派
    //接收別人的hero id 並藉由此對自己的介面做更改
    //有傳過來的一定是隊友
    public void heroidReceiver(Dictionary<byte,object> packet)
    {
        Debug.Log("event 15 in");
        var Suid = int.Parse(packet[0].ToString());
        var Sheroid = int.Parse(packet[1].ToString());
        int Senderplayerid = playerGroup.GetPlayerNumber(Suid);
        Debug.Log("發送者的 playerid : " + Senderplayerid);
        Debug.Log("發送者的 heroid : " + Sheroid);
            if (Senderplayerid >= 5)
                Senderplayerid = Senderplayerid - 5;
        Debug.Log("發送者的 playerid 改成 -5 : " + Senderplayerid);
        //CreateHeroOnBottom(Senderplayerid,heroid); 
        characterdatabase.InitializeCharacter(Sheroid, initialize.chair[Senderplayerid].transform.position);
    }
    //取得最終的heroid值，並加入playerGroup
    public void finalheroReceiver(Dictionary<byte, object> packet)
    {

        var SenderPlayerid = int.Parse(packet[1].ToString());
        var SenderHeroid = int.Parse(packet[2].ToString());
        playerGroup.playersGroup[SenderPlayerid].heroId = SenderHeroid;
        if (IsTeammate(SenderPlayerid))
        {
            if(SenderPlayerid>=5)
                characterdatabase.InitializeCharacter(SenderHeroid, initialize.chair[SenderPlayerid-5].transform.position);
            else
                characterdatabase.InitializeCharacter(SenderHeroid, initialize.chair[SenderPlayerid].transform.position);
        }
        this.checkreadlly++;
    }
    //委派使用的功能
    /// <summary>
    /// 如果目標uid是隊友的話
    /// </summary>
    /// <param name="playeruid">目標uid</param>
    /// <returns></returns>
    private bool isTeammate(int playeruid)
    {
        int myPlayerid = playerGroup.GetPlayerNumber((int)ConnectFunction.F.playerInfo.UID);
        int targetid = playerGroup.GetPlayerNumber(playeruid);
        if ((myPlayerid > 4 && targetid > 4) || (myPlayerid  <= 4 && targetid <= 4))
        {
            return true;
        }
        else
            return false;
    }
    private bool IsTeammate(int TargetPlayerid)
    {
        int myPlayerid = playerGroup.GetPlayerNumber((int)ConnectFunction.F.playerInfo.UID);
        if ((myPlayerid > 4 && TargetPlayerid > 4) || (myPlayerid <= 4 && TargetPlayerid <= 4))
        {
            return true;
        }
        else
            return false;
    }
    #endregion
    

    #region 選角用的參數
    //黑洞的物件
    public GameObject Blackhole;
    //記錄現在選到誰
    int heroid = -1;
    //點擊mybotton後要出現的物件
    public GameObject Plane1, MenuPlane,runway;
    //存放角色的預置物件
    characterDatabase characterdatabase;
    #endregion

    int myNumber; //我是在遊戲房中的記幾個 1~5
    string mychair = ""; //我的底座的名子，chair1~chair5
	// Update is called once per frame
	void Update () {
        if (!Key)
        {
            //click manager 主體
            //eventsystem 好像是同樣做法 (題外話)
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    #region 正常的click
                    if (hit.collider.gameObject.name == mychair)
                    {
                        Plane1.SetActive(true);
                        MenuPlane.SetActive(true);
                        Blackhole.SetActive(false);
                        runway.SetActive(true);
                    }
                    else if (hit.collider.gameObject.name == "CircleButton")
                    {
                        if (heroid != -1)
                            ConnectFunction.F.DeliverheroID(heroid);
                        characterdatabase.InitializeCharacter(heroid, initialize.chair[myNumber].transform.position); //在mybottom中出現
                        Plane1.SetActive(false);
                        MenuPlane.SetActive(false);
                        runway.SetActive(false);
                        Blackhole.SetActive(true);
                    }
                    #endregion

                    #region 英雄按鈕item
                    if (hit.collider.gameObject.GetComponentInParent<item>() != null)
                    {
                        int hitedname = int.Parse(hit.collider.gameObject.GetComponentInParent<item>().gameObject.name);
                        for (int i = 0; i < ConnectFunction.F.gamingInfo.herostring.Length; i++)
                        {
                            if (hitedname == i)
                            {
                                characterdatabase.InitializeCharacter(i, runway.transform.position);
                                heroid = i;
                                break;
                            }
                        }
                    }
                    #endregion
                    Debug.Log(hit.collider.gameObject.name);
                }
            }
        }
        else //總結 //當倒數規0時 ， 也可以增加個"準備完成按鈕"直接將Key=0
        {       
            switch (checkStep)
            {
                case 0:
                    if (Plane1.activeSelf)
                    {
                        Plane1.SetActive(false);
                        MenuPlane.SetActive(false);
                        //Blackhole.SetActive(true);
                    }
                    if (heroid == -1)
                    {
                        heroid = 1; //這裡random英雄id//要寫個隨機功能
                        characterdatabase.InitializeCharacter(heroid, initialize.chair[myNumber].transform.position);
                    }

                    int myuid = (int)ConnectFunction.F.playerInfo.UID;
                    Dictionary<byte, int> target = new Dictionary<byte, int>();
                    //將所有Group中的uid都放入，包跨自己的
                    foreach (KeyValuePair<int, Baseplayer> player in playerGroup.playersGroup)
                    {
                        target.Add(player.Value.playId, player.Value.uid);
                    }

                    Dictionary<byte,object> packet = new Dictionary<byte,object>
                    {
                        {(byte)0,ConnectFunction.F.playerInfo.UID},
                        {(byte)1,playerGroup.GetPlayerNumber((int)ConnectFunction.F.playerInfo.UID)},
                        {(byte)2,this.heroid},
                        {(byte)3,target},
                    };
                    Debug.Log("packet 的資料 : " + packet[0] + "//" + packet[1] + "//" + packet[2]);

                    
                    ConnectFunction.F.Deliver((byte)16, packet);
                    
                    checkStep++;
                    Debug.Log("第一關 通過 ");
                    break;
                case 1:
                    if (this.checkreadlly==10)
                    {
                        checkStep++;
                        Debug.Log("第二關 通過 ");
                    }
                    break;
                case 2:
                    //可以進入遊戲場景了
                    //Application.loadLevel(xx);
                    foreach (KeyValuePair<int, Baseplayer> player in playerGroup.playersGroup)
                    {
                        Debug.Log("所有玩家的 Number/heroid : "+player.Value.playId+"//"+ player.Value.heroId);
                    }
                    Application.LoadLevel("S5");
                    Debug.Log("success in the gaming scene");
                    break;
            }
        }
	}

}
