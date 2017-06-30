using UnityEngine;
using System.Collections;

public class attackObject : MonoBehaviour,IPropertyBasic {
    
    
    

    //attack 的最大存活時間
    public float LiftTime;
    private float timer; //計時器，用於比較LiftTime

    //傷害量
    private float damage;
    public float Damage 
    { get{return damage; }
      set{ damage = ComputingDamageFormula(value); }
    }

    private void Awake()
    {
        this.FALL = false;
    }
	// Use this for initialization
	void Start () {
        timer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        timer = timer + Time.deltaTime;
        if (timer >= LiftTime)
            Destroy(this.gameObject);
	}

    public void OnDisable(){
        //在這裡寫 Instantate 出特效物件
    }

    public void OnTriggerEnter(Collider other)
    {
        IPropertyBasic target = other.gameObject.GetComponent<IPropertyBasic>();
        if (target != null)
        {
            if (target.TEAM != this.TEAM)
            {
                if (target != null)
                    target.BLO = target.BLO - (this.ATK - target.DEF);
                if (FALL) //如果這個攻擊物件是要讓別人倒地的 //那就將目標的FALL設成true
                    target.FALL = this.FALL;
                target.SuspendOn(); //開啟對方的硬質狀態
                if (target.BLO <= 0) //表示我殺了一個人，在GameManager中殺人數+1
                {
                    GameManager.Kill[this.NUMBER]++;
                    if (this.NUMBER == GameManager.MyPlayerId) //只在Sender中進行energy的改變
                        GameManager.IncreateAssignPlayerEnergy(this.NUMBER, 80);
                }
                //target.BOOL = true; //考慮要不要將這個刪掉 包跨全部的BOOL
                /*uiBoold noControllUI = other.gameObject.GetComponent<uiBoold>();
                if (noControllUI != null)
                {
                    noControllUI.b = true;
                    noControllUI.Blo -= 10;
                }
                 * */
                GameManager.CreateText(this.ATK, this.gameObject.transform.position);
                Destroy(this.gameObject);
            }
        }

       
    }


    /// <summary>
    /// 計算傷害公式。用於複寫
    /// </summary>
    /// <param name="damage">第一個參數</param>
    /// <returns></returns>
    public virtual float ComputingDamageFormula(float damage)
    {
        return damage;
    }

    #region 實做的參數

    public float BLO
    {
        get;
        set;
    }

    public float ATK
    {
        get;
        set;
    }

    public float DEF
    {
        get;
        set;
    }

    public float SPE
    {
        get;
        set;
    }

    public float SUS
    {
        get;
        set;
    }
    public byte TEAM
    {
        get;
        set;
    }


    public void SuspendOn()
    {
        
    }

    public void SuspendOff()
    {
        
    }

    /// <summary>
    /// 在attackObject中這個NUMBER代表 攻擊者的playerid
    /// </summary>
    public byte NUMBER
    {
        get;
        set;
    }


    public bool FALL{get;set; }

    public bool BOOL
    {
        get;
        set;
    }
    #endregion



}
