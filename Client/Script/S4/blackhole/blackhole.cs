using UnityEngine;
using System.Collections;

public class blackhole : MonoBehaviour {

    void OnTriggerStay(Collider other)
    {
        Destroy(other.gameObject);
    }

    void OnDisable()
    {
        //Instantiate(FirstDestroy);
    }

    public GameObject FirstDestroy;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
