using UnityEngine;
using System.Collections.Generic;

public class chatController : MonoBehaviour {

    /// <summary>
    /// 正在聊天的人 資料的集合 uid 和 mainchat(實體物件)
    /// </summary>
    static public Dictionary<int, mainchat> chatingCollection;
    public GameObject chatPrefab;
    void Awake(){
        chatingCollection = new Dictionary<int, mainchat>();
    }
	// Use this for initialization
	void Start () {
        ConnectFunction.F.chatevent +=this.chatevent;
        ConnectFunction.F.chateventE += this.chateventE;
        ConnectFunction.F.stateevent += this.stateevent;
	}

    public void OnDestroy()
    {
        ConnectFunction.F.chatevent -= this.chatevent;
        ConnectFunction.F.chateventE -= this.chateventE;
        ConnectFunction.F.stateevent -= this.stateevent;
    }
    #region 委派
    /// <summary>
    /// 如果自己是發送者的話
    /// </summary>
    /// <param name="returncode"></param>
    /// <param name="packet"></param>
    public void chatevent(short returncode, Dictionary<byte, object> packet)
    {
        switch (returncode)
        {
            case 0:
                Vector3 chatposition = CalculatechatPos();
                GameObject chat = (GameObject)Instantiate(chatPrefab,chatposition,chatPrefab.transform.rotation);
                mainchat chatm = chat.GetComponent<mainchat>();
                
                var Tuid = int.Parse(packet[0].ToString());
                var Tname = packet[1].ToString();
                Debug.Log(Tuid);
                Debug.Log(Tname);
                chatm.SetUid(Tuid);
                chatm.SetName(Tname);
                chatingCollection.Add(Tuid, chatm);
                chatm.chat1.SetTopic(packet[1].ToString());

                //chatm.gchat2.SetActive(true);
                chatm.chat2.SetTopic(packet[1].ToString());
                Debug.Log(packet[1].ToString());
                //chatm.gchat2.SetActive(false);
                break;
            case 1:
                //該玩家不在線上
                //測試用暫時做在這邊
                /*Vector3 chatposition = CalculatechatPos();
                GameObject chat = (GameObject)Instantiate(chatPrefab,chatposition,chatPrefab.transform.rotation);
                mainchat chatm = chat.GetComponent<mainchat>();
                
                var Tuid = int.Parse(packet[0].ToString());
                var Tname = packet[1].ToString();
                Debug.Log(Tuid);
                Debug.Log(Tname);
                chatm.SetUid(Tuid);
                chatm.SetName(Tname);
                chatingCollection.Add(Tuid, chatm);
                chatm.chat1.SetTopic(packet[1].ToString());
                Debug.Log("回來了");
                 * */
                break;
        }
    }
    /// <summary>
    /// 處理別人發過來的字串
    /// </summary>
    /// <param name="packet"></param>
    public void chateventE(Dictionary<byte, object> packet)
    {
        var Suid = int.Parse(packet[0].ToString());
        if (chatController.chatingCollection.ContainsKey(Suid))
        {
            mainchat Smainchat = chatingCollection[Suid];
            if (Smainchat.whoActive().Equals(2)) // 如果是chat2 active的話
            {
                Smainchat.Setchat(packet[4].ToString());
            }
            else // 如果是 chat1 active的話 // 存入 list中 // 當按下後，在set到chat2 中
            {
                Smainchat.chatstorage.Add(packet[4].ToString());
                //chat1 startSpark
                Smainchat.chat1Spark(true);
            }
        }
        else
        {
            Vector3 chatposition = CalculatechatPos();
            GameObject chat = (GameObject)Instantiate(chatPrefab, chatposition, chatPrefab.transform.rotation);
            mainchat chatm = chat.GetComponent<mainchat>();

            var Tuid = int.Parse(packet[0].ToString());
            var Tname = packet[1].ToString();
            Debug.Log(Tuid);
            Debug.Log(Tname);
            chatm.SetUid(Tuid);
            chatm.SetName(Tname);
            chatingCollection.Add(Tuid, chatm);
            chatm.chat1.SetTopic(packet[1].ToString());
            chatm.chat2.SetTopic(packet[1].ToString());

            chatm.chatstorage.Add(packet[4].ToString());
            //chat1 startSpark
            chatm.chat1Spark(true);
        }
    }
    /// <summary>
    /// 當好友上線時傳入，要求更改狀態
    /// </summary>
    /// <param name="packet"></param>
    public void stateevent(Dictionary<byte, object> packet)
    {
        if (int.Parse(packet[1].ToString()).Equals(0))
        {
            var Suid = int.Parse(packet[0].ToString());
            if (DataManager.datapool.FriendsState.ContainsKey(Suid))
                DataManager.datapool.FriendsState[Suid] = true;
        }
        else
        {
            var Suid = int.Parse(packet[0].ToString());
            if (DataManager.datapool.FriendsState.ContainsKey(Suid))
                DataManager.datapool.FriendsState[Suid] = false;
        }
        ButtonSpark.Spark(true);
    }
    #endregion
    #region myfunction

    /// <summary>
    /// 計算chat要在哪Instantiate出來，傳入現在是第幾個人=chatingCollection.count
    /// </summary>
    /// <param name="chatnumber"></param>
    /// <returns></returns>
    public Vector3 CalculatechatPos()
    {
        Vector3 result;
        result = new Vector3(Random.Range(-0.37f, 1.03f), Random.Range(0.72f, 1.96f), -8.38f);
        return result;
    }
    public void chooseOther(int UID)
    {
        foreach (KeyValuePair<int, mainchat> item in chatingCollection)
        {
            if (!item.Value.GetUid().Equals(UID))
            {
                item.Value.controllchat(1);
            }
        }
    }
    #endregion

    
}
