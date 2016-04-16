using UnityEngine;
using System.Collections;

public class WaterFountainScripts : MonoBehaviour {

	[SerializeField]
	private ParticleSystem water;

	private AudioSource audio;

	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown ("space")) {
			if (!water.isPlaying) {
				audio.volume = 1;
				water.Play ();
			} else {
				audio.volume = 0;
				water.Stop ();
			}
		} 
	}
}
