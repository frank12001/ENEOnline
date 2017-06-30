using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class S2 : MonoBehaviour {

    //异步对象
    AsyncOperation async;

    //读取场景的进度，它的取值范围在0 - 1 之间。
    int progress = 0;

    void Start()
    {
        ConnectFunction.F.S2checkevent += S2checkevent;
        object[] o = new object[1]{ConnectFunction.F.playerInfo.UID};
        Dictionary<byte,object> dic = ConnectFunction.F.ChangeToDic(o);
       // int i = Convert.ToInt32(ConnectFunction.F.playerInfo.UID);
       // Dictionary<byte,object> dic = new Dictionary<byte,object>{{(byte)1,i}};
        ConnectFunction.F.Deliver((byte)3, dic);

        //在这里开启一个异步任务，
        //进入loadScene方法。
        //StartCoroutine(loadScene());
    }

    public void OnDestroy()
    {
        ConnectFunction.F.S2checkevent += S2checkevent;
    }
    
    public void S2checkevent(short returncode) //S2check 的委派
    {
        if (returncode == 0)
            StartCoroutine(loadScene(3));
        else if (returncode == 1)
            StartCoroutine(loadScene(21));
    }


    void Update()
    {

          //在这里计算读取的进度，
          //progress 的取值范围在0.1 - 1之间， 但是它不会等于1
          //也就是说progress可能是0.9的时候就直接进入新场景了
          //所以在写进度条的时候需要注意一下。
          //为了计算百分比 所以直接乘以100即可
        //progress = (int)(async.progress * 100);
          //有了读取进度的数值，大家可以自行制作进度条啦。
        //Debug.Log(progress);
    }
    //注意这里返回值一定是 IEnumerator
    IEnumerator loadScene(short scene)
    {
        if (scene == 3)
        {
            //异步读取场景。
            async = Application.LoadLevelAsync("S3");
            //读取完毕后返回， 系统会自动进入C场景
            yield return async;
        }
        else if (scene == 21)
        {
            async = Application.LoadLevelAsync("S21");
            yield return async;
        }

    }
}
