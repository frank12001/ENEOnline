using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Script.S4.Character;

namespace Assets.Script.S4.MainThread
{
    public class Sample_Input : MonoBehaviour
    {

        
        void Start()
        {           
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

            #region  掛勾事件
            this.render_OperatorRotation += this.GetComponentInChildren<Render>().Render_OperatorRotation;
            #endregion
        }
        #region 事件
        private delegate void renderOperatorRotationHandler(Vector3 eulerAngles);
        private event renderOperatorRotationHandler render_OperatorRotation;//render旋轉
        #endregion
        #region 參數

        /// <summary>
        /// 設定成false之後，要打開技能設成true
        /// </summary>
        public bool KeyDown_Switch=false; //用來控制所有的按鍵能不能被觸發 

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

        /// <summary>
        /// 我自己定義的Key Code 用於和 伺服器溝通 ，伺服器也用這個Key Code 判斷。
        /// </summary>
        private enum keyCode : byte { W = 0, S, A, D, Mouse_Left, Mouse_Right, Q, E, F, Space = 9 }  //我自己定義的Key Code 用於和 伺服器溝通 ，伺服器也用這個Key Code 判斷。

        //-------------偵側滑鼠=>旋轉參數
        public float RotateSpeed;                   //旋轉的速度
        private float euler_y=0; //紀錄360度，並傳做render
        private float euler_Y
        {
            get { return euler_y; } //自動易位
            set
            {
                if (value > 360)
                    euler_y = 0;
                else if (value < 0)
                    euler_y = 359;
                else
                    euler_y = value;
            }
        }   //自動易位

        //private bool moving = false; //判斷是不是在移動中

        /// <summary>
        /// 從編輯器付予，用於採樣瞄準點。(他的世界位置就是瞄準點)
        /// </summary>
        public GameObject Sample_CameraPoint;
        #if UNITY_EDITOR
        public string Note;
        #endif
        [SerializeField]
        private float maximumRange;        //點採樣時 的最大距離
         [SerializeField]
        private float maximunRange_Remote; //點採樣時 的最大距離(遠程)
        #endregion
        // Update is called once per frame
        void Update()
        {

            #region 用滑鼠的左右位移來旋轉
            
            if (Input.GetAxis("Mouse X") < 0)
                euler_Y -= RotateSpeed;
            else if (Input.GetAxis("Mouse X") > 0)
                euler_Y += RotateSpeed;
            #endregion

            #region 方向 Render 的方式 =>  先傳給伺服器，在本地 Render 出來。1.操作者物件的方向直接Render不經由伺服器 2. 其它玩家物件經由伺服器取得方向並賦予
            deliver_OperatorRotation();
            render_OperatorRotation(new Vector3(0.0f, this.euler_Y, 0.0f)); //直接render出來
            #endregion

            
            #region move,attack
            if (KeyDown_Switch)    //用來控制所有的按鍵能不能被觸發
            {            
                #region 普通攻擊 Mouse_0
                if (Input.GetKeyDown(KeyCode.Mouse0))     //按下左鍵   (普通攻擊、連擊)
                { //keyCode = (byte)4       //將射出Ray 撞到的第一個點(瞄準點)傳給伺服器，沒撞到的話就傳 往瞄準的方向延升 距離 maximumRange 的點

                    deliver_OperatorRotation();
                    Vector3 cameraRayPoint = Get_CameraRayPoint(maximumRange);
                    buttonEvent((byte)keyCode.Mouse_Left, true, cameraRayPoint);
                    
                }               
                #endregion
            
                #region 技能 Mouse_1,Q,E,F,Space
                if (Input.GetKeyDown(KeyCode.Mouse1))   //按下右鍵    (技能 1 、有CD)
                { //keyCode = (byte)5
                    Vector3 cameraRayPoint = Get_CameraRayPoint(maximumRange);                   
                    buttonEvent((byte)keyCode.Mouse_Right, true,cameraRayPoint);
                }
                if (Input.GetKeyDown(KeyCode.Q))       //按下Q        (技能 1 、有CD)
                { //keyCode = (byte)6
                    Vector3 cameraRayPoint = Get_CameraRayPoint(maximumRange); 
                    buttonEvent((byte)keyCode.Q, true,cameraRayPoint);
                }
                if (Input.GetKeyDown(KeyCode.E))       //按下E         (技能 1 、有CD)
                { //keyCode = (byte)7
                    Vector3 cameraRayPoint = Get_CameraRayPoint(maximumRange);
                    buttonEvent((byte)keyCode.E, true,cameraRayPoint);
                }
                if (Input.GetKeyDown(KeyCode.F))       //按下F          (技能 1 、有CD)
                { //keyCode = (byte)8
                   // buttonEvent((byte)keyCode.F, true);
                }
                if (Input.GetKeyDown(KeyCode.Space))  //按下Space       (技能 1 、有CD)
                { //keyCode = (byte)9
                    buttonEvent((byte)keyCode.Space, true);
                }
                #endregion
                #region 移動 W,S,A,D 只有移動需要做 ButtonUp
                //------------只有移動需要做 ButtonUp------------//
                #region Key  W
                if (Input.GetKeyDown(KeyCode.W))      //按下W     (前進、移動)
                { //keyCode = (byte)0
                    buttonEvent((byte)keyCode.W, true);
                }
                else if (Input.GetKeyUp(KeyCode.W)) //拿起W     (前進、移動)
                { //keyCode = (byte)0
                    if (!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                    {
                        buttonEvent((byte)keyCode.W, false);
                    }
                    else
                    {
                        buttonEvent((byte)keyCode.W, false, false);
                    }

                }
                #endregion
                #region Key  S
                if (Input.GetKeyDown(KeyCode.S))      //按下S         (後退、移動)
                { //keyCode = (byte)1
                    buttonEvent((byte)keyCode.S, true);
                }
                else if (Input.GetKeyUp(KeyCode.S))   //拿起S         (後退、移動)
                { //keyCode = (byte)1
                    if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                    {
                        buttonEvent((byte)keyCode.S, false);
                    }
                    else
                    {
                        buttonEvent((byte)keyCode.S, false, false);
                    }

                }
                #endregion
                #region Key  A
                if (Input.GetKeyDown(KeyCode.A))    //按下A         (左走、移動)
                { //keyCode = (byte)3
                    buttonEvent((byte)keyCode.A, true);
                }
                else if (Input.GetKeyUp(KeyCode.A)) //拿起A         (左走、移動)
                { //keyCode = (byte)3
                    if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
                    {
                        buttonEvent((byte)keyCode.A, false);
                    }
                    else
                    {
                        buttonEvent((byte)keyCode.A, false, false);
                    }
                }
                #endregion
                #region Key  D
                if (Input.GetKeyDown(KeyCode.D))     //按下D         (右走、移動)
                { //keyCode = (byte)4
                    buttonEvent((byte)keyCode.D, true);
                }
                else if (Input.GetKeyUp(KeyCode.D)) //拿起D         (右走、移動)
                { //keyCode = (byte)4
                    if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A))
                    {
                        buttonEvent((byte)keyCode.D, false);
                    }
                    else
                    {
                        buttonEvent((byte)keyCode.D, false, false);
                    }
                    
                }
                #endregion
                #endregion
            }
                #endregion
            
        }
        
