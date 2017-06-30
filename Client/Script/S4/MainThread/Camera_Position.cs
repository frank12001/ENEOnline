using UnityEngine;
using System.Collections;

namespace Assets.Script.S4.MainThread
{
    public class Camera_Position : MonoBehaviour
    {
        #if UNITY_EDITOR
        public string s,s1,s3;
        //舊的 Script 相機的旋轉和位置 都不用這個 Script 了
        //將 Center_Camera_Point(Mani Camera的parent) 放入 操作者物件下 ， 控制 相機位置和左右的方向(不控制上下)。
        //讓 Camera_Rotation.cs 值接控制 Main Camera 的 上下旋轉
        #endif
        // Use this for initialization
        void Start() {
            getOperatorGameObject += this.gameObject.GetComponentInChildren<Render>().GetOperatorGameObject;
	    }
        private delegate GameObject GetOperatorGameObjectHandler();
        private event GetOperatorGameObjectHandler getOperatorGameObject;
        public GameObject Camera_Center_Point;
        #region 用於旋轉的參數
        //-------------偵側滑鼠=>旋轉參數
        private float mousePosition1, mousePosition2; //紀錄先前和現在的滑鼠位置
        public float RotateSpeed;                   //旋轉的速度
        private float euler_y = 0; //紀錄360度，並傳做render
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
        #endregion
        // Update is called once per frame
        void Update()
        {
            GameObject target = getOperatorGameObject();
            //if(target!=null)
            //    Camera_Center_Point.transform.position = target.transform.position;

            #region 紀錄滑鼠位子，用於旋轉
            mousePosition2 = Input.mousePosition.x;
            // 偵測、處理 //
            if (mousePosition2 > mousePosition1)       //滑鼠往右移
                euler_Y += RotateSpeed;
            else if (mousePosition2 < mousePosition1)  //滑鼠往左移
                euler_Y -= RotateSpeed;
            else //滑鼠不動
            {
                //目前無處理
            }
            mousePosition1 = mousePosition2; //紀錄現在的滑鼠位置
            #endregion
            Camera_Center_Point.transform.eulerAngles = new Vector3(0.0f,euler_Y,0.0f);
        }
    }
}