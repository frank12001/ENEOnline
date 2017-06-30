using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OutputResults : MonoBehaviour {

    private Text text;
	// Use this for initialization
	void Start () {
        text = this.gameObject.GetComponentInChildren<Text>();
        string result = this.ResultsOutput() ;
        text.text = result;
        this.Reward();
        
	}
    private string ResultsOutput()
    {
        string result="";
        if (GameOverInfo.IAmWin)
            result += "Win\r\n";
        else
            result += "Lose \r\n";
        for (int i = 0; i < GameOverInfo.KillNumber.Length; i++)
        {
            result += GameOverInfo.NameParticipator[i] +"  Kill : "+GameOverInfo.KillNumber[i]+"  Death :"+GameOverInfo.DeathNumber[i]+"\r\n";

        }
        return result;
    }
    /// <summary>
    /// 計算獎勵。還沒寫更改資料庫
    /// </summary>
    /// <returns>獎勵的值</returns>
    private int Reward()
    {
        int energy = 0;
        #region  獎勵計算
        int totalkill = 0;
        foreach (byte kill in GameOverInfo.KillNumber)
        {
            totalkill += kill;
        }

        int averagekill = totalkill / 10;
        int p = GameOverInfo.KillNumber[GameOverInfo.MyPlayerNumber] - averagekill;

        if (GameOverInfo.IAmWin)
        {        
            if (p >= 0)
            {
                energy = (10 * p) + 24;
            }
            else
            {
                energy = 24 + (2 * (-p));
            }

        }
        else
        {
            if (p >= 0)
            {
                energy = -(24 + (2 * (-p)));
            }
            else
            {
                energy = -((10 * p) + 24);
            }
        }
        #endregion

        //回傳資料庫
        Dictionary<byte,object> packet = new Dictionary<byte,object>
        {
            {(byte)0,energy},
        };
        Debug.Log("uid " + ConnectFunction.F.playerInfo.UID + " energy = " + energy);
        ConnectFunction.F.Deliver((byte)19,packet);
        
        return energy;
    }
    public void Back()
    {
        //將GameOverInfo裡的資料清完再跳轉
        GameOverInfo.fromWhichScene = 6;

        ConnectFunction.F.Deliver((byte)20, new Dictionary<byte, object>());

        Application.LoadLevel("S22");
    }
}
