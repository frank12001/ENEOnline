using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SwordAnimController : MonoBehaviour
{

    //取得這個Gameobject 上的Component
    public Animator animator;
    private AnimatorStateInfo stateInfo;
    SenderMotion sendermotion;
    private bool animable = true;
    private SwordProperty property;

    //技能的 CD 時間
    #region 一個技能處理cd所需的參數
    private const float cdMouse01 = 10.0f;
    public float CdMouse01 { get { return cdMouse01; } private set { } } // cdMouse01的後備參數
    private float cDMouse01 { get { return cdMouse01 * this.property.CDReduceMouse01; } }
    private float mouse01Timer = cdMouse01;
    public float Mouse01Timer { get { return mouse01Timer; } private set { } } // mouse01Timer的後備參數
    private bool mouse01able
    {
        get
        {
            if (cDMouse01 <= mouse01Timer)
            { return true; }
            else
            { return false; }
        }

    }
    #endregion

    //Animator 的 參數、狀態 Hash
    private int hashIdStateint; //參數
    private int hashSwordIdleState;
    private int hashIdRunState;
    private int hashIdAttackSmall1State;
    private int hashIdAttackSmall2State;
    private int hashIdAttackSmall3State;
    private int hashIdAttackBig1State;
    private int hashIdAttackSkill1State;

    //計算連擊數
    private int curComboCount = 0;

    //Sender//用於傳送要移動訊息//所有要和伺服器溝通都要有這個Script
    Sender sender;
    SenderMotion motionfunction;     //裡面有移動功能

    //用於計算 網路延遲 當按下 對印的動作像前進就是Front。
    bool starttimerFront = false;
    float timerFront = 0.0f;
    bool starttimerBack = false;
    float timerBack = 0.0f;
    bool starttimerRight = false;
    float timerRight = 0.0f;
    bool starttimerLeft = false;
    float timerLeft = 0.0f;

    byte movingState = (byte)moveState.Idle;
    /// <summary>
    /// 在這個class中自己使用的，方便判斷的enum不要讓其它class存取它
    /// </summary>
    enum moveState : byte { Idle,Front,Back,Left,Right }

    // Use this for initialization
    void Start()
    {
        animator = this.GetComponent<Animator>();
        sendermotion = this.GetComponent<SenderMotion>();
        property = this.GetComponent<SwordProperty>();
        #region 初始化hHashId
        hashIdStateint = Animator.StringToHash("state");
        hashSwordIdleState = Animator.StringToHash("Base Layer.SwordIdle");
        hashIdRunState = Animator.StringToHash("Base Layer.Run");
        hashIdAttackSmall1State = Animator.StringToHash("Base Layer.AttackSmall1");
        hashIdAttackSmall2State = Animator.StringToHash("Base Layer.AttackSmall2");
        hashIdAttackSmall3State = Animator.StringToHash("Base Layer.AttackSmall3");
        hashIdAttackBig1State = Animator.StringToHash("Base Layer.AttackBig1");
        hashIdAttackSkill1State = Animator.StringToHash("Base Layer.AttackSkill1");
        #endregion

        sender = gameObject.GetComponent<Sender>();
        motionfunction = gameObject.GetComponent<SenderMotion>();
        ConnectFunction.F.catchMoveRequire += catchMoveRequire;
    }

    void OnDestroy()
    {
        ConnectFunction.F.catchMoveRequire -= catchMoveRequire;
    }

    // Update is called once per frame
    void Update()
    {
        if (animable)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.fullPathHash != hashSwordIdleState &&
                stateInfo.fullPathHash != hashIdRunState)
            {
                animator.SetInteger(hashIdStateint, 1);
            }

            #region move anim 當按下時呼叫Sender 中向伺服器溝通的功能，和開啟"誤差計算計時器"

            #region MouseDown
            if (Input.GetKeyDown(KeyCode.W) && movingState == (byte)moveState.Idle)
            {
                movingState = (byte)moveState.Front;
                sender.Move(GameManager.Motion.FrontGo);
                timerFront = 0.0f;
                starttimerFront = true;
                Debug.Log("MouseDown State = FrontGo");
            }
            else if (Input.GetKeyDown(KeyCode.S) && movingState == (byte)moveState.Idle)
            {
                movingState = (byte)moveState.Back;
                sender.Move(GameManager.Motion.BackGo);
                timerBack = 0.0f;
                starttimerBack = true;
                Debug.Log("MouseDown State = BackGo");
            }
            else if (Input.GetKeyDown(KeyCode.D) && movingState == (byte)moveState.Idle)
            {
                movingState = (byte)moveState.Right;
                sender.Move(GameManager.Motion.RightGo);
                timerRight = 0.0f;
                starttimerRight = true;
                Debug.Log("MouseDown State = RightGo");
            }
            else if (Input.GetKeyDown(KeyCode.A) && movingState == (byte)moveState.Idle)
            {
                movingState = (byte)moveState.Left;
                sender.Move(GameManager.Motion.LeftGo);
                timerLeft = 0.0f;
                starttimerLeft = true;
                Debug.Log("MouseDown State = LeftGo");
            }
            #endregion
            #region MouseUp
            if (Input.GetKeyUp(KeyCode.W) && movingState == (byte)moveState.Front)
            {
                /* 原本的樣子
                 * movingState = (byte)moveState.Idle;
                 *  sender.Move(GameManager.Motion.FrontStop, motionfunction.PredictPosition(timerFront, GameManager.Motion.FrontStop));
                 */
                movingState = (byte)moveState.Idle;
                //--
                Vector3 v =  motionfunction.PredictPosition(timerFront, GameManager.Motion.FrontStop);
                sender.Move(GameManager.Motion.FrontStop, v);
                Debug.Log("MouseDown State = FrontStop");
                Debug.Log(" 預測的點 : x = " + v.x + " y = " + v.y + " z = " + v.z);
            }
            else if (Input.GetKeyUp(KeyCode.S) && movingState == (byte)moveState.Back)
            {
                //原本的樣子
                /*movingState = (byte)moveState.Idle;
                sender.Move(GameManager.Motion.BackStop, motionfunction.PredictPosition(timerBack, GameManager.Motion.BackStop));   
                */
                movingState = (byte)moveState.Idle;
                Vector3 v =  motionfunction.PredictPosition(timerBack, GameManager.Motion.BackStop);
                sender.Move(GameManager.Motion.BackStop,v);
                Debug.Log("MouseDown State = BackStop");
                Debug.Log(" 預測的點 : x = " + v.x + " y = " + v.y + " z = " + v.z);
            }
            else if (Input.GetKeyUp(KeyCode.D) && movingState == (byte)moveState.Right)
            {
                /* 原本的樣子
                movingState = (byte)moveState.Idle;
                sender.Move(GameManager.Motion.RightStop, motionfunction.PredictPosition(timerRight, GameManager.Motion.RightStop));     
                 * */
                movingState = (byte)moveState.Idle;
                Vector3 v = motionfunction.PredictPosition(timerRight, GameManager.Motion.RightStop);
                sender.Move(GameManager.Motion.RightStop,v );
                Debug.Log("MouseDown State =RightStop");
                Debug.Log(" 預測的點 : x = " + v.x + " y = " + v.y + " z = " + v.z);
            }
            else if (Input.GetKeyUp(KeyCode.A) && movingState == (byte)moveState.Left)
            {
                /*  原本的樣子
                movingState = (byte)moveState.Idle;
                sender.Move(GameManager.Motion.LeftStop, motionfunction.PredictPosition(timerLeft, GameManager.Motion.LeftStop));
                 * */
                movingState = (byte)moveState.Idle;
                Vector3 v = motionfunction.PredictPosition(timerLeft, GameManager.Motion.LeftStop);
                sender.Move(GameManager.Motion.LeftStop,v );
                Debug.Log("MouseDown State =LeftStop");
                Debug.Log(" 預測的點 : x = " + v.x + " y = " + v.y + " z = " + v.z);
            }
            #endregion

            #endregion

            #region attack anim
            //AttackSmall連擊所用的程式碼
            if ((stateInfo.fullPathHash == hashIdAttackSmall1State) &&
                stateInfo.normalizedTime < 1.0 && (this.curComboCount == 2))
            {
                this.animator.SetInteger(hashIdStateint, 6);
            }

            if (stateInfo.fullPathHash == hashIdAttackSmall2State && (this.curComboCount == 3))
            {
                this.animator.SetInteger(hashIdStateint, 7);
            }

            //開始攻擊
            if (Input.GetKeyDown(KeyCode.Mouse0)) //裡面包含對連擊的判斷
            {

                if (stateInfo.fullPathHash == hashSwordIdleState ||
                    stateInfo.fullPathHash == hashIdRunState)
                {
                    this.animator.SetInteger(hashIdStateint, 3);

                    this.curComboCount = 1;
                }
                else if (stateInfo.fullPathHash == hashIdAttackSmall1State)
                {
                    this.curComboCount = 2;
                }
                else if (stateInfo.fullPathHash == hashIdAttackSmall2State)
                {
                    this.curComboCount = 3;
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                animator.SetInteger(hashIdStateint, 4);
                //Debug.Log("mouse1 down");
            }
            #region 技能mouse 0 1
            if ((Input.GetKeyDown(KeyCode.Mouse0) && Input.GetKeyDown(KeyCode.Mouse1)) && mouse01able) //技能mouse 0 1
            {
                animator.SetInteger(hashIdStateint, 5);
                mouse01Timer = 0.0f;
                //Debug.Log("mouse0 && 1 down");
            }
            else if (!mouse01able)
            {
                mouse01Timer = mouse01Timer + Time.deltaTime;
                Debug.Log("現在的cd 時間 跑到 " + mouse01Timer);
            }
            #endregion
            //攻擊時，角色不能移動
            if (stateInfo.fullPathHash == hashIdAttackSmall1State ||
                stateInfo.fullPathHash == hashIdAttackSmall2State ||
                stateInfo.fullPathHash == hashIdAttackSmall3State ||
                stateInfo.fullPathHash == hashIdAttackBig1State ||
                stateInfo.fullPathHash == hashIdAttackSkill1State)
            {
                if (sendermotion.movable)
                    sendermotion.movable = false;
            }
            else
            {
                if (!sendermotion.movable)
                    sendermotion.movable = true;
            }

            //離開攻擊狀態 //已由在每偵都重制(state=1)取代
            /*
            if (Input.GetKeyUp(KeyCode.Mouse0)) //普功1
            {
               // if(animator)
                animator.SetInteger(hashIdStateint, 1);
            }
            if (Input.GetKeyUp(KeyCode.Mouse1)) //普功2
                animator.SetInteger(hashIdStateint, 1);
            if (Input.GetKeyUp(KeyCode.Mouse0) && Input.GetKeyUp(KeyCode.Mouse1)) //技能mouse 0 1
            {
                animator.SetInteger(hashIdStateint, 1);
            }
           */
            #endregion


        }
        #region 誤差計時器 
        if (starttimerFront)
           timerFront = timerFront + Time.deltaTime;
        if (starttimerBack)
            timerBack = timerBack + Time.deltaTime;
        if (starttimerRight)
            timerRight = timerRight + Time.deltaTime;      
        if (starttimerLeft)
            timerLeft = timerLeft + Time.deltaTime;
        #endregion
    }

    void FixedUpdate()
    {

    }

    public void StartAnim()
    {
        this.animable = true;
    }
    public void StopAnim()
    {
        this.animable = false;
    }

    /// <summary>
    /// 接收 operationResponce 的回傳，等於處理Sender自己的移動
    /// </summary>
    /// <param name="packet"></param>
    public void catchMoveRequire(Dictionary<byte, object> packet)
    {
        byte motion = byte.Parse(packet[1].ToString());
        switch (motion)
        {
                /*移動的case*/
            case (byte)GameManager.Motion.FrontGo:
                motionfunction.MoveFront(true);
                starttimerFront = false;
                Debug.Log("Sender 接收到 FrontGo ");
                break;
            case (byte)GameManager.Motion.BackGo:
                motionfunction.MoveBack(true);
                starttimerBack = false;
                Debug.Log("Sender 接收到 BackGo ");
                break;
            case (byte)GameManager.Motion.RightGo:
                motionfunction.MoveRight(true);
                starttimerRight = false;
                Debug.Log("Sender 接收到 RightGo ");
                break;
            case (byte)GameManager.Motion.LeftGo:
                motionfunction.MoveLeft(true);
                starttimerLeft = false;
                Debug.Log("Sender 接收到 LeftGo ");
                break;


            case (byte)GameManager.Motion.FrontStop:
                motionfunction.MoveFront(false);
                Vector3 newPosition1 = new Vector3(float.Parse(packet[2].ToString()), float.Parse(packet[3].ToString()), float.Parse(packet[4].ToString()));
                //motionfunction.SmoothAssignPosition(newPosition1);
                StartCoroutine(motionfunction.SmoothAssignPosition(newPosition1));
                Debug.Log("Sender 接收到 FrontStop ");
                Debug.Log(" 現在的點是 x = " + transform.position.x + " y = " + transform.position.y + " z = " + transform.position.z);
                Debug.Log(" 要移動到的點是 x = " + newPosition1.x + " y = " + newPosition1.y + "  z = " + newPosition1.z);
                break;           
            case (byte)GameManager.Motion.BackStop:
                motionfunction.MoveBack(false);
                Vector3 newPosition2 = new Vector3(float.Parse(packet[2].ToString()), float.Parse(packet[3].ToString()), float.Parse(packet[4].ToString()));
                //motionfunction.SmoothAssignPosition(newPosition2);
                StartCoroutine(motionfunction.SmoothAssignPosition(newPosition2));
                Debug.Log("Sender 接收到 BackStop ");
                Debug.Log(" 現在的點是 x = " + transform.position.x + " y = " + transform.position.y + " z = " + transform.position.z);
                Debug.Log(" 要移動到的點是 x = " + newPosition2.x + " y = " + newPosition2.y + "  z = " + newPosition2.z);
                break;           
            case (byte)GameManager.Motion.RightStop:
                motionfunction.MoveRight(false);
                Vector3 newPosition3 = new Vector3(float.Parse(packet[2].ToString()), float.Parse(packet[3].ToString()), float.Parse(packet[4].ToString()));
                //motionfunction.SmoothAssignPosition(newPosition3);
                StartCoroutine(motionfunction.SmoothAssignPosition(newPosition3));
                Debug.Log("Sender 接收到 RightStop ");
                Debug.Log(" 現在的點是 x = " + transform.position.x + " y = " + transform.position.y + " z = " + transform.position.z);
                Debug.Log(" 要移動到的點是 x = " + newPosition3.x + " y = " + newPosition3.y + "  z = " + newPosition3.z);
                break;       
            case (byte)GameManager.Motion.LeftStop:
                motionfunction.MoveLeft(false);
                Vector3 newPosition4 = new Vector3(float.Parse(packet[2].ToString()), float.Parse(packet[3].ToString()), float.Parse(packet[4].ToString()));
                //motionfunction.SmoothAssignPosition(newPosition4);
                StartCoroutine(motionfunction.SmoothAssignPosition(newPosition4));
                Debug.Log("Sender 接收到 LeftStop ");
                Debug.Log(" 現在的點是 x = " + transform.position.x + " y = " + transform.position.y + " z = " + transform.position.z);
                Debug.Log(" 要移動到的點是 x = " + newPosition4.x + " y = " + newPosition4.y + "  z = " + newPosition4.z);
                break;

                /*  除了移動之外，其他case  */
            case (byte)GameManager.Motion.SyncPosition:
                try
                {
                    Vector3 newPositionSync = new Vector3(float.Parse(packet[2].ToString()), float.Parse(packet[3].ToString()), float.Parse(packet[4].ToString()));
                    motionfunction.SmoothAssignPosition(newPositionSync);
                }
                catch (KeyNotFoundException)
                {
                    Debug.Log("這裡是otherPlayerController.cs.catchMoveRequires 問題: 想要同步位置，可是在packet裡 key=2&&3&&4的地方沒有值");
                }
                break;
            default:
                Debug.Log("Sender 接到了 未知的狀態");
                break;
        }
    }
}
