using UnityEngine;
using System.Collections;
using TMPro;

public class LogoAll : MonoBehaviour {

    //字串集合
    public S1String stringpool;
    //音訊預置物件
    public GameObject kaka;
    public GameObject kakab;
    //Logo動畫
    static public Animator anim; //<<<<<-----在這裡將anim設成static，方便其他人取用
    //中間的那個字串2
    //帳號密碼的字
    private TextMeshPro connectText;
    #region Mono實作介面
    void Awake() {
        anim = this.gameObject.GetComponent<Animator>();
    }
    // Use this for initialization
	void Start () {
        stringpool = new S1String();
        Loginidle.logining += logingevent;
        Registeridle.register += registerevent;

	}
	
	// Update is called once per frame
	void Update () {
    
	}
    public void OnDestroy()
    {
        Loginidle.logining -= logingevent;
        Registeridle.register -= registerevent;
    }
    #endregion
    //case 5 需要的物件 AutoLogin 狀態的 UI 物件
    public GameObject autologintext1, autologintext2, autologintext3;
    //控制現在Logo的動畫
    public void LogoStateChange(int index)
    {
        switch (index)
        {
            case 1:
                    anim.SetInteger("state", 1);
                    //Instantiate(kaka);
                    if (ConnectFunction.F.ConnectToServer())
                    {
                        //不用做事
                    }
                    else
                    {
                        //connectText.text = stringpool.connect3;
                    }
                    break;
            case 2:
                    anim.SetInteger("state", 2);
                    //Instantiate(kakab);
                    break;

            case 3: //離開遊戲in Logo2
                    Application.Quit();
                    break;

            case 4: //在開始寫注冊資訊前還是要先連線
                    anim.SetInteger("state", 3);
                    Instantiate(kaka);
                    if (!ConnectFunction.F.playerInfo.isConnect) //先判斷有沒有連過線
                    {
                        if (ConnectFunction.F.ConnectToServer()) //連線
                        {
                            //處理註冊//和下面一樣看要不要寫個function
                        }
                        else
                        {
                            
                        }
                    }
                    else //處理註冊
                    {
                        
                    }
                    break;
            default:
                    break;
 
        }
    }

    public GameObject input;
    public GameObject text1;
    public void logingevent(bool appear)     //Loginidle 的委派
    {
        if (appear)
        {
            input.SetActive(true);
            text1.SetActive(true);
        }
        else
        {
            input.SetActive(false);
            text1.SetActive(false);
        }
    }
    public GameObject input2;      //Registeridle 的委派
    public void registerevent(bool b)
    {
        if (b)
            input2.SetActive(true);
        else
            input2.SetActive(false);
    }

    public void Playerkaka()
    {
        AudioController_S1.PlayerMusic(AudioController_S1.AudioID.kaka);
    }
    public void PlayerkakaB()
    {
        AudioController_S1.PlayerMusic(AudioController_S1.AudioID.kakaB);
    }
    public void PlayerHurt()
    {
        AudioController_S1.PlayerMusic(AudioController_S1.AudioID.hurt);
    }
}
