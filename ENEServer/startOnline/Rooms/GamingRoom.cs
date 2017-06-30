using System;
using System.Linq;
using System.Collections.Generic;
using System.Timers;
using startOnline.Rooms.WorldState;

namespace startOnline.Rooms
{
    public class GamingRoom :IDisposable
    {
        #region 注意事項
        /*參數attack_Mouse0_Distance,Valid_Range_Attack 會依據角色有所不同，以後要改成從角色上抓
         * 目前是測試所以使用 常數*/
        /*只能分為兩隊*/
        /*(2. 目標示隊友時,打不到).這個功能由於不方便測試所以先關掉,在 15ms 轉一次裡*/
        #region 技術債
        /*技術債-Bug*/
        /*1. 攻擊判定有問題，當瞄準的ray有撞到人時，反而打不到(可能是因為角色碰撞器和ray的撞點和自己形成的線，目標點會在這條線之外)(也可能是演算法本身的問題)*/
        /*2. 就算加了自己的處理方法，在角色攻擊後攻擊動畫還沒跑玩前，就可以移動了。目前的做法是計算動畫時間存在Server，當按下攻擊後就開始計算，當時間結束才能移動。*/
        /*3. 打人時一定要將重點落在目標中心點上方(打在腰部以上)，不然判斷打不到。 原因可能是 -> 在判斷有沒有打到時，有將玩家位置轉成中心點。 
        /*                                                                          可能解法 -> 將攻擊分成進戰/遠程兩種 在判斷有沒有擊中時，做不一樣的設計。
        /*技術債-not Do*/
        /*
         *4. Get_DieWaitingTime     還沒實做，現在先用10.0f
         *5. Get_Maximum_HP         還沒實做，現在先用100.0f
         *6. maxNpcAccount          值要能動態更改  (存放這個遊戲房能同時容納的最多Npc數量)
         *7. 計算npc有沒有被擊中    的第一判斷點 (是不是隊友還沒做)
         *8. Get_HowMuchMoneyCanGet 還沒實做，先用 200 或 100 贏/輸
         *9. worldstate.Gamer/Npcs[x].Information_character.Collider_Radius 這個值先用定值/以後要在資料庫或其他地方取
         *10. private const float charactor_Height = 0.15f; 暫時先用 const ， 以後在考慮要在哪讀取  (在判斷子彈有沒有撞到 玩家/Npc 時使用)
         *11. private const float tower_Height = 0.32f;     暫時先用 const ， 以後在考慮要在哪讀取  (在判斷子彈有沒有撞到 玩家/Npc 時使用)     
         *12. 在 初始化 Npc 裡，兩個塔的 number 使用整數 1 and 2 (方便使用isTeammate判斷)
         *13. SkillCd_Max[][]       還沒實做，現在先用10s (未來要從角色資料庫取)
         */
        #endregion

        #endregion
        #region 初始化用的參數
        private const float team1Score = 3.5f; //team1 的 初始位置中心 
        private const float team2Score = -3.5f; //team2 的 初始位置中心 
        private const float between = 0.15f;  //同隊每位玩家初始的間格
        #endregion
        public GamingRoom(List<gamePeer[]> players, List<byte[]> result_herosArray)
        {
            #region 初始化遊戲地圖_陣列
            #region 新增基本遊戲地圖 - 方形
            /*
             * 注意 建置方形時 點的配置要順時鐘 ，第一個面的四點完後換另一個面的四點，兩個面的起始點都要一樣，一樣左上或左下等等
             */

            map = new float[2][][];      //目前是由2個長方體組成，有需要在加
            for (byte i = 0; i < map.Length; i++) //先進行存放空間初始化
                 map[i] = new float[8][];

            map[0][0] = new float[] { 0.028f, 0.0f, 4.625f }; //rectangle1_1
            map[0][1] = new float[] { 4.746f, 0.0f, -0.1f };  //rectangle1_2
            map[0][2] = new float[] { 3.873f, 0.0f, -0.978f };  //rectangle1_3
            map[0][3] = new float[] { -1.0584f, 0.0f, 3.7626f };//rectangle1_4
            for(byte i=4;i<map[0].Length;i++)
            {
                map[0][i] = new float[] { map[0][i - 4][0], map[0][i - 4][1] + 5, map[0][i - 4][2] }; //等於是把0~3點往上移5，當作 4~7 點
            }

            map[1][0] = new float[] { 3.731f, 0.0f, 0.937f };   //rectangle2_1
            map[1][1] = new float[] { 4.831f, 0.0f, 0.052f };   //rectangle2_2
            map[1][2] = new float[] { -0.044f, 0.0f, -4.849f }; //rectangle2_3
            map[1][3] = new float[] { -1.235f, 0.0f, -3.993f };  //rectangle2_4
            for (byte i = 4; i < map[0].Length; i++)
            {
                map[1][i] = new float[] { map[1][i - 4][0], map[1][i - 4][1] + 5, map[1][i - 4][2] }; //等於是把0~3點往上移5，當作 4~7 點
            }
            
            #endregion 
            #region 新增地圖裡的物件 - 方形 
            /*
             * 注意 建置方形時 點的配置要順時鐘 ，第一個面的四點完後換另一個面的四點，兩個面的起始點都要一樣，一樣左上或左下等等
             */
            map_object = new List<float[][]>();
            #region 正式用方形
            //AddNew_MapObject_Square(new float[]{},new float[]{},new float[]{},new float[]{},);

            AddNew_MapObject_Square(new float[] { -3.16f, 0.0f, 1.09f }, new float[] { -2.67f, 0.0f, 0.65f }, new float[] { -2.1f, 0.0f, 1.2f }, new float[] { -2.56f, 0.0f, 1.7f }, 0.8f); //LeftFrontRoad_1
            AddNew_MapObject_Square(new float[] { -1.64f, 0.0f, 1.67f }, new float[] { -1.35f, 0.0f, 1.39f }, new float[] { -1.213f, 0.0f, 1.52f }, new float[] { -1.5f, 0.0f, 1.81f }, 0.38f); //LeftFrontRoad_2
            AddNew_MapObject_Square(new float[] { -2.22f, 0.0f, 2.58f }, new float[] { -1.8f, 0.0f, 2.18f }, new float[] { -1.66f, 0.0f, 2.3f }, new float[] { -2.08f, 0.0f, 2.73f }, 0.38f); //LeftFrontRoad_3
            AddNew_MapObject_Square(new float[] { -1.49f, 0.0f, 2.65f }, new float[] { -1.2f, 0.0f, 2.37f }, new float[] { -1.06f, 0.0f, 2.5f }, new float[] { -1.35f, 0.0f, 2.79f }, 0.38f); //LeftFrontRoad_3

            AddNew_MapObject_Square(new float[] { -1.82f, 0.0f, -1.41f }, new float[] { -1.68f, 0.0f, -1.54f }, new float[] { -1.54f, 0.0f, -1.41f }, new float[] { -1.68f, 0.0f, -1.27f }, 0.16f); //LeftBackRoad_1
            AddNew_MapObject_Square(new float[] { -1.87f, 0.0f, -2.88f }, new float[] { -1.73f, 0.0f, -3.02f }, new float[] { -1.59f, 0.0f, -2.88f }, new float[] { -1.73f, 0.0f, -2.74f }, 0.16f); //LeftBackRoad_2

            AddNew_MapObject_Square(new float[] { 3.062f, 0.0f, 1.638f }, new float[] { 2.749f, 0.0f, 1.354f }, new float[] { 2.6007f, 0.0f, 1.5118f }, new float[] { 2.8606f, 0.0f, 1.795f }, 0.38f); //RightFrontRoad_1
            AddNew_MapObject_Square(new float[] { 1.8878f, 0.0f, 1.7544f }, new float[] { 1.4731f, 0.0f, 1.3349f }, new float[] { 1.279f, 0.0f, 1.483f }, new float[] { 1.73f, 0.0f, 1.917f }, 0.38f); //RightFrontRoad_2
            AddNew_MapObject_Square(new float[] { 1.187f, 0.0f, 2.885f }, new float[] { 0.883f, 0.0f, 2.575f }, new float[] { 1.046f, 0.0f, 2.403f }, new float[] { 1.35f, 0.0f, 2.714f }, 0.38f); //RightFrontRoad_3

            AddNew_MapObject_Square(new float[] { 4.433f, 0.0f, 0.218f }, new float[] { 4.429f, 0.0f, -0.37f }, new float[] { 4.622f, 0.0f, -0.388f }, new float[] { 4.647f, 0.0f, 0.216f }, 0.4f); //RightMiddleRoad_1

            AddNew_MapObject_Square(new float[] { 2.3379f, 0.0f, -1.0805f }, new float[] { 2.0199f, 0.0f, -0.7659f }, new float[] { 1.852f, 0.0f, -0.908f }, new float[] { 2.1855f, 0.0f, -1.2314f }, 0.4f); //RightBackRoad_1
            AddNew_MapObject_Square(new float[] { 2.515f, 0.0f, -1.993f }, new float[] { 2.383f, 0.0f, -2.126f }, new float[] { 1.984f, 0.0f, -1.748f }, new float[] { 2.126f, 0.0f, -1.611f }, 0.4f); //RightBackRoad_2
            AddNew_MapObject_Square(new float[] { 1.296f, 0.0f, -1.881f }, new float[] { 1.166f, 0.0f, -2.008f }, new float[] { 1.549f, 0.0f, -2.404f }, new float[] { 1.688f, 0.0f, -2.266f }, 0.4f); //RightBackRoad_3
            AddNew_MapObject_Square(new float[] { 1.653f, 0.0f, -3.148f }, new float[] { 1.508f, 0.0f, -3.292f }, new float[] { 1.168f, 0.0f, -2.951f }, new float[] { 1.301f, 0.0f, -2.811f }, 0.4f); //RightBackRoad_4

            #endregion
            #endregion
            #endregion

            #region 初始化 AngleofAspect

            float radians = 57.2958f; //(一個弧 = 57.2958角度)
            float degrees = 0.0f;
            for (byte i = 1; i < AngleofAspect.Length; i++) // [0]不放，是null 所以 i=1 
            {
                AngleofAspect[i] = new float[3];
                if ((degrees % 90) != 0) //暴力法 //因為當 degrees = 0,90,180,270 時會錯 ，不知道為啥
                {
                    AngleofAspect[i][0] = (float)Math.Sin(degrees / radians); //x 軸向左右 代表原本(x,y) 的 x軸
                    AngleofAspect[i][1] = 0.0f;                               //y 軸向上下 所以無解
                    AngleofAspect[i][2] = (float)Math.Cos(degrees / radians); //z 軸向前 代表原本(x,y) 的 y軸  
                }
                else
                {
                    byte which = (byte)(degrees / 90);
                    switch (which)
                    {
                        case 0:
                            AngleofAspect[i][0] = 0.0f;
                            AngleofAspect[i][1] = 0.0f;
                            AngleofAspect[i][2] = 1.0f;
                            break;
                        case 1:
                            AngleofAspect[i][0] = 1.0f;
                            AngleofAspect[i][1] = 0.0f;
                            AngleofAspect[i][2] = 0.0f;
                            break;
                        case 2:
                            AngleofAspect[i][0] = 0.0f;
                            AngleofAspect[i][1] = 0.0f;
                            AngleofAspect[i][2] = -1.0f;
                            break;
                        case 3:
                            AngleofAspect[i][0] = -1.0f;
                            AngleofAspect[i][1] = 0.0f;
                            AngleofAspect[i][2] = 0.0f;
                            break;
                    }
                }
                degrees += 11.25f;
                //Debug.Log(" Number = " + i + " (x,y) = " + AngleofAspect[i][0] + "," + AngleofAspect[i][2]);
            }
            
            #endregion

            #region 製作並傳送給每個人初始化的世界狀態
            //MessageBox.Show("okok GamingRoom is create");
            Dictionary<byte, object> packet = new Dictionary<byte, object> 
            {
                {(byte)0,(byte)4}, //switch code //Send Result
            };
            Worldstate worldstate = new Worldstate();
            #region 初始化玩家
            worldstate.Gamers  = new Gamer[4];
            for (byte i = 0; i < worldstate.Gamers.Length; i++)
            {
                byte whichTeamInAGame = (byte)(i / result_herosArray.Count); // 0 and 1
                byte whichNumberInATeam = (byte)(i % (result_herosArray[0].Length));
                // 計算初始的位置方向 ， 先用 000 測試
                float[] position = new float[3];
                byte rotation = 1; //預設32方位 面北
                switch (whichTeamInAGame)
                {
                    case 0:
                        position[0] = whichNumberInATeam * between;
                        position[1] = 0.0f;
                        position[2] = team1Score;
                        rotation = (byte)17; //32方位 面南
                        break;
                    case 1:
                        position[0] = whichNumberInATeam * between;
                        position[1] = 0.0f;
                        position[2] = team2Score;
                        rotation = (byte)1;  //32方位 面北
                        break;
                }
                //-----
                worldstate.Gamers[i] = new Gamer(i, result_herosArray[whichTeamInAGame][whichNumberInATeam], position, rotation, 100, 20, 0, 0.066f*4, 0.04f, charactor_Height);
            }
            #endregion
            #region 初始化 Npc
            worldstate.Npcs = new Gamer[maxNpcAccount];
            for (byte i = 0; i < worldstate.Npcs.Length; i++)
            {
                float[] position = new float[3];
                switch (i)
                {
                    case 0: //新增塔1 
                        position[0] = 0.0f;
                        position[1] = 0.0f;
                        position[2] = 4.35f;
                        worldstate.Npcs[i] = new Gamer(1, 0, position, 1, 100, 20, 0, 0.0f,0.08f,tower_Height);
                        break;
                    case 1: //新增塔2
                        position[0] = 0.0f;
                        position[1] = 0.0f;
                        position[2] = -4.35f;
                        worldstate.Npcs[i] = new Gamer(2, 0, position, 1, 100, 20, 0, 0.0f, 0.08f, tower_Height);
                        break;
                }
            }
            #endregion
            packet.Add((byte)5, startOnline.Serializate.ToByteArray(worldstate));
            this.worldState = worldstate; //將初始化得世界狀態 存在這個 遊戲房中
            temporaryStorage_WorldState = new TemporaryStorage_WorldState(this.worldState); //初始化前1秒的世界狀態
            #region 初始化陣列
            player_Peers = new gamePeer[players.Count * players[0].Length];     //存放Peer的空間
            player_Pings = new float[players.Count * players[0].Length];        //存放每位玩家Ping的空間
            this.readly = new bool[players.Count * players[0].Length];          //用來確認每位玩家在客戶端都進入遊戲房的場景了
            players_KeyBoard = new KeyBoard[players.Count * players[0].Length]; //需要幾個模擬鍵盤
            for (byte i = 0; i < players_KeyBoard.Length; i++)                  //創建每個模擬鍵盤
            {
                players_KeyBoard[i] = new KeyBoard();
            }
            aiming_Point = new float[players.Count * players[0].Length][];      //存放每位玩家在客戶端的瞄準點
            for (byte i = 0; i < aiming_Point.Length; i++)                      //初始化存放瞄準點的空間
            {
                aiming_Point[i] = new float[3];
            }
            stiff_Time = new float[players.Count * players[0].Length];          //存放每位玩家"現在的"僵直時間
            for (byte i = 0; i < stiff_Time.Length; i++)
            {
                stiff_Time[i] = 0.0f;
            }
            invincible_is = new bool[players.Count * players[0].Length];        //存放每位玩家是否在無敵狀態
            for (byte i = 0; i < invincible_is.Length; i++)
            {
                invincible_is[i] = false;
            }
            felling_Time = new float[players.Count * players[0].Length];        //存放每位玩家現在的倒地時間
            for (byte i = 0; i < felling_Time.Length; i++)
            {
                felling_Time[i] = 0.0f;
            }

            attack_AnimationRunTime = new float[players.Count * players[0].Length][]; //存放角色的攻擊動畫跑多久 /sec
            for (byte i = 0; i < attack_AnimationRunTime.Length; i++)
            {
                //固定最多六種攻擊(+技能)
                /*--原版　-> 將裡面的值從資料庫或其他地方取出來並賦予 //[playerid][(0,1,2,3,4,5)] = [playerid][(Mouse0,Nouse1,Q,E,F,Space)
                float[] thisPlayerAnimationRunTime = new float[6];
                //thisPlayerAnimationRunTime = xxxx
                attack_AnimationRunTime[i] = thisPlayerAnimationRunTime;
                /*---*/
                /*--測試版-> 直接給予一個值*/
                attack_AnimationRunTime[i] = new float[6];
                for (byte j = 0; j < attack_AnimationRunTime[i].Length; j++)
                {
                    switch (this.worldState.Gamers[i].heroid)
                    {
                        case 0: //sword
                            attack_AnimationRunTime[i][j] = 0.25f;
                            break;
                        case 1: //cat robot
                            if(j==0)
                              attack_AnimationRunTime[i][j] = 0.25f;
                            break;
                        case 2: //girl robot
                            if(j==0)
                                attack_AnimationRunTime[i][j] = 0.25f;
                            if (i == 3)
                                attack_AnimationRunTime[i][j] = 0.25f;
                            if(j==5)
                                attack_AnimationRunTime[i][j] = 0.8f;
                            break;
                        default:
                            attack_AnimationRunTime[i][j] = 0.25f;
                            break;
                    }
                    
                }
                /*---*/
            }
            character_Height = new float[players.Count * players[0].Length];   //存放角色遊戲物件的高度(從地上到最上面、身高)
            for (byte i = 0; i < character_Height.Length; i++)
            {
                character_Height[i] = 0.15f; //暫時使用 sword 的身高 0.15f
            }
            attacking_AnimationRunTime = new float[players.Count * players[0].Length]; //存放現在這位玩家正在跑的動畫剩餘時間
            for (byte i = 0; i < attacking_AnimationRunTime.Length; i++)
            {
                attacking_AnimationRunTime[i] = 0.0f;
            }
            attacking_AnimationState = new short[players.Count * players[0].Length];  //存放現在正在跑的 動畫狀態索引   //如果attacking_AnimationRunTime[me]＞0，才成真。
            for (byte i = 0; i < attacking_AnimationState.Length; i++)
            {
                attacking_AnimationState[i] = 0; //預設為　idle 狀態　
            }
            dieWaitingTime = new float[players.Count * players[0].Length];
            for (byte i = 0; i < dieWaitingTime.Length; i++)
            {
                dieWaitingTime[i] = 0.0f;  //一開始死亡時間都是零
            }
            aLive_Point = new float[players.Count * players[0].Length][];     //存放玩家的復活點
            for (byte i = 0; i < aLive_Point.Length; i++)
            {
                aLive_Point[i] = new float[3];
                for (byte j = 0; j < aLive_Point[i].Length; j++)
                {
                    aLive_Point[i][j] = this.worldState.Gamers[i].Information_position.Position[j]; //將現在的位置傳進去，因為現在的位置就是復活點
                }
            }
            skillCd_Max = new float[players.Count * players[0].Length][]; //存放每位玩家角色技能的最大冷卻時間
            for (byte i = 0; i < skillCd_Max.Length; i++)
            {
                skillCd_Max[i] = new float[5];
                for (byte j = 0; j < skillCd_Max[i].Length; j++)
                {
                    switch (this.worldState.Gamers[i].heroid)
                    {
                        case 1: //cat robot
                            if (j == 4) //skill move_Space
                                skillCd_Max[i][j] = 5.0f;
                            else if(j==2) //skill E
                                skillCd_Max[i][j] = 5.0f;
                            else
                                skillCd_Max[i][j] = 0.0f;
                            break;
                        case 2: //girl robot
                            if (j == 4)
                                skillCd_Max[i][j] = 5.0f;
                            else if(j==3)
                                skillCd_Max[i][j] = 2.0f;
                            else
                                skillCd_Max[i][j] = 0.0f;
                            break;
                        default:
                            skillCd_Max[i][j] = 0.0f;
                            break;
                    }
                }
            }
            changingRotation = new bool[players.Count * players[0].Length];
            for(byte i=0;i<changingRotation.Length;i++)
            {
                changingRotation[i] = true;
            }
            #endregion

            for (byte i = 0; i < (players.Count * players[0].Length); i++)
            {
                packet.Add((byte)6, (byte)i);
                byte whichTeamInAGame = (byte)(i / result_herosArray.Count);
                byte whichNumberInATeam = (byte)(i % (result_herosArray[0].Length));
                player_Peers[i] = players[whichTeamInAGame][whichNumberInATeam]; //將Peer存入這個遊戲房
                player_Peers[i].PlayerNumber = (byte)i;                          //傳入用於減少傳輸封包的參數
                player_Peers[i].GamingRoom_Messages += this.gamingRoom_Messages; //hook up event
                players[whichTeamInAGame][whichNumberInATeam].SendEvent((byte)EventCode.SendPrepareRoomMessage, packet);
                packet.Remove((byte)6);
            }
                
            //Above Codes :  Send packet to Every Body
            //packet including :  EventCode.SendPrepareRoomMessage , 0 switch code ,5 operator player number ,6 heroid
            #endregion            

            #region 設定計時器
            // Create a timer with a two second interval.
            upDate = new Timer(15);
            // Hook up the Elapsed event for the timer. 
            upDate.Elapsed += update;
            upDate.AutoReset = true;
            upDate.Enabled = true;
            #endregion 

        }
        #region 計時器管理
        private Timer upDate; private bool Execute30MSFunction = false;
        private void update(Object source, ElapsedEventArgs e)
        {
            if (readly_All)
                timer_Gaming += 0.015f;
            upDate15MS();
            if (Execute30MSFunction)
            {
                upDate30MS();
                Execute30MSFunction = false;
            }
            else
            {
                Execute30MSFunction = true;
            }
          
        }
        #endregion

