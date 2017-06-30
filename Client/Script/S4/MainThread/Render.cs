using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Script.S4.Character;
using System.Timers;
using System;
using TMPro;

namespace Assets.Script.S4.MainThread
{
    public class Render : MonoBehaviour
    {
        #region 技術債
        /*技術債-Bug*/

        /*技術債-not Do*/
        /*
         * 1. 實做 Effect_Death(playerid) , Effect_ALive(playerid)  。 死亡和復活效果
         */
        #endregion
        void Start()
        {
            #region 設定這個腳本的參數

            #region 用於設定物件角度的陣列
            float degrees = 0.0f;
            for (byte i = 1; i < direction_Vector3.Length; i++) //從1開始，0不用
            {
                direction_Vector3[i] = new Vector3(0.0f,degrees, 0.0f);
                degrees += 11.25f; //11.25度 是32方位角 之間的間格
            }
            #endregion

            #region 介面設定 MouseVisible
            MouseVisible = Mouse_Visible;
            #endregion

            #region 用於打人時顯示傷害的文字
            //做"文字"(用於打人時顯示傷害) 的 DataPool
            dataPoolTextMesh = new GameObject[dataPoolCountTextMesh];
            for (int i = 0; i < dataPoolTextMesh.Length; i++)
            {
                dataPoolTextMesh[i] = Instantiate(Floating_Text, Vector3.zero, Quaternion.identity) as GameObject;
                dataPoolTextMesh[i].name = "0";
                dataPoolTextMesh[i].SetActive(false);
            }
            mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            textInScene = new List<GameObject>();
            #endregion

            #endregion

            #region 利用暫存區的資料初始化
            this.operatorNumber = (byte)Temporary_Storage._Byte;   //讀取暫存區的資料
            Gamer[] gamers = Temporary_Storage._WorldState.Gamers; //讀取暫存區的資料
            #region 初始化 內插陣列
            position_Before = new Vector3[gamers.Length];          //初始化 內插陣列
            position_Latest = new Vector3[gamers.Length];          //初始化 內插陣列
            duraiotnTime = new float[gamers.Length];               //初始化 內插陣列
            for (byte i = 0; i < duraiotnTime.Length; i++)         //初始化 內插陣列
            {
                duraiotnTime[i] = 1.0f;
            }
            durationTime_Accumulate = new float[gamers.Length];    //初始化 內插陣列
            for (byte i = 0; i < durationTime_Accumulate.Length; i++)//初始化 內插陣列
            {
                durationTime_Accumulate[i] = 1.0f;
            }
            characters = new Basic_Character[gamers.Length];       //初始化玩家遊戲物件的控制腳本陣列
            #endregion 
            #region 初始化玩家資訊
            for (byte i = 0; i < gamers.Length; i++)
            {
                //------利用暫存區的資料初始化
                GameObject g = Resources.Load(heroAsset_Path[gamers[i].heroid], typeof(GameObject)) as GameObject; //讀取遊戲模型
                Vector3 position = ToVector3(gamers[i].Information_position.Position);                             //位置初始化
                Sample_Input.Direction rotation = (Sample_Input.Direction)gamers[i].Information_position.Rotation; //方向初始化
                GameObject gamer = Instantiate(g, position, Quaternion.identity) as GameObject;                    //遊戲物件初始化
                gamer.transform.eulerAngles = direction_Vector3[(byte)rotation];                                   //遊戲物件方向初始化
                this.gamers_GameObject.Add(gamer);                                                                 //加入索引陣列
                position_Before[i] = position;                                                                     //初始化內插用陣列位置
                position_Latest[i] = position;                                                                     //初始化內插用陣列位置
                //------設定 gamer 裡 Basic_Character 的初始值
                Basic_Character character = gamer.GetComponent<Basic_Character>();
                character.PlayerId = i;                                                                            //設定 Player Id
                character.OperatorId = operatorNumber;                                                             //設定 Operator Id
                character.Set_CharacterInformation(gamers[i].Information_character.Hp, gamers[i].Information_character.Animation, 
                                                   gamers[i].Information_character.Atk, gamers[i].Information_character.MoveSpeed, 
                                                   gamers[i].Information_character.SkillCd);                       //能力值初始化
                characters[i] = character;    //將Basic_Character存入這個腳本中的索引
                if (i == operatorNumber)
                {
                    character.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                    Camera_Center_Point.transform.position = this.gamers_GameObject[i].transform.position; //將相機 中心 的位置等於這個物件
                    Camera_Center_Point.transform.parent = this.gamers_GameObject[i].transform;            //將相機 指定給 遊戲物件

                }

            }
            whichMembersInAGame = (byte)gamers.Length;             //將有多少玩家存入此參數
            beforeHP = new short[whichMembersInAGame];             //存放前一偵的血量
            for (byte i = 0; i < beforeHP.Length; i++)             //隨便初始化一個值，在接收到第一個世界狀態就會更改
            {
                beforeHP[i] = 100;
            }
            #endregion 
            #region 初始化 Npc 資訊
            Gamer[] npcs_characters = Temporary_Storage._WorldState.Npcs;  //讀取暫存區資料

            npcs = new Basic_Character[npcs_characters.Length];       //初始化玩家遊戲物件的控制腳本陣列
            for (byte i = 0; i < npcs_characters.Length; i++)
            {

                #region 讀取遊戲模型 i <= 1，讀取Tower模型
                GameObject g;
                if (i <= 1) //0 和 1 是塔 //只要改 1 紅方塔的 E
                {
                    if (i == 0)
                        g = Resources.Load("S4/Tower/TowerBall0", typeof(GameObject)) as GameObject;
                    else //(i == 1)
                        g = Resources.Load("S4/Tower/TowerBall1", typeof(GameObject)) as GameObject;
                }
                else //讀取其他的 NPC //目前只有塔
                {
                    g = Resources.Load("S4/Tower/TowerBall", typeof(GameObject)) as GameObject;
                }
                #endregion
                Vector3 position = ToVector3(npcs_characters[i].Information_position.Position);    //位置初始化
                GameObject npc = Instantiate(g, position, Quaternion.identity) as GameObject;      //遊戲物件初始化
                npc.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);                         //遊戲物件方向
                this.npcs_GameObject.Add(npc);                                                     //加入索引陣列
                //------設定 gamer 裡 Basic_Character 的初始值
                Basic_Character character = npc.GetComponent<Basic_Character>();
                character.Set_CharacterInformation(npcs_characters[i].Information_character.Hp, npcs_characters[i].Information_character.Animation,
                                                   npcs_characters[i].Information_character.Atk, npcs_characters[i].Information_character.MoveSpeed,
                                                   npcs_characters[i].Information_character.SkillCd);                       //能力值初始化
                npcs[i] = character;    //將Basic_Character存入這個腳本中的索引

            }
            #endregion

