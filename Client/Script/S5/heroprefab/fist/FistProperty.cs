using UnityEngine;
using System.Collections;

public class FistProperty : MonoBehaviour,IPropertyBasic
{
    //存放Basic 能力值和 Unique 能力值

    #region PropertyBasic
    public float BLO { get; set; }

    public float ATK { get; set; }

    public float DEF { get; set; }

    public float SPE { get; set; }

    public float SUS { get; set; }

    public byte TEAM { get; set; }
    #endregion
     

    // Use this for initialization
	void Start () {
        BLO = 100.0f;
        ATK = 10.0f;
        DEF = 10.0f;
        SPE = 10.0f;
        SUS = 10.0f;
        //正式版
        //角色名子為索引
        if (byte.Parse(this.gameObject.name) <= 4)
            TEAM = 1;
        else
            TEAM = 2;
        
        //測試版
        //TEAM = 1;
	}
	
	// Update is called once per frame
	void Update () {
        if (BLO <= 0)
            this.gameObject.GetComponent<Animator>().SetInteger("state", 0);
	}
    public GameObject Attackobject;
    void Attack1()
    {
        //攻擊物件創造
        //g 就是攻擊物件
        GameObject g = Instantiate(Attackobject, this.gameObject.transform.position, this.gameObject.transform.rotation) as GameObject;
        //target 是 從 g裡面取出的 IProgertyBasic
        IPropertyBasic target = g.GetComponent<IPropertyBasic>();
        target.ATK = 10.0f;
        target.TEAM = this.TEAM;
        g.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 500);

    }



    public void SuspendOn()
    {
        throw new System.NotImplementedException();
    }

    public void SuspendOff()
    {
        throw new System.NotImplementedException();
    }


    public byte NUMBER
    {
        get
        {
            throw new System.NotImplementedException();
        }
        set
        {
            throw new System.NotImplementedException();
        }
    }


    public bool FALL
    {
        get
        {
            throw new System.NotImplementedException();
        }
        set
        {
            throw new System.NotImplementedException();
        }
    }


    public bool BOOL
    {
        get;
        set;
    }
}