        #region 這個class主要的參數
        private const short maxNpcAccount = 2; //存放這個遊戲房能同時容納的最多Npc數量
        private float timer_Gaming = 0.0f; //遊戲時間
        private Worldstate worldState;     //世界狀態
        /// <summary>
        /// 由很多的方型組成 代表地圖。第一個陣列是 哪個方形 第二個陣列是方形的哪個點 第三個陣列是點的位置
        /// </summary>
        float[][][] map;                   //基本地圖 
        /// <summary>
        /// 在基本地圖裡的不能被進入的各種物件(由於要可新增所以用List)。第一個List是 哪個方形 第二個陣列是方形的哪個點 第三個陣列是點的位置
        /// </summary>
        List<float[][]> map_object;        //在基本地圖裡的不能被進入的各種物件(由於要可新增所以用List)
        private gamePeer[] player_Peers;   //玩家的peer 
        private float[] player_Pings;      //玩家的Ping
        #region 用於確認是不是每位玩家都在遊戲房中
        private bool[] readly;
        private bool readly_All
        {
            get
            {
                foreach (bool b in readly)
                {
                    if (b == false)
                        return false;
                }
                return true;
            }
        }
        #endregion
        #region 模擬鍵盤 和 keyCode(enum)
        /// <summary>
        /// 用playerNumber索引
        /// </summary>
        private KeyBoard[] players_KeyBoard;//用playerNumber索引
        /// <summary>
        /// 我自己定義的Key Code 用於和 伺服器溝通 ，伺服器也用這個Key Code 判斷。
        /// </summary>
        private enum keyCode : byte { W = 0, S, A, D, Mouse_Left, Mouse_Right, Q, E, F, Space = 9 }
        #endregion
        #region 用於代替方向的32方位角和向量
        
        public enum Direction : byte
        {
            N = 1, NbE, NNE, NEbN, NE, NEbE, ENE, EbN,
            E = 9, EbS, ESE, SEbE, SE, SEbS, SSE, SbE,
            S = 17, SbW, SSW, SWbS, SWm, SWbW, WSW, WbS,
            W = 25, WbN, WNW, NWbW, NW, NWbN, NNW, NbW,
        }
        /// <summary>
        /// 代表32方位角的各個向量，[0]不放，是null
        /// </summary>
        public float[][] AngleofAspect = new float[33][]; //存放固定的向量，代表32方位角的各個向量  
        
        #endregion
        private TemporaryStorage_WorldState temporaryStorage_WorldState; //存放之前1秒內世界狀態空間
        /// <summary>
        /// 存放每位玩家在客戶端的瞄準點
        /// </summary>
        private float[][] aiming_Point;    //存放每位玩家在客戶端的瞄準點
        private float[] stiff_Time;        //存放每位玩家"現在的"僵直時間
        /*const*/
        private const float stiffTime = 0.600f; //固定僵直時間
        private const float fellingTime = 3.6f; //固定倒地時間
        /// <summary>
        /// 起身後無敵延遲
        /// </summary>
        private const float invincible_delay = 1.5f; //起身後無敵延遲
        /*---*/
        #region 以後要改成從角色資料庫上取得的參數 (攻擊距離、攻擊有效範圍、角色攻擊動畫花費時間(在初始化陣列中給予)、)
        private const float attack_Mouse0_Distance = 0.15f; //用於測試的攻擊距離 
        private const float Valid_Range_Attack = 0.07f; //攻擊範圍 //目標到射擊直線的可接受最短距離
        /// <summary>
        /// 角色的攻擊動畫跑多久 [playerid][(0,1,2,3,4,5)] = [playerid][(Mouse0,Mouse1,Q,E,F,Space)]
        /// </summary>
        private float[][] attack_AnimationRunTime;  //角色的攻擊動畫跑多久 /sec   //[playerid][(0,1,2,3,4,5)] = [playerid][(Mouse0,Mouse1,Q,E,F,Space)
        private float[] character_Height; //角色遊戲物件的高度(從地上到最上面、身高)
        #endregion 
        /// <summary>
        /// 被打後的位移量
        /// </summary>
        private const float stiff_move = 0.01f; //被打後的位移量
        /// <summary>
        /// 代表那位玩家是否無敵,索引是 Player Id
        /// </summary>
        private bool[] invincible_is; //代表那位玩家是否無敵   //在按鈕觸發中，會藉由判斷這個參數，來判斷能不能對他造成傷害
        private float[] felling_Time;  //每位玩家現在的倒地時間
        /// <summary>
        /// 現在這位玩家正在跑的動畫剩餘時間
        /// </summary>
        private float[] attacking_AnimationRunTime; //現在這位玩家正在跑的動畫剩餘時間
        /// <summary>
        /// 如果attacking_AnimationRunTime[me]＞0，才成真。存放現在正在跑的 動畫狀態索引
        /// </summary>
        private short[] attacking_AnimationState;   //如果attacking_AnimationRunTime[me]＞0，才成真。存放現在正在跑的 動畫狀態索引
        /// <summary>
        /// 現在的死亡時間 沒死就是零
        /// </summary>
        private float[] dieWaitingTime; //現在的死亡時間 沒死就是零
        private float[][] aLive_Point;  //每位玩家的復活點
        /// <summary>
        /// 存放每位玩家角色技能的最大冷卻時間 (第一個索引是 player id ， 第二個索引是 哪個技能 0~4)
        /// </summary>
        private float[][] skillCd_Max;  //存放每位玩家角色技能的最大冷卻時間

        #region 子彈 處理參數
        private List<Bullet> bullets_InScene = new List<Bullet>();
        private byte available_Bullet_Number=0;
        private byte test = 0;
        #endregion 
        private const float charactor_Height = 0.15f;
        private const float tower_Height = 0.32f;

        /// <summary>
        /// 要不要同步方向，(就算是 true 只要不在移動中就不會同步)
        /// </summary>
        private bool[] changingRotation; //現在要不要同步方向，(就算是 true 只要不在移動中就不會同步)

        #endregion

