using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


    interface IPropertyBasic
    {
        //血量 Blood
        float BLO {get;set;}

        //攻擊力 ATK
        float ATK { get; set; }

        //防禦力 Defense
        float DEF { get; set; }

        //移動速度 Speed
        float SPE { get; set; }

        //硬值 Suspend 
        float SUS { get; set; }

        //自己是第幾隊
        byte TEAM { get; set; }

        /// <summary>
        /// 給攻擊物件使用，是否要給被打到的人倒地?
        /// </summary>
        bool FALL { get; set; }

        #region 這些參數依據撞到的人或發出的人不同而有不同效果
        /// <summary>
        /// 用來記錄數字，可多種使用
        /// </summary>
        byte NUMBER { get; set; }
        /// <summary>
        /// 用來記錄布林，可多種使用
        /// </summary>
        bool BOOL { get;set; }
        #endregion
        #region Suspend On & Off
        void SuspendOn();
        void SuspendOff();
        #endregion

    }

