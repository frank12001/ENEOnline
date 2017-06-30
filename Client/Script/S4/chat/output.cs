using UnityEngine;
using UnityEngine.UI;

public class output : MonoBehaviour {
    float timer = 0.0f;
    void Update(){
        timer = timer + Time.deltaTime;
        if ((int)timer > 3)
            Destroy(this.gameObject);
    }

    public void Awake(){
        text = this.gameObject.GetComponentInChildren<Text>();
    }

	// Use this for initialization
	void Start () {
	
	}
    Text text;
    public void SetText(string chatcontent)
    {
        this.text.text = chatcontent;
    }
}
