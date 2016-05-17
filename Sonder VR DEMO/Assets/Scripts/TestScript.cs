using UnityEngine;
using System.Collections;

public class TestScript : VRInteractiveItem {
	
	private bool over;
	private Transform originalTransform;

	// Use this for initialization
	void Start () {
		this.OnOver += HandleOver;
		this.OnOut += HandleOut;

		originalTransform = this.GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Handle the Over event
	private void HandleOver()
	{
		this.GetComponent<Transform> ().Translate (0,0,-0.3F);
		over = true;
	}

	//Handle the Out Event
	private void HandleOut()
	{
		this.GetComponent<Transform> ().Translate (0,0,0.3F);
		over = false;
	}

	/*
	private IEnumerator bodySwap() {
		while (Camera.main.fieldOfView > 1 && over) {
			Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, 0f, Time.deltaTime);
			yield return new WaitForSeconds (0.0001f);
		}
	}
	*/

}
