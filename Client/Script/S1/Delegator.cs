using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Delegator : MonoBehaviour {

    //string
    S1String stringpool;
    //UIevent() 參數
    public GameObject star009,textregister2;
	// Use this for initialization
	void Start () 
    {
        Logo1.UIevent += this.UIevent;
        ConnectFunction.F.registerevent += this.registerevent;
        ConnectFunction.F.loginerevent += this.loginerevent;
        stringpool = new S1String();
	}

    void OnDestroy()
    {
        Logo1.UIevent -= this.UIevent;
        ConnectFunction.F.registerevent -= this.registerevent;
        ConnectFunction.F.loginerevent += this.loginerevent;
    }

    public void registerevent(short returncode,Dictionary<byte,object> d)
    {
        switch (returncode)
        {
            case (short)0:
                //UI的變換
                textregister2.SetActive(false);
                star009.GetComponent<MeshCollider>().enabled = true;
                ConnectFunction.F.playerInfo.UID = Convert.ToInt32(d[0]);
                ConnectFunction.F.playerInfo.ID = Convert.ToString(d[1]);
                ConnectFunction.F.playerInfo.PSD = Convert.ToString(d[2]);
                //Debug.Log("註冊成功");
                //在這邊 要進入 新狀態
                LogoAll.anim.SetInteger("state",4);
                break;
            case (short)1:
                textregister2.SetActive(false);
                star009.GetComponent<MeshCollider>().enabled = true;
                //Debug.Log("已存在相同的ID");
                GameObject g1 = Instantiate(log);
                g1.GetComponent<TextMeshPro>().text = stringpool.Delegator3;
                break;
            case (short)2:
                textregister2.SetActive(false);
                star009.GetComponent<MeshCollider>().enabled = true;
                GameObject g2 = Instantiate(log);
                g2.GetComponent<TextMeshPro>().text = stringpool.Delegator4;
              //Debug.Log("已存在相同的email");
                break;
            case (short)3:
                textregister2.SetActive(false);
                star009.GetComponent<MeshCollider>().enabled = true;
                GameObject g3 = Instantiate(log);
                g3.GetComponent<TextMeshPro>().text = stringpool.Delegator2;
                //Debug.Log("資料寫入失敗");
                break;
        }
    }
    public GameObject log;
    //登入方便測試用變數
    private int number = 1;
    public void loginerevent(short returncode,Dictionary<byte, object> dic)
    {
        switch (returncode)
        {
            case (short)0:
                LogoAll.anim.SetInteger("state", 5);
                ConnectFunction.F.playerInfo.UID = Int32.Parse(dic[0].ToString());
                ConnectFunction.F.playerInfo.ID = dic[1].ToString();
                ConnectFunction.F.playerInfo.PSD = dic[2].ToString();
                //Debug.Log("成功連線並返回");
                break;
            case (short)1:
                GameObject g1 = Instantiate(log);
                g1.GetComponent<TextMeshPro>().text = stringpool.Delegator1;
                //Debug.Log("帳號或密碼錯誤");
                break;
            case (short)2:
                //正式版
                //GameObject g2 = Instantiate(log);
                //g2.GetComponent<TextMeshPro>().text = stringpool.Delegator5;
               // Debug.Log("重複登入");
                
                //測試版
                number = number +1;
                string id = "frank" + number.ToString();
                string psd = "t80529";
                Dictionary<byte, object> packet = new Dictionary<byte, object> { { 1, id }, { 2, psd } };
                ConnectFunction.F.Deliver((byte)1, packet);
                break;
        }
    }
    public void UIevent()
    {
        textregister2.SetActive(true);
        star009.GetComponent<MeshCollider>().enabled = false;
    }
}