        #region 接收客戶端傳來的訊息 operationCode = 13
        /// <summary>
        /// 接收客戶端傳來的訊息 operationCode = 13
        /// </summary>
        /// <param name="packet"></param>
        private void gamingRoom_Messages(Dictionary<byte, object> packet)
        {
            
            byte switchCode = (byte)packet[(byte)0];
            byte player_number = (byte)packet[(byte)124];
            switch (switchCode)
            {
                case 1: //當一位玩家進入遊戲場景後都會呼叫一次ready，用於檢察是不是所有玩家都進入遊戲場景了                   
                    readly[player_number] = true;
                    break;
                #region case 2~3 處理 Ping
                case 2: //計算Ping-純粹在把一個包傳回去
                    player_Peers[player_number].SendEvent((byte)EventCode.SendGamingRoomMessage, new Dictionary<byte, object> {  {(byte)0,(byte)2},{(byte)1,this.timer_Gaming} }); //這個Dictionary包含SwitchCode和現在遊戲房時間
                    //DisplayMessageBox(this.timer_Gaming.ToString()); //顯示現在的伺服自時間
                    break;
                case 3: //計算Ping-將算好的Ping傳給伺服器儲存
                    player_Pings[player_number] = (float)packet[1];
                    //DisplayMessageBox("number =  "+player_number+" "+player_Pings[player_number].ToString()); //顯示 誰 和 Ping 
                    break;
                #endregion
                #region case 4 處理 玩家輸入
                case 4: //處理玩家輸入
                    #region packet 格式
                    /*
                     * 0 = switchCode
                     * 1 = KeyCode
                     * 2 = resultValue , true or false
                     * 3 = (byte)Direction
                     */
                    #endregion
                    //代表能夠接收有/沒有 包含 packet[3]和packet[4]的封包
                    try
                    {   //代表能夠接 收有/沒有 包含 packet[3]的封包
                        this.worldState.Gamers[player_number].Information_position.Rotation = (byte)packet[3]; //賦予方向
                    }
                    catch (Exception e)
                    {

                    }
                    try
                    {   //代表能夠接 收有/沒有 包含 packet[4]的封包
                        float[] animingPoint = (float[])packet[4];
                        this.aiming_Point[player_number] = animingPoint;
                        //DisplayMessageBox("(x,y,z) = " + aiming_Point[player_number][0] + "," + aiming_Point[player_number][1] + "," + aiming_Point[player_number][2]);
                    }
                    catch (Exception e)
                    {
 
                    }
                    byte whichkey = (byte)packet[1];
                    switch (whichkey)
                    {
                        #region 收到哪個Key Code
                        case (byte)keyCode.Mouse_Left:
                            //DisplayMessageBox(player_number.ToString() + " Button " + (keyCode)whichkey);
                            players_KeyBoard[player_number].Mouse_Left = (bool)packet[2];
                            break;
                        case (byte)keyCode.Mouse_Right:
                            //DisplayMessageBox(player_number.ToString() + " Button " + (keyCode)whichkey);
                            players_KeyBoard[player_number].Mouse_Right = (bool)packet[2];
                            break;
                        case (byte)keyCode.E:
                            //DisplayMessageBox(player_number.ToString() + " Button " + (keyCode)whichkey);
                            players_KeyBoard[player_number].E = (bool)packet[2];
                            break;
                        case (byte)keyCode.Q:
                            //DisplayMessageBox(player_number.ToString() + " Button " + (keyCode)whichkey);
                            players_KeyBoard[player_number].Q = (bool)packet[2];
                            break;
                        case (byte)keyCode.F:
                            //DisplayMessageBox(player_number.ToString() + " Button " + (keyCode)whichkey);
                            players_KeyBoard[player_number].F = (bool)packet[2];
                            break;
                        case (byte)keyCode.Space:
                            //DisplayMessageBox(player_number.ToString() + " Button " + (keyCode)whichkey);
                            players_KeyBoard[player_number].Space = (bool)packet[2];
                            break;
                        case (byte)keyCode.W:
                            //DisplayMessageBox(player_number.ToString() + " Button " + (keyCode)whichkey);
                            players_KeyBoard[player_number].W = (bool)packet[2];
                           break;
                        case (byte)keyCode.A:
                           //DisplayMessageBox(player_number.ToString() + " Button " + (keyCode)whichkey);
                           players_KeyBoard[player_number].A = (bool)packet[2];
                           break;
                        case (byte)keyCode.D:
                           //DisplayMessageBox(player_number.ToString() + " Button " + (keyCode)whichkey);
                           players_KeyBoard[player_number].D = (bool)packet[2];
                           break;
                        case (byte)keyCode.S:
                           //DisplayMessageBox(player_number.ToString() + " Button " + (keyCode)whichkey);
                           players_KeyBoard[player_number].S = (bool)packet[2];
                           break;
                        #endregion
                    }
                    break;
                #endregion
                #region case 5 移動時不斷付予方向
                case 5:
                    if (changingRotation[player_number])
                      this.worldState.Gamers[player_number].Information_position.Rotation = (byte)packet[1];
                    break;
                #endregion
            }
        }
        #endregion
        #region 固定毫秒執行的事件
        /// <summary>
        /// Execute every 15 ms
        /// </summary>
        private void upDate15MS()
        {

           

            #region 偵測模擬鍵盤 並 做移動  (玩家)
            /*一偵只能做一件事
             * 按鈕事件 執行過一個就不能執行第二個
             * 如果在同一偵按下兩個按鈕，只會執行比較上面的按鈕事件
             * */
                for (byte i = 0; i < this.players_KeyBoard.Length; i++)
                {
                    Gamer gamer = this.worldState.Gamers[i];
                    bool isDie = false;
                    if (gamer.Information_character.Hp <= 0)
                        isDie = true;
                    #region 能被操作
                    if (stiff_Time[i] <= 0 && felling_Time[i] <= 0 && attacking_AnimationRunTime[i] <= 0 && !isDie) //"僵直狀態最大" 如果這位玩家的僵直時間不等於零的話,強迫他在此偵進入僵直 
                    {  //如果他現在的僵直時間和倒地時間都為零的話

                        #region 起身後可以動作，並延後一點無敵時間
                        /*--起身後可以動作，並延後一點無敵時間--*/
                        if (invincible_is[i])
                        {
                            felling_Time[i] -= 0.015f;
                            if (felling_Time[i] < (invincible_delay * -1))
                            {
                                invincible_is[i] = false;
                            }
                        }
                        /*-----*/
                        #endregion

                        bool hasDoSomeThingInThisframe = false;

                        #region 移動
                        if (players_KeyBoard[i].W && !hasDoSomeThingInThisframe)
                        {
                            #region 按鈕按下去後對遊戲做的處理
                            this.worldState.Gamers[i].Information_position.Position = PlayerMove(gamer.Information_position.Position, (Direction)gamer.Information_position.Rotation, gamer.Information_character.MoveSpeed, 0.015f);
                            this.worldState.Gamers[i].Information_character.Animation = 1;
                            #region 測試
                            //DisplayMessageBox("W");
                            #endregion
                            #endregion

                            hasDoSomeThingInThisframe = true;

                        }
                        if (players_KeyBoard[i].S && !hasDoSomeThingInThisframe)
                        {
                            #region 按鈕按下去後對遊戲做的處理
                            byte direct = gamer.Information_position.Rotation;
                            if (direct > (byte)16)
                                direct -= 16;
                            else
                                direct += 16;
                            this.worldState.Gamers[i].Information_position.Position = PlayerMove(gamer.Information_position.Position, (Direction)direct, gamer.Information_character.MoveSpeed, 0.015f);
                            this.worldState.Gamers[i].Information_character.Animation = 2;
                            #region 測試
                            //DisplayMessageBox("S");
                            #endregion
                            #endregion

                            hasDoSomeThingInThisframe = true;

                        }
                        if (players_KeyBoard[i].D && !hasDoSomeThingInThisframe)
                        {
                            #region 按鈕按下去後對遊戲做的處理
                            byte direct = gamer.Information_position.Rotation;
                            if (direct <= (byte)24)
                                direct += 8;
                            else
                                direct = (byte)(8 - (32 - direct));
                            this.worldState.Gamers[i].Information_position.Position = PlayerMove(gamer.Information_position.Position, (Direction)direct, gamer.Information_character.MoveSpeed, 0.015f);
                            this.worldState.Gamers[i].Information_character.Animation = 4;
                            #region 測試
                            //DisplayMessageBox("D");
                            #endregion
                            #endregion

                            hasDoSomeThingInThisframe = true;

                        }
                        if (players_KeyBoard[i].A && !hasDoSomeThingInThisframe)
                        {
                            #region 按鈕按下去後對遊戲做的處理
                            byte direct = gamer.Information_position.Rotation;
                            if (direct > (byte)8)
                                direct -= 8;
                            else
                                direct = (byte)(32 - (8 - direct));
                            this.worldState.Gamers[i].Information_position.Position = PlayerMove(gamer.Information_position.Position, (Direction)direct, gamer.Information_character.MoveSpeed, 0.015f);
                            this.worldState.Gamers[i].Information_character.Animation = 3;
                            #region 測試
                            //DisplayMessageBox("A");
                            #endregion
                            #endregion

                            hasDoSomeThingInThisframe = true;

                        }
                        #endregion
                        #region button onclick
                        #region Mouse_Left
                        if (players_KeyBoard[i].Mouse_Left && !hasDoSomeThingInThisframe)
                        {
   
                            #region 按鈕按下去後對遊戲做的處理
                            switch (this.worldState.Gamers[i].heroid)
                            {
                                case 0: // Sword 
                                    #region 測試
                                    //DisplayMessageBox("Mouse_Left");
                                    //players_KeyBoard[i].Mouse_Left = false;
                                    #endregion

                                    //判斷誰會被打到
                                    //並更改他們的狀態
                                    this.worldState.Gamers[i].Information_character.Animation = 5;

                                    #region 受限於射程，算出真正的瞄準點 real_animing_Point

                                    //真的要傳進去做範圍運算的點
                                    float[] real_aiming_Point = Get_LimitDistancePointInLine(gamer.Information_position.Position, aiming_Point[i], attack_Mouse0_Distance);

                                    #endregion
                                    #region 判斷某個目標有沒有在攻擊範圍內
                                    /* 用自己到真瞄準點所形成的直線，目標到這條直線的垂直距離和攻擊範圍比大小，看有沒有比較小
                             * 形成的攻擊範圍是向前圓柱形，像龜派氣功那樣
                             * 如果要變成以真瞄準點為圓心目標在固定範圍的話要修改(像手榴彈爆炸)
                             * */
                                    //DisplayMessageBox("real_animing_Point (x,y,z)" + real_aiming_Point[0] + "," + real_aiming_Point[1] + "," + real_aiming_Point[2]); //顯示real_aiming_Point裡的值

                                    float[] v1 = gamer.Information_position.Position;  //瞄準者原點

                                    float[] direction_vector_2 = Get_Direction_Vector(v1, real_aiming_Point); //用自己的位置 和 真正的瞄準點 算出的方向向量 //之前有同樣名稱的變數所以用 _2

                                    for (byte j = 0; j < this.worldState.Gamers.Length; j++)
                                    {
                                        #region 計算 玩家 有沒有被擊中
                                        //在哪些情況下不用算(goto NPC) 1. 目標是自己時  2. 目標示隊友時(沒開)  3.  當他處於無敵狀態時,(從無敵狀態索引陣列中得知)  4. 如果目標點在線上的投影點沒有在 瞄準點和攻擊者原點中間的話

                                        #region 1. 目標是自己時
                                        if (j == i)  //1. 目標是自己時
                                        {
                                            //DisplayMessageBox("1");
                                            //continue;
                                            goto NPC;
                                        }
                                        #endregion
                                        #region 2. 目標示隊友時 (沒開)
                                        //if (isTeammate(i, j))
                                        //    goto NPC;
                                        #endregion
                                        #region 3. 當他處於無敵狀態時
                                        if (invincible_is[j]) //如果他處於無敵中
                                        {
                                            //DisplayMessageBox("3");
                                            //continue;
                                            goto NPC;
                                        }
                                        #endregion
                                        #region 4. 如果目標點在線上的投影點沒有在 瞄準點和攻擊者原點中間的話

                                        float[] v3 = new float[3]; //先取目標的位置  
                                        v3[0] = this.worldState.Gamers[j].Information_position.Position[0];
                                        v3[1] = this.worldState.Gamers[j].Information_position.Position[1];
                                        v3[2] = this.worldState.Gamers[j].Information_position.Position[2];

                                        v3[1] += (character_Height[j] / 2); //將目標轉換為角色的中心點，原本是底部的中心

                                        float[] projected_point = Get_ProjectedPoint(v1, direction_vector_2, v3);


                                        bool between = true;
                                        for (byte k = 0; k < projected_point.Length; k++) //比較(x,y,z) 看投影點有沒有在 瞄準點 和 攻擊者原點中間
                                        {
                                            if (!Between(v1[k], real_aiming_Point[k], projected_point[k]))
                                                between = false;
                                        }
                                        if (!between)
                                        {
                                            //DisplayMessageBox("4");
                                            //continue;
                                            goto NPC;
                                        }
                                        #endregion
                                        //之前有同樣名稱的變數所以用 _2 //目標點 到 目標投影點 的距離
                                        float distance_2 = (float)Math.Sqrt(((projected_point[0] - v3[0])
                                                                              * (projected_point[0] - v3[0]))
                                                                              + ((projected_point[1] - v3[1])
                                                                              * (projected_point[1] - v3[1]))
                                                                              + ((projected_point[2] - v3[2])
                                                                              * (projected_point[2] - v3[2]))
                                                                              );
                                        #region 顯示內部變數數值(以注解)
                                        //DisplayMessageBox("Player "+j+ " distance_2 = " + distance_2);
                                        /* DisplayMessageBox("Distance = " + distance_2 + "  攻擊者原點 = " + "(" + v1[0] + " , " + v1[1] + " , " + v1[2] + ")" +
                                                              "  投影點 = " + "( " + projected_point[0] + " , " + projected_point[1] + " , " + projected_point[2]+" ) "+
                                                              " 瞄準點 = " + " ( " + real_aiming_Point[0] + " , " + real_aiming_Point[1] + " , " + real_aiming_Point[2]+" ) ");
                                        */
                                        #endregion
                                        if (distance_2 <= Valid_Range_Attack) //被打到// 目標到直線的距離 小於 攻擊的範圍 = 目標在攻擊範圍內
                                        {
                                            short damage = this.worldState.Gamers[i].Information_character.Atk;
                                            Attack_Stiff(i, j, damage);
                                        }
                                        #endregion
                                        #region 計算 npc  有沒有被擊中
                                    NPC:
                                        if (j < this.worldState.Npcs.Length)
                                        {
                                            #region 1. 目標示隊友時
                                            if ((i / 2) == j)
                                                continue;
                                            #endregion
                                            #region 2. 如果目標點在線上的投影點沒有在 瞄準點和攻擊者原點中間的話
                                            float[] v5 = new float[3];
                                            v5[0] = this.worldState.Npcs[j].Information_position.Position[0];
                                            v5[1] = this.worldState.Npcs[j].Information_position.Position[1];
                                            v5[2] = this.worldState.Npcs[j].Information_position.Position[2];

                                            float[] projected_point_2 = Get_ProjectedPoint(v1, direction_vector_2, v5);

                                            bool between_2 = true;
                                            for (byte k = 0; k < projected_point_2.Length; k++) //比較(x,y,z) 看投影點有沒有在 瞄準點 和 攻擊者原點中間
                                            {
                                                if (!Between(v1[k], real_aiming_Point[k], projected_point_2[k]))
                                                    between_2 = false;
                                            }
                                            if (!between_2)
                                            {
                                                //DisplayMessageBox("4");
                                                //continue;
                                                continue;
                                            }
                                            #endregion s
                                            float distance_3 = (float)Math.Sqrt(((projected_point_2[0] - v5[0])
                                                                                     * (projected_point_2[0] - v5[0]))
                                                                                     + ((projected_point_2[1] - v5[1])
                                                                                     * (projected_point_2[1] - v5[1]))
                                                                                     + ((projected_point_2[2] - v5[2])
                                                                                     * (projected_point_2[2] - v5[2]))
                                                                                     );
                                            if (distance_3 < 0.1f)
                                            {
                                                short damage = this.worldState.Gamers[i].Information_character.Atk;
                                                this.worldState.Npcs[j].Information_character.Hp -= damage;
                                                //DisplayMessageBox(" distance_3 < 1 " + distance_3);
                                            }
                                        }
                                        continue;
                                        #endregion
                                    }

                                    #endregion
                                    break;
                                case 1: // Cat Robot
                                    this.worldState.Gamers[i].Information_character.Animation = 5;
                                    IncreateBullet(1, 10.0f, worldState.Gamers[i]);
                                    break;
                                case 2: //Boy Robot
                                    this.worldState.Gamers[i].Information_character.Animation = 5;
                                    IncreateBullet_Melee(3,0.030f, worldState.Gamers[i], 0.06f);
                                    break;
                            }
                            
                            /*--攻擊完後的必要設定*/
                            attacking_AnimationRunTime[i] = attack_AnimationRunTime[i][0];  //有按下攻擊就要算他的剩餘動畫時間
                            attacking_AnimationState[i] = 5; //讓後幾偵能知道現在是在跑甚麼動畫
                            /*-----*/

                            players_KeyBoard[i].Mouse_Left = false; //執行完一次後就將他設為false
                            #endregion

                            hasDoSomeThingInThisframe = true;
                        }
                        else if (players_KeyBoard[i].Mouse_Left && hasDoSomeThingInThisframe) //基於一偵只能執行一個按建的效果 ， 所以這個效果不執行 
                        //所以 如果( players_KeyBoard[i].Mouse_Right && hasDoSomeThingInThisframe = true) 這個效果不執行 
                        {
                            //如果有甚麼原本效果結束要設定的在這裡設定
                            players_KeyBoard[i].Mouse_Left = false; //執行完一次後就將他設為false
                        }

                        #endregion
                        #region Mouse_Right
                        if (players_KeyBoard[i].Mouse_Right && !hasDoSomeThingInThisframe)
                        {
                            #region 按鈕按下去後對遊戲做的處理
                            #region 測試
                            //DisplayMessageBox("Mouse_Right");
                            //players_KeyBoard[i].Mouse_Right = false;
                            #endregion
                            switch (this.worldState.Gamers[i].heroid)
                            {
                                case 0: //sword
                                //判斷誰會被打到
                                //並更改他們的狀態
                               this.worldState.Gamers[i].Information_character.Animation = 6;

                                #region 受限於射程，算出真正的瞄準點 real_animing_Point

                            //真的要傳進去做範圍運算的點
                            float[] real_aiming_Point = Get_LimitDistancePointInLine(gamer.Information_position.Position, aiming_Point[i], attack_Mouse0_Distance);

                            #endregion
                                #region 判斷某個目標有沒有在攻擊範圍內
                            /* 用自己到真瞄準點所形成的直線，目標到這條直線的垂直距離和攻擊範圍比大小，看有沒有比較小
                             * 形成的攻擊範圍是向前圓柱形，像龜派氣功那樣
                             * 如果要變成以真瞄準點為圓心目標在固定範圍的話要修改(像手榴彈爆炸)
                             * */
                            //DisplayMessageBox("real_animing_Point (x,y,z)" + real_aiming_Point[0] + "," + real_aiming_Point[1] + "," + real_aiming_Point[2]); //顯示real_aiming_Point裡的值
                            float[] v1 = gamer.Information_position.Position;  //瞄準者原點

                            float[] direction_vector_2 = Get_Direction_Vector(v1, real_aiming_Point); //用自己的位置 和 真正的瞄準點 算出的方向向量 //之前有同樣名稱的變數所以用 _2

                            for (byte j = 0; j < this.worldState.Gamers.Length; j++)
                            {
                                //在哪些情況下不用算(continue) 1. 目標是自己時  2. 目標示隊友時(沒開)  3.  當他處於無敵狀態時,(從無敵狀態索引陣列中得知)  4. 如果目標點在線上的投影點沒有在 瞄準點和攻擊者原點中間的話

                                #region 1. 目標是自己時
                                if (j == i)  //1. 目標是自己時
                                {
                                    //DisplayMessageBox("1");
                                    continue;
                                }
                                #endregion
                                #region 2. 目標示隊友時 (沒開)
                                //if (isTeammate(i, j))
                                //    continue;
                                #endregion
                                #region 3. 當他處於無敵狀態時
                                if (invincible_is[j]) //如果他處於無敵中
                                {
                                    //DisplayMessageBox("3");
                                    continue;
                                }
                                #endregion
                                #region 4. 如果目標點在線上的投影點沒有在 瞄準點和攻擊者原點中間的話

                                float[] v3 = new float[3]; //先取目標的位置  
                                v3[0] = this.worldState.Gamers[j].Information_position.Position[0];
                                v3[1] = this.worldState.Gamers[j].Information_position.Position[1];
                                v3[2] = this.worldState.Gamers[j].Information_position.Position[2];

                                v3[1] += (character_Height[j] / 2); //將目標轉換為角色的中心點，原本是底部的中心

                                float[] projected_point = Get_ProjectedPoint(v1, direction_vector_2, v3);


                                bool between = true;
                                for (byte k = 0; k < projected_point.Length; k++) //比較(x,y,z) 看投影點有沒有在 瞄準點 和 攻擊者原點中間
                                {
                                    if (!Between(v1[k], real_aiming_Point[k], projected_point[k]))
                                        between = false;
                                }
                                if (!between)
                                {
                                    //DisplayMessageBox("4");
                                    continue;
                                }
                                #endregion
                                //之前有同樣名稱的變數所以用 _2 //目標點 到 目標投影點 的距離
                                float distance_2 = (float)Math.Sqrt(((projected_point[0] - v3[0])
                                                                      * (projected_point[0] - v3[0]))
                                                                      + ((projected_point[1] - v3[1])
                                                                      * (projected_point[1] - v3[1]))
                                                                      + ((projected_point[2] - v3[2])
                                                                      * (projected_point[2] - v3[2]))
                                                                      );
                                #region 顯示內部變數數值(以注解)
                                //DisplayMessageBox("Player "+j+ " distance_2 = " + distance_2);
                                /* DisplayMessageBox("Distance = " + distance_2 + "  攻擊者原點 = " + "(" + v1[0] + " , " + v1[1] + " , " + v1[2] + ")" +
                                                      "  投影點 = " + "( " + projected_point[0] + " , " + projected_point[1] + " , " + projected_point[2]+" ) "+
                                                      " 瞄準點 = " + " ( " + real_aiming_Point[0] + " , " + real_aiming_Point[1] + " , " + real_aiming_Point[2]+" ) ");
                                */
                                #endregion
                                if (distance_2 <= Valid_Range_Attack) //被打到// 目標到直線的距離 小於 攻擊的範圍 = 目標在攻擊範圍內
                                {
                                    short damage = this.worldState.Gamers[i].Information_character.Atk;
                                    Attack_Stiff(i, j, damage);
                                }

                            }
                            #endregion

                               /*--攻擊完後的必要設定*/
                                  attacking_AnimationRunTime[i] = attack_AnimationRunTime[i][1];  //有按下攻擊就要算他的剩餘動畫時間
                                  attacking_AnimationState[i] = 6; //讓後幾偵能知道現在是在跑甚麼動畫
                                  /*-----*/
                                  break;
                            
                            }
                            #endregion
                            players_KeyBoard[i].Mouse_Right = false; //執行完一次後就將他設為false
                            hasDoSomeThingInThisframe = true;
                        }
                        else if (players_KeyBoard[i].Mouse_Right && hasDoSomeThingInThisframe) //基於一偵只能執行一個按建的效果 ，
                        //所以 如果( players_KeyBoard[i].Mouse_Right && hasDoSomeThingInThisframe = true) 這個效果不執行 
                        {
                            //如果有甚麼原本效果結束要設定的在這裡設定
                            players_KeyBoard[i].Mouse_Right = false; //執行完一次後就將他設為false
                        }
                        #endregion
                        #region Q
                        if (players_KeyBoard[i].Q && !hasDoSomeThingInThisframe)
                        {
                            #region 按鈕按下去後對遊戲做的處理
                            #region 測試
                            //DisplayMessageBox("Q");
                            //players_KeyBoard[i].Q = false;
                            #endregion
                            switch (this.worldState.Gamers[i].heroid)
                            {
                                case 0: //sword
                                 IncreateBullet(0,10.0f, worldState.Gamers[i]);
                                 break;
                             }
                            #endregion
                            players_KeyBoard[i].Q = false; //執行完一次後就將他設為false
                            hasDoSomeThingInThisframe = true;
                        }
                        #endregion
                        #region E
                        if (players_KeyBoard[i].E && !hasDoSomeThingInThisframe)
                        {
                            #region 按鈕按下去後對遊戲做的處理
                            #region 測試
                            //DisplayMessageBox("E");
                            //players_KeyBoard[i].E = false;
                            #endregion
                            if (this.worldState.Gamers[i].Information_character.SkillCd[2] <= 0)
                            {
                                switch (this.worldState.Gamers[i].heroid)
                                {
                                    #region case 1 => cat robot 一次多發子彈
                                    case 1:  //cat robot
                                        this.worldState.Gamers[i].Information_character.Animation = 7;

                                        float[] fort = (float[])worldState.Gamers[i].Information_position.Position.Clone();
                                        fort[1] += 0.08f;

                                        sbyte attackerRotation = (sbyte)this.worldState.Gamers[i].Information_position.Rotation;
                                        sbyte rotation = (sbyte)(attackerRotation - 8);
                                        if (rotation <= 0)
                                            rotation = (sbyte)(32 + rotation);
                                        float[] leftShift = (float[])AngleofAspect[(byte)rotation];
                                        fort[0] += (float)((leftShift[0] * 0.06) * 2) + (AngleofAspect[attackerRotation][0] * (this.worldState.Gamers[i].Information_character.Collider_Radius * 1.2f));
                                        fort[1] += (float)((leftShift[1] * 0.02) * 2);
                                        fort[2] += (float)((leftShift[2] * 0.06) * 2) + (AngleofAspect[attackerRotation][2] * (this.worldState.Gamers[i].Information_character.Collider_Radius * 1.2f));
                                        for (byte k = 0; k <= 5; k++)
                                        {
                                            IncreateBullet_AssignPosition(2, worldState.Gamers[i], fort);
                                            fort[0] -= (float)(leftShift[0] * 0.06);
                                            fort[1] -= (float)(leftShift[1] * 0.02);
                                            fort[2] -= (float)(leftShift[2] * 0.06);
                                        }
                                    #endregion
                                        /*--攻擊完後的必要設定*/
                                        attacking_AnimationRunTime[i] = attack_AnimationRunTime[i][3];  //有按下攻擊就要算他的剩餘動畫時間
                                        attacking_AnimationState[i] = 7; //讓後幾偵能知道現在是在跑甚麼動畫
                                        break;
                                    #region case 2 => girl robot 橫向劈砍
                                    case 2: //girl robot 橫向劈砍 
                                        this.worldState.Gamers[i].Information_character.Animation = 7;
                                        IncreateBullet_AttackerFront(3,5,0.045f, worldState.Gamers[i]);
                                    #endregion 
                                        /*--攻擊完後的必要設定*/
                                        attacking_AnimationRunTime[i] = attack_AnimationRunTime[i][3];  //有按下攻擊就要算他的剩餘動畫時間
                                        attacking_AnimationState[i] = 7; //讓後幾偵能知道現在是在跑甚麼動畫
                                        break;
                                    

                                }
                                this.worldState.Gamers[i].Information_character.SkillCd[2] = skillCd_Max[i][2];
                            }
                            #endregion
                            
                            /*-----*/
                            players_KeyBoard[i].E = false; //執行完一次後就將他設為false
                            hasDoSomeThingInThisframe = true;
                        }
                        #endregion
                        #region F
                        if (players_KeyBoard[i].F && !hasDoSomeThingInThisframe)
                        {
                            #region 按鈕按下去後對遊戲做的處理
                            #region 測試
                            //DisplayMessageBox("F");
                            //players_KeyBoard[i].F = false;
                            #endregion
                            #endregion
                            players_KeyBoard[i].F = false;
                            hasDoSomeThingInThisframe = true;
                        }
                        #endregion
                        #region Space
                        if (players_KeyBoard[i].Space && !hasDoSomeThingInThisframe)
                        {
                            if (this.worldState.Gamers[i].Information_character.SkillCd[4] <= 0)
                            {
                                #region 按鈕按下去後對遊戲做的處理
                                #region 測試
                                //DisplayMessageBox("Space");
                                //players_KeyBoard[i].Space = false;
                                #endregion
                                switch (this.worldState.Gamers[i].heroid)
                                {
                                    case 1: //cat robot
                                        this.worldState.Gamers[i].Information_position.Position = PlayerMove(gamer.Information_position.Position, (Direction)gamer.Information_position.Rotation, 36.0f, 0.015f);
                                        this.worldState.Gamers[i].Information_character.Animation = 9;
                                        break;
                                    case 2: //Girl robot
                                        this.worldState.Gamers[i].Information_character.Animation = 9;
                                        attacking_AnimationRunTime[i] = attack_AnimationRunTime[i][5]; //有按下攻擊就要算他的剩餘動畫時間
                                        attacking_AnimationState[i] = 9; //讓後幾偵能知道現在是在跑甚麼動畫
                                        break;
                                }
                                players_KeyBoard[i].Space = false; //執行完一次後就將他設為false
                                this.worldState.Gamers[i].Information_character.SkillCd[4] = skillCd_Max[i][4];
                                #endregion
                                
                            }
                            hasDoSomeThingInThisframe = true;
                        }
                        else if (players_KeyBoard[i].Space && hasDoSomeThingInThisframe) //基於一偵只能執行一個按建的效果 ， 所以這個效果不執行 
                        {
                            //如果有甚麼原本效果結束要設定的在這裡設定
                            players_KeyBoard[i].Space = false; //執行完一次後就將他設為false
                        }
                        #endregion
                        #endregion
                    }
                    #endregion
                    #region 不能被操作、連擊判定
                    else if (stiff_Time[i] > 0 && felling_Time[i] <= 0 && !isDie) //有僵直沒倒地
                    {
                        stiff_Time[i] -= 0.015f;
                        this.worldState.Gamers[i].Information_character.Animation = 11; //11是僵直狀態
                        //DisplayMessageBox("Number " + i + " is in stiff ,stiff tome is  " + stiff_Time[i]);

                        /*僵直狀態不管在哪個狀態都會強迫進入，所以如果是在攻擊動畫時也會強制撥放僵直動畫。
                         * 將攻擊動畫設為零。*/
                        attacking_AnimationRunTime[i] = 0.0f; //將 現在這位玩家正在跑的動畫剩餘時間 設為零
                    }
                    else if (stiff_Time[i] > 0 && felling_Time[i] > 0 && !isDie)  //有僵直有倒地
                    {
                        /*僵直狀態不管在哪個狀態都會強迫進入，所以如果是在攻擊動畫時也會強制撥放僵直動畫。
                         * 將攻擊動畫設為零。*/
                        stiff_Time[i] -= 0.015f;
                        felling_Time[i] -= 0.015f;
                        this.worldState.Gamers[i].Information_character.Animation = 12; //12是倒地狀態
                        invincible_is[i] = true;
                        attacking_AnimationRunTime[i] = 0.0f; //將 現在這位玩家正在跑的動畫剩餘時間 設為零
                    }
                    else if (stiff_Time[i] <= 0 && felling_Time[i] > 0 && !isDie) //沒僵直有倒地
                    {
                        felling_Time[i] -= 0.015f;
                        invincible_is[i] = true;
                        this.worldState.Gamers[i].Information_character.Animation = 12; //12是倒地狀態

                        /*僵直狀態不管在哪個狀態都會強迫進入，所以如果是在攻擊動畫時也會強制撥放僵直動畫。
                         * 將攻擊動畫設為零。*/
                        attacking_AnimationRunTime[i] = 0.0f; //將 現在這位玩家正在跑的動畫剩餘時間 設為零
                    }
                    else if (stiff_Time[i] <= 0 && felling_Time[i] <= 0 && attacking_AnimationRunTime[i] > 0 && !isDie) //沒僵直沒倒地 正在跑攻擊動畫
                    {
                        attacking_AnimationRunTime[i] -= 0.015f;
                        this.worldState.Gamers[i].Information_character.Animation = attacking_AnimationState[i];

                        #region  偵測有無連擊或移動 並 執行
                        switch (this.worldState.Gamers[i].heroid )
                        {
                            case 2: //Girl Robot
                                #region 連擊
                                if ((this.worldState.Gamers[i].Information_character.Animation == 5 && players_KeyBoard[i].Mouse_Left) ||
                                    (this.worldState.Gamers[i].Information_character.Animation == 14 && !players_KeyBoard[i].Mouse_Left) ) //如果現在正在執行攻擊動畫，且玩家又按下攻擊
                                {
                                    // 在第一擊的狀態中，按下左建
                                    if (this.worldState.Gamers[i].Information_character.Animation == 5 && players_KeyBoard[i].Mouse_Left) 
                                    {
                                        attacking_AnimationRunTime[i] += 0.5f; //增加整體動畫時間
                                    }
                                    if (attacking_AnimationRunTime[i] < 0.100f && attacking_AnimationRunTime[i] >= 0.085f) //當 動畫時間跑到這時，新增子彈
                                    {
                                        //IncreateBullet_AttackerFront(0, 0.045f, worldState.Gamers[i]);
                                        IncreateBullet_Melee(3, 0.030f, worldState.Gamers[i], 0.06f);
                                    }
                                    this.worldState.Gamers[i].Information_character.Animation = 14; //要一直設定動畫為  14 
                                    attacking_AnimationState[i] = 14;
                                    players_KeyBoard[i].Mouse_Left = false;                                   
                                    
                                }
                                else if ((this.worldState.Gamers[i].Information_character.Animation == 14 && players_KeyBoard[i].Mouse_Left)||
                                    this.worldState.Gamers[i].Information_character.Animation==15 )
                                {
                                    // 在第二擊的狀態中，按下左建
                                    // 已進入第二擊狀態，不代表客戶端動畫進入第二擊動畫。可能客戶端還在跑第一個動畫。對於這兩種狀況進行處理
                                    //主要用動畫時間判斷
                                    if (this.worldState.Gamers[i].Information_character.Animation == 5 && players_KeyBoard[i].Mouse_Left)
                                    {
                                        attacking_AnimationRunTime[i] += 0.5f;
                                    }
                                    if (attacking_AnimationRunTime[i] < 0.6f && attacking_AnimationRunTime[i] >= 0.585f)
                                    {
                                        //IncreateBullet_AttackerFront(0, 0.045f, worldState.Gamers[i]); //新增子彈
                                        IncreateBullet_Melee(3, 0.030f, worldState.Gamers[i], 0.06f);
                                        //DisplayMessageBox(" back  batter 1 ");
                                    }
                                    if (attacking_AnimationRunTime[i] < 0.100f && attacking_AnimationRunTime[i] >= 0.085f)
                                    {
                                        //IncreateBullet_AttackerFront(0, 0.045f, worldState.Gamers[i]); //新增子彈
                                        IncreateBullet_Melee(3, 0.030f, worldState.Gamers[i], 0.06f, true);
                                        //DisplayMessageBox(" back  batter 2 ");
                                    }
                                    this.worldState.Gamers[i].Information_character.Animation = 15;
                                    attacking_AnimationState[i] = 15;
                                    players_KeyBoard[i].Mouse_Left = false;  
                                }
                                #endregion 
                                #region move space
                                else if (this.worldState.Gamers[i].Information_character.Animation == 9)
                                {
                                    this.worldState.Gamers[i].Information_position.Position =
                                             PlayerMove(gamer.Information_position.Position, (Direction)gamer.Information_position.Rotation, 1.2f, 0.015f); //移動
                                    changingRotation[i] = false; //不允許在 move space 中 ，進行方向同步
                                    if (players_KeyBoard[i].Mouse_Left) //在向前衝時，按下攻擊鍵
                                    {
                                        this.worldState.Gamers[i].Information_character.Animation = 16; 
                                        IncreateBullet_AttackerFront(3, 0.045f, worldState.Gamers[i]);
                                        attacking_AnimationRunTime[i] = 0.030f;
                                        attacking_AnimationState[i] = 16;
                                        players_KeyBoard[i].Mouse_Left = false;
                                        changingRotation[i] = true;
                                    }
                                    if (attacking_AnimationRunTime[i] <= 0.015f) //在最後一偵設定回來
                                        changingRotation[i] = true;
                                }
                                #endregion 
                                break;
                            default:
                                //this.worldState.Gamers[i].Information_character.Animation = attacking_AnimationState[i];
                                break;
                        }
                        #endregion 
                    }
                    #region 死亡狀態
                    else if ((dieWaitingTime[i] <= 0) && isDie) //HP 小於零 ，  且還沒有死亡時間 。表示是剛要進入死亡狀態。
                    {
                        //做剛要進入死亡狀態的設定

                        dieWaitingTime[i] = Get_DieWaitingTime(gamer);                  //先設定這位玩家的現在死亡等待時間
                        this.worldState.Gamers[i].Information_character.Animation = 13; //先設定成死亡的狀態
                    }
                    else if ((dieWaitingTime[i] > 0) && isDie) //HP 小於零 ，可是進入死亡狀帶的設定已經設定過了
                    {
                        dieWaitingTime[i] -= 0.015f;
                        this.worldState.Gamers[i].Information_character.Animation = 13; //先設定成死亡的狀態

                        if (dieWaitingTime[i] <= 0) //如果在//HP 小於零 ，可是進入死亡狀帶的設定已經設定過了的情況下，這偵的死亡等待時間已經減掉小於等於零後
                        {
                            //進行結束死亡的設定 (復活設定)
                            this.worldState.Gamers[i].Information_character.Animation = 0;                  //回到 Idle 狀態 
                            this.worldState.Gamers[i].Information_character.Hp = Get_Maximum_HP(gamer);     //回覆滿血

                            float[] resurrection_Point = new float[3];                                      //取得復活點
                            resurrection_Point[0] = aLive_Point[i][0];
                            resurrection_Point[1] = aLive_Point[i][1];
                            resurrection_Point[2] = aLive_Point[i][2];

                            this.worldState.Gamers[i].Information_position.Position = resurrection_Point;   //在復活點復活
                        }
                    }
                    #endregion

                    #endregion

                }
                #endregion

                #region 1. 檢測玩家 cd    2. 檢查所有玩家的位置，如果超出所限制的範圍，則將位置設為上一偵的位置 3.檢查所有玩家的位置，如果在不能進入的地方將位置變成上一偵的位置
                for (byte i = 0; i < worldState.Gamers.Length; i++)
                {
                    #region 檢測玩家 cd  (CD 時間減掉這偵時間)
                    for (byte j = 0; j < worldState.Gamers[i].Information_character.SkillCd.Length; j++)
                    {
                        if (worldState.Gamers[i].Information_character.SkillCd[j] > 0)
                        {
                            worldState.Gamers[i].Information_character.SkillCd[j] -= 0.015f;
                        }
                    }
                    #endregion
                    bool needRemark = true;  //決定要不要位置還原
                    #region 檢查有沒有在基本地圖裡面
                for (byte j = 0; j < map.Length; j++) //由於世界地圖是由方型組成所以要跟所有的方形比較
                {
                    if ((bool)(IsInThisAera_Square(map[j], worldState.Gamers[i].Information_position.Position)))
                    {
                        needRemark = false;
                    }
                }
                #endregion
                    #region 檢查有沒有在不能在的位置   ex 路邊車子裡
                for (byte j = 0; j < map_object.Count; j++)
                {
                    if ((bool)(IsInThisAera_Square(map_object[j], worldState.Gamers[i].Information_position.Position)))
                    {
                        needRemark = true;
                    }
                }
                #endregion 
                    #region 設定現在的位置為上一偵的位置 (位置還原)
                if (needRemark)
                    {
                        float[] before_position = this.temporaryStorage_WorldState.Get_LastState_Position(i);
                        worldState.Gamers[i].Information_position.Position = before_position;
                    }
                #endregion 
                    
                }
            #endregion


                #region 在場景中子彈的行為 (子彈行為只受自己影響，和撞到人)
                //先創建要消除的子彈索引清單，這真得子彈處理算完後在統一清除
                List<byte> indexs_Remove = new List<byte>();
                for (byte i = 0; i < bullets_InScene.Count; i++)
                {
                    #region 各種不同子彈的移動方法 (目前只有寫 2 子彈的) (默認依現在的速度移動，往現在的方向，直線移動)
                    switch (bullets_InScene[i].BulletMeshNumber)
                    {
                        case 2: //子彈2 cat_Robot 的大絕 (先停留幾秒鐘後，在向前飛)
                            if (bullets_InScene[i].LifeTime_Now < (0.6f - player_Pings[bullets_InScene[i].AttackerNumber]) && bullets_InScene[i].LifeTime_Now >= 0.030f)
                            {
                                //位置不改變 //- (player_Pings[bullets_InScene[i].AttacterNumber]/2)
                                bullets_InScene[i].State = 2;
                            }
                            else
                            {
                                //跟原本的一樣
                                bullets_InScene[i].State = 2;
                                bullets_InScene[i].Position_now[0] = bullets_InScene[i].Position_now[0] + (bullets_InScene[i].Direction[0] * bullets_InScene[i].Speed);
                                bullets_InScene[i].Position_now[1] = bullets_InScene[i].Position_now[1] + (bullets_InScene[i].Direction[1] * bullets_InScene[i].Speed);
                                bullets_InScene[i].Position_now[2] = bullets_InScene[i].Position_now[2] + (bullets_InScene[i].Direction[2] * bullets_InScene[i].Speed);
                            }
                                
                        break;
                        default:
                        //讓子彈依照速度移動
                        bullets_InScene[i].State = 2;
                        bullets_InScene[i].Position_now[0] = bullets_InScene[i].Position_now[0] + (bullets_InScene[i].Direction[0] * bullets_InScene[i].Speed);
                        bullets_InScene[i].Position_now[1] = bullets_InScene[i].Position_now[1] + (bullets_InScene[i].Direction[1] * bullets_InScene[i].Speed);
                        bullets_InScene[i].Position_now[2] = bullets_InScene[i].Position_now[2] + (bullets_InScene[i].Direction[2] * bullets_InScene[i].Speed);
                        break;
                    }
                    #endregion 
                    bullets_InScene[i].LifeTime_Now += 0.015f;

                    float[] bullet_position = bullets_InScene[i].Position_now;  //注意 : 這裡是傳參考，要複製值要用 Clone方法
                    #region 判斷玩家有沒有被擊中

                    for (byte j = 0; j < worldState.Gamers.Length; j++)
                    {
                        /*
                         * 判斷方式 
                         * 玩家的 y 軸形成一條線，判斷離子彈的最近距離 => 算法 玩家(x,z) 到 子彈(x,z) 的距離 
                         * 這個距離等於或小於 player.collider.radius + bullet.radius 就等於被打到
                         */
                        #region 1.判斷是不是自己人
                        
                        if (isTeammate(bullets_InScene[i].AttackerNumber, j))
                            continue;
                        
                        #endregion 
                        #region 2.判斷是不是無敵
                        if (invincible_is[j])
                            continue;
                        #endregion 
                        #region 3.用上述的判斷方式
                        float[] target_position = worldState.Gamers[j].Information_position.Position;//注意 : 這裡是傳參考，要複製值要用 Clone方法
                        if (!((bullet_position[1] <= (target_position[1] + worldState.Gamers[j].Information_character.Collider_Height)) && (bullet_position[1] >= target_position[1]))) //檢查 Y 座標
                            continue;
                        float distance = (float)Math.Sqrt(((bullet_position[0] - target_position[0]) * (bullet_position[0] - target_position[0])) +
                                                   ((bullet_position[2] - target_position[2]) * (bullet_position[2] - target_position[2]))); //玩家和子彈的距離 (x,z) 平面
                        distance = distance - bullets_InScene[i].Radius - worldState.Gamers[j].Information_character.Collider_Radius;        //減掉 子彈的半徑 和玩家的半徑
                        if (distance <= 0 ) //檢查距離
                        {
                            //撞到處理
                            // 1. 給傷害
                            if (!bullets_InScene[i].Fell) //沒倒地，就是僵直
                                Attack_Stiff_Bullet(bullets_InScene[i].Attacket_Direction, j, (short)bullets_InScene[i].Damage);
                            else  //倒地
                                Attack_Felling_Bullet(bullets_InScene[i].Attacket_Direction, j, (short)bullets_InScene[i].Damage);
                            // 2.  加入自毀清單，等這偵全部算玩在從 bullets_InScene 裡，清除
                            if(!indexs_Remove.Contains(i))
                                indexs_Remove.Add(i);
                        }
                        #endregion 
                    }

                    #endregion 
                    #region 判斷NPC有沒有被擊中
 
                    for (byte j = 0; j < this.worldState.Npcs.Length; j++)
                    {
                        #region 1.判斷是不是自己人 (沒開)
                        /*
                        if (isTeammate(worldState.Npcs[j].number, bullets_InScene[i].Team))
                            continue;
                         * */
                        #endregion
                        float[] target_position = worldState.Npcs[j].Information_position.Position;//注意 : 這裡是傳參考，要複製值要用 Clone方法
                        if (!((bullet_position[1] <= (target_position[1] + worldState.Npcs[j].Information_character.Collider_Height)) && (bullet_position[1] >= target_position[1]))) //檢查 Y 座標
                            continue;
                        float distance = (float)Math.Sqrt(((bullet_position[0] - target_position[0]) * (bullet_position[0] - target_position[0])) +
                                                   ((bullet_position[2] - target_position[2]) * (bullet_position[2] - target_position[2])));
                        distance = distance - bullets_InScene[i].Radius - worldState.Npcs[j].Information_character.Collider_Radius;
                        if (distance <= 0) //檢查距離
                        {
                            //撞到處理
                            short damage = (short)bullets_InScene[i].Damage;
                            this.worldState.Npcs[j].Information_character.Hp -= damage;
                            //加入自毀清單，等這偵全部算玩在從 bullets_InScene 裡，清除
                            if (!indexs_Remove.Contains(i))
                                indexs_Remove.Add(i);
                        }                       
                    }
 
                    #endregion
                    #region 判斷有沒有撞到地圖中的物件 ex 路邊車子裡                    
                    for (byte j = 0; j < map_object.Count; j++)
                    {
                        if ((bool)(IsInThisAera_Square(map_object[j], bullets_InScene[i].Position_now)))
                        {
                            //加入自毀清單，等這偵全部算玩在從 bullets_InScene 裡，清除
                            if (!indexs_Remove.Contains(i))
                                indexs_Remove.Add(i);
                        }
                    }
                    #endregion
                    #region 檢查有沒有在基本地圖裡面
                    bool notIn = true;
                    for (byte j = 0; j < map.Length; j++) //由於世界地圖是由方型組成所以要跟所有的方形比較
                    {
                        if ((bool)(IsInThisAera_Square(map[j], bullets_InScene[i].Position_now)))
                            notIn = false;
                    }
                    if (notIn)
                    {
                        //加入自毀清單，等這偵全部算玩在從 bullets_InScene 裡，清除
                        if (!indexs_Remove.Contains(i))
                            indexs_Remove.Add(i);
                    }
                    #endregion
                    #region 子彈沒撞到人還在移動中的處理
                    /*如果他的 life time > max life time 讓他銷毀*/
                    /*銷毀要寫個功能*/
                    /*如果他的 life time < max life time 讓他的 life time + 0.15(這偵的時間)*/
                    if (bullets_InScene[i].LifeTime_Now >= bullets_InScene[i].LifeTime_Max)
                    {
                        bullets_InScene[i].State = 3;
                        indexs_Remove.Add(i);
                    }

                    #endregion 

                }
            
                
                #endregion
                #region 將子彈狀態打包，加入worldstate
                Bullet[] bullets_InScene_Array = ((IEnumerable<Bullet>)bullets_InScene).ToArray();
                this.worldState.Bullets = (Bullet[])bullets_InScene_Array.Clone();

                #endregion
                #region 清除清單中索引的 子彈
                //清除清單中索引的 子彈 
                if (indexs_Remove.Count > 0)
                {
                    foreach (byte index in indexs_Remove)
                    {
                        bullets_InScene.Remove(bullets_InScene[index]);
                    }
                }
                #endregion 

                #region Npc 藉由這偵玩家的狀態，做出反應
                for (byte l = 0; l < this.worldState.Npcs.Length; l++)
                    {
                        Gamer[] npcs_last = this.worldState.Npcs;
                        if (l == 0 || l == 1) //這時是輪到兩座主堡
                        {
                            if (npcs_last[l].Information_character.Hp <= 0.0f)
                            {
                                #region 結束遊戲

                                #region 計算獎勵,在資料庫修改,傳送結束封包
                                List<byte> winner = new List<byte>();
                                List<byte> loser = new List<byte>();
                                for (byte m = 0; m < this.worldState.Gamers.Length; m++)
                                {
                                    if ((m / 2) != l) //是贏的那隊
                                    {
                                        winner.Add(m);
                                    }
                                    else if ((m / 2) == l) //是輸的那隊
                                    {
                                        loser.Add(m);
                                    }
                                }
                                npcs_last[l].Information_character.Hp = 200;
                                Dictionary<byte, object> packet = new Dictionary<byte, object> //打包世界狀態
                        {
                             {(byte)0,(byte)3},
                        };
                                packet.Add((byte)1, true);
                                foreach (byte n in winner)
                                {
                                    player_Peers[n].Increase_Money(Get_HowMuchMoneyCanGet(n, true));
                                    player_Peers[n].SendEvent((byte)13, packet);
                                }
                                packet.Remove((byte)1);
                                packet.Add((byte)1, false);
                                foreach (byte o in loser)
                                {
                                    player_Peers[o].Increase_Money(Get_HowMuchMoneyCanGet(o, false));
                                    player_Peers[o].SendEvent((byte)13, packet);
                                }
                                #endregion

                                StopGame(); //停止遊戲繼續進行
                                this.Dispose();
                                #endregion
                            }
                        }

                    }
            #endregion
        }
        