            #endregion

            #region 初始化完畢 嘗試將暫存的空間釋放 (由於ReceiveAndStoreWorldStates.cs也要使用這個世界狀態進行初始化，所以增加一個布林判斷，可不可以刪除)
                if (Temporary_Storage._Bool == null)
                {
                    Temporary_Storage._Bool = true;//用來當作 初始化得資料能不能被刪除的索引
                    Debug.Log("Temporary_Storage._Bool  null");
                }
                else
                {
                    Debug.Log("Temporary_Storage._Bool  true");
                    Dictionary<byte, object> packet = new Dictionary<byte, object>
                {
                    {(byte)0,(byte)1},
                    {(byte)1,(byte)Temporary_Storage._Byte},
                };
                    ConnectFunction.F.Deliver((byte)13, packet);
                    Debug.Log("Temporary_Storage._Bool  out");
                    Temporary_Storage._Byte = null;
                    Temporary_Storage._WorldState = null;
                    Temporary_Storage._Bool = null;
                }
            #endregion

            #region UI 物件初始設定
            #region 關閉無用物件
                MidTop_HPBarEnemy_Frame.SetActive(false);
                MidTop_Icon_Kill.SetActive(false);
            #endregion
            #region 初始化參數
            maxcd = new float[5];
            for(byte i=0;i<maxcd.Length;i++)
            {
                maxcd[i] = 0.0f; 
            }

