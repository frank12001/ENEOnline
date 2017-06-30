using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SwordUI : MonoBehaviour {
    

    private GameObject LVUPObject; //升等專用的UI
    private SwordProperty property;
    private SwordAnimController animController;

    //暫時用於顯示BLO ENERGY SKILL1CD
    private Text uiText1;

    public void Awake(){
       LVUPObject = GameObject.Find("LVUPObject");
    }
	// Use this for initialization
	void Start () {
        LVUPObject.SetActive(false);
        property = this.gameObject.GetComponent<SwordProperty>();
        animController = this.gameObject.GetComponent<SwordAnimController>();
        uiText1 = this.gameObject.GetComponentInChildren<Text>(); //從GameObject由上而下第一個找到的Text //如果位置變了就抓不到了
        //測試用-----
        //uiText1.text = "BLO : " + property.BLO + " \r\n ENERGY : " + property.Energy + " \r\n Skill1 CD : ";
        uiText1.text = " BLO : " + property.BLO + " \r\n ENERGY : " + property.Energy + " \r\n Skill1 CD : " + (animController.CdMouse01 - animController.Mouse01Timer);
        //-----------

	}
	
	// Update is called once per frame
	void Update ()
    {
        #region open/close 升等面板
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (LVUPObject.activeSelf)
                LVUPObject.SetActive(false);
            else
                LVUPObject.SetActive(true);
        }
        #endregion

        #region 觸發生等
        if (property.Energy>=100)
        {
            if (LVUPObject.activeSelf)
            {
                //升級頭
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    bool success = property.HeadLVUp();
                    if (success)
                    {
                        //成功升等
                        //呼叫LVUPObject.GetCompenent<LvupObjectEffect>裡的特效功能 
                        //Send packet to Receiver
                        Dictionary<byte, object> packet = new Dictionary<byte, object> 
                        {
                        {(byte)0,byte.Parse(this.gameObject.name)},
                        {(byte)1,1},
                        };
                        ConnectFunction.F.Deliver((byte)18, packet);
                    }
                    property.Energy = 0;
                }
                //升級手
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    bool success = property.HandLVUp();
                    if (success)
                    {
                        Debug.Log("atk= " + property.ATK);
                        //成功升等
                        //呼叫LVUPObject.GetCompenent<LvupObjectEffect>裡的特效功能 
                        //Send packet to Receiver
                        Dictionary<byte, object> packet = new Dictionary<byte, object> 
                        {
                        {(byte)0,byte.Parse(this.gameObject.name)},
                        {(byte)1,2},
                        };
                        ConnectFunction.F.Deliver((byte)18, packet);
                        property.Energy = 0;
                    }
                }
                //升級身體
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    bool success = property.BodyLVUp();
                    if (success)
                    {
                        Debug.Log("blo= " + property.BLO + "def  " + property.DEF);
                        //成功升等
                        //呼叫LVUPObject.GetCompenent<LvupObjectEffect>裡的特效功能 
                        //Send packet to Receiver
                        Dictionary<byte, object> packet = new Dictionary<byte, object> 
                        {
                        {(byte)0,byte.Parse(this.gameObject.name)},
                        {(byte)1,3},
                        };
                        ConnectFunction.F.Deliver((byte)18, packet);
                        property.Energy = 0;
                    }
                }
                //升及腿
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    bool success = property.LegLVUp();
                    if (success)
                    {
                        Debug.Log("spe= " + property.SPE);
                        //成功升等
                        //呼叫LVUPObject.GetCompenent<LvupObjectEffect>裡的特效功能 
                        //Send packet to Receiver
                        Dictionary<byte, object> packet = new Dictionary<byte, object> 
                    {
                        {(byte)0,byte.Parse(this.gameObject.name)},
                        {(byte)1,4},
                    };
                        ConnectFunction.F.Deliver((byte)18, packet);
                        property.Energy = 0;
                    }
                }
            }
        }
        #endregion
           
        #region 正式UI介面
            uiText1.text = "BLO : "+property.BLO +" \r\n ENERGY : "+ property.Energy +" \r\n Skill1 CD : " +(animController.CdMouse01-animController.Mouse01Timer);

            #endregion


    }
}
