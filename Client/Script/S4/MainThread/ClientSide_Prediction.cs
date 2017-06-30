using UnityEngine;
using System;
using System.Collections.Generic;

namespace Assets.Script.S4.MainThread
{
    public class ClientSide_Prediction : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            #region 初始化 AngleofAspect

            float radians = 57.2958f; //(一個弧 = 57.2958角度)
            float degrees2 = 0.0f;
            for (byte i = 1; i < AngleofAspect.Length; i++) // [0]不放，是null 所以 i=1 
            {
                AngleofAspect[i] = new float[3];
                if ((degrees2 % 90) != 0) //暴力法 //因為當 degrees = 0,90,180,270 時會錯 ，不知道為啥
                {
                    AngleofAspect[i][0] = (float)Math.Sin(degrees2 / radians); //x 軸向左右 代表原本(x,y) 的 x軸
                    AngleofAspect[i][1] = 0.0f;                               //y 軸向上下 所以無解
                    AngleofAspect[i][2] = (float)Math.Cos(degrees2 / radians); //z 軸向前 代表原本(x,y) 的 y軸  
                }
                else
                {
                    byte which = (byte)(degrees2 / 90);
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
                degrees2 += 11.25f;
                //Debug.Log(" Number = " + i + " (x,y) = " + AngleofAspect[i][0] + "," + AngleofAspect[i][2]);
            }

            #endregion

        }

        private List<Command_Move> command_List; //流水號從0開始


        #region 方向(取代360度的32方向)
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

        #region 功能 PlayerMove
        /// 傳回經過一段時間後的位置
        /// </summary>
        /// <param name="fromposition">起始位置</param>
        /// <param name="fromrotation">面對方向，吃Direction參數</param>
        /// <param name="speed">每秒速度</param>
        /// <param name="duration">經過時間</param>
        /// <returns>float[] [0] =x,[1] =y,[2]=z</returns>
        public float[] PlayerMove(float[] fromposition, byte fromrotation, float speed, float duration)
        {
            float[] newposition = new float[3];
            newposition[0] = fromposition[0] + ((speed * duration) * AngleofAspect[(byte)fromrotation][0]);
            newposition[1] = fromposition[1] + ((speed * duration) * AngleofAspect[(byte)fromrotation][1]);
            newposition[2] = fromposition[2] + ((speed * duration) * AngleofAspect[(byte)fromrotation][2]);
            return newposition;
        }
        #endregion
    }

    public struct Command_Move
    {
        public Command_Move(byte number,float timestep, float euler_y)
        {
            this.number = number;
            this.timestep = timestep;
            this.euler_Y = euler_y;
        }
        public byte number; //流水號從0開始
        public float timestep; //時間搓
        public float euler_Y;
    }
}