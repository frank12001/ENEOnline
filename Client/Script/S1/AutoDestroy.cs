using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour {

    float timer;
    public float AlifeSeconds;
	// Use this for initialization
	void Start () {
        timer = 0.0f;

	}
	
	// Update is called once per frame
	void Update () {
        timer = timer + Time.deltaTime;
        if (timer > AlifeSeconds)
        {
            Destroy(this.gameObject);
        }
	}
}
