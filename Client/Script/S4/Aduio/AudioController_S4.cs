using UnityEngine;
using System.Collections;

public class AudioController_S4 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        AudioSources = gameObject.GetComponents<AudioSource>();
	}

    static public AudioSource[] AudioSources;
    public enum AudioUid : byte
    {
        Background =0,
        Player_Attack,
        Enemy,
    }

	// Update is called once per frame
	void Update () {
	
	}
}
