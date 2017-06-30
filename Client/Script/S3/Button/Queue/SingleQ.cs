using UnityEngine;
using System.Collections.Generic;

public class SingleQ : ButtonBase {

    public GameObject startgamtimer;
    bool state = false; //用來控制，OnMouseUp的兩段功能
    private enum switchCode { QueueResquest=1,QueueCanel=2, }

    // Use this for initialization
    void Start()
    {
        this.SetText("   START");
        startgamtimer.SetActive(false);
        ConnectFunction.F.CanelQueue += this.CanelQueue;
    }
    void OnDestroy()
    {
        ConnectFunction.F.CanelQueue -= this.CanelQueue;
    }
    void OnMouseUp()
    {
        if (!state)
        {
            Dictionary<byte, object> packet = new Dictionary<byte, object>()
            {{(byte)0,(byte)switchCode.QueueResquest},};
            ConnectFunction.F.Deliver((byte)OperationCode.QueuingRequest, packet);
            startgamtimer.SetActive(true);
            this.SetText("    STOP");
            state = true;
        }
        else
        {
            Dictionary<byte, object> packet = new Dictionary<byte, object>() 
            { { (byte)0, (byte)switchCode.QueueCanel }, };
            ConnectFunction.F.Deliver((byte)OperationCode.QueuingRequest, packet);
     
        }
    }
    private void CanelQueue(short returnCode)
    {
        if (returnCode == (short)1)
        {
            startgamtimer.SetActive(false);
            this.SetText("   START");
            state = false;
        }
    }
    
}
