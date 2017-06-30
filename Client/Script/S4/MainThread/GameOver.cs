using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;


public class GameOver : MonoBehaviour {


    /// <summary>
    /// 面板物件的路徑
    /// </summary>
    public string End_PanelPath;

    public AudioClip Win_Audio, Lose_Audio;

    /// <summary>
    /// 結束遊戲封包收到後要執行的功能。 要變成統計圖表之類的可以直接改傳入的參數，和從Server傳過來的最後一個封包，用新封包裡的資料來做統計。 (掛勾給ReceiveAndStoreWorldStates)
    /// </summary>
    /// <param name="win">有沒有贏 (現在的封包只有傳這個值。可以改)</param>
    public void GameOverProcess(bool win)
    {
        GameObject.Find("MainCanvas").SetActive(false);
        GameObject g = Resources.Load(End_PanelPath, typeof(GameObject)) as GameObject;                                 //讀取遊戲模型
        GameObject gamer = Instantiate(g, g.transform.position, g.transform.rotation) as GameObject;                    //遊戲物件初始化   
        //---對這個EndPlane進行一些設定
        Button button = gamer.GetComponentInChildren<Button>();
        if (button != null)
        {
            button.onClick.AddListener(delegate { BackToScene3(); });
        }
        Text title = gamer.GetComponentInChildren<Text>();
        if (title != null)
        {
            if (win)
            {
                title.text = "勝 利";
                AudioController_S4.AudioSources[(byte)AudioController_S4.AudioUid.Background].clip = Win_Audio;
                AudioController_S4.AudioSources[(byte)AudioController_S4.AudioUid.Background].Play();
            }
            else
            {
                title.text = "失 敗";
                AudioController_S4.AudioSources[(byte)AudioController_S4.AudioUid.Background].clip = Lose_Audio;
                AudioController_S4.AudioSources[(byte)AudioController_S4.AudioUid.Background].Play();
            }
        }
    }
    public void BackToScene3()
    {
        Application.LoadLevel("S3");
    }

}
