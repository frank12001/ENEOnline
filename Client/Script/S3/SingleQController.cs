using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Assets.Script;

public class SingleQController : MonoBehaviour {

    [SerializeField]
    private GameObject PrepareRoom;
    [SerializeField]
    private GameObject startgamtimer;
    [SerializeField]
    private TextMeshPro countdowntimer;
    private float timer;
    private bool hasSendResult;
    private enum prepareRoomMessagesCategories : byte 
    {
        Joining = 1,
        ChangeHero = 2,
        LockHero = 3,
        ResultAndChangeScene=4, //Result 的資料包會不一樣
    }
    private enum packetCode : byte
    {
        SwitchCode = 0,
        WhoAmI =1,
        TeamsNumber =2,
        MumbersInATeam = 3,
        HeroId = 4,
        Result_WorldState = 5,
        Operator_Number = 6,
    }
   
    [SerializeField]
    private byte TeamsNumberInAGame;
    [SerializeField]
    private byte WhichMembersInATeam;
    /// <summary>
    /// 0 永遠為ShowHerosPlane。 在這個case裡 0 = ShowHerosPlane, 1 = Left1_ShowHeroPlane,2 = Left2_ShowHeroPlane, 3 = Right1_ShowHeroPlane , 4 = Right2_ShowHeroPlane
    /// </summary>
    [SerializeField]
    private Image[] images_Output;
    [SerializeField]
    private Texture2D[] images_HeroImages;
    private Player_PrepareRoom me;
    private List<byte> myheros;
    private byte myherosForce = 0;

    void Start()
    {
        ConnectFunction.F.Receive_PreRoomMessages += this.Receive_PreRoomMessages;
        timer = 30.0f;
    }
    void OnDestroy()
    {
        ConnectFunction.F.Receive_PreRoomMessages -= this.Receive_PreRoomMessages;
    }
    void OnDisable()
    {
        me = null;
        myheros = null;
    }
    void Update()
    {
        if (timer > 0.0f && PrepareRoom.activeSelf)
        {
            timer -= Time.deltaTime;
            countdowntimer.text = ((int)timer).ToString();
        }
        else if (timer <= 0.0f && PrepareRoom.activeSelf)
        {
            SendResult();
            hasSendResult = true;
        }
    }
    private void SendResult()
    {
        if (!hasSendResult)
        {
            /*--傳送結果--*/
            /*--結果是我選擇的HeroId--*/
            /*--EventCode.Receive_PreRoomMessages--*/
            /*--switchcode = (byte)3--*/
            Dictionary<byte, object> whoAmI = new Dictionary<byte, object>
            {
                    {(byte)0,(byte)me.Team},
                    {(byte)1,(byte)me.NumberInTeam},
            };
            Dictionary<byte, object> packet = new Dictionary<byte, object> 
            {
                    {(byte)0,prepareRoomMessagesCategories.LockHero},
                    {(byte)packetCode.WhoAmI,whoAmI},
                    {(byte)packetCode.HeroId,(byte)myheros[myherosForce]},
            };
            ConnectFunction.F.Deliver((byte)EventCode.Receive_PreRoomMessages, packet);            
        }
    }
    /*接收PrepareRoom訊息的event*/
    public void Receive_PreRoomMessages(Dictionary<byte, object> packet)
    {
        Dictionary<byte, object> localpacket = new Dictionary<byte, object>(packet); //複製一份
        byte switchCode = (byte)localpacket[0];
        localpacket.Remove((byte)0);
        switch (switchCode)
        {
            case (byte)prepareRoomMessagesCategories.Joining:
                hasSendResult = false; //傳完結果後將她设唯true
                timer = 30.0f; //將倒數計時器設定為30秒
                countdowntimer.text = ((int)timer).ToString();//將倒數計時器設定為30秒

                startgamtimer.SetActive(false); //將之前在排隊記時的物件SetActive(false)
                PrepareRoom.SetActive(true);    //開啟PrepareRoom
                Dictionary<byte, object> packetplayer = (Dictionary<byte, object>)localpacket[(byte)packetCode.WhoAmI];
                me = new Player_PrepareRoom((byte)packetplayer[0], (byte)packetplayer[1]);  //將自己存下來
                
                string heroString = DataManager.datapool.herostring; //將自己擁有的英雄做成陣列
                myheros = new List<byte>();  //實體化陣列
                for (int i = 0; i < heroString.Length; i++)
                {
                    string s = heroString.Substring(i,1);
                    if (s == "1")
                        myheros.Add((byte)i);
                }
                //設定一開始的自己"選擇地方"的英雄圖
                myherosForce = 0;
                Sprite herotexture = Sprite.Create(images_HeroImages[myheros[myherosForce]], new Rect(0, 0, images_HeroImages[myheros[myherosForce]].width, images_HeroImages[myheros[myherosForce]].height), new Vector2(0.5f, 0.5f));
                images_Output[0].sprite = herotexture;
                break;
            case (byte)prepareRoomMessagesCategories.ChangeHero: //接收從Server傳過來，進行每個人英雄圖片的改變
                Dictionary<byte, object> player = (Dictionary<byte, object>)localpacket[1];
                byte whichTeam = (byte)player[0];
                byte teamNumber = (byte)player[1];
                byte number = (byte)((whichTeam * WhichMembersInATeam) + (teamNumber+1));
                byte heroid = (byte)localpacket[(byte)packetCode.HeroId];
                Sprite texture = Sprite.Create(images_HeroImages[heroid], new Rect(0, 0, images_HeroImages[heroid].width, images_HeroImages[heroid].height), new Vector2(0.5f, 0.5f));
                images_Output[number].sprite = texture;
                break;
            case(byte)prepareRoomMessagesCategories.ResultAndChangeScene:
                
                Temporary_Storage._WorldState = (Worldstate)Assets.Script.Serializate.ToObject((byte[])localpacket[(byte)packetCode.Result_WorldState]);
                Temporary_Storage._Byte = (byte)localpacket[(byte)packetCode.Operator_Number];

                Application.LoadLevel("S4");
                break;
            default:
                Debug.Log("gggg");
                break;
        }
    }
    /*---賦予Prepare介面的OnClick事件---*/
    public void LockHero_ButtonOnClick()
    {
        /*--LockHero--*/
        SendResult();
        hasSendResult = true;
    }
    public void ChooseHeroLeft_ButtonOnClick()
    {
        /*change force*/
        if (myherosForce > 0)
        {
            myherosForce -= 1;
            Sprite texture = Sprite.Create(images_HeroImages[myheros[myherosForce]], new Rect(0, 0, images_HeroImages[myheros[myherosForce]].width, images_HeroImages[myheros[myherosForce]].height), new Vector2(0.5f, 0.5f));
            images_Output[0].sprite = texture;
        }
    }
    public void ChooseHeroRight_ButtonOnClick()
    {
        /*change force*/
        if (myherosForce < myheros.Count-1)
        {
            myherosForce += 1;
            Sprite texture = Sprite.Create(images_HeroImages[myheros[myherosForce]], new Rect(0, 0, images_HeroImages[myheros[myherosForce]].width, images_HeroImages[myheros[myherosForce]].height), new Vector2(0.5f, 0.5f));
            images_Output[0].sprite = texture;
        }
    }
}
public class Player_PrepareRoom
{
    public Player_PrepareRoom(byte team,byte numberInTeam)
    {
        this.Team = team;
        this.NumberInTeam = numberInTeam;
    }
    public byte Team;
    public byte NumberInTeam;
}
