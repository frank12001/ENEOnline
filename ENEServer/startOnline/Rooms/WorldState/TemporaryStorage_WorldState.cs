using System;
using System.Windows.Forms;


namespace startOnline.Rooms.WorldState
{
    public class TemporaryStorage_WorldState
    {
        public TemporaryStorage_WorldState(Worldstate worldstate)
        {
            this.worldStates = new Worldstate[33];
            for (byte i = 0; i < worldStates.Length; i++)
            {
                worldStates[i] = new Worldstate();
                worldStates[i].Time_Gaming = worldstate.Time_Gaming;
                worldStates[i].Gamers = (Gamer[])worldstate.Gamers.Clone();
            }
        }
        /*--只備份 WorldState 中的，玩家資訊和其時間--*/
        Worldstate[] worldStates;
        float lastTimestamp = 0.0f;


        public void Add(Worldstate worldstate)
        {
            for (byte i = 1; i < 33; i++) //為了追求快速，使用實數做為判斷 
            {
                this.worldStates[i - 1].Time_Gaming = this.worldStates[i].Time_Gaming;
                this.worldStates[i - 1].Gamers = this.worldStates[i].Gamers;
            }

            this.worldStates[32].Time_Gaming = worldstate.Time_Gaming;
            this.worldStates[32].Gamers = (Gamer[])worldstate.Gamers.Clone();
            this.lastTimestamp = worldstate.Time_Gaming;
        }
        /// <summary>
        /// 用時間去取世界狀態
        /// </summary>
        /// <param name="Timestamp"></param>
        /// <returns></returns>
        public Worldstate Get(float Timestamp)
        {
            Worldstate worldstate = new Worldstate();
            if (Timestamp < (lastTimestamp - 990)) //超出下限了
                return null;
            if (Timestamp >= (lastTimestamp - 60)) //如果是落在 32 31(大多數人)，順便處裡太大的可能性
            {
                if (Timestamp >= (lastTimestamp - 30))
                {
                    worldstate.Time_Gaming = this.worldStates[32].Time_Gaming;
                    worldstate.Gamers = (Gamer[])this.worldStates[32].Gamers.Clone();
                }
                else
                {
                    worldstate.Time_Gaming = this.worldStates[31].Time_Gaming;
                    worldstate.Gamers = (Gamer[])this.worldStates[31].Gamers.Clone();
                }
            }
            else
            {
                int whichWorldstate = (int)((Timestamp - (lastTimestamp - 990)) / 30);
                worldstate.Time_Gaming = this.worldStates[whichWorldstate].Time_Gaming;
                worldstate.Gamers = (Gamer[])this.worldStates[whichWorldstate].Gamers.Clone();
            }
            return worldstate;      
        }
        /// <summary>
        /// 取得最近的世界狀態
        /// </summary>
        /// <returns>最近的世界狀態</returns>
        public Worldstate Get_LastState()
        {
            Worldstate worldstate = new Worldstate();
            worldstate.Gamers = this.worldStates[this.worldStates.Length - 1].Gamers;
            worldstate.Time_Gaming = this.worldStates[this.worldStates.Length - 1].Time_Gaming;
            return worldstate;
        }
        /// <summary>
        /// 取得最後存進去的狀態 某人的位置
        /// </summary>
        /// <param name="playernumber">某人的 player id</param>
        /// <returns>最後狀態某人的位置</returns>
        public float[] Get_LastState_Position(byte playernumber)
        {           
            float[] before_position = this.worldStates[this.worldStates.Length - 1].Gamers[playernumber].Information_position.Position;
            float[] position = new float[]{before_position[0],before_position[1],before_position[2]};
            return position;
        }

    }
}