            startfixTime = Time.fixedTime;
            #endregion
            #region 初始化兩對殺人數
            MidTop_Red_Text.GetComponent<Text>().text = killedNumber_team1.ToString();
            MidTop_Blue_Text.GetComponent<Text>().text = killedNumber_team2.ToString();
            #endregion
            #region 關掉 Operator角色用不到的UI
            byte operatorHeroid = gamers[operatorNumber].heroid;
            switch (operatorHeroid)
            {
                case 1: //Cat Robot
                    GameObject.Find("MidButton_Button_Mouse1").SetActive(false);
                    GameObject.Find("MidButton_Button_Q").SetActive(false);
                    GameObject.Find("MidButton_Button_F").SetActive(false);
                    break;
                case 2: //Boy Robot
                    GameObject.Find("MidButton_Button_Mouse1").SetActive(false);
                    GameObject.Find("MidButton_Button_Q").SetActive(false);
                    GameObject.Find("MidButton_Button_F").SetActive(false);
                    break;
                default:
                    break;
            }
            #endregion

            #endregion

            #region 修改相機方向
            if (operatorNumber == 0 || operatorNumber == 1)
                Camera_Center_Point.transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
            else
                Camera_Center_Point.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            #endregion 

        }
        void Update() //將會用到 Update 這個函式的功能寫進來
        {
            #region 各個功能中需要用到 Update() 的函式統一放到這裡執行
            #region render position (內插)
            render_PositionInUpdate();
            #endregion

            #region 子彈 Render 
            /*
             * 將收到的子彈陣列，做成顯示用的子彈。
             * 子彈只有顯示，血量的變動在伺服器中做完。
             */
            List<byte> index_toRemove = new List<byte>();

            foreach (Bullet bullet in bullets_Propertys_InScene)
            {
                switch (bullet.State)
                {
                    #region 剛出生狀態
                    case 1:       //現在是剛出生狀態
                        if (!bullets_Objects_InScene.ContainsKey(bullet.Unique_Number))   //如果這個子彈 剛出生狀態 + 沒有在場景中被 Render 
                        {
                            /*
                             * 
                             */
                            string bulletPrefab_Path = bulletsAsset_Path + bullet.BulletMeshNumber;                      //bullet.BulletMeshNumber = 子彈預置物件的名子
                            GameObject g = Resources.Load(bulletPrefab_Path, typeof(GameObject)) as GameObject;          //讀取子彈模型
                            Vector3 position = ToVector3(bullet.Position_now);                                           //初始化位置
                            GameObject bullet_object = Instantiate(g, position, Quaternion.identity) as GameObject;      //初始化遊戲物件
                            bullet_object.transform.LookAt(ToVector3(bullet.Position_now) + ToVector3(bullet.Direction));//初始化角度
                            //bullet_object.transform.eulerAngles = direction_Vector3[bullet.Direction];          
                            bullets_Objects_InScene.Add(bullet.Unique_Number, bullet_object);                            //加入索引字典
                            bullet.State = 2;                                                                            //讓它變成移動狀態
                            bullet_object.AddComponent<AutoDestroy>().AlifeSeconds = bullet.LifeTime_Max * 2;
                                
                            Debug.Log("現在出生了子彈 uniqueNumber = " + bullet.Unique_Number);
                        }
                        break;
                    #endregion
                    #region 移動狀態
                    case 2:       //現在是移動狀態
                        if (bullets_Objects_InScene.ContainsKey(bullet.Unique_Number))   //如果這個子彈 移動狀態 + 有在場景中被 Render 
                        {
                            //讓他移動
                            Vector3 position = ToVector3(bullet.Position_now);
                            if (bullets_Objects_InScene[bullet.Unique_Number]!=null)
                                bullets_Objects_InScene[bullet.Unique_Number].transform.position = position;

                        }
                        else if (!bullets_Objects_InScene.ContainsKey(bullet.Unique_Number)) //如果這個子彈 移動狀態 + 沒有在場景中被 Render  //代表有掉過包 ， 原本在狀態 剛出生 時，掉了包 //所以沒有出生到
                        {
                            /*
                             * 將他出生 並移到現在的位置
                             */
                            goto case 1;
                        }
                        break;
                    #endregion
                    #region 剛撞到人，要爆炸的狀態
                    case 3:       //現在是剛撞到人，要爆炸的狀態
                        Debug.Log("state = 3");
                        if (bullets_Objects_InScene.ContainsKey(bullet.Unique_Number))  //如果這個子彈 剛撞到人，要爆炸的狀態 + 有在場景中被 Render 
                        {
                            index_toRemove.Add(bullet.Unique_Number);

                            Effect_Explosion(ToVector3(bullet.Position_now), bullet.BulletMeshNumber); //創造爆炸效果
                        }
                        break;
                    #endregion 
                }

            }

            #region 如果 要刪除子彈(staet=3)的封包掉了，的處理
            foreach (Bullet bullet_last in bullets_Propertys_InScene_Last) //檢查 上一偵存在的子彈，在這偵是否還在
            {
                bool bullet_last_InNow = false;
                foreach (Bullet bullet_now in bullets_Propertys_InScene) 
                {
                    if (bullet_last.Unique_Number == bullet_now.Unique_Number) //這偵是否還在
                    {
                        bullet_last_InNow = true;
                        break;
                    }
                }
                if (!bullet_last_InNow && bullets_Objects_InScene.ContainsKey(bullet_last.Unique_Number)) //如果上一偵在，這一偵不在。 可是 子彈物件卻還在場上。 (代表有掉包)
                {
                    index_toRemove.Add(bullet_last.Unique_Number);  //加入刪除清單
                    Effect_Explosion(ToVector3(bullet_last.Position_now), bullet_last.BulletMeshNumber); //創造爆炸效果
                }
            }
            #endregion
            #region 刪除 清單中的子彈實體
            try
            {
                foreach (byte uniqueNumber in index_toRemove) //刪除  bullets_Objects_InScene 裡的子彈
                {
                    Debug.Log(" 現在的字典長度  = " + bullets_Objects_InScene.Count + "。 現在index_toRemove裡得值 = " + uniqueNumber);
                    GameObject bullet_object = bullets_Objects_InScene[uniqueNumber];
                    bullets_Objects_InScene.Remove(uniqueNumber);
                    Destroy(bullet_object);

                }
            }
            catch (Exception e)
            {
                Debug.Log(" 在刪除子彈時錯誤 ， 錯誤訊息是 " + e.Message);
            }
            #endregion 

            bullets_Propertys_InScene_Last = bullets_Propertys_InScene; //這偵處理完後，將它弄進去。處理時有用這兩包的差異做判斷。

            #endregion 

            #region 打到人後出現浮動文字處理
            floating_Text(); //Text 的消失 用name記錄時間 textExistTime為存在的時間
            #endregion 

            #region UI 處理
            #region 遊戲時間
            float minute = ((uint)(Time.fixedTime - startfixTime)) / 60;
            float second = ((uint)(Time.fixedTime - startfixTime)) % 60;
            MidTop_Timer_Text.GetComponent<Text>().text = minute.ToString() + ":" + second.ToString();
            #endregion 
            #region Skill Button
            
            float[] skillcd = characters[operatorNumber].skillCd;
            for (byte i = 0; i < skillcd.Length; i++)
            {
                float percentage = 0.0f;
                if (skillcd[i] > maxcd[i])
                    maxcd[i] = skillcd[i];
                if (maxcd[i] > 0 && skillcd[i] > 0)
                    percentage = skillcd[i] / maxcd[i];
                setCDTime(Button_UI[i], percentage, false);
                //setCDTime(Button_UI[i], 0, false);
            }
            #endregion
            #region HP
            float percentage_hp =0.0f;
            float nowHP = (float)characters[operatorNumber].GetHP();
            if(nowHP > maxHP)
                maxHP = nowHP;
            if (maxHP > 0 && nowHP > 0)
                percentage_hp = nowHP / maxHP;
            setCDTime(HPBar_Blood, percentage_hp, true);
            
            #endregion
            #region 殺人數 顯示
            for (byte i = 0; i < characters.Length; i++) //檢查誰死了 在對面的殺敵數 + 1 。 用 wasKill記錄死過而且記過殺人數的人。
            {
                if (characters[i].GetHP() <= 0)
                {
                    if (wasKill.Contains(i))
                        break;
                    if (i == 0 || i == 1)
                        killedNumber_team2 += 1;
                    else
                        killedNumber_team1 += 1;
                    wasKill.Add(i);
                    MidTop_Blue_Text.GetComponent<Text>().text = killedNumber_team1.ToString();
                    MidTop_Red_Text.GetComponent<Text>().text = killedNumber_team2.ToString();                   

                }
                else
                {
                    if (wasKill.Contains(i))
                        wasKill.Remove(i);
                }
            }
            #endregion
            #endregion
            #endregion
        }

