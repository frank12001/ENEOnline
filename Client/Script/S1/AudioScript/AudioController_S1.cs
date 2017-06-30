using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioController_S1 : MonoBehaviour {
    

    // Use this for initialization
    void Start()
    {
        interactive = gameObject.GetComponents<AudioSource>()[1];
        audioClip = (AudioClip[])audioClipGet.Clone();
        Hurt = HurtSnapshot;
        snapShot = Snapshot;
    }

    static public AudioSource interactive;
    static public AudioClip[] audioClip;
    static public AudioMixerSnapshot Hurt,snapShot;
    public AudioClip[] audioClipGet;
    public AudioMixerSnapshot HurtSnapshot,Snapshot;

    public Animator BugAnimator;
    private bool isInBug= false;

    public enum AudioID : byte
    {
        kaka = 0,
        kakaB,
        hurt,
    }
    void Update()
    {
        if (!isInBug && BugAnimator.GetCurrentAnimatorStateInfo(0).IsName("anim2-3"))
        {
            interactive.PlayOneShot(audioClip[3], 1);
            isInBug = true;
        }
    }
    static public void PlayerMusic(AudioID audioID)
    {
        if ((byte)audioID==2)
            Hurt.TransitionTo(0.0f);
        interactive.PlayOneShot(audioClip[(byte)audioID], 1);
    }
}
