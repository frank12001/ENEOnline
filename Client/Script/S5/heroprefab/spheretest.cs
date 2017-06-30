using UnityEngine;
using System.Collections;

public class spheretest : MonoBehaviour,IPropertyBasic {
    public void OnCollisionExit(Collision collision)
    {
        IPropertyBasic target = collision.gameObject.GetComponent<IPropertyBasic>();
        if (target != null)
            target.BLO = target.BLO - this.ATK;
        Debug.Log(" 塔現在的 BLO = " + target.BLO);
    }

	// Use this for initialization
	void Start () {
        BLO = 10.0f;
        ATK = 100.0f;
        DEF = 10.0f;
        SPE = 10.0f;
        SUS = 10.0f;
	}
	
	// Update is called once per frame
	void Update () {
        
	}

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
        get
        {
            throw new System.NotImplementedException();
        }
        set
        {
            throw new System.NotImplementedException();
        }
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
        get
        {
            throw new System.NotImplementedException();
        }
        set
        {
            throw new System.NotImplementedException();
        }
    }
}
