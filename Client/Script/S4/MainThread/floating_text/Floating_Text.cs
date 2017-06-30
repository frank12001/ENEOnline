using UnityEngine;
using TMPro;

public class Floating_Text : MonoBehaviour {


    void Awake()
    {
        text = gameObject.GetComponent<TextMeshPro>();
        text.color = new Color32(255, 0, 0, 255);
        text.fontSize = 0.4f;
    }

    TextMeshPro text;
    bool small = true;

	// Update is called once per frame
	void Update () {

        if (small) //在縮小時 
        {
            if (text.fontSize > 0.35f && text.fontSize <= 0.4f)
                text.fontSize -= 0.7f * Time.deltaTime;
            else if (text.fontSize <= 0.35f)
                small = false;
        }
        else      //放大時
        {
            if (text.color.a > 0 && text.fontSize > 0.4f)
            {

                byte a = (byte)(text.color.a - (byte)(255 * Time.deltaTime));
                if (text.color.a <= 10)
                    a = 0;
                text.color = new Color32(255, 0, 0, a);
            }
            if (text.fontSize < 0.5f)
                text.fontSize += 1.0f * Time.deltaTime;
        }
        
	}
    void OnDisable()
    {
         text.color = new Color32(255, 0, 0, 255);
         text.fontSize = 0.4f;
         small = true;
    }
    
}
