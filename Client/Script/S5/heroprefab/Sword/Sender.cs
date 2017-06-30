using UnityEngine;
using System;
using System.Collections.Generic;

public class Sender : MonoBehaviour {

#if (UNITY_EDITOR)
    public string ScriptIntroduction1 = "所有跟伺服器溝通的功能";
    public string ScriptIntroduction2 = "未封裝完整";
    public string ScriptIntroduction3 = "";
    public string ScriptIntroduction4 = "";
#endif
    private byte playerid;
    private bool functionCanUse=false;

    #region 用來吸收傳送延遲誤差的參數 ， 可是由於只考慮到 一個軸 所以誤差算要改
    public float FrontDelayMove = 0.0f, BackDelayMove = 0.0f, RightDelayMove = 0.0f, LeftDelayMove=0.0f;
    public Vector3 newMove = new Vector3();
    #endregion

    void Update()
    {
        ///檢查playerid是否正常
        if (!functionCanUse)
        {
            try
            {
                playerid = byte.Parse(this.gameObject.name);
                functionCanUse = true;
            }
            catch(FormatException)
            {
                functionCanUse = false;
            }
        }
        
    }

    /// <summary>
    /// 想要前進移動時呼叫，向伺服器發出我想前進的訊息。這裡只處理xxxGo狀態 ex:FrontGo,BackGo。
    /// </summary>
    /// <param name="GameManager.Motion">要傳的移動狀態。只處理xxxGo狀態，放入其他的狀態的話不做事回直接回傳</param>
    public void Move(GameManager.Motion motion)
    {
        if (!functionCanUse || ((byte)motion % 2 == 1)) 
            return;
        Dictionary<byte, object> packet = new Dictionary<byte, object>() 
        {
            {0,playerid},
        };
        packet.Add(1, (byte)motion);
        
        ConnectFunction.F.Deliver(17, packet);
    }
    /// <summary>
    /// 想要前進移動時呼叫，向伺服器發出我想前進的訊息。
    /// </summary>
    /// <param name="GameManager.Motion">要傳的移動狀態。要傳的移動狀態。只處理xxxStop狀態，放入其他的狀態的話不做事回直接回傳</param>
    public void Move(GameManager.Motion motion,Vector3 predictPosition)
    {
        if (!functionCanUse || !((byte)motion % 2 == 1))
            return;
        Dictionary<byte, object> packet = new Dictionary<byte, object>() 
        {
            {0,playerid},
        };
        packet.Add(1, (byte)motion);
        switch ((byte)motion)
        {
                case (byte)GameManager.Motion.FrontStop:
                    packet.Add(2, predictPosition.x);
                    packet.Add(3, predictPosition.y);
                    packet.Add(4, predictPosition.z);
                    break;
                case (byte)GameManager.Motion.BackStop:
                    packet.Add(2, predictPosition.x);
                    packet.Add(3, predictPosition.y);
                    packet.Add(4, predictPosition.z);
                    break;
                case (byte)GameManager.Motion.RightStop:
                    packet.Add(2, predictPosition.x);
                    packet.Add(3, predictPosition.y);
                    packet.Add(4, predictPosition.z);
                    break;
                case (byte)GameManager.Motion.LeftStop:
                    packet.Add(2, predictPosition.x);
                    packet.Add(3, predictPosition.y);
                    packet.Add(4, predictPosition.z);
                    break;
                default:
                    break;
        }
        ConnectFunction.F.Deliver(17, packet);
    }
    /// <summary>
    /// 同步"自己的位置"，在別人和自己的電腦上
    /// </summary>
    public void SyncPosition()
    {
        Dictionary<byte, object> packet = new Dictionary<byte, object>() 
        {
            {0,playerid},
            {1,(byte)GameManager.Motion.SyncPosition},
            {2,this.gameObject.transform.position.x},
            {3,this.gameObject.transform.position.y},
            {4,this.gameObject.transform.position.z},
        };
        ConnectFunction.F.Deliver(17, packet);
    }
    /// <summary>
    /// 將自己和在別人電腦上的自己，同步到指定的點
    /// </summary>
    /// <param name="position">要同步到的那一點</param>
    public void SyncPosition(Vector3 position)
    {
        Dictionary<byte, object> packet = new Dictionary<byte, object>() 
        {
            {0,playerid},
            {1,(byte)GameManager.Motion.SyncPosition},
            {2,position.x},
            {3,position.y},
            {4,position.z},
        };
        ConnectFunction.F.Deliver(17, packet);
    }
}