        /// <summary>
        /// Execute every 30 ms
        /// </summary>
        private void upDate30MS()
        {

            if (readly_All) //如果大家都進入遊戲場景並準備好了
            {
                #region 每30ms傳一次這個class的worldstate給所有玩家
                this.worldState.Time_Gaming = this.timer_Gaming; //將時間放入世界狀態


                Dictionary<byte, object> packet = new Dictionary<byte, object> //打包世界狀態
                {
                        {(byte)0,(byte)1},
                        {(byte)1,startOnline.Serializate.ToByteArray(this.worldState)},
                };
                for (byte i = 0; i < player_Peers.Length; i++)
                {
                    player_Peers[i].SendEvent((byte)13, packet);
                    packet.Remove(2);
                }
                #endregion
                this.temporaryStorage_WorldState.Add(this.worldState); //加入暫存

                #region  Reset Animation State  每偵一開始都將狀態歸零(設為Idle)
                for (byte i = 0; i < this.worldState.Gamers.Length; i++)
                {
                    this.worldState.Gamers[i].Information_character.Animation = 0;
                }
                #endregion
                // DisplayMessageBox("123");
            }
            //DisplayMessageBox("321");
        }
        #endregion
        #region 釋放 Dispose()
        public void Dispose()
        {
            player_Peers[0].RemoveGamingRoom(this);
        }
        #endregion