        #region 參數
        // Hero string 從0 開始

        /// <summary>
        /// 這台電腦操作者的 Number
        /// </summary>
        private byte operatorNumber;
        [SerializeField]
        private byte whichMembersInAGame;

        /// <summary>
        /// 相機物件中心點
        /// </summary>
        public GameObject Camera_Center_Point; 
       
        #region 設定滑鼠能不能被看到 ， 直接設定MouseVisible。 MouseVisible = true(看得到)、MouseVisible = false(看不得)
        private bool mouseVisible = true;
        public bool MouseVisible
        {
            get { return mouseVisible; }
            set
            {
                mouseVisible = value;
                Cursor.visible = mouseVisible;
            }
        }

        public bool Mouse_Visible; //能夠透過介面設定
        #endregion

        /// <summary>
        /// 用來存放 heroAsset 的路徑
        /// </summary>
        [SerializeField]
        private string[] heroAsset_Path; //在 編集器視窗 // 中設定 用來存放 heroAsset 的路徑

        private List<GameObject> gamers_GameObject = new List<GameObject>(); //存放每位玩家遊戲物件的集合

        private List<GameObject> npcs_GameObject = new List<GameObject>();   //存放每位 Npc 遊戲物件的集合

        /// <summary>
        /// 索引是 32方位，1~32
        /// </summary>
        private Vector3[] direction_Vector3 = new Vector3[33]; //存放角度，索引是 32方位，1~32

