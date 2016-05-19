using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KelseyBehavior : MonoBehaviour {

	private NavMeshAgent agent;
	private Animation anim;

	//Audio Clips
	public List<AudioClip> audioClips;
	public AudioSource audioSource;

	// Use this for initialization
	void Start () {
		agent = this.GetComponent<NavMeshAgent> ();
		anim = this.GetComponent<Animation> ();
		audioSource = this.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void goToTarget(Vector3 target){
		agent.SetDestination (target);
	}

	public void setAnimation(string animationName){
		anim.CrossFade (animationName);
	}
}