        #region 功能 ( 普通 - DisplayMessageBox,PlayerMove,Get_HowMuchMoneyCanGet,StopGame,Get_PlayerTeam, 空間 - Between,isTeammate,Get_LimitDistancePointInLine,Get_Direction_Vector,Get_ProjectedPoint,IsInThisAera_Square , 攻擊 - Attack_Stiff)
        /// <summary>
        /// 新增一個執行緒 並用它新增一個MessageBox
        /// </summary>
        /// <param name="context"></param>
        private void DisplayMessageBox(string context)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                System.Windows.Forms.MessageBox.Show(context);
            })).Start();
        }
        /// <summary>
        /// 傳回經過一段時間後的位置
        /// </summary>
        /// <param name="fromposition">起始位置</param>
        /// <param name="fromrotation">面對方向，吃Direction參數</param>
        /// <param name="speed">每秒速度</param>
        /// <param name="duration">經過時間</param>
        /// <returns>float[] [0] =x,[1] =y,[2]=z</returns>
        public float[] PlayerMove(float[] fromposition, Direction fromrotation, float speed, float duration)
        {
            float[] newposition = new float[3];
            newposition[0] = fromposition[0] + ((speed * duration) * AngleofAspect[(byte)fromrotation][0]);
            newposition[1] = fromposition[1] + ((speed * duration) * AngleofAspect[(byte)fromrotation][1]);
            newposition[2] = fromposition[2] + ((speed * duration) * AngleofAspect[(byte)fromrotation][2]);
            return newposition;

        }
        /// <summary>
        /// 移動到相對於自己指定方向，指定距離的位置 . torotation 吃相對方向
        /// </summary>
        /// <param name="fromposition">起始位置</param>
        /// <param name="fromrotation">原本面對方向，吃Direction參數</param>
        /// <param name="torotation">要移動的方向(相對)，吃Direction參數</param>
        /// <param name="distance">要移動的距離</param>
        /// <returns>float[] [0] =x,[1] =y,[2]=z</returns>
        public float[] PlayerMove(float[] fromposition, Direction fromrotation,Direction torotation, float distance)
        {
            //fromrotation 是絕對方向
            //torotation 是相對方向 
            //先將真正的絕對方向算出來
            byte real_direction = (byte)((byte)fromrotation + (byte)torotation);
            if (real_direction > 32) //大於就遞回
            {
                real_direction = (byte)(real_direction % 32);
            }
            float[] newposition = new float[3];
            newposition[0] = fromposition[0] + (distance * AngleofAspect[real_direction][0]);
            newposition[1] = fromposition[1] + (distance * AngleofAspect[real_direction][1]);
            newposition[2] = fromposition[2] + (distance * AngleofAspect[real_direction][2]);
            return newposition;
        }
        /// <summary>
        /// 回傳 指定方向,距離 的位置
        /// </summary>
        /// <param name="fromposition">要移動的人，原本的位置</param>
        /// <param name="torotation">往甚麼方向移動 (絕對)</param>
        /// <param name="distance">移動多少距離</param>
        /// <returns></returns>
        public float[] PlayerMove(float[] fromposition, Direction torotation, float distance)
        {
            float[] newposition = new float[3];
            newposition[0] = fromposition[0] + (distance * AngleofAspect[(byte)torotation][0]);
            newposition[1] = fromposition[1] + (distance * AngleofAspect[(byte)torotation][1]);
            newposition[2] = fromposition[2] + (distance * AngleofAspect[(byte)torotation][2]);
            return newposition;
        }
        /// <summary>
        /// 回傳 compareValue 的大小 有沒有在 value1 和 value2 之間
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="compareValue"></param>
        /// <returns>傳入的三個值相等 也回傳 true</returns>
        private bool Between(float value1, float value2, float compareValue)
        {
            if (value1 == value2 && value2 == compareValue)
                return true;
            else if (value1 == value2 && value2 !=compareValue)
                return false;
            float big,small;            
            if (value1 > value2)
            {
                big = value1;
                small = value2;
            }
            else
            {
                big = value2;
                small = value1;
            }
            if (compareValue < big && compareValue > small)
                return true;
            else if (compareValue == big || compareValue == small)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 傳入兩個Player ID ，回傳他們是不是同隊.只能在只有兩隊時使用。 0,1 一組、2,3 一組
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <returns></returns>
        private bool isTeammate(byte player1, byte player2)
        {
            byte total = (byte)worldState.Gamers.Length; // 總共有多少位玩家 //共4位。player id 是 0,1,2,3
            if (player1 < (total / 2) && player2 < (total / 2))
                return true;
            else if (player1 >= (total / 2) && player2 >= (total / 2))
                return true;
            else
                return false;
        }
        /// <summary>
        /// 用player id 取得自己是哪隊， 0 or 2。
        /// </summary>
        /// <param name="playerid">玩家的編號</param>
        /// <returns>自己是哪隊， 0 or 2。</returns>
        private byte Get_PlayerTeam(byte playerid)
        {
            byte total = (byte)worldState.Gamers.Length;
            byte whichteam = (byte)(playerid / (total / 2));
            if(whichteam==1)
                whichteam =2;
            return whichteam;
        }
        /// <summary>
        /// 在一條由point1和point2組成的直線，如果長度 > limit_distance，將point2 內縮直到長度 = limit_distance 在回傳 
        /// </summary>
        /// <param name="point1">空間中一點</param>
        /// <param name="point2">空間中另一點</param>
        /// <param name="limit_distance">限制的長度</param>
        /// <returns></returns>
        private float[] Get_LimitDistancePointInLine(float[] point1, float[] point2, float limit_distance)
        {
            float[] point;

            //distance 是自己到瞄準點的距離
            float distance = (float)Math.Sqrt((point2[0] - point1[0]) * (point2[0] - point1[0]) +
                                         (point2[1] - point1[1]) * (point2[1] - point1[1]) +
                                         (point2[2] - point1[2]) * (point2[2] - point1[2]));

            if (distance <= limit_distance) //如果瞄準點本來就在射程內，將真正的點等於瞄準點
                point = point2;
            else
            {   //瞄準點 如果不在射程內
                //用在這個方向向量上,符合射程的最大點替代
                point = new float[3];
                float[] direction_vector = new float[3];
                direction_vector[0] = point2[0] - point1[0];
                direction_vector[1] = point2[1] - point1[1];
                direction_vector[2] = point2[2] - point1[2];

                point[0] = point1[0] + ((direction_vector[0] / distance) * limit_distance);
                point[1] = point1[1] + ((direction_vector[1] / distance) * limit_distance);
                point[2] = point1[2] + ((direction_vector[2] / distance) * limit_distance);
            }

            return point;
        }
        /// <summary>
        /// 算出point1 往 point2 方向 的方向向量
        /// </summary>
        /// <param name="point1">空間中的一點</param>
        /// <param name="point2">空間中的另一點</param>
        /// <returns>方向向量</returns>
        private float[] Get_Direction_Vector(float[] point1,float[] point2)
        {
            float[] direction_vector = new float[3];
            direction_vector[0] = point2[0] - point1[0];
            direction_vector[1] = point2[1] - point1[1];
            direction_vector[2] = point2[2] - point2[2];
            return direction_vector;
        }
        /// <summary>
        /// 計算空間中的一點，到某條直線上的投影點
        /// </summary>
        /// <param name="onLine_Point">線上的一點</param>
        /// <param name="direction_vector">線的方向向量</param>
        /// <param name="targrt_Point">要投影的點</param>
        /// <returns>targrt_Point 到直線的投影點</returns>
        private float[] Get_ProjectedPoint(float[] onLine_Point, float[] direction_vector, float[] targrt_Point)
        {
             /*先把 t 算出來
            
             * t 是 在假設投影點的常數
             * 投影點.x = (方向向量.x *t) + 線上的一點.x , 以此類推x,y,z
             */
            float t = (((targrt_Point[0] - onLine_Point[0]) * direction_vector[0]) + ((targrt_Point[1] - onLine_Point[1]) * direction_vector[1]) + ((targrt_Point[2] - onLine_Point[2]) * direction_vector[2]))
                    / ((direction_vector[0] * direction_vector[0]) + (direction_vector[1] * direction_vector[1]) + (direction_vector[2] * direction_vector[2]));

            float[] projected_point = new float[3];//投影點 //目標點 在 直線上的投影點,  直線是由(瞄準點 和 攻擊者原點 所構成 的)
            projected_point[0] = onLine_Point[0] + (t * direction_vector[0]);
            projected_point[1] = onLine_Point[1] + (t * direction_vector[1]);
            projected_point[2] = onLine_Point[2] + (t * direction_vector[2]);
            return projected_point;
        }
        /// <summary>
        /// 要造成普通僵直的話就用這個功能 (不包含動畫的更改)
        /// </summary>
        /// <param name="attacker">攻擊者 player id</param>
        /// <param name="attacked">被攻擊者 player id</param>
        /// <param name="damage">要對被攻擊者扣掉的血量，通常是 attacker 的攻擊力 atk</param>
        /// <returns>有沒有成功執行</returns>
        private bool Attack_Stiff(byte attacker,byte attacked,short damage)
        {
            //當playerid = j 的玩家被判定打中時的處理
            /*---如果只有造成僵直的攻擊就只有這行*/
            stiff_Time[attacked] = stiffTime; //將他的僵直時間設為  stiffTime
            //造成傷害的程式碼
            //short damage = this.worldState.Gamers[i].Information_character.Atk;
            this.worldState.Gamers[attacked].Information_character.Hp -= damage;

            //將他的位置稍微往攻擊者的方向後退一點
            byte r1 = this.worldState.Gamers[attacker].Information_position.Rotation;// 取得攻擊者方向
            this.worldState.Gamers[attacked].Information_position.Position = PlayerMove(this.worldState.Gamers[attacked].Information_position.Position, (Direction)r1, 0.01f);
            return true;
        }
        /// <summary>
        /// 要造成普通僵直的話就用這個功能，給子彈使用的 (不包含動畫的更改)。差別是 要自己輸入攻擊者方向，之前是透過攻擊者 id 幫忙取
        /// </summary>
        /// <param name="bulletRotation">攻擊者 方向 ， 32 方位</param>
        /// <param name="attacked">被攻擊者 player id</param>
        /// <param name="damage">要對被攻擊者扣掉的血量，通常是 attacker 的攻擊力 atk</param>
        /// <returns>有沒有成功執行</returns>
        private bool Attack_Stiff_Bullet(byte bulletRotation, byte attacked, short damage)
        {
            //當playerid = j 的玩家被判定打中時的處理
            /*---如果只有造成僵直的攻擊就只有這行*/
            stiff_Time[attacked] = stiffTime; //將他的僵直時間設為  stiffTime
            //造成傷害的程式碼
            //short damage = this.worldState.Gamers[i].Information_character.Atk;
            this.worldState.Gamers[attacked].Information_character.Hp -= damage;

            //將他的位置稍微往攻擊者的方向後退一點
            byte r1 = bulletRotation;// 取得攻擊者方向
            this.worldState.Gamers[attacked].Information_position.Position = PlayerMove(this.worldState.Gamers[attacked].Information_position.Position, (Direction)r1, 0.01f);
            return true;
        }
        /// <summary>
        /// 要造成普通倒地的話就用這個功能 (不包含動畫的更改)
        /// </summary>
        /// <param name="attacker">攻擊者 player id</param>
        /// <param name="attacked">被攻擊者 player id</param>
        /// <param name="damage">要對被攻擊者扣掉的血量，通常是 attacker 的攻擊力 atk</param>
        /// <returns>有沒有成功執行</returns>
        private bool Attack_Felling(byte attacker, byte attacked, short damage)
        {
            // 當playerid = j 的玩家被判定打中時的處理
            /*---造成倒地的攻擊*/
            felling_Time[attacked] = fellingTime;
            /*-----*/
            // 造成傷害的程式碼
            //short damage = this.worldState.Gamers[i].Information_character.Atk;
            this.worldState.Gamers[attacked].Information_character.Hp -= damage;


            //將他的位置稍微往攻擊者的方向後退一點
            byte r1 = this.worldState.Gamers[attacker].Information_position.Rotation;// 取得攻擊者方向
            this.worldState.Gamers[attacked].Information_position.Position = PlayerMove(this.worldState.Gamers[attacked].Information_position.Position, (Direction)r1, 0.01f);
            return true;
        }
        /// <summary>
        /// 要造成普通倒地的話就用這個功能 (不包含動畫的更改)
        /// </summary>
        /// <param name="attacker">攻擊者 player id</param>
        /// <param name="attacked">被攻擊者 player id</param>
        /// <param name="damage">要對被攻擊者扣掉的血量，通常是 attacker 的攻擊力 atk</param>
        /// <returns>有沒有成功執行</returns>
        private bool Attack_Felling_Bullet(byte bulletRotation, byte attacked, short damage)
        {
            // 當playerid = j 的玩家被判定打中時的處理
            /*---造成倒地的攻擊*/
            felling_Time[attacked] = fellingTime;
            /*-----*/
            // 造成傷害的程式碼
            //short damage = this.worldState.Gamers[i].Information_character.Atk;
            this.worldState.Gamers[attacked].Information_character.Hp -= damage;


            //將他的位置稍微往攻擊者的方向後退一點
            byte r1 = bulletRotation;// 取得攻擊者方向
            this.worldState.Gamers[attacked].Information_position.Position = PlayerMove(this.worldState.Gamers[attacked].Information_position.Position, (Direction)r1, 0.01f);
            return true;
        }
        /// <summary>
        /// 取得現在角色的死亡等待時間。要用Gamer 裡的資訊算出他現在的的死亡等待時間。由於是測試都先用10.0f。
        /// </summary>
        /// <param name="gamer">哪位要計算的玩家</param>
        /// <returns>死亡等待時間</returns>
        private float Get_DieWaitingTime(Gamer gamer)
        {
            /* 
             * 要用Gamer 裡的資訊算出他的死亡等待時間 
             * 由於是測試都先用10.0f
             */
            return 10.0f;
        }
        /// <summary>
        /// 取得角色的最大血量。要用Gamer 裡的資訊算出他現在的的最大血量。由於是測試都先用100.0f。
        /// </summary>
        /// <param name="gamer">>哪位要計算的玩家</param>
        /// <returns>最大血量</returns>
        private short Get_Maximum_HP(Gamer gamer)
        {
            /* 
             * 要用Gamer 裡的資訊算出他的最大血量
             * 由於是測試都先用100.0f
             */
            return 100;
        }
        /// <summary>
        /// 回傳這場遊戲結束後能拿多少錢
        /// </summary>
        /// <param name="playerid">是哪位玩家</param>
        /// <param name="win">這位玩家有沒有贏</param>
        /// <returns>拿多少錢</returns>
        private int Get_HowMuchMoneyCanGet(byte playerid, bool win)
        {
            //----還沒實做
            if (win)
                return 200;
            else
                return 100;
        }
        /// <summary>
        /// 讓遊戲停止 (停止計時器觸發事件)
        /// </summary>
        private void StopGame()
        {
            this.upDate.Enabled = false;
        }
        /// <summary>
        /// 藉由一平面做區隔，判斷兩點是否在同一區。(都在面上算在同一區)
        /// </summary>
        /// <param name="point1_FormingFace">用來形成面的點1</param>
        /// <param name="point2_FormingFace">用來形成面的點2</param>
        /// <param name="point3_FormingFace">用來形成面的點3</param>
        /// <param name="point1_target">目標點 1 </param>
        /// <param name="point2_target">目標點 2 </param>
        /// <returns>目標點 1 和 目標點 2 有沒有在同一區 (都在面上算在同一區) 。有一點在面上，另一點沒有，也算在同一區</returns>
        private bool IsInSameAera(float[] point1_FormingFace, float[] point2_FormingFace, float[] point3_FormingFace, float[] point1_target, float[] point2_target)
        {
            float[] normal_vector = new float[3]; 
            /*point1和point2形成的向量
              和
              point1和point3形成的向量
              的外積 = 構成此平面的法向量
              (外積公式)
            * */
            normal_vector[0] = ((point2_FormingFace[1] - point1_FormingFace[1]) * (point3_FormingFace[2] - point1_FormingFace[2])) - ((point2_FormingFace[2] - point1_FormingFace[2]) * (point3_FormingFace[1] - point1_FormingFace[1]));
            normal_vector[1] = ((point2_FormingFace[2] - point1_FormingFace[2]) * (point3_FormingFace[0] - point1_FormingFace[0])) - ((point2_FormingFace[0] - point1_FormingFace[0]) * (point3_FormingFace[2] - point1_FormingFace[2]));
            normal_vector[2] = ((point2_FormingFace[0] - point1_FormingFace[0]) * (point3_FormingFace[1] - point1_FormingFace[1])) - ((point2_FormingFace[1] - point1_FormingFace[1]) * (point3_FormingFace[0] - point1_FormingFace[0]));

            //平面方程式用point1和法向量構成
            float result1 = (normal_vector[0] * (point1_target[0] - point1_FormingFace[0])) + (normal_vector[1] * (point1_target[1] - point1_FormingFace[1])) + (normal_vector[2] * (point1_target[2] - point1_FormingFace[2])); //將目標點1 帶入平面方程式
            float result2 = (normal_vector[0] * (point2_target[0] - point1_FormingFace[0])) + (normal_vector[1] * (point2_target[1] - point1_FormingFace[1])) + (normal_vector[2] * (point2_target[2] - point1_FormingFace[2])); //將目標點2 帶入平面方程式

            if ((result1 > 0 && result2 > 0) || (result1 < 0 && result2 < 0) || result1 == 0 || result2 == 0)
                return true;
            else
                return false;

        }
        /// <summary>
        /// 判斷targetpoint有沒有在此區域內。只能用於方形    (如果未來要增加圖形可以考慮直接增加一參數，並用此判斷不同形狀，並用不同判斷方式)
        /// </summary>
        /// <param name="thisAera">構成此方形的 8個點 。 在陣列裡的點要照順序 ， 0~3 前面四邊形四點， 4~7 後面四邊形四點 ，都從四邊形的左上開始 </param>
        /// <param name="targtpoint">目標點</param>
        /// <returns>targetpoint有沒有在此區域內</returns>
        private bool? IsInThisAera_Square(float[][] thisAera, float[] targetpoint)
        {
            if (thisAera.Length != 8)  //檢查是不是八個點
                return null;
            #region 計算重心 center
            float[] center = new float[]{0.0f,0.0f,0.0f}; //重心
            /*--計算重心 -> x,y,z各自的平均--*/
            for (byte i = 0;i < thisAera.Length; i++) 
            {
                center[0] += thisAera[i][0];
                center[1] += thisAera[i][1];
                center[2] += thisAera[i][2];
            }
            center[0] = center[0] / thisAera.Length;
            center[1] = center[1] / thisAera.Length;
            center[2] = center[2] / thisAera.Length;
            /*----*/
            #endregion
            #region 暴力法 檢查重心和目標有沒有在六個面的同測 (沒有的話直接回傳 false)
            for (byte i = 0; i < 6; i++)
            {
                switch (i)
                {
                    case 0:
                        if (!IsInSameAera(thisAera[0], thisAera[1], thisAera[2], targetpoint, center))
                            return false;
                        break;
                    case 1:
                        if (!IsInSameAera(thisAera[4], thisAera[5], thisAera[6], targetpoint, center))
                            return false;
                        break;
                    case 2:
                        if (!IsInSameAera(thisAera[0], thisAera[3], thisAera[4], targetpoint, center))
                            return false;
                        break;
                    case 3:
                        if (!IsInSameAera(thisAera[0], thisAera[1], thisAera[4], targetpoint, center))
                            return false;
                        break;
                    case 4:
                        if (!IsInSameAera(thisAera[1], thisAera[2], thisAera[5], targetpoint, center))
                            return false;
                        break;
                    case 5:
                        if (!IsInSameAera(thisAera[2], thisAera[3], thisAera[6], targetpoint, center))
                            return false;
                        break;
                }
            }
            #endregion 
            return true;
        }
        /// <summary>
        /// 按下攻擊後，製造子彈用 (砲台位置是 發射者位置像上移0.08f)
        /// </summary>
        /// <param name="bullet_MeshNumber">要用的子彈種類</param>
        /// <param name="attacker">攻擊者</param>
        private void IncreateBullet(byte bullet_MeshNumber,float bullet_LifeMaxTime, Gamer attacker)
        {
            /*場景中最多有 125個子彈 0~124*/
            /*先算出可用的編號*/
            /*編號是 0~124 124~0 遞回 */
            if (bullets_InScene.Count >= 125)
                return;
            byte available_bullet_number = this.available_Bullet_Number;
            this.available_Bullet_Number++;
            if (this.available_Bullet_Number > 124) //遞回
                this.available_Bullet_Number = 0;
            //DisplayMessageBox(available_bullet_number.ToString());

            float[] fort_Location = (float[])attacker.Information_position.Position.Clone(); //砲台位置
            fort_Location[1] += 0.08f;
            //砲台要在自己的碰撞器外面
            fort_Location[0] = fort_Location[0] + (AngleofAspect[attacker.Information_position.Rotation][0] * attacker.Information_character.Collider_Radius + 0.01f); 
            fort_Location[2] = fort_Location[2] + (AngleofAspect[attacker.Information_position.Rotation][2] * attacker.Information_character.Collider_Radius + 0.01f);
            float[] vector_direction = new float[3];
            
            vector_direction[0] = aiming_Point[attacker.number][0] - fort_Location[0];
            vector_direction[1] = aiming_Point[attacker.number][1] - fort_Location[1];
            vector_direction[2] = aiming_Point[attacker.number][2] - fort_Location[2];
            float disance = (float)Math.Sqrt((vector_direction[0] * vector_direction[0]) + (vector_direction[1] * vector_direction[1]) + (vector_direction[2] * vector_direction[2]));
            vector_direction[0] = vector_direction[0] / disance;
            vector_direction[1] = vector_direction[1] / disance;
            vector_direction[2] = vector_direction[2] / disance;

            Bullet bullet = new Bullet(available_bullet_number, bullet_MeshNumber, attacker.number, 1, fort_Location, vector_direction, attacker.Information_position.Rotation, 0.066f, attacker.Information_character.Atk, 0.005f, bullet_LifeMaxTime, 0.0f);
            bullets_InScene.Add(bullet);
        }
        /// <summary>
        /// 按下攻擊後，製造子彈用 (指定發射位置) (原本的砲台位置是 發射者位置像上移0.08f)
        /// </summary>
        /// <param name="bullet_MeshNumber">要用的子彈種類</param>
        /// <param name="attacker">攻擊者</param>
        /// <param name="position">指定發射位置</param>
        private void IncreateBullet_AssignPosition(byte bullet_MeshNumber, Gamer attacker,float[] position)
        {
            /*場景中最多有 125個子彈 0~124*/
            /*先算出可用的編號*/
            /*編號是 0~124 124~0 遞回 */
            if (bullets_InScene.Count >= 125)
                return;
            byte available_bullet_number = this.available_Bullet_Number;
            this.available_Bullet_Number++;
            if (this.available_Bullet_Number > 124) //遞回
                this.available_Bullet_Number = 0;
            //DisplayMessageBox(available_bullet_number.ToString());

            float[] fort_Location = position; //砲台位置


            float[] vector_direction = new float[3];

            vector_direction[0] = aiming_Point[attacker.number][0] - fort_Location[0];
            vector_direction[1] = aiming_Point[attacker.number][1] - fort_Location[1];
            vector_direction[2] = aiming_Point[attacker.number][2] - fort_Location[2];
            float disance = (float)Math.Sqrt((vector_direction[0] * vector_direction[0]) + (vector_direction[1] * vector_direction[1]) + (vector_direction[2] * vector_direction[2]));
            vector_direction[0] = vector_direction[0] / disance;
            vector_direction[1] = vector_direction[1] / disance;
            vector_direction[2] = vector_direction[2] / disance;

            Bullet bullet = new Bullet(available_bullet_number, bullet_MeshNumber, attacker.number, 1, fort_Location, vector_direction, attacker.Information_position.Rotation, 0.066f, attacker.Information_character.Atk, 0.005f, 10.0f, 0.0f);
            bullets_InScene.Add(bullet);
        }
        /// <summary>
        /// 按下攻擊後，製造子彈用 (子彈的方向向量固定為，功擊者前方)
        /// </summary>
        /// <param name="bullet_MeshNumber">要用的子彈種類</param>
        /// <param name="attacker">攻擊者</param>
        private void IncreateBullet_AttackerFront(byte bullet_MeshNumber, float bullet_LifeMaxTime, Gamer attacker)
        {
            /*場景中最多有 125個子彈 0~124*/
            /*先算出可用的編號*/
            /*編號是 0~124 124~0 遞回 */
            if (bullets_InScene.Count >= 125)
                return;
            byte available_bullet_number = this.available_Bullet_Number;
            this.available_Bullet_Number++;
            if (this.available_Bullet_Number > 124) //遞回
                this.available_Bullet_Number = 0;
            //DisplayMessageBox(available_bullet_number.ToString());

            float[] fort_Location = (float[])attacker.Information_position.Position.Clone(); //砲台位置
            fort_Location[1] += 0.08f;
            //砲台要在自己的碰撞器外面
            fort_Location[0] = fort_Location[0] + (AngleofAspect[attacker.Information_position.Rotation][0] * attacker.Information_character.Collider_Radius + 0.01f);
            fort_Location[2] = fort_Location[2] + (AngleofAspect[attacker.Information_position.Rotation][2] * attacker.Information_character.Collider_Radius + 0.01f);

            float[] vector_direction = (float[])AngleofAspect[attacker.Information_position.Rotation].Clone(); //子彈飛行 方向向量(vector_direction) 和 功擊者方向一樣
           

            Bullet bullet = new Bullet(available_bullet_number, bullet_MeshNumber, attacker.number, 1, fort_Location, vector_direction, attacker.Information_position.Rotation, 0.066f, attacker.Information_character.Atk, 0.005f, bullet_LifeMaxTime, 0.0f);
            bullets_InScene.Add(bullet);
        }
        /// <summary>
        /// 按下攻擊後，製造子彈用 (子彈的方向向量固定為，功擊者前方)
        /// </summary>
        /// <param name="bullet_MeshNumber">要用的子彈種類</param>
        /// <param name="bullet_Damage_Multiple">攻擊力是功擊者攻擊力的幾倍?</param>
        /// <param name="attacker">攻擊者</param>
        private void IncreateBullet_AttackerFront(byte bullet_MeshNumber, byte bullet_Damage_Multiple, float bullet_LifeMaxTime, Gamer attacker)
        {
            /*場景中最多有 125個子彈 0~124*/
            /*先算出可用的編號*/
            /*編號是 0~124 124~0 遞回 */
            if (bullets_InScene.Count >= 125)
                return;
            byte available_bullet_number = this.available_Bullet_Number;
            this.available_Bullet_Number++;
            if (this.available_Bullet_Number > 124) //遞回
                this.available_Bullet_Number = 0;
            //DisplayMessageBox(available_bullet_number.ToString());

            float[] fort_Location = (float[])attacker.Information_position.Position.Clone(); //砲台位置
            fort_Location[1] += 0.08f;
            //砲台要在自己的碰撞器外面
            fort_Location[0] = fort_Location[0] + (AngleofAspect[attacker.Information_position.Rotation][0] * attacker.Information_character.Collider_Radius + 0.01f);
            fort_Location[2] = fort_Location[2] + (AngleofAspect[attacker.Information_position.Rotation][2] * attacker.Information_character.Collider_Radius + 0.01f);

            float[] vector_direction = (float[])AngleofAspect[attacker.Information_position.Rotation].Clone(); //子彈飛行 方向向量(vector_direction) 和 功擊者方向一樣


            Bullet bullet = new Bullet(available_bullet_number, bullet_MeshNumber, attacker.number, 1, fort_Location, vector_direction, attacker.Information_position.Rotation, 0.066f, attacker.Information_character.Atk * bullet_Damage_Multiple, 0.005f, bullet_LifeMaxTime, 0.0f);
            bullets_InScene.Add(bullet);
        }
        /// <summary>
        /// 按下攻擊後，製造子彈用 (近戦專用) (子彈的方向向量固定為，功擊者前方)
        /// </summary>
        /// <param name="bullet_MeshNumber">要用的子彈種類</param>
        /// <param name="attacker">攻擊者</param>
        /// <param name="bullet_radius">子彈的半徑</param>
        private void IncreateBullet_Melee(byte bullet_MeshNumber, float bullet_LifeMaxTime, Gamer attacker, float bullet_radius)
        {
            /*場景中最多有 125個子彈 0~124*/
            /*先算出可用的編號*/
            /*編號是 0~124 124~0 遞回 */
            if (bullets_InScene.Count >= 125)
                return;
            byte available_bullet_number = this.available_Bullet_Number;
            this.available_Bullet_Number++;
            if (this.available_Bullet_Number > 124) //遞回
                this.available_Bullet_Number = 0;
            //DisplayMessageBox(available_bullet_number.ToString());

            float[] fort_Location = (float[])attacker.Information_position.Position.Clone(); //砲台位置
            fort_Location[1] += 0.08f;

            float[] vector_direction = (float[])AngleofAspect[attacker.Information_position.Rotation].Clone(); //子彈飛行 方向向量(vector_direction) 和 功擊者方向一樣


            Bullet bullet = new Bullet(available_bullet_number, bullet_MeshNumber, attacker.number, 1, fort_Location, vector_direction, attacker.Information_position.Rotation, 0.026f, attacker.Information_character.Atk, bullet_radius, bullet_LifeMaxTime, 0.0f);
            bullets_InScene.Add(bullet);
        }
        /// <summary>
        /// 按下攻擊後，製造子彈用 (近戦專用) (子彈的方向向量固定為，功擊者前方)
        /// </summary>
        /// <param name="bullet_MeshNumber">要用的子彈種類</param>
        /// <param name="attacker">攻擊者</param>
        /// <param name="bullet_radius">子彈的半徑</param>
        private void IncreateBullet_Melee(byte bullet_MeshNumber, float bullet_LifeMaxTime, Gamer attacker, float bullet_radius,bool fell)
        {
            /*場景中最多有 125個子彈 0~124*/
            /*先算出可用的編號*/
            /*編號是 0~124 124~0 遞回 */
            if (bullets_InScene.Count >= 125)
                return;
            byte available_bullet_number = this.available_Bullet_Number;
            this.available_Bullet_Number++;
            if (this.available_Bullet_Number > 124) //遞回
                this.available_Bullet_Number = 0;
            //DisplayMessageBox(available_bullet_number.ToString());

            float[] fort_Location = (float[])attacker.Information_position.Position.Clone(); //砲台位置
            fort_Location[1] += 0.08f;

            float[] vector_direction = (float[])AngleofAspect[attacker.Information_position.Rotation].Clone(); //子彈飛行 方向向量(vector_direction) 和 功擊者方向一樣


            Bullet bullet = new Bullet(available_bullet_number, bullet_MeshNumber, attacker.number,fell, 1, fort_Location, vector_direction, attacker.Information_position.Rotation, 0.026f, attacker.Information_character.Atk, bullet_radius, bullet_LifeMaxTime, 0.0f);
            bullets_InScene.Add(bullet);
        }
        #endregion
        #region 專門 新增 map_object 的功能
        /// <summary>
        /// 用來創建立方形的功能 。先建造出底面 在用 height ，將四個點向上推移，建造出立方體 (建造底面時要順時針輸入 point1~4)
        /// </summary>
        /// <param name="square_Point1">底面點1 ( point1~4要順時鐘輸入)</param>
        /// <param name="square_Point2">底面點2 ( point1~4要順時鐘輸入)</param>
        /// <param name="square_Point3">底面點3 ( point1~4要順時鐘輸入)</param>
        /// <param name="square_Point4">底面點4 ( point1~4要順時鐘輸入)</param>
        /// <param name="height">底面向上移動多少</param>
        /// <returns></returns>
        private void AddNew_MapObject_Square(float[] square_Point1, float[] square_Point2, float[] square_Point3, float[] square_Point4,float height)
        {
            float[][] object1 = new float[8][];
            object1[0] = square_Point1;
            object1[1] = square_Point2;
            object1[2] = square_Point3;
            object1[3] = square_Point4;
            for (byte i = 4; i < object1.Length; i++)
            {
                object1[i] = new float[] { object1[i - 4][0], object1[i - 4][1] + height, object1[i - 4][2] };   //等於是把0~3點往上移3，當作 4~7 點
            }
            map_object.Add(object1);    //加入List
        }
        #endregion
    }

    public class KeyBoard
    {
        public bool W;
        public bool S;
        public bool A;
        public bool D;
        public bool Mouse_Left;
        public bool Mouse_Right;
        public bool Q;
        public bool E;
        public bool F;
        public bool Space;

        public KeyBoard()
        {
            W = false;
            S = false;
            A = false;
            D = false;
            Mouse_Left = false;
            Mouse_Right = false;
            Q = false;
            E = false;
            F = false;
            Space = false;
        }
    }
}
