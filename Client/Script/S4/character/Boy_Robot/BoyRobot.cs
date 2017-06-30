using UnityEngine;
using System.Collections;


namespace Assets.Script.S4.Character
{
public class BoyRobot : Basic_Character{

    //在 Boy_Robot/Root/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/RH_WP 裡有 Trail Render 的 Component 
    //在 Boy_Robot/Root/Hips/Spine/Spine1/Spine2/LeftShoulder/LeftArm/LeftForeArm/LeftHand/LH_WP     裡有 Trail Render 的 Component 

	// Use this for initialization
	void Start () {

        base.Start();
        rightHand = gameObject.transform.Find("Root/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/RH_WP"); //取得右手骨架的Transform
        leftLeg =   gameObject.transform.Find("Root/Hips/LeftUpLeg/LeftLeg"); //取得左腿骨架的Transform

        #region 關掉左右手的軌跡效果
        
        rightHand_TrailEffect = gameObject.transform.Find("Root/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/RH_WP").GetComponent<TrailRenderer>();
        rightHand_TrailEffect.enabled = false;

        leftHand_TrailEffect = gameObject.transform.Find("Root/Hips/Spine/Spine1/Spine2/LeftShoulder/LeftArm/LeftForeArm/LeftHand/LH_WP").GetComponent<TrailRenderer>();
        leftHand_TrailEffect.enabled = false;
        
        #endregion 

    }

    #region 參數
    /// <summary>
    /// 做好的效果
    /// </summary>
    public GameObject Effect_Attack0,Effect_Attack0_Batter2,Effect_Stiff,Effect_MoveSpace,Effect_Attack_E;
    /// <summary>
    /// 手掌的位置，用於觸發特效
    /// </summary>
    private Transform rightHand,leftLeg;
    /// <summary>
    /// 左右手的軌跡效果
    /// </summary>
    private TrailRenderer leftHand_TrailEffect, rightHand_TrailEffect;

    public AudioClip Attack0_Audio, Attack0_batter2_Audio, Move_Space_Audio,Attack0_E_Audio,Stiff_Audio;
    #endregion

    // Update is called once per frame
	void Update ()
    {

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Die")) //如果是在死亡狀態，則這偵不做特效
            return;
        #region 依照動畫狀態，製造特效

        #region Attack_Mouse0 連擊特效
        CreateEffect(Effect_Attack0, rightHand.position, "Attack_Mouse0", 0.8f);           //attack0 連擊第一下
        CreateEffect(Effect_Attack0, leftLeg.position, "Attack_batter1", 0.6f);            //attack0 連擊第二下
        CreateEffect(Effect_Attack0_Batter2, rightHand.position, "Attack_batter2", 0.5f); //attack0 連擊第三下

        #region 在連擊時 ， 將左右手的攻擊軌跡效果打開
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_Mouse0") || animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_batter1") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_batter2"))
        {
            if (leftHand_TrailEffect.enabled && rightHand_TrailEffect.enabled)
            {
                leftHand_TrailEffect.enabled = true;
                rightHand_TrailEffect.enabled = true;
            }
        }
        else
        {
            if (leftHand_TrailEffect.enabled && rightHand_TrailEffect.enabled)
            {
                leftHand_TrailEffect.enabled = false;
                rightHand_TrailEffect.enabled = false;
            }
        }
        #endregion 
        #endregion 


        CreateEffect(Effect_Attack_E, gameObject.transform.position,gameObject.transform.rotation, "Attack_E", 0.5f);//大絕特效

        CreateEffect(Effect_Stiff, gameObject.transform.position, "Stiff", 0.5f); //被打效果

        CreateEffect(Effect_MoveSpace, gameObject.transform.position, "Move_Space", 0.1f, true); //衝刺效果
        #endregion 

    }
    protected override void ExeWhenEffectInstantiate(string stateName)
    {
        bool isOperator = (OperatorId == PlayerId);
        if (stateName == "Attack_Mouse0" || stateName == "Attack_batter1")
            playeSound(isOperator,Attack0_Audio);
        else if (stateName == "Attack_batter2")
            playeSound(isOperator, Attack0_batter2_Audio);           
        else if (stateName == "Move_Space")
            playeSound(isOperator, Move_Space_Audio);
        else if(stateName == "Attack_E")
            playeSound(isOperator, Attack0_E_Audio);
        else if(stateName == "Stiff")
            playeSound(isOperator, Stiff_Audio);
           
    }
    private void playeSound(bool isoperator, AudioClip audio)
    {
        if (isoperator)
            AudioController_S4.AudioSources[(byte)AudioController_S4.AudioUid.Player_Attack].PlayOneShot(audio);
        else
            AudioController_S4.AudioSources[(byte)AudioController_S4.AudioUid.Enemy].PlayOneShot(audio);
    }
}
}
