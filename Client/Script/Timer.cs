using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{

    /// <summary>
    /// 記錄現在記時的時間
    /// </summary>
    public float NowTime { get; private set; }

    /// <summary>
    /// 控制計時器的開關
    /// </summary>
    public bool TimerOn;
    // Use this for initialization
    void Start()
    {
        this.NowTime = 0.0f;
        TimerOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (TimerOn)
            NowTime = NowTime + Time.deltaTime;
    }
    /// <summary>
    /// 檢察這個計時器時部是在被使用
    /// </summary>
    /// <returns>有沒有</returns>
    public bool IsUsing()
    {
        if (TimerOn) //檢查是不是打開的
            return false;
        if (this.NowTime > 0) //檢查是不是有在記時了
            return false;
        return true;
    }
    /// <summary>
    /// 歸零
    /// </summary>
    public void Zero()
    {
        NowTime = 0.0f;
    }
    /// <summary>
    /// 直接跳到指定的時間
    /// </summary>
    /// <param name="time">要跳到的時間</param>
    public void ToAssignTime(float time)
    {
        NowTime = time;
    }

    /// <summary>
    /// 停止記時並歸零
    /// </summary>
    public void StopAndReset()
    {
        TimerOn = false;
        Zero();
    }
}
