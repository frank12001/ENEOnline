using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class Gaming_Time : MonoBehaviour
{

    
    #region Start() OnDestroy()
    void Start()
    {      
        ConnectFunction.F.Receive_Ping += this.Receive_Ping;
        Compute_Ping(); //進遊戲場景時先計算一次Ping
        #region 初始化 固定更改遊戲時間的timer
        //-----Set Timer
        // Create a timer with a two second interval.
        aTimer = new System.Timers.Timer(15);
        // Hook up the Elapsed event for the timer. 
        aTimer.Elapsed += gamingTime_Event;
        aTimer.AutoReset = true; //設定Elapsed要被執行不只一次
        aTimer.Enabled = true; //表示 Timer 是否應引發 Elapsed 事件
        #endregion
    }
    void OnDestroy()
    {
        ConnectFunction.F.Receive_Ping -= this.Receive_Ping;
    }
    #endregion

    #region 遊戲時間，Ping
    private float gamingTime = 0.0f; //真正紀載遊戲時間的參數 //除了每秒向Server算Ping實會更改，還要固定在timer裡更改
    #region 用System的timer 固定更改gamingTime
    private System.Timers.Timer aTimer;
    /// <summary>
    /// 用於計算 Ping ，time1 是會隨者時間增加的變數
    /// </summary>
    private float timer1 = 0.0f,timer2=0.0f; //用於計算 Ping ，time1 是會隨者時間增加的變數
    private void gamingTime_Event(object sender, System.Timers.ElapsedEventArgs e)
    {
        this.gamingTime += 0.015f;
        timer1 += 0.015f;
    }
    #endregion
    /// <summary>
    /// 唯獨變數
    /// </summary>
    public float GamingTime { get { return this.gamingTime; } } //用來給外部取遊戲時間用
    public float Ping = 0.0f; //存放目前算的Ping

    public Text text;
    #endregion 

    #region 一秒呼叫一次Compute_Ping計算Ping和同步時間
    private float last_time; private float now_time;
    #endregion

    void Update()
    {
        /* //用於測試Compute_Ping();功能
        if (Input.GetKeyDown(KeyCode.A)) 
            Compute_Ping();
         * */
        //如果大家都進入遊戲場景並準備好了
        now_time += Time.deltaTime;
        if ((int)now_time - (int)last_time >= 1) //一秒呼叫一次Compute_Ping， 計算Ping 和 跟伺服器時間同步
        {
            Compute_Ping();
            last_time = now_time;
        }
        //Debug.Log(" 現在遊戲時間 = " + GamingTime);
        text.text = "Ping = " + Ping;
    }

    #region 功能 Compute_Ping
    /// <summary>
    /// 開始計算Ping 
    /// </summary>
    /// <returns>false=timer開關要先關才能呼叫</returns>
    public void Compute_Ping()
    {
        this.timer2 = timer1;
        Dictionary<byte, object> packet = new Dictionary<byte, object> 
        {
           { (byte)0, (byte)2 }, //switch Code
        };
        ConnectFunction.F.Deliver((byte)EventCode.Receive_GamingRoomMessages, packet);
    }
    #endregion
    #region 事件 Receive_Ping
    /// <summary>
    /// 接收伺服氣回傳並計算總共消耗的時間。如果回傳封包裡有伺服器時間的話，做時間同步
    /// </summary>
    /// <param name="packet"></param>
    public void Receive_Ping(Dictionary<byte,object> packet)
    {
        try  //如果伺服器傳來的封包中有Server的遊戲時間的話，用它修改這個客戶端的遊戲時間
        {
            float server_Time = (float)packet[1]; //接收 Server 現在的遊戲時間
            this.gamingTime = server_Time + ((timer1 - this.timer2) / 2); //計算現在的遊戲時間 用伺服器發過來的時間-(Ping/2)
            //Debug.Log("Server time " + server_Time + " Ping = " + (timer1 - this.timer2));
            //Debug.Log(" 算出的遊戲時間 " + this.gamingTime);
        }
        catch (Exception e)  //如果伺服器傳來的封包中沒有Server的遊戲時間的話
        {
            Debug.Log(e.Message);
        }
        finally //一定會將算好的Ping傳給伺服器
        {
            this.Ping = timer1 - this.timer2; 
            Dictionary<byte, object> packet1 = new Dictionary<byte, object> 
            {
                {(byte)0,(byte)3}, //switch code
                {(byte)1,this.Ping},
            };
            ConnectFunction.F.Deliver((byte)EventCode.Receive_GamingRoomMessages, packet1); //將計算好的 Ping傳回給 伺服器
        }
    }
    #endregion


}
