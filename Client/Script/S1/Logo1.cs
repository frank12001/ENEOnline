using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;

public class Logo1 : MonoBehaviour {

    S1String thisstring;
    //登入用的物件
    public GameObject IDobj,psdobj;
    //註冊用的物件
    public GameObject gid, gpsd, gemail;
    Animator anim;
    //case3 deledge; 委派給 delegator
    public delegate void UI();
    public static event UI UIevent;
    public void OnMouseDown()
    {
        switch (anim.GetInteger("state"))
        {
                //進入註冊
            case 0:
                SendMessageUpwards("LogoStateChange", 4);
                break;

            case 1: //傳送登入資料
               #region 正式版
                /*
                string id = IDobj.GetComponent<InputField>().text;
                string psd = psdobj.GetComponent<InputField>().text;
                Dictionary<byte, object> dic = new Dictionary<byte, object>
                {
                    {(byte)1,id},{(byte)2,psd},
                };
                if (ConnectFunction.F.playerInfo.isConnect)
                {
                    ConnectFunction.F.Deliver((byte)1, dic);
                }
                else
                {

                }
                 * */
                #endregion
                
                #region 測試版
                string id = "frank1";
                string psd ="t80529";
                Dictionary<byte, object> dic = new Dictionary<byte, object>
                {
                    {(byte)1,id},{(byte)2,psd},
                };
                if (ConnectFunction.F.playerInfo.isConnect)
                {
                    ConnectFunction.F.Deliver((byte)1, dic);
                }
                #endregion
                break;

            case 3: //傳送註冊資料
                if (ConnectFunction.F.playerInfo.isConnect)
                {
                    string setid = gid.GetComponent<InputField>().text;
                    string setpsd = gpsd.GetComponent<InputField>().text;
                    string setemail = gemail.GetComponent<InputField>().text;
                    //寫一個function檢查 id,psd,mail的正確性
                    switch (checkregisterInfo(setid, setpsd, setemail))
                    {
                        case 0: //所有檢查正確
                            object[] b = new object[] { setid, setpsd, setemail };
                            Dictionary<byte, object> d = ConnectFunction.F.ChangeToDic(b);
                            ConnectFunction.F.Deliver(2, d);
                            //委派 處理介面變化
                            UIevent();
                            break;
                        case 1:
                            Log(thisstring.Logo1);
                            break;
                        case 2:
                            Log(thisstring.Logo2);
                            break;
                        case 3:
                            Log(thisstring.Logo3);
                            break;
                        case 4:
                            Log(thisstring.Logo4);
                            break;
                        case 5:
                            Log(thisstring.Logo5);
                            break;
                        case 6:
                            Log(thisstring.Logo6);
                            break;
                    }
  
                    
                }
                else
                { ConnectFunction.F.ConnectToServer(); }
                break;
            case 4:
                anim.SetInteger("state", 5);
                break;
        }
    }
    public int checkregisterInfo(string id, string psd, string email)
    {
        if (IsValidId(id) == 0)
        {                              
            if (IsValidPsd(psd)==0)
            {
                if (IsValidEmail(email))
                {
                    if (email.Length < 50)
                    { }
                    else if (email.Length >= 50)
                    { return 6; }
                }
                else if (!IsValidEmail(email))
                { return 5; }
            }
            else if (IsValidPsd(psd) == 1)
            { return 3; }
            else if (IsValidPsd(psd) == 2)
            { return 4; }
        }
        else if (IsValidId(id) == 1)
        { return 1; }
        else if (IsValidId(id) == 2)
        { return 2; }

        return 0;
    }
    //log文字 
    public GameObject logText;
    public void Log(string s)
    {
        GameObject g = GameObject.Instantiate(logText);
        g.GetComponent<TextMeshPro>().text = s;
    }
    #region 檢查帳號密碼email功能
    public bool IsValidEmail(string strIn) //檢查email格式
    {
        // Return true if strIn is in valid e-mail format.
        return Regex.IsMatch(strIn,
               @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
               @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
    }
    public int IsValidId(string strIn) //回傳錯誤代碼
    {                                  //0為沒錯
        if (strIn.Length>15||strIn.Length<6)
            return 1; //錯長度
        if (!IsNumberEnglish(strIn))
            return 2; //不能是漢字

        return 0;
    }
    public bool IsNumberEnglish(string InputString)
    {
        return (InputString != string.Empty && Regex.IsMatch(InputString, "[a-zA-Z0-9]")) ? true : false;
    }
    public int IsValidPsd(string InputString) //傳回錯誤代碼
    {
        if (InputString.Length > 15 || InputString.Length < 6)
            return 1; //長度錯誤
        // 0為正確 2為字符錯誤
        return (InputString != string.Empty && Regex.IsMatch(InputString, "[a-zA-Z0-9]")) ? 0 : 2; 
    }
    #endregion

    // Use this for initialization
    void Start()
    {
        anim = this.GetComponentInParent<Animator>();
        thisstring = new S1String();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
