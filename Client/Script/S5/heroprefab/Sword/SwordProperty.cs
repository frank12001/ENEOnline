using UnityEngine;
using System.Collections;
using TMPro;

public class SwordProperty : MonoBehaviour, IPropertyBasic
{


    //存放Basic 能力值和 Unique 能力值

    #region PropertyBasic

    #region BLO 由於太多所以自己用一個
    private float blo = 0.0f;
    public float BLO
    {
        get
        {
            return blo;
        }
        set
        {
            if (maxBlood < blo)
                maxBlood = blo;
            if (this.noControllUI != null)
            {
                noControllUI.b = true;
                noControllUI.Blo -= this.bloodPercentage(this.blo - value, maxBlood);
            }
            blo = value;
        }
    }
    private uiBoold noControllUI;
    /// <summary>
    /// Sender以後可能會用到，現在等級血量的最大值
    /// </summary>
    private float maxBlood = 0.0f;
    private float bloodPercentage(float nowblood, float maxblood)
    {
        if (maxblood == 0)
            return 0.0f;
        return (nowblood * 100) / maxblood;
    }

    #endregion

    public float ATK { get; set; }

    public float DEF { get; set; }

    public float SPE { get; set; }

    private const float maxSus = 8.0f;
    private const float originalSus = 0.5f;

    private float sus = 0.0f;
    public float SUS
    {
        get { return this.sus; }
        set { if (SUS <= maxSus) { sus = sus + value; } }
    }

    public byte TEAM { get; set; }

    public bool FALL
    {
        get;
        set;
    }
    public byte NUMBER
    {
        get
        {
            throw new System.NotImplementedException();
        }
        set
        {
            throw new System.NotImplementedException();
        }
    }
    public bool BOOL
    {
        get;
        set;
    }

    #endregion

    #region Energy 由程式碼比較多所以自己使用一個標籤
    /// <summary>
    /// 由於Energy不需被對手(其他人)存取，所以沒寫在PropertyBasic裡，以減少空間 min=0,max=100
    /// </summary>
    private byte energy = 0;//要寫它的值變化方法
    /// <summary>
    /// 能量 min=0,max=100
    /// </summary>
    public byte Energy
    {
        get { return energy; }
        set
        {
            if (value > 100)
                this.energy = 100;
            else if (value < 0)
                this.energy = 0;
            else
                this.energy = value;
        }
    }
    #endregion

    #region 自己額外的Property

    //復活點
    public Vector3 ResurrectionPoint;

    //倒地時間
    public float FallTime = 5.0f;

    //死亡等待時間
    public float DeathTime = 10.0f; //暫時設成10.0f

    //技能的 CD時間 減少的比例
    public float CDReduceMouse01 = 1.0f; //(每個技能都有一個) 
    #endregion

    //倒地和死亡使用的和計時器比較的參數
    private float dietimer = -1.0f;

    #region 目前等級 + 升等數值存放陣列

    private byte headLV = 0;
    private byte handLV = 0;
    private byte bodyLV = 0;
    private byte legLV = 0;

    /// <summary>
    /// 1.其他部位滿等才可升級頭部 2.頭升一級 +( 現在所有的能力值* x%) x為此陣列裡的值 
    /// </summary>
    private float[] swordHeadArray;
    private float[] swordHandArray;
    private float[] swordBodyBLOArray;
    private float[] swordBodyDEFArray;
    private float[] swordLegArray;
    #endregion