        #region 以下為功能 buttonEvent,PlayerMove,ToDirection,
        //-------以下為功能 buttonEvent,PlayerMove,ToDirection
        /// <summary>
        /// 按鍵採樣後的處理，沒有方向，在拿起一個鍵，卻還有其他建按下時使用。傳入 ( keyCode、有沒有按下 )
        /// </summary>
        /// <param name="keyCode">我自己定義的KeyCode</param>
        /// <param name="value">按下是true,拿起是false</param>
        private void buttonEvent(byte keyCode, bool value, bool isMoving)
        {
            Dictionary<byte, object> packet = new Dictionary<byte, object> 
            {
                {(byte)0,(byte)4}, //switch code
                {(byte)1,keyCode}, //我自己定義的Key Code
                {(byte)2,value},   //按下 或 拿起 ， true or false
            };
            ConnectFunction.F.Deliver_SampleKey(packet);
        }
        /// <summary>
        /// 按鍵採樣後的處理，傳入 ( Key Code、有沒有按下、euler_y )
        /// </summary>
        /// <param name="keyCode">我自己定義的KeyCode</param>
        /// <param name="value">按下是true,拿起是false</param>
        private void buttonEvent(byte keyCode, bool value)
        {
            Dictionary<byte, object> packet = new Dictionary<byte, object> 
            {
                {(byte)0,(byte)4}, //switch code
                {(byte)1,keyCode}, //我自己定義的Key Code
                {(byte)2,value},   //按下 或 拿起 ， true or false
                {(byte)3,(byte)ToDirection(this.euler_y)},
            };
            ConnectFunction.F.Deliver_SampleKey(packet);
        }      
        /// <summary>
        /// 按鍵採樣後的處理，沒有方向，在拿起一個鍵，卻還有其他建按下時使用。傳入 ( keyCode、有沒有按下、瞄準點 )
        /// </summary>
        /// <param name="keyCode">我自己定義的KeyCode</param>
        /// <param name="value">按下是true,拿起是false</param>
        private void buttonEvent(byte keyCode, bool value,Vector3 cameraRayPoint)
        {
            Dictionary<byte, object> packet = new Dictionary<byte, object> 
            {
                {(byte)0,(byte)4}, //switch code
                {(byte)1,keyCode}, //我自己定義的Key Code
                {(byte)2,value},   //按下 或 拿起 ， true or false
                {(byte)4,ToFloatArray(cameraRayPoint)},// 瞄準點 (要用 4 不能用 3)
            };
            ConnectFunction.F.Deliver_SampleKey(packet);
        }
        /// <summary>
        /// 傳送現在操作者的方向給 Server
        /// </summary>
        private void deliver_OperatorRotation()
        {
            Dictionary<byte, object> packet = new Dictionary<byte, object>
            {
                 {(byte)0,(byte)5},
                 {(byte)1,(byte)ToDirection(euler_Y)},
            };
            ConnectFunction.F.Deliver_SampleKey(packet);
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
        /// 傳入歐拉角(0~360度)，回傳Direction
        /// </summary>
        /// <param name="y_axis">傳入Transform.eulerAngles.y(其實就是角度)</param>
        /// <returns>傳回Direction</returns>
        public Direction ToDirection(float y_axis)
        {
            byte count = (byte)(y_axis / 11.25f); //11.25f 是 每個
            count = (byte)(count + 1); //先轉換為正確的count
            if ((y_axis % 11.25) > (11.25f / 2))
            {
                count = (byte)(count + 1);
            }
            if (count > 32) //如果加完後大於32了 ，要讓它等於1
                count = 1;
            return (Direction)count;
        }
        /// <summary>
        /// 將Vector3 轉成 float[]
        /// </summary>
        /// <param name="position">要轉換的Vector3</param>
        /// <returns>float array (0,1,2) = (x,y,z)</returns>
        private float[] ToFloatArray(Vector3 position)
        {
            float[] positionArray = new float[3];
            positionArray[0] = position.x;
            positionArray[1] = position.y;
            positionArray[2] = position.z;
            return positionArray;
        }

        /// <summary>
        /// 取得 從相機發出 Ray 的碰撞點 ， 撞到人時的處理是 將在碰撞器外框的撞點 轉換成在碰撞器中心線的投影點
        /// </summary>
        /// <param name="maximumRange">射線的最大距離</param>
        /// <returns></returns>
        private Vector3 Get_CameraRayPoint(float maxdistance)
        {
            Vector3 cameraRayPoint;           
            Vector3 forward = Sample_CameraPoint.transform.TransformDirection(Vector3.down);
            Ray ray = new Ray(Sample_CameraPoint.transform.position, forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxdistance)) //有撞到人
            {
                Basic_Character character = hit.collider.gameObject.GetComponent<Basic_Character>();
                //Debug.Log("外部碰撞點 (x,y,z) = ( " + hit.point.x + " , " + hit.point.y + " , " + hit.point.z + ")");
                if (character != null) // 代表撞到的是角色
                {
                    #region 將在中心線上 碰撞點 的投影點回傳
                            
                    Vector3? real_point = character.GetProjectedPointsOnTheCapsuleColliderCenterLine(hit.point); //將原本在碰撞器外面的點，轉換成在破撞器中心線的投影點
                    if (real_point == null) //代表這個 character 沒有腳色碰撞器 //不用修改
                    {
                        cameraRayPoint = hit.point;
                        //Debug.Log("real_point = null");
                    }
                    else //(real_point != null) //代表完成回傳新的點 (在碰撞器中心線上的投影點)
                    {
                        cameraRayPoint = (Vector3)real_point;
                        //Debug.Log("real_point (x,y,z) = ( " + cameraRayPoint.x + " , " + cameraRayPoint.y + " , " + cameraRayPoint.z + " ) ");
                    }
                            
                    #endregion 
                }
                else                  //撞到的不是角色
                {
                    cameraRayPoint = hit.point;    //將撞到的點回傳
                }
                //TEST.transform.position = cameraRayPoint;
                //Debug.DrawRay(Sample_CameraPoint.transform.position, forward, Color.green,100.0f);
                //Debug.Log(" 撞到 "+"x,y,z" + cameraRayPoint.x + "," + cameraRayPoint.y + "," + cameraRayPoint.z);
            }
            else                       //沒撞到人
            {
                cameraRayPoint = Sample_CameraPoint.transform.position;
                cameraRayPoint += forward * maxdistance;
                //TEST.transform.position = cameraRayPoint;
                //Debug.DrawRay(Sample_CameraPoint.transform.position, forward, Color.green,100.0f);
                //Debug.Log("沒撞到 "+"x,y,z" + cameraRayPoint.x + "," + cameraRayPoint.y + "." + cameraRayPoint.z);
            }

            return cameraRayPoint;             
        }
        #endregion
    }
}
