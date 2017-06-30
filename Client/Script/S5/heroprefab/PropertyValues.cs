using UnityEngine;
using System.Collections;

public class PropertyValues : MonoBehaviour
{

    #region Script 定義
    //這裡個Script用來記錄角色的能力值
    //給其他Script使用
    //有繼承Mono
    #endregion

    //血量 Blood
    private float bLO;
    public float BLO { get { return bLO; }
                       set { bLO = ComputingBLOFormula(value); }
                     }

    //攻擊力 ATK
    private float aTK;
    public float ATK { get { return aTK; }
                       set { aTK = ComputingATKFormula(value); }
                     }

    //防禦力 Defense
    private float dEF;
    public float DEF{ get { return dEF; }
                      set { dEF = ComputingDEFormula(value); }
                    }

    //移動速度 Speed
    private float sPE;
    public float SPE { get { return sPE; }
                       set { sPE = ComputingSPEormula(value); }
                     }
    /// <summary>
    /// 變更ATK的計算公式。
    /// </summary>
    /// <param name="value1">參數一</param>
    /// <returns></returns>
    public virtual float ComputingATKFormula(float value1)
    {
        return value1;
    }
    /// <summary>
    /// 變更DEF的計算公式
    /// </summary>
    /// <param name="value1">參數一</param>
    /// <returns></returns>
    public virtual float ComputingDEFormula(float value1)
    {
        return value1;
    }
    /// <summary>
    /// 變更SPE的計算公式
    /// </summary>
    /// <param name="value1">參數一</param>
    /// <returns></returns>
    public virtual float ComputingSPEormula(float value1)
    {
        return value1;
    }
    /// <summary>
    /// 變更BLO的計算公式。
    /// </summary>
    /// <param name="value1">參數一</param>
    /// <returns></returns>
    public virtual float ComputingBLOFormula(float value1)
    {
        return value1;
    }
}
