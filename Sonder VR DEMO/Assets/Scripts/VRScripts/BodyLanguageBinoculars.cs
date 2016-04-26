using System;
using UnityEngine;

public class BodyLanguageBinoculars : VRInteractiveItem
{
	// Use this for initialization
	void Start () {
		this.OnOver += HandleOver;
		this.OnOut += HandleOut;
	}

	// Update is called once per frame
	void Update () {

	}

	//Handle the Over event
	private void HandleOver()
	{
		//transform.localScale = originalScale * 2;
		Debug.Log("Show over state");

	}

	//Handle the Out Event
	private void HandleOut()
	{
		//transform.localScale = originalScale;
		Debug.Log("Show out state");
	}
}
