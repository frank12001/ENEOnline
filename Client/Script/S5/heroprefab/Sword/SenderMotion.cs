using UnityEngine;
using System.Collections;
using TMPro;

public class SenderMotion : MonoBehaviour
{

    #region original
    /*
    //此Class控制角色怎麼移動

    //控制此時能不能移動
    public bool movable = true;

    private SwordProperty property;
	// Use this for initialization
	void Start () {
        property = this.gameObject.GetComponent<SwordProperty>();
	}
    private Vector3 nextPos=new Vector3();
    private Vector3 nextPos2=new Vector3();
    public float forwardspeed = 0.2f;
    public float rightspeed = 0.2f;
    private float SPE;
	// Update is called once per frame
	void Update ()
    {
        
        this.SPE = property.SPE;
        #region move
        if (movable)
        {
            nextPos = Vector3.zero;
            nextPos2 = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
                nextPos = Vector3.forward * 1 * (forwardspeed+(this.SPE/1000));
            if (Input.GetKey(KeyCode.S))
                nextPos2 = Vector3.forward * (-1) * (forwardspeed+(this.SPE/1000));
            if (Input.GetKey(KeyCode.A))
                nextPos2 = Vector3.left * 1 * (rightspeed+(this.SPE/1000));
            if (Input.GetKey(KeyCode.D))
                nextPos2 = Vector3.left * -1 * (rightspeed+(this.SPE/1000));
            this.transform.position = transform.TransformPoint(nextPos + nextPos2);
        }
        #endregion
       
       

    }
    public void StartMotion()
    {
        this.movable = true;
    }
    public void StopMotion()
    {
        this.movable = false;
    }
     * */
    #endregion
    #if (UNITY_EDITOR)
    public string ScriptIntroduction1 = "裡面包含移動的方法";
    public string ScriptIntroduction2 = "要變速度直接改下面的值";
    public string ScriptIntroduction3 = "這4個介紹字串只會在編級模式下出現";
    public string ScriptIntroduction4 = "封裝完成的Script勁亮別動內部程式碼";
    #endif
    private SwordProperty property;
    private float SPE;
    public bool movable = true; //控制此時能不能移動
    public float Forwardspeed = 0.3f, Backspeed = 0.3f, Leftspeed = 0.3f, Rightspeed = 0.3f;
    bool moveFront=false,moveBack=false,moveLeft=false,moveRight=false;