        /// <summary>
        /// 玩家遊戲物件的控制腳本陣列 (Basic_Character)
        /// </summary>
        public Basic_Character[] characters; //玩家遊戲物件的控制腳本陣列

        /// <summary>
        /// Npc 遊戲物件的控制腳本陣列 (Basic_Character)
        /// </summary>
        private Basic_Character[] npcs;      // Npc 遊戲物件的控制腳本陣列

        #region Render 世界
        float serverPakcetTime_Latest = 0.0f, serverPakcetTime_Before = 0.0f; //紀錄封包發送的時間
        #region render position 的參數
        Vector3[] position_Before, position_Latest; //起始/目標 點
        float[] duraiotnTime;                       //需要花多少時間到達目標點
        float[] durationTime_Accumulate;            //累加百分比 //用於累積內插了多少
        #endregion
        #endregion

        #region 紀錄前一個角色血量  =>當血量變動時做出效果
        private short[] beforeHP;
        #endregion 

        #region Render 子彈用的參數 
        [SerializeField]
        private string bulletsAsset_Path;  //存放子彈預置物件路徑  // 預置物件的名子是 Bullet.BulletMeshNumber ， "路徑 + Bullet.BulletMeshNumber" 才是完整的讀取路徑
        private Dictionary<byte, GameObject> bullets_Objects_InScene = new Dictionary<byte,GameObject>(); //在場景中 子彈的實體，byte索引 是子彈的 Unique_Number
        private List<Bullet> bullets_Propertys_InScene = new List<Bullet>();   //用於詮釋子彈的單位 = 封包中索傳送的 。 這邊的 new 純脆給他一個值。
        private List<Bullet> bullets_Propertys_InScene_Last = new List<Bullet>();              //上一個封包中的，用於比較的
        [SerializeField]
        private GameObject[] Effect_Explosion_Prefab;//爆炸效果的預置物件

        #endregion

        #endregion

        #if UNITY_EDITOR
        public string Introduction;
        #endif
        #region UI 物件
        public GameObject HPBar_Blood, MPBar_Blood, MidTop_Timer_Text, MidTop_Red_Text, MidTop_Blue_Text, MidTop_Icon_Kill, MidTop_HPBarEnemy_Frame, HPBar_Blood_Enemy;
        public GameObject[] Button_UI;

        #region Render UI 參數
        /// <summary>
        /// 最大的 cd 時間
        /// </summary>
        private float[] maxcd;
        private float maxHP=0,maxMP=0;
        private float startfixTime;
        #region 記殺人數
        private byte killedNumber_team1 = 0, killedNumber_team2 = 0;
        private List<byte> wasKill = new List<byte>(); //寫歸零，而且記過殺人數的人。
        #endregion 
        #endregion
        #endregion

        #region 釋放
        void OnDestroy()
        {
            //----停止並釋放 Tiner
            //aTimer.Stop();
            //aTimer.Dispose();
        }
        #endregion

        private bool operator_moving = false; //操作者是不是在移動中

