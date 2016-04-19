using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	[SerializeField] private VRInteractiveItem m_InteractiveItem;

	private Vector3 originalScale;
	private bool over;

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
		//transform.localScale = originalScale * 2;
		Debug.Log("Show over state");
		over = true;
		StartCoroutine (bodySwap ());

	}

	//Handle the Out Event
	private void HandleOut()
	{
		//transform.localScale = originalScale;
		Debug.Log("Show out state");
		over = false;
		Camera.main.ResetFieldOfView ();
	}

	private IEnumerator bodySwap() {
		while (Camera.main.fieldOfView > 1 && over) {
			Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, 0f, Time.deltaTime);
			yield return new WaitForSeconds (0.0001f);
		}
	}

}
