using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class chat2 : MonoBehaviour {

    void OnMouseDown()
    {
        this.gameObject.SendMessageUpwards("controllchat", 1);
    }

    InputField inputfield;
    Text[] text;
    void Awake()
    {
        inputfield = this.gameObject.GetComponentInChildren<InputField>();
        text = this.gameObject.GetComponentsInChildren<Text>();
        //text[0] = 標題 
        //text[1] = input的內容顯示
        //text[2] = input 輸入地方
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    /// <summary>
    /// 取得inputfield裡的內容
    /// </summary>
    /// <returns></returns>
    public string GetInputField()
    {
       return inputfield.text;
    }
    /// <summary>
    /// 刪除input裡的內容
    /// </summary>
    public void ClearInputField()
    {
        inputfield.text = "";
    }
    /// <summary>
    /// 給他焦點
    /// </summary>
    public void Focus()
    {
        inputfield.Select();
        inputfield.MoveTextStart(false);
    }
    /// <summary>
    /// 取得chat2的gameobject
    /// </summary>
    /// <returns></returns>
    public GameObject Getchat2()
    {
        return this.gameObject;
    }
    /// <summary>
    /// 設定chat2的標題
    /// </summary>
    /// <param name="name"></param>
    public void SetTopic(string name)
    {
        text[0].text = name;
    }
    /// <summary>
    /// 回傳這個Inputfeild有沒有被選
    /// </summary>
    /// <returns></returns>
    public bool isSelect()
    {
        return inputfield.IsInteractable();
    }
}
