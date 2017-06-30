using UnityEngine;
using System.Collections;

public class AnimController : MonoBehaviour {
    void Awake(){
        
    }

    //取得這個Gameobject 上的Component
    public Animator animator;
    SenderMotion sendermotion;
	// Use this for initialization
	void Start () {
        animator = this.GetComponent<Animator>();
        sendermotion = this.GetComponent<SenderMotion>();
	}
   
	// Update is called once per frame
	void Update ()
    {
        #region move anim
        if (Input.GetKeyDown(KeyCode.W))
            animator.SetInteger("state", 2);
        if (Input.GetKeyUp(KeyCode.W))
            animator.SetInteger("state", 1);
        #endregion

        #region attack anim 
        //開始攻擊
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            animator.SetInteger("state", 3);  
        }

        //攻擊時，角色不能移動
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("attack"))
        {
            if (sendermotion.movable)
                sendermotion.movable = false;
        }
        else
        {
            if (!sendermotion.movable)
                sendermotion.movable = true;
        }

        //離開攻擊狀態
        if (Input.GetKeyUp(KeyCode.Mouse0))
            animator.SetInteger("state", 1);
        #endregion
    }
}