        #region 事件
        //-------------掛勾給其他腳本的事件----------------//
        #region Render 世界

        #region 功能render position + Render World

        public void Render_World(Worldstate worldState) //掛勾給ReceiveAndStoreWorldStates
        {
            #region 內插所需參數取得
            //Debug.Log("render_World");
            //取出Server時間 //要做內插用
            serverPakcetTime_Before = serverPakcetTime_Latest; //將之前在新的時間存入暫存
            serverPakcetTime_Latest = worldState.Time_Gaming;  //存放最新的包裹時間
            #endregion


            #region Render CharacterInformation
            for (byte i = 0; i < gamers_GameObject.Count; i++)
            {
                #region 測試 (以注解)
                /*
                if (worldState.Gamers[i].Information_character.Animation == 11)
                {
                    Debug.Log("player " + i + " 現在的動畫 " + worldState.Gamers[i].Information_character.Animation);
                }
                 * */
                #endregion

                #region Render CharacterInformation

                #region 如果在這次的封包中血量不一樣，做出浮動文字(floating text)
                beforeHP[i] = characters[i].GetHP();
                short nowHP = worldState.Gamers[i].Information_character.Hp;
                Vector3 targetPostion = ToVector3(worldState.Gamers[i].Information_position.Position);
                if (beforeHP[i] != nowHP) //如果血量有變動
                {
                    this.CreateText((short)(nowHP - beforeHP[i]), targetPostion); //做出文字效果
                }
                #endregion

                #region 藉由動畫狀態得更動作為索引，製作的效果 1. 死亡狀態 開始/死亡等待/復活
                if ((worldState.Gamers[i].Information_character.Animation == 13) || (characters[i].GetAnimation() == 13)) //如果封包 傳來 這偵的動畫是 13 (死亡狀態) 或 前一偵是在死亡狀態
                {
                    //做死亡狀態跟別人不一樣的變動
                    int before_AnimationState = characters[i].GetAnimation();                                 //取得前一偵的動畫狀態
                    int now_AnimationState = worldState.Gamers[i].Information_character.Animation;            //取得前這一偵的動畫狀態 
                    if ((before_AnimationState != now_AnimationState) && (before_AnimationState != 13))       //剛要進入死亡狀態
                    {
                        /*
                         * 顯示特效
                         * 關閉capsule collider (如果不是操作者的話)
                         * 將game object.GetCompenentInChildren.Skinn Mesh Render 設為 false
                         */
                        if (i != operatorNumber)
                            characters[i].gameObject.GetComponent<CapsuleCollider>().enabled = false;
                        //-源-//
                        //characters[i].gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                        characters[i].gameObject.transform.Find("BodyMesh").gameObject.SetActive(false);
                    }
                    else if (before_AnimationState == now_AnimationState)                                     //死亡中
                    {
                        //計算設於死亡時間 並 顯示
                    }
                    else if ((before_AnimationState != now_AnimationState) && (before_AnimationState == 13))  //復活                        
                    {
                        /*
                         * 顯示特效
                         * 開起capsule collider (如果不是操作者的話)
                         * 將game object.GetCompenentInChildren.Skinn Mesh Render 設為 true
                         */
                        if (i != operatorNumber)
                            characters[i].gameObject.GetComponent<CapsuleCollider>().enabled = true;
                        //-源-//
                        //characters[i].gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
                        characters[i].gameObject.transform.Find("BodyMesh").gameObject.SetActive(true);
                        SetPosition_RightNow(i, ToVector3(worldState.Gamers[i].Information_position.Position));
                    }
                }
                #endregion

                //將新封包的角色資訊設定進自己世界的角色物件
                characters[i].Set_CharacterInformation(worldState.Gamers[i].Information_character.Hp, worldState.Gamers[i].Information_character.Animation, //Render CharacterInformation
                                                       worldState.Gamers[i].Information_character.Atk, worldState.Gamers[i].Information_character.MoveSpeed,
                                                       worldState.Gamers[i].Information_character.SkillCd);



                #region Render Poistion

                render_position(i, ToVector3(worldState.Gamers[i].Information_position.Position), serverPakcetTime_Latest - serverPakcetTime_Before); // Render Position

                #endregion

                #region Render Rotation
                if (i != operatorNumber)   //如果不等於操作者，操作者的方向控制不一樣
                    gamers_GameObject[i].transform.eulerAngles = direction_Vector3[worldState.Gamers[i].Information_position.Rotation];  //render rotation
                #endregion
                #endregion
            }
            #endregion

            #region Render Bullets
            if (worldState.Bullets != null)
            {
                //將子彈放入  bullets_Propertys_InScene ， 在 Updata 中實際 Render
                bullets_Propertys_InScene = new List<Bullet>((IEnumerable<Bullet>)worldState.Bullets.Clone());
            }
            #endregion

            #region Render Npc
            Gamer[] now_npcs = worldState.Npcs; //這偵的 Npc 狀態 
            for (byte i = 0; i < this.npcs.Length; i++)
            {
                if (now_npcs[i].Information_character.Hp != npcs[i].GetHP())
                {
                    Vector3 text_Position = ToVector3(worldState.Npcs[i].Information_position.Position);
                    text_Position.y += 0.2f;
                    this.CreateText((short)(now_npcs[i].Information_character.Hp - npcs[i].GetHP()), text_Position); //做出文字效果
                }

                //將新封包的角色資訊設定進自己世界的角色物件
                npcs[i].Set_CharacterInformation(now_npcs[i].Information_character.Hp, now_npcs[i].Information_character.Animation, //Render CharacterInformation
                                                       now_npcs[i].Information_character.Atk, now_npcs[i].Information_character.MoveSpeed,
                                                       now_npcs[i].Information_character.SkillCd);
            }
            //由於現在的 Npc 只有 Tower 所以不用內插

            #endregion

        }

