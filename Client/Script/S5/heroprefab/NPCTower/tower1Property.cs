using UnityEngine;
using System.Collections;

public class tower1Property : MonoBehaviour,IPropertyBasic {

    #region implement IPropertyBasic

    public float BLO
    {
        get;
        set;
    }
    public float ATK
    {
        get;
        set;
    }

    public float DEF
    {
        get;
        set;
    }

    public float SPE
    {
        get;
        set;
    }

    public float SUS
    {
        get;
        set;
    }

    public byte TEAM
    {
        get;
        set;
    }
    #endregion

    //自己是哪一隊
    public byte team;

    private bool startCalculationResults = false;
    void OnCollisionEnter(Collision collision)
    {
     
    }

	// Use this for initialization
	void Start () {
        BLO = 200.0f;
        ATK = 1.0f;
        DEF = 5.0f;
        SPE = 0.0f;
        SUS = 0.0f;
        TEAM = team;
	}
	
	// Update is called once per frame
	void Update () {
        if (BLO <= 0&&!startCalculationResults)
        {
            try
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().FinishGame(this.TEAM);
                startCalculationResults = true;
            }
            catch { }
            //不要直接回S3先進入緩衝Scene S6
           
        }
	}





    public void SuspendOn()
    {
        
    }

    public void SuspendOff()
    {
        
    }


    public byte NUMBER
    {
        get;
        set;
    }


    public bool FALL
    {
        get;
        set;
    }


    public bool BOOL
    {
        get;
        set;
    }
}
