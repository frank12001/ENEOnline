using UnityEngine;
using System.Collections;

public class ButtonSpark : MonoBehaviour {
    public void Awake()
    {
        mesh = this.gameObject.GetComponent<MeshRenderer>();
        color = mesh.material.color;
    }

    /// <summary>
    /// Button閃爍開關
    /// </summary>
    /// <param name="start"></param>
    static public void Spark(bool start)
    {
        //寫spark程式碼
        Buttonspark = start;
    }
    MeshRenderer mesh;
    static public bool Buttonspark=false;
    Color color;
    float timer = 0.0f;
    void FixedUpdate()
    {
        if (Buttonspark)
        {
            timer = timer + Time.deltaTime;
            if ((int)timer == 1)
            {
                mesh.material.color = Color.yellow;
            }
            else if ((int)timer == 2)
            {
                mesh.material.color = color;
                timer = 0.0f;
            }
        }
        else
        {
            timer = 0.0f;
            mesh.material.color = color;
        }
    }
}
