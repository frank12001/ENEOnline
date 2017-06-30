using System.Collections.Generic;

/// <summary>
/// 玩家基本資料
/// </summary>
public class Data{

    public Data()
    {
        FriendsList = new Dictionary<int, string>();
        FriendsState = new Dictionary<int, bool>();
    }
    //玩家基本資料
    public string name;
    public int level, energy,laststate;
    public string Property, herostring;
    //玩家好友資料
    public Dictionary<int, string> FriendsList; //<int,string> = <uid,name>
    public Dictionary<int, bool> FriendsState;  // online or offline

    /// <summary>
    /// 替換現有的"好友清單"
    /// </summary>
    /// <param name="friendList">要替換的 好友清單</param>
    /// <param name="friendState">要替換的 好友狀態 </param>
    public void InsertInfriendslist(Dictionary<int, string> friendList, Dictionary<int, bool> friendState) 
    {
        FriendsList.Clear();
        FriendsState.Clear();
        if (friendList.Count != friendState.Count)
            return;
        foreach (KeyValuePair<int,string> item in friendList)
        {
            if (item.Key == ConnectFunction.F.playerInfo.UID)
                continue;

            FriendsList.Add(item.Key, item.Value);
            bool b = false;
            friendState.TryGetValue(item.Key,out b);
            FriendsState.Add(item.Key,b);
        }
    }
    /// <summary>
    /// 增加一位好友到好友清單中
    /// </summary>
    /// <param name="uid">此玩家的 uid</param>
    /// <param name="name">此玩家的 name</param>
    /// <param name="state">此玩家的 在線狀態</param>
    public void AddFriends(int uid,string name,bool state)
    {
        if (!FriendsList.ContainsKey(uid))
        {
            FriendsList.Add(uid, name);
            FriendsState.Add(uid, state);
        }
    }
    /// <summary>
    /// 從 FriendsList 裡，藉由 Name 取得 Uid
    /// </summary>
    /// <param name="name">要取得誰的 Uid</param>
    /// <returns>該人的 Uid</returns>
    public int GetUidbyNameInfriendslist(string name) //如果都沒有的話回傳-1
    {
        int result=-1;
        foreach (KeyValuePair<int, string> item in FriendsList)
        {
            if (item.Value.Equals(name))
            {
                result = item.Key;
                break;
            }
        }
        return result;
    }
}
