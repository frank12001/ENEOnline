using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputLogGroupmode : MonoBehaviour {

   
    /// <summary>
    /// 不管是隊長或隊呼叫她都要將自己傳進來，因為都繼承GroupModeBase
    /// </summary>
    public GroupModeBase Base;
    InputField inputfield;

    private byte state=0;

    void Awake()
    {
        inputfield = this.GetComponentInChildren<InputField>();
    }
    void OnEnable()
    {
        inputfield.ActivateInputField();
        inputfield.Select();
    }


    void OnDisable()
    {
        //歸零
        inputfield.text = "";
        Base = null;
        
    }

    void Update()
    {
        //處理input enter
        if (Input.GetKeyUp(KeyCode.Return))
        {      
            state++;
            Debug.Log(GetTextInput());
            if (state == 2)
            {
                deliever();
            }
        }
    }
    void Button1Onclick()
    {
        state++;
        if (state == 2)
        {
            deliever();
        }
    }
    private void deliever()
    {
        if (this.GetTextInput().Length > 0 && Base != null)
        {
            Dictionary<byte, object> packet = new Dictionary<byte, object>();
            packet.Add((byte)0, 6);
            packet.Add((byte)1, Base.GetUidDic((byte)0, true));
            Debug.Log((int)Base.GetNumber((int)ConnectFunction.F.playerInfo.UID));
            packet.Add((byte)2, (int)Base.GetNumber((int)ConnectFunction.F.playerInfo.UID));
            packet.Add((byte)3, this.GetTextInput());
            
            
            ConnectFunction.F.Deliver((byte)21, packet);

            this.gameObject.SetActive(false);
        }
        else if (this.GetTextInput().Length <= 0)
        {
            this.gameObject.SetActive(false);
        }
        state = 0;
    }
    public string GetTextInput()
    {
        string s = inputfield.text.ToString();
        return s;
    }
}
