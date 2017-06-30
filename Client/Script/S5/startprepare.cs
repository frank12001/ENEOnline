using UnityEngine;
using System.Collections.Generic;

public class startprepare : MonoBehaviour {


    public GameObject[] heroprefab = new GameObject[3];
    public GameObject[] heroNoCrollerprefab = new GameObject[3];
    public GameObject[] ResurrectionPoint = new GameObject[10]; //復活點/出生點

    otherPlayerController otherPlayercontroller;
    void Awake() 
    {
        
    }
	// Use this for initialization
	void Start () {
        otherPlayercontroller = this.gameObject.GetComponent<otherPlayerController>();
        GameManager gameManager = this.gameObject.GetComponent<GameManager>();
        //做到進入遊戲房後出生heroprefab
        if (playerGroup.playersGroup.Count == 10)
        {
            byte whichplayer=0;
            
            //用 heroid 去出生英雄
            //用 playerid 決定出生的位置
            //出生後設定名子為 在 playerGroup.playersGroup 中的index
            byte myplayerid = (byte)playerGroup.GetPlayerNumber((int)ConnectFunction.F.playerInfo.UID);
            foreach (KeyValuePair<int,Baseplayer> player in playerGroup.playersGroup)
            {
                    int myuid = (int)ConnectFunction.F.playerInfo.UID;
                    GameObject hero;
                    Debug.Log("現在出生的 hero id = " + player.Value.heroId);
                    if (player.Value.uid == myuid)
                    {
                        hero = (GameObject)Instantiate(heroprefab[player.Value.heroId], ResurrectionPoint[whichplayer].transform.position, ResurrectionPoint[whichplayer].transform.rotation);
                        hero.name = player.Key.ToString();

                        GameManager.MyPlayerId =byte.Parse(player.Key.ToString());
                        GameManager.SenderGameobject = hero;

                    }
                    else
                    {
                        //如果這個hero不是自己控制的話，就是otherplayer
                        //將他加入otherPlayercontroller 裡的 Dictionary otherPlayers中
                        //用的key是 playerId = playernumber 
                        hero = (GameObject)Instantiate(heroNoCrollerprefab[player.Value.heroId], ResurrectionPoint[whichplayer].transform.position, ResurrectionPoint[whichplayer].transform.rotation);
                        hero.name = player.Key.ToString();
                        
                        //將索引加入otherPlayerController
                        otherPlayercontroller.otherPlayers.Add(player.Value.playId, hero);
                    }
                    //給予復活點
                    hero.GetComponent<SwordProperty>().ResurrectionPoint = new Vector3(ResurrectionPoint[whichplayer].transform.position.x, ResurrectionPoint[whichplayer].transform.position.y, ResurrectionPoint[whichplayer].transform.position.z);
                    //判斷是不是隊友，在呼叫createIcon
                    bool isteammate= playerGroup.IsTeammate(int.Parse(player.Key.ToString()));
                    gameManager.CreateIcon((byte)player.Value.heroId, hero, isteammate);
                whichplayer++;

            }
        }

        //在全部準備完後將自己刪除釋放記憶體//可能會導致預置物件丟失所以測試看看需不需要對已選的預置物件做保留
        //Destroy(this); 先測試其它的在測試她

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
