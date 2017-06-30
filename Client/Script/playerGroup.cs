using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 包含自己。這是要一起遊戲的玩家資訊   
/// </summary>
static public class playerGroup{

    /// <summary>
    /// 這裡的key int 是 playerId 不是playerUid
    /// </summary>
    static public Dictionary<int, Baseplayer> playersGroup = new Dictionary<int, Baseplayer>();

    //考慮要不要刪掉group1和group2，盡量先不使用它
    static public Dictionary<int, Baseplayer> playersGroup1 = new Dictionary<int,Baseplayer>();
    static public Dictionary<int, Baseplayer> playersGroup2 = new Dictionary<int, Baseplayer>();

    /// <summary>
    /// 在playersGroup中，取得指定uid的Baseplayer
    /// </summary>
    /// <param name="myuid"></param>
    /// <returns></returns>
    static public Baseplayer GetBaseplayerInplayersGroup(int myuid)
    {        if (playersGroup.Count == 10)
        {
            foreach (KeyValuePair<int, Baseplayer> bp in playersGroup)
            {
                if (bp.Value.uid==myuid)
                {
                    return bp.Value;
                }
            }
        }
        return null;
    }
    /// <summary>
    /// 將playersGroup依照隊伍分給Group1和Group2
    /// </summary>
    static public void DestributeplayerGroupToOneAndTwo()
    {
        playersGroup1.Clear();
        playersGroup2.Clear();
        foreach(KeyValuePair<int,Baseplayer> player in playersGroup)
        {
            if (player.Value.teamNumber == 1)
                playersGroup1.Add(player.Value.playId, player.Value);
            else
                playersGroup2.Add(player.Value.playId - 5, player.Value);
        }
        foreach (KeyValuePair<int, Baseplayer> i in playersGroup1)
        {
            Debug.Log("playerGruop1 的資料 :" + i.Value.name);
        }
        foreach (KeyValuePair<int, Baseplayer> i in playersGroup2)
        {
            Debug.Log("playerGruop2 的資料 :" + i.Value.name);
        }

    }
    /// <summary>
    /// 回傳-1的話就是沒有，從0開始，到9。從playersGroup中取
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    static public int GetPlayerNumber(int uid)
        {
            int number = -1;
            if (playersGroup.Count ==10)
            {
                foreach (KeyValuePair<int,Baseplayer> player in playersGroup)
                {

                    if (uid == player.Value.uid)
                    {
                        return player.Value.playId;
                    }
                }
            }
            return number;
        }
    /// <summary>
    /// 回傳這個人是不是隊友
    /// </summary>
    /// <param name="TargetPlayerid">這裡是這個房間的playerID</param>
    /// <returns></returns>
    static public bool IsTeammate(int TargetPlayerid)
    {
        int myPlayerid = playerGroup.GetPlayerNumber((int)ConnectFunction.F.playerInfo.UID);
        if ((myPlayerid > 4 && TargetPlayerid > 4) || (myPlayerid <= 4 && TargetPlayerid <= 4))
        {
            return true;
        }
        else
            return false;
    }
}