        /// <summary>
        /// 讓某位玩家，從一個位置，移動到另一個位置
        /// </summary>
        /// <param name="playerNumber">玩家編號</param>
        /// <param name="position_From">起始點</param>
        /// <param name="position_To">終點</param>
        /// <param name="duration">花費時間</param>
        public void render_position(byte playerNumber, Vector3 position_To, float duration)
        {
            gamers_GameObject[playerNumber].transform.position = position_Latest[playerNumber]; //先將這位玩家移到初始位置
            this.position_Before[playerNumber] = position_Latest[playerNumber];
            this.position_Latest[playerNumber] = position_To;
            durationTime_Accumulate[playerNumber] = 0.0f;                                  //將累加百分比規零
        }
        private void render_PositionInUpdate()
        {
            #region render position
            for (byte i = 0; i < whichMembersInAGame; i++)
            {
                Vector3 position = new Vector3();
                position.x = Mathf.Lerp(position_Before[i].x, position_Latest[i].x, durationTime_Accumulate[i]);
                position.y = Mathf.Lerp(position_Before[i].y, position_Latest[i].y, durationTime_Accumulate[i]);
                position.z = Mathf.Lerp(position_Before[i].z, position_Latest[i].z, durationTime_Accumulate[i]);

                gamers_GameObject[i].transform.position = position;

                durationTime_Accumulate[i] = Time.deltaTime / duraiotnTime[i];
                if (durationTime_Accumulate[i] > 1.0f)
                    durationTime_Accumulate[i] = 1.0f;
            }
            #endregion 
        }
        #endregion
        /// <summary>
        /// 把從伺服器街到的世界狀態Render出來
        /// </summary>
        /// <param name="worldState"></param>

        
        #endregion

        /// <summary>
        /// 設定操作者的方向
        /// </summary>
        /// <param name="eulerAngles"></param>
        public void Render_OperatorRotation(Vector3 eulerAngles) //掛勾給Operator_Rotation
        {
            gamers_GameObject[operatorNumber].transform.eulerAngles = eulerAngles;  //操作者的方向控制
        }
        /// <summary>
        /// 取得要跟隨的遊戲物件
        /// </summary>
        /// <returns>遊戲物件</returns>
        public GameObject GetOperatorGameObject()    //掛勾給 Camera_Position //取得要跟隨的遊戲物件
        {
            if (operatorNumber != null)
                return gamers_GameObject[operatorNumber];
            else
                return null;
        }
        //----------------------------------------------//

        #endregion

