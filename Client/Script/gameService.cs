using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class gameService : IPhotonPeerListener
{

    #region 函數
    //暫存玩家資料
    public PlayerInfo playerInfo = new PlayerInfo();
    public GamingInfo gamingInfo = new GamingInfo();
    //Server 連線資訊 ip 和 port
    private const string Servername = "startOnline";
    //private const string Address = "localhost" + ":" + "5055";
    //private const string Address = "23.99.124.117" + ":" + "5055";
    //private const string Address = "36.227.54.32" + ":" + "5055";
    private const string Address = "207.46.129.4" + ":" + "5055";
    //                               Azure pip

    private PhotonPeer peer;
    #endregion
    //建構式
    public gameService()
    {
        
    }

    //---------------------------------MyFunction
    #region Deliver
    public void Deliver(byte code, Dictionary<byte, object> dic)
    {    //普通的傳送
        try
        {
            peer.OpCustom(code, dic, true);
        }
        catch (Exception ex) { throw ex; }
    }
    #region S4 專用
    /// <summary>
    /// S4 採樣輸入專用
    /// </summary>
    /// <param name="packet">要傳的包裹</param>
    public void Deliver_SampleKey(Dictionary<byte, object> packet)
    {
        try
        {
            peer.OpCustom((byte)EventCode.Receive_GamingRoomMessages,packet, true);
        }
        catch (Exception ex) { throw ex; }
    }
    public void Deliver_Rotation(Dictionary<byte, object> packet)
    {
        try
        {
            peer.OpCustom((byte)EventCode.Receive_GamingRoomMessages, packet, true);
        }
        catch (Exception ex) { throw ex; }
    }
    #endregion
    #endregion
    #region 不常用的 ChangeToDic
    public Dictionary<byte, object> ChangeToDic(object[] o) //dic從1開始放
    {
        Dictionary<byte, object> dic = new Dictionary<byte, object>();
        for (int i = 0; i < o.Length; i++)
        {
            if (o[i] != null)
            {
                byte b = (byte)(i + 1);
                dic.Add((byte)b, o[i]);
            }
        }
        return dic;
    }
    #endregion
    #region 連線到Server
    public bool ConnectToServer()
    {
        if (this.peer == null)
            this.peer = new PhotonPeer(this, ConnectionProtocol.Udp);

        if (this.peer.Connect(Address, Servername))
        {
            return true;
        }
        else
            return false;
    }
    #endregion
    //--------------------------------MyFunctionEnd


    #region IPhotonPeerListener 的實作介面
    //Deleage
    public delegate void ReceiveDictionaryHandler(Dictionary<byte, object> packet);
    public delegate void ReceiveReturncodeHandler(short returncode);
    public delegate void ReceiveDictionaryAndReturncodeHandler(short returncode, Dictionary<byte, object> packet);
    #region OnOperation裡用的事件
    #region S1
    //註冊事件
    public event ReceiveDictionaryAndReturncodeHandler registerevent;
    //登入事件
    public event ReceiveDictionaryAndReturncodeHandler loginerevent;
    #endregion
    #region S2
    public event ReceiveReturncodeHandler S2checkevent;
    #endregion
    #region S21
    public event ReceiveReturncodeHandler S21checkevent;
    #endregion
    #region S3
    public event ReceiveDictionaryAndReturncodeHandler getGameinfo;     //in DataManager
    public event ReceiveReturncodeHandler addfriend1;                   //In Community
    public event ReceiveDictionaryAndReturncodeHandler removefriend;    //In Community
    public event ReceiveDictionaryAndReturncodeHandler chatevent;       //In chatController
    public event ReceiveReturncodeHandler CanelQueue;                //In SingleQ
    #endregion
    
    #endregion
    public void OnOperationResponse(OperationResponse operationResponse)
    {
        switch (operationResponse.OperationCode)
        {
            case (byte)0:  //測試使用(目前沒用)
                
                break;
            case (byte)2: //註冊的返回處理
                registerevent(operationResponse.ReturnCode, operationResponse.Parameters);
                break;
            case (byte)1: //登入的返回處理
                loginerevent(operationResponse.ReturnCode, operationResponse.Parameters);
                break;
            case (byte)3:
                S2checkevent(operationResponse.ReturnCode);
                break;
            case 4:
                S21checkevent(operationResponse.ReturnCode);
                break;
            case 5:
                getGameinfo(operationResponse.ReturnCode, operationResponse.Parameters);
                break;
            case 6:
                addfriend1(operationResponse.ReturnCode);
                break;
            case 7:
                //放 friendslist's buttonMouseDown 委派   之一
                chatevent(operationResponse.ReturnCode, operationResponse.Parameters);
                break;
            case 9:
                removefriend(operationResponse.ReturnCode, operationResponse.Parameters);
                break;
            case 11:
                CanelQueue(operationResponse.ReturnCode);
                break;

        }
    }

    #region OnEvent裡的事件
    #region S3
    public event ReceiveDictionaryHandler stateevent;  //in chatController
    public event ReceiveDictionaryHandler addfriend2;  //in Community
    public event ReceiveDictionaryHandler addfriend3;  //in Community
    public event ReceiveDictionaryHandler removefriendevent; //in Community
    public event ReceiveDictionaryHandler chateventE;  //in chatController
    public event ReceiveDictionaryHandler Receive_PreRoomMessages; //in SingleQController 
    #endregion
    #region S4
    public event ReceiveDictionaryHandler Receive_WorldState; //in ReceiveAndStoreWorldStates
    public event ReceiveDictionaryHandler Receive_Ping;       //in Gaming_Time
    #endregion

    #endregion
    public void OnEvent(EventData eventData)
    {
        switch (eventData.Code)
        {
            case 5:
                //接收好友上線訊息
                stateevent(eventData.Parameters);
                break;
            case 6:
                //Debug.Log("成功接收到event");
                addfriend2(eventData.Parameters);
                break;
            case 8: //新增好友 第三關 回傳有沒有答應
                //Debug.Log("成功接收到event8");
                addfriend3(eventData.Parameters);
                break;
            /* case 7:
                //放 friendslist's buttonMouseDown 委派 之二
                chateventE(eventData.Parameters);
                break;
             * */
            case 9:
                removefriendevent(eventData.Parameters);
                break;
            case 10:
                //接收聊天字串
                chateventE(eventData.Parameters);
                break;
            case (byte)EventCode.Receive_PreRoomMessages: 
                Receive_PreRoomMessages(eventData.Parameters);
                break;
                //----S4----//
            case (byte)EventCode.Receive_GamingRoomMessages:
                //Debug.Log("Receive_GamingRoomMessages");
                #region 用Switch Code 對 遊戲房的訊息做分類
                byte switchCode = (byte)eventData.Parameters[0];
                switch (switchCode)
                { 
                    case 2: //計算 Ping 
                        Receive_Ping(eventData.Parameters); 
                        break;
                    default://接收世界狀態
                        Receive_WorldState(eventData.Parameters);
                        break;
                }
                #endregion
                break;
        }
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
        switch (statusCode)
        {
            case StatusCode.Connect:
                this.playerInfo.isConnect = true;
                break;
            case StatusCode.Disconnect:
                break;
            default:
                break;
        }
    }

    public void DebugReturn(DebugLevel level, string message)
    {

    }
    #endregion

    //要一直呼叫的，要擺在一個Updata中
    public void Service()
    {
        if (this.peer != null)
            this.peer.Service();
    }

}
/// <summary>
/// 玩家的個人基本資料資訊
/// </summary>
public class PlayerInfo
{
    public PlayerInfo()
    {
        this.isConnect = false;
        this.UID = null;
        this.ID = null;
        this.PSD = null;
        this.name = null;
    }
    public bool isConnect;
    public int? UID;
    public string ID;
    public string PSD;
    public string name;
}
/// <summary>
/// 玩家在遊戲中的資訊，離開S3之前，將需要的資料存在這裡(可擴增)(目前在S3內未寫)
/// </summary>
public class GamingInfo
{
    public GamingInfo()
    {
        herostring = null;
    }
    public string herostring;
}
