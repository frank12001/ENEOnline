using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioController_S3 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        interactive = gameObject.GetComponents<AudioSource>()[1];
        audioClip = (AudioClip[])audioClipGet.Clone();
	}
    static public AudioSource interactive;
    static public AudioClip[] audioClip;
    public AudioClip[] audioClipGet;

    public enum ButtonClick : byte
    {
        onclick = 0,
        onMouseEnter,        
    }
    static public void PlayerMusic(ButtonClick buttonstate)
    {
        interactive.PlayOneShot(audioClip[(byte)buttonstate], 1);
    }
}
