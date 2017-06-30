using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OutputChat : MonoBehaviour {

    float time = 0.0f;
    void Update()
    {
        time += Time.deltaTime;
        if (time > 3.0f)
            Destroy(this.gameObject);
    }

    public void SetText(string s)
    {
        this.GetComponentInChildren<Text>().text = s;
    }
}
