using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace Assets.Script.S4.Character
{
    public class GoldenBird :   Basic_Character{


        public AudioClip Stiff_Audio, Die_Audio;
        short lastFrameHP = 0;
	    // Use this for initialization
	    void Start () {
	
	    }
	
	    // Update is called once per frame
	    void Update () {
            if (lastFrameHP != this.hp)
            {
                //--//
                bool isOperator = (OperatorId == PlayerId);
                playeSound(isOperator,Stiff_Audio);
                   
                if(this.hp <= 0)
                   playeSound(isOperator, Die_Audio);
                lastFrameHP = this.hp;
            }
            
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