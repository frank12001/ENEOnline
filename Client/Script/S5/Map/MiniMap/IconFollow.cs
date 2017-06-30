using UnityEngine;
using System.Collections.Generic;

public class IconFollow : MonoBehaviour {

    public byte PlayerNumber;

    //每個Icon物件都要加入這個Script
    bool startfollow;
    GameObject target;
    float consty;

    private bool isSenderTeammate;

    // Use this for initialization
    void Start()
    {

    }
    #region 碰撞事件 變換目標layer

    /// <summary>
    /// 當Icon 撞到人時。
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("enter");
        /*
        if (isSenderTeammate && other.gameObject.name == "IconSword(Clone)")
        {
            //如果Icon 的碰撞會撞到其他東西的話(因為是使用圓collider)，將所有的collider圓心調更高，讓他們去更上面撞
            //先判斷它是不是在 enumLayers.Hide這層裡
            //在取IconFollow 如果它有的話，表示它是敵人角色，呼叫AssignLayer將它的Layer變成enumLayers.normal
            IconFollow script = other.gameObject.GetComponent<IconFollow>();
            if (other.gameObject.layer == (int)enumLayers.Hide&&!iconisTeammate(this.PlayerNumber,script.PlayerNumber))
            {
                
                if (script != null) //是玩家~
                    script.AssignLayer((byte)enumLayers.normal);
                else
                    other.gameObject.layer = (int)enumLayers.normal;

                Debug.Log("撞到 9 layer的人將她加入 8 layer");
            }

        }
        */
    }

    public void OnTriggerStay(Collider other)
    {
        //Debug.Log("stay");
        if (other.gameObject.layer == (int)enumLayers.Hide)
        {
            if (isSenderTeammate && other.gameObject.name == "IconSword(Clone)")
            {
                //如果Icon 的碰撞會撞到其他東西的話(因為是使用圓collider)，將所有的collider圓心調更高，讓他們去更上面撞
                //先判斷它是不是在 enumLayers.Hide這層裡
                //在取IconFollow 如果它有的話，表示它是敵人角色，呼叫AssignLayer將它的Layer變成enumLayers.normal
                IconFollow script = other.gameObject.GetComponent<IconFollow>();
                if (!iconisTeammate(this.PlayerNumber, script.PlayerNumber))
                {

                    if (script != null) //是玩家~
                        script.AssignLayer((byte)enumLayers.normal);
                    else
                        other.gameObject.layer = (int)enumLayers.normal;

                    Debug.Log("撞到 9 layer的人將她加入 8 layer");
                }

            }
        }
    }


    private bool iconisTeammate(byte player1number, byte player2number)
    {
        if ((player1number > 4 && player2number > 4) || (player1number <= 4 && player2number <= 4))
        {
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// 當有東西離開時。
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerExit(Collider other)
    {
        if (isSenderTeammate && other.gameObject.name == "IconSword(Clone)")
        {
            //Debug.Log("exit");
            //用相同的方法將Layer改回來
                IconFollow script = other.gameObject.GetComponent<IconFollow>();
                if (!iconisTeammate(this.PlayerNumber, script.PlayerNumber))
                {
                    if (script != null) //是玩家~
                        script.AssignLayer((byte)enumLayers.Hide);
                    else //是其他奇怪的東東
                        other.gameObject.layer = (byte)enumLayers.Hide;
                }
                Debug.Log("將之前被換到 layer 8 的 轉回9");
        }

        
    }
    #endregion
    // Update is called once per frame
    void Update()
    {
        //如果可以開始跟隨了
        //就開始跟隨，y固定
        if (startfollow && target != null)
        {
            Vector3 pos = new Vector3(target.transform.position.x, consty, target.transform.position.z);
            this.gameObject.transform.position = pos;
        }
    }
    /// <summary>
    /// 開始跟隨x,z軸，並初始化需要的值
    /// </summary>
    /// <param name="follow">要跟隨的物件</param>
    /// <param name="updistance">這個物件在多高(世界座標)</param>
    public void StartFollow(GameObject follow, float updistance,bool issenderteammate)
    {
        this.target = follow;
        startfollow = true;
        consty = updistance;
        this.isSenderTeammate = issenderteammate;
        if (issenderteammate)
            this.gameObject.GetComponent<SphereCollider>().radius = 20;
        else
            this.gameObject.GetComponent<SphereCollider>().radius = 3;
        PlayerNumber = byte.Parse(this.target.name);
    }
    /// <summary>
    /// 一次設定自己和target的layer
    /// </summary>
    /// <param name="layernumber">要設定的layer int</param>
    /// <returns>true為更改成功</returns>
    public bool AssignLayer(byte layernumber)
    {
        if (startfollow && target != null) //如果已經被指派target了
        {
            target.layer = layernumber;
            this.gameObject.layer = layernumber;
            return true;
        }
        else
            return false;
    }
}