    #region 測試位置
    //TextMeshPro text;
    //bool key = false;
    #endregion
    void Start()
    {
        property = this.gameObject.GetComponent<SwordProperty>();
        //text 存脆測試用
       // text = GameObject.Find("Text1").GetComponent<TextMeshPro>();
    }
    //測試用
    //public GameObject ggg;
    bool ismove = false;
    void Update()
    {
        #region 方便測試用 (已注解)      
        /*
        if (Input.GetKeyDown(KeyCode.W) )
        {
            ismove = true;
            MoveFront(true);
            Debug.Log("down");
        }
        else if (Input.GetKeyDown(KeyCode.S) && !ismove)
        {
            ismove = true;
            MoveBack(true);
        }
        else if (Input.GetKeyDown(KeyCode.A) && !ismove)
        {
            ismove = true;
            MoveLeft(true);
        }
        else if (Input.GetKeyDown(KeyCode.D) && !ismove)
        {
            ismove = true;
            MoveRight(true);
        }
        else if (Input.GetKeyUp(KeyCode.W) )
        {
            ismove = false;
            MoveFront(false);
            Debug.Log(PredictPosition(2, GameManager.Motion.FrontGo).z);
            // Instantiate(ggg, PredictPosition(Time.deltaTime, GameManager.Motion.FrontGo), ggg.transform.rotation); 
        }
        
        else if (Input.GetKeyUp(KeyCode.S) )
        {
            ismove = false;
            MoveBack(false);
        }
        
        else if (Input.GetKeyUp(KeyCode.A) )
        {
            ismove = false;
            MoveLeft(false);
        }
        
        else if (Input.GetKeyUp(KeyCode.D) )
        {
            ismove = false;
            MoveRight(false);
        }
       */
        #endregion
        if (movable)
        {
                    this.SPE = property.SPE;
                    this.SPE = 1;
                    Vector3 nextPos = Vector3.zero;
                    Vector3 nextPos2 = Vector3.zero;
                    if (moveFront)
                        nextPos = Vector3.forward * Time.deltaTime * Forwardspeed * this.SPE ;
                    if (moveBack)
                        nextPos = Vector3.back * Time.deltaTime * Backspeed * this.SPE ;
                    if (moveLeft)
                        nextPos2 = Vector3.left * Time.deltaTime * Leftspeed *this.SPE ;
                    if (moveRight)
                        nextPos2 = Vector3.right * Time.deltaTime * Rightspeed *this.SPE ;
                    this.gameObject.transform.position = transform.TransformPoint(nextPos + nextPos2);
                    //transform.Translate(nextPos + nextPos2);
                   /* #region 測試用
                    if (key)
                        text.text = " playerid 1 X = " + gameObject.transform.position.x.ToString()
                                  + "\n  Z = " + gameObject.transform.position.z.ToString()
                                  + "\n" + gameObject.name ;
                    #endregion 
                    * */
        }
       
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="futuretime"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public Vector3 PredictPosition(float futuretime, GameManager.Motion state)
    {
        Vector3 nextPos = Vector3.zero;
        switch (state)
        {
            case GameManager.Motion.FrontStop:
                nextPos = Vector3.forward * futuretime * Forwardspeed * this.SPE;
                break;
            case GameManager.Motion.BackStop:
                nextPos = Vector3.back * futuretime * Backspeed * this.SPE;
                break;
            case GameManager.Motion.LeftStop:
                nextPos = Vector3.left * futuretime * Leftspeed * this.SPE;
                break;
            case GameManager.Motion.RightStop:
                nextPos = Vector3.right * futuretime * Rightspeed * this.SPE;
                break;
            default:
                Debug.Log("傳入狀態錯誤");
                break;
        }
        return transform.TransformPoint(nextPos);
    }
    /// <summary>
    /// 開啟/關閉前進移動
    /// </summary>
    /// <param name="startorstop">開啟/關閉</param>
    public void MoveFront(bool startorstop)
    {
        moveFront = startorstop;
    }
    /// <summary>
    /// 開啟/關閉後退移動
    /// </summary>
    /// <param name="startorstop">開啟/關閉</param>
    public void MoveBack(bool startorstop)
    {
        moveBack = startorstop;
    }
    /// <summary>
    /// 開啟/關閉左邊移動
    /// </summary>
    /// <param name="startorstop">開啟/關閉</param>
    public void MoveLeft(bool startorstop)
    {
        moveLeft = startorstop;
    }
    /// <summary>
    /// 開啟/關閉右邊移動
    /// </summary>
    /// <param name="startorstop">開啟/關閉</param>
    public void MoveRight(bool startorstop)
    {
        moveRight = startorstop;
    }

    public void StartMotion()
    {
        this.movable = true;
    }
    public void StopMotion()
    {
        this.movable = false;
    }

    /// <summary>
    /// assign position
    /// </summary>
    /// <param name="position"></param>
    public void AssignPosition(Vector3 position)
    {
        this.gameObject.transform.position = position;
    }
    /// <summary>
    /// only assign position.x
    /// </summary>
    /// <param name="x"></param>
    public void AssignPositionX(float x)
    {
        Vector3 position = new Vector3(x, gameObject.transform.position.y, gameObject.transform.position.z);
        this.gameObject.transform.position = position;
        #region 方便測試用
       /* if (byte.Parse(gameObject.name) <= 1)
        {
            text.text = " playerid 1 X = " + gameObject.transform.position.x.ToString()
                      + "\n  Z = " + gameObject.transform.position.z.ToString();
        }
        */
        #endregion

    }
    /// <summary>
    /// only assign position.y
    /// </summary>
    /// <param name="z"></param>
    public void AssignPositionY(float y)
    {
        Vector3 position = new Vector3(gameObject.transform.position.x, y, gameObject.transform.position.z);
        this.gameObject.transform.position = position;
        /*
        if (byte.Parse(gameObject.name) <= 1)
        {
            text.text = " playerid 1 X = " + gameObject.transform.position.x.ToString()
                      + "\n  Z = " + gameObject.transform.position.z.ToString();
        }
         * */
    }
    /// <summary>
    /// only assign position.z
    /// </summary>
    /// <param name="z"></param>
    public void AssignPositionZ(float z)
    {
        Vector3 position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, z);
        this.gameObject.transform.position = position;
    }
    /// <summary>
    /// only assign position.z
    /// </summary>
    /// <param name="z"></param>
    public void AssignPositionXZ(float x,float z)
    {
        Vector3 position = new Vector3(x, gameObject.transform.position.y, z);
        this.gameObject.transform.position = position;
    }
    /// <summary>
    /// 花固定的時間，用線性差值平滑的到達一個點。(要重寫)
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public IEnumerator SmoothAssignPosition(Vector3 position)
    {
        Debug.Log("SmoothAssignPositionFunction In");
        Vector3[] v = new Vector3[50];
        for (int i = 0; i < v.Length; i++)
        {
            //v[i] = new Vector3();
            v[i] = Vector3.Lerp(this.gameObject.transform.position, position, 0.2f * (i + 1));
        }
        for (int i = 0; i < v.Length; i++)
        {
            this.gameObject.transform.position = v[i];
            Debug.Log("SmoothAssignPositionFunction yield return "+i);
            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("SmoothAssignPositionFunction Out");
    }
}
