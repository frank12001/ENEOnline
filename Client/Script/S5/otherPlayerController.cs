using UnityEngine;
using System.Collections.Generic;

public class otherPlayerController : MonoBehaviour {

    //這個Script用來控制不是自己之外的英雄，所以接收的委派應該要寫在這邊
    //像是之前"傀儡師"的功能

    /// <summary>
    /// 存放其他玩家Gameobject的字典，index(key)為playerid = playernumber 0 ~9 (不包含自己) 
    /// </summary>
    public Dictionary<byte, GameObject> otherPlayers = new Dictionary<byte, GameObject>();

    /// <summary>
    /// 每個otherPlayer的SenderMotion快取
    /// </summary>
    private SenderMotion[] motionFunction = new SenderMotion[10];
    private bool motionFunctionHasValue = false; //檢查motionFunction裡面有沒有取到


    
    void Start()
    {
        
        ConnectFunction.F.catchMoveRequires += this.catchMoveRequires;
        ConnectFunction.F.playerLvUpReceiver += this.playerLvUpReceiver;
    }
    void OnDestroy()
    {
        ConnectFunction.F.catchMoveRequires -= this.catchMoveRequires;
        ConnectFunction.F.playerLvUpReceiver -= this.playerLvUpReceiver;
    }

    void Update()
    {
        #region 檢察並賦予motionFunction裡的值
        //檢察並賦予motionFunction裡的值，motionFunction陣列 為 每位玩家(不包含自己)身上的SenderMotion索引 
        if (!motionFunctionHasValue)
        {
            if(otherPlayers.Count>=9)
            {
                foreach (KeyValuePair<byte, GameObject> player in otherPlayers)
                {
                    motionFunction[player.Key] = player.Value.GetComponent<SenderMotion>();
                }
                motionFunctionHasValue = true;
            }
        }
        #endregion
    }


    /// <summary>
    /// 接收移動要求，並執行指定人物的移動
    /// </summary>
    /// <param name="packet"></param>
    public void catchMoveRequires(Dictionary<byte, object> packet)
    {
        //Debug.Log("catch pos packet  Sender = uid" + packet[0]);

        byte playerid = byte.Parse(packet[0].ToString());
        byte motion = byte.Parse(packet[1].ToString());
        switch (motion)
        {
             /* 移動的case */
            case (byte)GameManager.Motion.FrontGo:
                motionFunction[playerid].MoveFront(true);
                Debug.Log("otherplayer 接收到 FrontGo ");
                break;
            case (byte)GameManager.Motion.BackGo:
                motionFunction[playerid].MoveBack(true);
                Debug.Log("otherplayer 接收到 BackGo ");
                break;
            case (byte)GameManager.Motion.RightGo:
                motionFunction[playerid].MoveRight(true);
                Debug.Log("otherplayer 接收到 RightGo ");
                break;
            case (byte)GameManager.Motion.LeftGo:
                motionFunction[playerid].MoveLeft(true);
                Debug.Log("otherplayer 接收到 LeftGo ");
                break;


            case (byte)GameManager.Motion.FrontStop:
                motionFunction[playerid].MoveFront(false);
                Vector3 newPosition = new Vector3(float.Parse(packet[2].ToString()), float.Parse(packet[3].ToString()), float.Parse(packet[4].ToString()));
                //motionFunction[playerid].SmoothAssignPosition(newPosition);
                StartCoroutine(motionFunction[playerid].SmoothAssignPosition(newPosition));
                break;           
            case (byte)GameManager.Motion.BackStop:
                motionFunction[playerid].MoveBack(false);
                Vector3 newPosition1 = new Vector3(float.Parse(packet[2].ToString()), float.Parse(packet[3].ToString()), float.Parse(packet[4].ToString()));
                //motionFunction[playerid].SmoothAssignPosition(newPosition1);
                StartCoroutine(motionFunction[playerid].SmoothAssignPosition(newPosition1));
                break;           
            case (byte)GameManager.Motion.RightStop:
                motionFunction[playerid].MoveRight(false);
                Vector3 newPosition2 = new Vector3(float.Parse(packet[2].ToString()), float.Parse(packet[3].ToString()), float.Parse(packet[4].ToString()));
                //motionFunction[playerid].SmoothAssignPosition(newPosition2);
                StartCoroutine(motionFunction[playerid].SmoothAssignPosition(newPosition2));
                break;
            case (byte)GameManager.Motion.LeftStop:
                motionFunction[playerid].MoveLeft(false);
                Vector3 newPosition3 = new Vector3(float.Parse(packet[2].ToString()), float.Parse(packet[3].ToString()), float.Parse(packet[4].ToString()));
                //motionFunction[playerid].SmoothAssignPosition(newPosition3);
                StartCoroutine(motionFunction[playerid].SmoothAssignPosition(newPosition3));
                break;

            /*  除了移動之外，其他case  */
            case (byte)GameManager.Motion.SyncPosition:
                try
                {
                    Vector3 newPositionSync = new Vector3(float.Parse(packet[2].ToString()), float.Parse(packet[3].ToString()), float.Parse(packet[4].ToString()));
                    motionFunction[playerid].SmoothAssignPosition(newPositionSync);
                }
                catch (KeyNotFoundException)
                {
                    Debug.Log("這裡是otherPlayerController.cs.catchMoveRequires 問題: 想要同步位置，可是在packet裡 key=2&&3&&4的地方沒有值");
                }
                break;
            default:
                break;
        }
        
    }

    //Select assign part in order to level up
    public void playerLvUpReceiver(Dictionary<byte, object> packet)
    {
        byte playerNumber = byte.Parse(packet[0].ToString());
        byte whichPart = byte.Parse(packet[1].ToString());
        otherPlayers[playerNumber].GetComponent<SwordProperty>().SelectPartLvUp(whichPart);
    }
    #region change player's position,rotation,animation funcions 
    /// <summary>
    /// 將一位玩家的位置換成指定的位置
    /// </summary>
    /// <param name="playerid">要改變玩家的playerid</param>
    /// <param name="position">新的位置</param>
    private void changePlayerPosition(byte playerid, Vector3 position)
    {
        if (otherPlayers.ContainsKey(playerid))
        {
            otherPlayers[playerid].transform.position = position;
        }
    }
    /// <summary>
    /// 將一位玩家的方向換成指定的方向
    /// </summary>
    /// <param name="playerid">要改變玩家的playerid</param>
    /// <param name="rotation">新的方向</param>
    private void changePlayerRotation(byte playerid, Quaternion rotation)
    {
        if (otherPlayers.ContainsKey(playerid))
        {
            otherPlayers[playerid].transform.rotation = rotation;
        }
    }
    /// <summary>
    /// 將一位玩家的動畫參數換成指定的動畫參數
    /// </summary>
    /// <param name="playerid">要改變玩家的playerid</param>
    /// <param name="animParameterName">要改的參數名稱(通常為"state")</param>
    /// <param name="animNumber">新的動畫參數</param>
    private void changePlayerAnim(byte playerid,string animParameterName, int animNumber)
    {
        if (otherPlayers.ContainsKey(playerid))
        {
            Animator anim = otherPlayers[playerid].GetComponent<Animator>();
            anim.SetInteger(animParameterName, animNumber);
        }
    }

    #endregion

}
