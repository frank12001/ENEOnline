using UnityEngine;
using System.Collections;

namespace Assets.Script.S4.Character
{
    public class CatRobot :  Basic_Character
    {

	    // Use this for initialization
	    void Start () {
            base.Start();
            cameraTransform = GameObject.Find("Main Camera").transform;
	    }
        public GameObject move_Space_Start_Effect, move_Space_End_Effect, stiff_Effect;

        private Transform cameraTransform;

        public AudioClip Attack0_Audio, Move_Space_Audio, Attack0_E_Audio, Stiff_Audio;

	    // Update is called once per frame
	    void Update ()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Die")) //如果是在死亡狀態，則這偵不做特效
                return;

            #region 依照動畫狀態，製造特效
            CreateEffect(null, this.gameObject.transform.position, "Attack_Mouse0",0.2f);
            CreateEffect(null, this.gameObject.transform.position, "Attack_E", 0.5f);

            CreateEffect(move_Space_Start_Effect, this.gameObject.transform.position + move_Space_Start_Effect.transform.position, "Move_Space", 0.5f,0.0f);

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stiff"))
            {
                Vector3? cameraRayPoint = GetHitPoints(cameraTransform.position);
                if (cameraRayPoint != null)
                    CreateEffect(stiff_Effect, (Vector3)cameraRayPoint, "Stiff", 0.5f, 0.0f);
            }
            #endregion 
        }
        /// <summary>
        /// 在 CreateEffect 功能中 ，特效出現後執行
        /// </summary>
        /// <param name="stateName"></param>
        protected override void ExeWhenEffectInstantiate(string stateName)
        {
            bool isOperator = (OperatorId == PlayerId);
            if (stateName == "Attack_Mouse0")
                playeSound(isOperator, Attack0_Audio);
            else if (stateName == "Move_Space")
                playeSound(isOperator, Move_Space_Audio);
            else if (stateName == "Attack_E")
                playeSound(isOperator, Attack0_E_Audio);
            else if (stateName == "Stiff")
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
