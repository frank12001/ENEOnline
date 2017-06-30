using UnityEngine;
using System.Collections.Generic;
using Assets.Script;

namespace Assets.Script.S4.MainThread
{
    public class ReceiveAndStoreWorldStates : MonoBehaviour
    {
        #region 定義暫存世界狀態 * 4 Worldstate //最精簡的快照 預定30ms傳一次

        private const byte worldAccount = 4; //暫存的世界數量

        private const byte howMuchPlayersInTheGame = 4; //一場遊戲中有幾人
        /// <summary>
        ///  0 , 2 一組 ， 1 , 3 一組 。
        /// </summary>
        private Worldstate[] storages_WorldState = new Worldstate[worldAccount]; //固定是 4 個暫存區。0 , 2 一組 ， 1 , 3 一組，互相內插彼此。

        #endregion

        /// <summary>
        /// 0,1,2,3
        /// </summary>
        private byte worldForce=0; //存放 4 個世界的現在的焦點;

        #region Render 出畫面的功能 ，委派給 Render.cs
        //private delegate void RenderWorldHandler(Worldstate fromShapshot, Worldstate toShapshot);   //包含內插功能
        private delegate void RenderWorldHandler(Worldstate worldState); //用來render出一個WorldState //測試用，不包含內插
        /// <summary>
        /// 吃兩個WorldState SnapShot 一個是from 一個是to
        /// </summary>
        //private event RenderWorldHandler renderWorld;//包含內插功能
        private event RenderWorldHandler renderWorld;  //測試用，不包含內插
        #endregion
        private delegate void GameOverHandler(bool win);
        private event GameOverHandler GameOver;
        // Use this for initialization
        void Start()
        {

            #region 掛勾事件 (Receive_WorldState,render_World,GameOver)
            ConnectFunction.F.Receive_WorldState += this.Receive_WorldState; //從 gameService.cs接收 世界狀態的功能
            //this.renderWorld += this.GetComponentInChildren<Render>().render_World;
            this.renderWorld += this.GetComponentInChildren<Render>().Render_World; //將 Render.cs 的功能，掛勾給這個 this.cs，讓這功能可以在這使用
            this.GameOver += this.GetComponentInChildren<GameOver>().GameOverProcess;
            #endregion 


            #region 在進入這個場景時，從暫存區取出世界狀態，並用於初始化

            for (byte i = 0; i < storages_WorldState.Length; i++) 
            {
                storages_WorldState[i] = Temporary_Storage._WorldState; //要將這個世界狀態存入所有的暫存區
            }

            
            #endregion

            #region 初始化完畢 嘗試將暫存的空間釋放 (由於Render.cs也要使用這個世界狀態進行初始化，所以增加一個布林判斷，可不可以刪除)
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
            
        }

        // This function is called when the MonoBehaviour will be destroyed
        void OnDestroy()
        {
            #region 掛勾事件-取消
            ConnectFunction.F.Receive_WorldState -= this.Receive_WorldState;
            //this.renderWorld -= this.GetComponentInChildren<Render>().render_World;
            this.renderWorld -= this.GetComponentInChildren<Render>().Render_World;
            #endregion
        }
        

        // Update is called once per frame
        void Update()
        {

        }
        /// <summary>
        /// 接收世界狀態
        /// </summary>
        /// <param name="packet">世界狀態</param>
        public void Receive_WorldState(Dictionary<byte, object> packet)
        {           
            byte switchCode = (byte)packet[0];
            //Debug.Log("Receive_WorldState");
            switch(switchCode)
            { 
                case 1: //先不要用暫存區和內插，存脆傳過來後render
                    /*
                Worldstate worldstate = (Worldstate) Assets.Script.Serializate.ToObject((byte[])packet[1]);

                Gamer[] gamer = worldstate.Gamers;

                storages_WorldState[worldForce] = worldstate; //將傳來的WorldState SnapShot 先存入 暫存區

                renderWorld(storages_WorldState[Mathf.Abs(((worldForce/2)-1))+1], storages_WorldState[worldForce]); //根據現在的worldForce推斷要把誰，當作renderWorld的參數 

                if (worldForce < worldAccount)
                    worldForce++;
                else
                    worldForce = 0;
                     */
                    Worldstate worldstate = (Worldstate)Assets.Script.Serializate.ToObject((byte[])packet[1]);
                    renderWorld(worldstate);
                break;
                case 3: //接收結束統計封包
                bool win = (bool)packet[1];
                this.GameOver(win);
                break;
            }
            
        }
      
    }
    
}
