using UnityEngine;
using System.Collections;

/// <summary>
/// 基本的玩家單位。(其實就只是存玩家的屬性，隨時可新增)
/// </summary>
public class Baseplayer
{
    /// <summary>
    /// 給予值
    /// </summary>
    /// <param name="uid">玩家的uid</param>
    /// <param name="name">玩家遊戲名子</param>
    public Baseplayer(int uid, string name)
    {
        this.uid = uid;
        this.name = name;
        this.heroId = -1;
    }
    public int uid;
    public string name;
    //玩家在房間中的第幾個
    public byte playId;
    //是在哪一隊
    public byte teamNumber;
    public int heroId;
}
