using UnityEngine;
using System.Collections.Generic;

public class initialize : MonoBehaviour {
    void Awake()
    {
        chair = new GameObject[5];
        chair[0] = GameObject.Find("chair1");
        chair[1] = GameObject.Find("chair2");
        chair[2] = GameObject.Find("chair3");
        chair[3] = GameObject.Find("chair4");
        chair[4] = GameObject.Find("chair5");

        bugcontroller = new bugController[5];
        bugcontroller[0] = GameObject.Find("Bug1").GetComponent<bugController>();
        bugcontroller[1] = GameObject.Find("Bug2").GetComponent<bugController>();
        bugcontroller[2] = GameObject.Find("Bug3").GetComponent<bugController>();
        bugcontroller[3] = GameObject.Find("Bug4").GetComponent<bugController>();
        bugcontroller[4] = GameObject.Find("Bug5").GetComponent<bugController>();
    }
    static public GameObject[] chair;
    static public bugController[] bugcontroller;
  	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