    // Use this for initialization
    void Start()
    {

        noControllUI = gameObject.GetComponentInChildren<uiBoold>();

        //初始化升等數值(定值)
        swordHeadArray = new float[] { 0.5f, 0.2f, 0.8f };
        swordHandArray = new float[] { 41.0f, 42.0f, 41.0f, 83.0f, 106.0f, 83.0f, 83.0f, 83.0f, 106.0f, 83.0f, 249.0f };
        swordBodyBLOArray = new float[] { 208.0f, 208.0f, 208.0f, 624.0f, 416.0f, 416.0f, 416.0f, 832.0f, 416.0f, 416.0f, 840.0f };
        swordBodyDEFArray = new float[] { 17.85f, 17.85f, 35.7f, 71.4f, 35.7f, 35.7f, 71.4f, 35.7f, 35.7f, 35.7f, 107.3f };
        swordLegArray = new float[] { 1.0f, 1.8f, 1.8f, 2.6f, 1.4f, 1.4f, 1.4f, 1.4f, 1.4f, 1.4f, 3.4f };

        blo = swordBodyBLOArray[0];
        ATK = swordHandArray[0];
        DEF = swordBodyDEFArray[0];
        SPE = swordLegArray[0];
        sus = 0.5f;

        handLV++;
        bodyLV++;
        legLV++;
        //headLV不用升等因為它沒有初始值，所以[0]直接當第一次升等的參數

        //角色名子為索引
        if (byte.Parse(this.gameObject.name) <= 4)
            TEAM = 1;
        else
            TEAM = 2;


    }
    // Update is called once per frame
    void Update()
    {
        #region 測試數值時使用
        /*
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            BodyLVUp();
            Debug.Log("blo= " + this.BLO +"def  " +this.DEF+"level ="+bodyLV);
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            HandLVUp();
            Debug.Log("atk= " + this.ATK + "level =" + handLV);

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LegLVUp();
            Debug.Log("spe= " + this.SPE + "level =" + legLV);

        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            HeadLVUp();

        }
         * */
        #endregion

        #region 倒地 死亡 硬質
        if (FALL & BLO <= 0) //同時倒地和死亡 這時是要進入死亡狀態
        {
            FALL = false;
            GameManager.Death[byte.Parse(this.gameObject.name)]++;
        }
        if (FALL & BLO > 0) //倒地狀態
        {
            //倒地 //當我的生命小於零時不一定是在Suspend狀態

            if (dietimer < 0.0f)
            {
                this.gameObject.GetComponent<Animator>().SetBool("Death", true);
                this.SuspendOff(); //在裡面會將Animator裡的 Suspend設為false

                DeathPrepare();
                dietimer = 0.0f;
            }
            else
            {
                dietimer = dietimer + Time.deltaTime;
                if (GameManager.MyPlayerId == byte.Parse(this.gameObject.name))
                {
                    DeathTimeOutput.GetComponent<TextMeshPro>().text = ((int)FallTime - (int)dietimer).ToString();
                }
            }
            if (dietimer >= FallTime)
            {
                Animator anim = this.gameObject.GetComponent<Animator>();
                anim.SetInteger("state", 1);
                anim.SetBool("Suspend", false);
                dietimer = -1.0f;
                DeathFinish();
                this.FALL = false; //取消倒地
            }

        }
        else if (!FALL & BLO <= 0) //死亡狀態
        {
            if (dietimer < 0.0f)
            {
                this.gameObject.GetComponent<Animator>().SetBool("Death", true);
                this.SuspendOff(); //在裡面會將Animator裡的 Suspend設為false

                DeathPrepare();
                dietimer = 0.0f;
            }
            else
            {
                dietimer = dietimer + Time.deltaTime;
                if (GameManager.MyPlayerId == byte.Parse(this.gameObject.name))
                {
                    DeathTimeOutput.GetComponent<TextMeshPro>().text = ((int)DeathTime - (int)dietimer).ToString();
                }
            }
            if (dietimer >= DeathTime) //死亡時間結束，在復活點復活
            {

                this.gameObject.transform.position = this.ResurrectionPoint; //復活點復活//在狀態結束前移到復活位置
                Animator anim = this.gameObject.GetComponent<Animator>();
                anim.SetInteger("state", 1);
                anim.SetBool("Suspend", false);
                this.BLO = maxBlood;
                dietimer = -1.0f;
                DeathFinish();

            }

            //將GameManeger中的死亡數+1
            GameManager.Death[byte.Parse(this.gameObject.name)]++;
        }
        if (Sustimer != null)
        {
            if (this.SUS <= Sustimer.NowTime)
                SuspendOff();
        }
        #endregion

    }

