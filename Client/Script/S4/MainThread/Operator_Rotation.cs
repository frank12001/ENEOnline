using UnityEngine;
using System.Collections.Generic;

namespace Assets.Script.S4.MainThread
{
    public class Operator_Rotation : MonoBehaviour
    {
        #if (UNITY_EDITOR)
        public string Introduction = "專門用來處理方向傳送的Script";
        #endif
        void Start()
        {          
            this.render_OperatorRotation += this.GetComponentInChildren<Render>().Render_OperatorRotation;
        }

        #region 事件 (render_OperatorRotation=掛勾Render的事件),(setToDeliver=將旋轉角度傳給Sample_Input)
        /// <summary>
        /// render的事件 
        /// </summary>
        /// <param name="eulerAngles">3個軸的歐拉角</param>
        private delegate void renderOperatorRotationHandler(Vector3 eulerAngles);
        private event renderOperatorRotationHandler render_OperatorRotation;
        #endregion
        #region 參數 
        

        
        #endregion

        void Update()
        {
            

            
        }

    }
}