        //-------------以下為功能---------------------//
        #region ToVector3,ToFloatArray,ToRotation,ToDirection,SetPosition_RightNow,Effect_Explosion,CreateText,
        /// <summary>
        /// 將float[3] 轉成 Vector3
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private Vector3 ToVector3(float[] array)
        {
            if (array.Length != 3)
                return Vector3.zero;
            else
            {
                return new Vector3(array[0], array[1], array[2]);
            }
        }
        private float[] ToFloatArray(Vector3 position)
        {
            float[] positionArray = new float[3];
            positionArray[0] = position.x;
            positionArray[1] = position.y;
            positionArray[2] = position.z;
            return positionArray;
        }
        /// <summary>
        /// 將float[4] 轉成 Quaternion
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private Quaternion ToRotation(float[] array)
        {
            if (array.Length != 4)
                return Quaternion.identity;
            else
            {
                return new Quaternion(array[1], array[2], array[3], array[0]);
            }
        }
        /// <summary>
        /// 傳入歐拉角(0~360度)，回傳byte
        /// </summary>
        /// <param name="y_axis">傳入Transform.eulerAngles.y(其實就是角度)</param>
        /// <returns>傳回byte(1~32)</returns>
        private byte ToDirection(float y_axis)
        {
            byte count = (byte)(y_axis / 11.25f); //11.25f 是 每個
            count = (byte)(count + 1); //先轉換為正確的count
            if ((y_axis % 11.25) > (11.25f / 2))
            {
                count = (byte)(count + 1);
            }
            if (count > 32) //如果加完後大於32了 ，要讓它等於1
                count = 1;
            return count;
        }
        /// <summary>
        /// 馬上將某位玩家的位置移動到指定的位置
        /// </summary>
        /// <param name="playerid">玩家的 player id</param>
        /// <param name="toPosition">要移動到的位置</param>
        private void SetPosition_RightNow(byte playerid, Vector3 toPosition)
        {
            //將內插的參數 中的位置都設為復活點
            position_Before[playerid] = toPosition;
            position_Latest[playerid] = toPosition;
        }
        /// <summary>
        /// 顯示爆炸效果
        /// </summary>
        /// <param name="position">要出現的位置</param>
        /// <param name="effectNumber">哪個爆炸效果</param>
        private void Effect_Explosion(Vector3 position, byte effectNumber)
        {
            if(Effect_Explosion_Prefab[effectNumber]!=null)
                Instantiate(Effect_Explosion_Prefab[effectNumber], position, Quaternion.identity);
            Debug.Log("執行爆炸韓式");
        }
        #region 增加文字(用於打人時顯示傷害)
        #region 浮動文字使用的參數(這裡的都為const)
        /// <summary>
        /// 浮動文字DataPool陣列的長度
        /// </summary>
        private const byte dataPoolCountTextMesh = 20;
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
        private const float yTextmove2 = 0.1f;
        #endregion
        public GameObject Floating_Text;
        private GameObject[] dataPoolTextMesh;
        private byte dataPoolIndex = 0;
        private Camera mainCamera;
        private List<GameObject> textInScene;
        /// <summary>
        /// 現在在場上有幾個text
        /// </summary>
        private byte textNumberInScene = 0;

        /// <summary>
        /// 主要用於在AttackObject 撞到目標時呼叫。(會自動上移0.1f)
        /// </summary>
        /// <param name="text">要顯示的文字</param>
        /// <param name="targetposition">顯示的位置WorldPosition</param>
        public void CreateText(float text, Vector3 targetposition)
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
            //Debug.Log("這個浮動文字的位置為 x" + v.x + "  y : " + v.y + "  z : " + v.z);
            textObj.transform.position = v;
            textObj.transform.rotation = mainCamera.gameObject.transform.rotation;
            textObj.GetComponent<TextMeshPro>().text = text.ToString();

        }
        /// <summary>
        /// 縮排用，打到人後出現浮動文字處理
        /// </summary>
        private void floating_Text()
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
        #endregion
        #region UI 功能
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ui">要移動的圖片，parent必須要有遮罩</param>
        /// <param name="percentage">0.xx</param>
        /// <param name="x">要上圖片水平(true)，垂直(false)移動</param>
        private void setCDTime(GameObject ui, float percentage,bool x)
        {
            RectTransform rect = ui.GetComponent<RectTransform>();
            if (x)               
                rect.localPosition = new Vector3(-(rect.rect.size.x - (rect.rect.width * percentage)), 0.0f, 0.0f);
            else
                rect.localPosition = new Vector3(0.0f, -(rect.rect.size.y - (rect.rect.height * percentage)), 0.0f);
        }
        #endregion


    }
}
