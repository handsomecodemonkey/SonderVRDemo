using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	[SerializeField] private VRInteractiveItem m_InteractiveItem;

	private Vector3 originalScale;

	// Use this for initialization
	void Start () {

		originalScale = transform.localScale;

		m_InteractiveItem.OnOver += HandleOver;
		m_InteractiveItem.OnOut += HandleOut;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Handle the Over event
	private void HandleOver()
	{
		transform.localScale = originalScale * 2;
		Debug.Log("Show over state");
	}

	//Handle the Out Event
	private void HandleOut()
	{
		transform.localScale = originalScale;
		Debug.Log("Show out state");
	}

}
