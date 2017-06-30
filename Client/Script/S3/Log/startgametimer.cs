using UnityEngine;
using TMPro;

public class startgametimer : MonoBehaviour {

    float Nowtimer = 0.0f;
    int Beforeimer = 0;
    TextMeshPro text;

    void Awake(){
        text = this.gameObject.GetComponent<TextMeshPro>();
    }
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    void FixedUpdate(){
        Nowtimer += Time.deltaTime;
        if ((int)Nowtimer > Beforeimer)
            SetTimer((int)Nowtimer);
        Beforeimer = (int)Nowtimer;
    }
    void OnDisable()
    {
        Nowtimer = 0.0f;
        Beforeimer = 0;
        SetTimer(0);
    }
    /// <summary>
    /// 傳入秒，要把它轉換為分鐘，並SetText
    /// </summary>
    /// <param name="second"></param>
    private void SetTimer(int Allsecond)
    {
        int minute = 0,second=0;
        minute = Allsecond/60;
        second = Allsecond % 60;
        if (minute == 0)
        {
            text.text = " "+minute + ":" + second;
            return;
        }
        text.text = minute+":"+second;
    }
	
	
}
