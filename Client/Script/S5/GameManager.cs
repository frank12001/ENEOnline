using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{


    #region 存放"小圖示"的陣列
    //存放"小圖示"的陣列，預計每個"種類"、英雄製作一種
    //目前暫時一個是fist 一個是Sword的
    //這裡的排序要和在GameManager中的英雄預置物件索引一樣
    public GameObject[] IconPrefabs = new GameObject[3]; //Icon物件都要加入IconFollow.cs
    private MiniMapMask miniMapMask; //MaskMap上的Script

    #endregion

    #region 浮動文字使用的參數(這裡的都為const)
    /// <summary>
    /// 浮動文字DataPool陣列的長度
    /// </summary>
    private const byte dataPoolCountTextMesh=20;
    /// <summary>
    /// 浮動文字與相機的距離
    /// </summary>
    private const float textDistanceWithCamera = 0.2f;
    /// <summary>
    /// 浮動文字存活的時間
    /// </summary>
    private const float textExistTime = 2.0f;
    /// <summary>
    /// 當新的浮動文字出現時，它的y,z變化量(稍微變一點讓浮動文字們看起來有層次)
    /// </summary>
    private const float yTextmove = 0.014f, zTextmove = 0.01f;
    /// <summary>
    /// 只是一個定值，用於將Text.pos.y+到敵人頭附近(可能要改)
    /// </summary>
    private const float yTextmove2=0.1f;
    #endregion

    #region 移動狀態
    /// <summary>
    /// 移動狀態
    /// </summary>
    public enum Motion : byte { FrontGo,FrontStop,
                                BackGo,BackStop,
                                RightGo,RightStop,
                                LeftGo,LeftStop,
                                SyncPosition
                              };
    #endregion

    void Start()
    {
        otherplayercontroller = this.gameObject.GetComponent<otherPlayerController>();
        //取得Script
        miniMapMask = GameObject.Find("MaskMap").GetComponent<MiniMapMask>();
        //做"文字"(用於打人時顯示傷害) 的 DataPool
        dataPoolTextMesh = new GameObject[dataPoolCountTextMesh];
        for(int i=0;i<dataPoolTextMesh.Length;i++)
        {
            dataPoolTextMesh[i] = Instantiate(Text, Vector3.zero, Quaternion.identity) as GameObject;
            dataPoolTextMesh[i].name = "0";
            dataPoolTextMesh[i].SetActive(false);
        }
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        textInScene = new List<GameObject>();
    }

    #region static 殺人數死亡數增加energy
    /// <summary>
    /// Sender's player id
    /// </summary>
    static public byte MyPlayerId; //在startprepare 中設定完


    static public byte[] Kill = new byte[10]; //殺人數
    static public byte[] Death = new byte[10]; //死亡數

    static public GameObject SenderGameobject; //在startprepare 中設定完
    static private otherPlayerController otherplayercontroller; 

    //增加指定玩家的Energy
    static public void IncreateAssignPlayerEnergy(byte playerid,byte increatevalue)
    {
        if (MyPlayerId == playerid)
            SenderGameobject.GetComponent<SwordProperty>().Energy = (byte)(SenderGameobject.GetComponent<SwordProperty>().Energy + increatevalue);
        else
        {
            if (otherplayercontroller.otherPlayers.ContainsKey(playerid))
                otherplayercontroller.otherPlayers[playerid].GetComponent<SwordProperty>().Energy = (byte)(otherplayercontroller.otherPlayers[playerid].GetComponent<SwordProperty>().Energy + increatevalue);
        }
    }
    #endregion

    #region static 增加文字(用於打人時顯示傷害)
    public GameObject Text;
    static private GameObject[] dataPoolTextMesh;
    static private byte dataPoolIndex = 0;
    static private Camera mainCamera;
    static private List<GameObject> textInScene;
    /// <summary>
    /// 現在在場上有幾個text
    /// </summary>
    static private byte textNumberInScene=0;
    
    /// <summary>
    /// 主要用於在AttackObject 撞到目標時呼叫。
    /// </summary>
    /// <param name="text">要顯示的文字</param>
    /// <param name="targetposition">顯示的位置WorldPosition</param>
    static public void CreateText(float text, Vector3 targetposition)
    {
        textNumberInScene++;
        //每次使用都要更改dataPoolIndex
        if (dataPoolIndex == dataPoolCountTextMesh - 1)
            dataPoolIndex = 0;
        else
            dataPoolIndex++;
        //將它加入textInScene
        textInScene.Add(dataPoolTextMesh[dataPoolIndex]);
        GameObject textObj = dataPoolTextMesh[dataPoolIndex];
        
        textObj.SetActive(true);
        textObj.name = "0";

        Vector3 v = mainCamera.ScreenPointToRay(mainCamera.WorldToScreenPoint(targetposition)).GetPoint(textDistanceWithCamera - (textNumberInScene * zTextmove));
        v.y = v.y + yTextmove2 + (textNumberInScene * yTextmove);
        Debug.Log("這個浮動文字的位置為 x" + v.x + "  y : " + v.y + "  z : " + v.z);
        textObj.transform.position = v;
        textObj.transform.rotation = mainCamera.gameObject.transform.rotation;
        textObj.GetComponent<TextMeshPro>().text = text.ToString();
        
    }
    void Update()
    {
        #region Text 的消失 用name記錄時間 textExistTime為存在的時間
        if (textInScene.Count > 0)
        {
            List<byte> deleteIndex = new List<byte>();
            byte index = 0;
            foreach (GameObject text in textInScene)
            {
                text.name = (float.Parse(text.name) + Time.deltaTime).ToString();
                if (float.Parse(text.name) >= textExistTime)
                {
                    text.name = "0";
                    text.SetActive(false);
                    deleteIndex.Add(index);
                    index++;
                }
            }
            deleteIndex.Reverse();
            if (deleteIndex.Count > 0)
            {
                foreach (byte b in deleteIndex)
                {
                    textInScene.RemoveAt(b);
                    textNumberInScene--;
                }
            }
            
        }
        if (textInScene.Count == 0 && textNumberInScene > 0)
        {
            textNumberInScene = 0;
        }
        #endregion
    }
    #endregion


    /// <summary>
    /// 製造Icon 。 加入miniMapMask的群集後，會在minimapmask中發亮
    /// </summary>
    /// <param name="iconNumber">icon預置物件在陣列的索引，要和startprepare中的英雄預置物件索引一樣</param>
    /// <param name="follow">icon 要跟隨的GameObject，在加入miniMapMask的群集之後，使用它的名子做為索引</param>
    /// <param name="IsTeammate">是不是我的隊友，決定要不要加入miniMapMask的群集</param>
    public void CreateIcon(byte iconNumber, GameObject follow,bool IsTeammate)
    {
        if (!miniMapMask.ContainInTeamates(follow.name)) //如果沒有在裡面才進入
        {
            GameObject icon = Instantiate(IconPrefabs[iconNumber]) as GameObject;
            if (icon == null)
                return;
            IconFollow iconScript = icon.GetComponent<IconFollow>();
            iconScript.StartFollow(follow, 7.1f,IsTeammate);
            if (IsTeammate)
            {
                miniMapMask.AddTeammate(follow.name, new Center(icon));
                iconScript.AssignLayer((byte)enumLayers.normal);
            }
            else
            {
                iconScript.AssignLayer((byte)enumLayers.Hide);
            }
        }
    }

    /// <summary>
    /// 結束遊戲時呼叫。填滿GameOverInfo裡的資訊，釋放記憶體。由BLO<=0的主堡呼叫 //In tower1Property.cs
    /// </summary>
    /// <param name="whoLose">哪對輸了</param>
    public void FinishGame(byte whoLose)
    {
        //不在發送角色位置資訊
        GameManager.SenderGameobject.GetComponent<Sender>().enabled = false;

        #region 將資料擺進GameOverInfo.cs裡 
        
        //取得是不是我贏
        byte senderTeam = GameManager.SenderGameobject.GetComponent<IPropertyBasic>().TEAM;
        if (whoLose != senderTeam)
            GameOverInfo.IAmWin = true;
        else
            GameOverInfo.IAmWin = false;
        //取得我在此遊戲中得PlayerNumber。用於計算獎勵
        GameOverInfo.MyPlayerNumber = byte.Parse( GameManager.SenderGameobject.name);
        //將殺人數加入
        for (int i = 0; i < GameManager.Kill.Length; i++)
        {
            GameOverInfo.KillNumber[i] = GameManager.Kill[i];
        }
        //加死亡數加入
        for (int i = 0; i < GameManager.Death.Length; i++)
        {
            GameOverInfo.DeathNumber[i] = GameManager.Death[i];
        }


        //將uid、name加入
        byte count=0;
        foreach (KeyValuePair<int, Baseplayer> player in playerGroup.playersGroup)
        {
            GameOverInfo.NameParticipator[count] = player.Value.name;
            GameOverInfo.uidList[count] = player.Value.uid;
            count++;
        }

        #endregion

        #region releace memory
        playerGroup.playersGroup.Clear();
        playerGroup.playersGroup1.Clear();
        playerGroup.playersGroup2.Clear();
        #endregion

        //進入S6
        Application.LoadLevel("S6");
    }
}
