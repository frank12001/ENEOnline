using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class content : MonoBehaviour {


    RectTransform rect;
    Text text;
    S3String stringpool;
    void Awake()
    {
        rect = this.gameObject.GetComponent<RectTransform>();
        text = this.gameObject.GetComponent<Text>();
        stringpool = new S3String("content");
    }
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
    int enternumber = 0; //用來補正每5次就會沒有換行的問題
    /// <summary>
    /// 加入聊天內容(自動換行)，SenderIsMe=true 代表是別人發的，false 代表是自己發的
    /// </summary>
    /// <param name="text"></param>
    public void Setchatcontent(string text,bool SenderIsMe)
    {
        enternumber++;
        string result="";
        int changeline = 1;
        if (text.Length > 13)
        {
            int line = text.Length / 13;
            if (line == 1)
            {
                result = text.Substring(0, 13) + "\n" + text.Substring(13, text.Length - 14);
                changeline = 2;
            }
            else if (line == 2)
            {
                result = text.Substring(0, 13) + "\n" + text.Substring(13, 13) + "\n" + text.Substring(13 * line, text.Length - 13 * line);
                changeline = 3;
            }
            else 
            {
                result = text.Substring(0, 13) + "\n" + text.Substring(13, 13) + "\n" + text.Substring(13 * line, text.Length - 13 * line);
                changeline = 4;
            }
        }
        else
        {
            result = text;
        }
        if (SenderIsMe)
            result = stringpool.contenttext1 + "\n" + result;
        this.text.text = this.text.text+"\n" + result;
        Vector2 v = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y + 90.0f * changeline);
        rect.sizeDelta = v;
    }
}