    #region 用於Animator中 給予製造攻擊物件的function
    public GameObject Attackobject;
    /// <summary>
    /// 創造出攻擊物件，預定的位置是從本身的Pos發出
    /// </summary>
    void Attack1()
    {
        //攻擊物件創造
        //g 就是攻擊物件
        GameObject g = Instantiate(Attackobject, this.gameObject.transform.position, this.gameObject.transform.rotation) as GameObject;
        //target 是 從 g裡面取出的 IProgertyBasic
        IPropertyBasic target = g.GetComponent<IPropertyBasic>();
        target.ATK = this.Damage(this.ATK);
        target.NUMBER = byte.Parse(this.gameObject.name);
        target.TEAM = this.TEAM;
        g.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 500);
    }
    void AttackFall()
    {
        //攻擊物件創造
        //g 就是攻擊物件
        GameObject g = Instantiate(Attackobject, this.gameObject.transform.position, this.gameObject.transform.rotation) as GameObject;
        //target 是 從 g裡面取出的 IProgertyBasic
        IPropertyBasic target = g.GetComponent<IPropertyBasic>();
        target.ATK = this.Damage(this.ATK);
        target.NUMBER = byte.Parse(this.gameObject.name);
        target.FALL = true;
        target.TEAM = this.TEAM;
        g.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 500);
    }
    void AttackMouse01()
    {

    }
    #endregion

    #region 給予攻擊一個隨機的方法和其變數
    /// <summary>
    /// 自訂義小亂數表
    /// </summary>
    private byte[] random = new byte[] { 3, 7, 1, 3, 9, 2, 4, 5, 6, 1, 5, 2, 8 };
    private byte randomIndex = 0; //小亂數表的index
    /// <summary>
    /// 用來對ATK做隨機變話
    /// </summary>
    /// <param name="atk"></param>
    /// <returns></returns>
    private float Damage(float atk)
    {
        randomIndex++;
        if (randomIndex >= random.Length - 1)
            randomIndex = 0;
        if (randomIndex % 2 == 0)
            return atk + random[randomIndex];
        else
            return atk - random[randomIndex];

    }
    #endregion

    #region Sus(硬直) 的處理
    /// <summary>
    /// 存放這個遊戲物件身上的Timer物件，主要是當不需要時設成null，這樣可以用來判斷
    /// </summary>
    private Timer Sustimer;
    /// <summary>
    /// 進入Animator中的Suspend狀態時呼叫
    /// </summary>
    public void SuspendOn()
    {
        if (Sustimer == null)
        {
            this.gameObject.GetComponent<Animator>().SetBool("Suspend", true);
            Sustimer = this.gameObject.GetComponent<Timer>();
            Sustimer.TimerOn = true;
        }
        else
        {
            this.SUS = 0.5f;
        }
    }

    /// <summary>
    /// 離開Animator中的Suspend狀態時呼叫
    /// </summary>
    public void SuspendOff()
    {
        if (Sustimer != null)
        {
            this.gameObject.GetComponent<Animator>().SetBool("Suspend", false);
            this.gameObject.GetComponent<Timer>().TimerOn = false;
            Sustimer.StopAndReset();
            this.sus = originalSus;
            this.Sustimer = null; //釋放記憶體
        }
    }
    #endregion

    #region 死亡狀態處理
    /// <summary>
    /// 死亡狀態動畫
    /// </summary>
    public GameObject DeathTimeOutput;
    /// <summary>
    /// 存放這個遊戲物件身上的Timer物件，主要是當不需要時設成null，這樣可以用來判斷
    /// </summary>
    private Timer Dietimer;


    /// <summary>
    /// 進入Animator中的Death狀態後要做的動作
    /// </summary>
    public void DeathPrepare()
    {
        if (GameManager.MyPlayerId == byte.Parse(this.gameObject.name))
        { //如果是Sender的話
            Debug.Log("DiePrepare Sender part in ");
            DeathTimeOutput.SetActive(true);
            DeathStateOn();

        }
        else
        {//如果不是Sender的話

            Debug.Log("DiePrepare Receiver part in ");
            this.gameObject.GetComponent<BoxCollider>().enabled = false;

        }
    }
    private void DeathFinish()
    {
        if (GameManager.MyPlayerId == byte.Parse(this.gameObject.name))
        {
            Debug.Log("DieFinish Sender part in ");
            DeathTimeOutput.SetActive(false);
            DeathStateOff();
            this.gameObject.GetComponent<Animator>().SetBool("Death", false);
        }
        else
        {
            Debug.Log("DieFinish Receiver part in ");
            this.gameObject.GetComponent<BoxCollider>().enabled = true;
            this.gameObject.GetComponent<Animator>().SetBool("Death", false);
        }
    }
    /// <summary>
    /// 用於在死亡狀態時，一些Script和Collider的停用
    /// </summary>
    private void DeathStateOn() //只有Sender才能呼叫
    {
        this.gameObject.GetComponent<SenderMotion>().StopMotion();
        this.gameObject.GetComponent<SwordAnimController>().StopAnim();
        this.gameObject.GetComponent<MouseLook>().enabled = false;
        this.gameObject.GetComponent<BoxCollider>().enabled = false;
    }
    private void DeathStateOff()//只有Sender才能呼叫
    {
        this.gameObject.GetComponent<SenderMotion>().StartMotion();
        this.gameObject.GetComponent<SwordAnimController>().StartAnim();
        this.gameObject.GetComponent<MouseLook>().enabled = true;
        this.gameObject.GetComponent<BoxCollider>().enabled = true;
    }

    #endregion

    #region 升等功能
    /// <summary>
    /// 只有在其他部位滿等候能升級一次
    /// </summary>
    /// <returns>有沒有升級成功</returns>
    public bool HeadLVUp()
    {
        if (headLV > 2)
            return false;
        if (bodyLV == 11 || handLV == 11 || legLV == 11)
        {
            this.ATK = this.ATK + (this.ATK * this.swordHeadArray[headLV]);
            this.DEF = this.DEF + (this.DEF * this.swordHeadArray[headLV]);
            this.BLO = this.BLO + (this.BLO * this.swordHeadArray[headLV]);
            this.SPE = this.SPE + (this.SPE * this.swordHeadArray[headLV]);
            headLV++;
            return true;
        }
        else
            return false;
    }
    public bool HandLVUp()
    {
        if (handLV > 10)
            return false;
        this.ATK = this.ATK + this.swordHandArray[handLV];
        handLV++;
        return true;
    }
    public bool BodyLVUp()
    {
        if (bodyLV > 10)
            return false;
        this.BLO = this.BLO + swordBodyBLOArray[bodyLV];
        this.DEF = this.DEF + swordBodyDEFArray[bodyLV];
        bodyLV++;
        return true;
    }
    public bool LegLVUp()
    {
        if (legLV > 10)
            return false;
        this.SPE = this.SPE + (swordLegArray[0] * swordLegArray[legLV]);
        legLV++;
        return true;
    }
    public void SelectPartLvUp(byte part)
    {
        switch (part)
        {
            case 1:
                HeadLVUp();
                break;
            case 2:
                HandLVUp();
                break;
            case 3:
                BodyLVUp();
                break;
            case 4:
                LegLVUp();
                break;
        }
    }
    #endregion


}
