using UnityEngine;
using System.Collections;

public class load : MonoBehaviour {

    //异步对象
    AsyncOperation async;

    //读取场景的进度，它的取值范围在0 - 1 之间。
    int progress = 0;
	// Use this for initialization
	void Start () {
        StartCoroutine(loadScene());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    IEnumerator loadScene()
    {
            //异步读取场景。
            async = Application.LoadLevelAsync("S3");
            //读取完毕后返回， 系统会自动进入C场景
            yield return async;
    }
}
