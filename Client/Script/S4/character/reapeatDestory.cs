using UnityEngine;
using System.Collections;

public class reapeatDestory : MonoBehaviour {
    bool b = true;
    public void OnTriggerEnter(Collider other)
    {
            if (b)
            {
                Destroy(other.gameObject);
                b = false;
            }
    }


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
