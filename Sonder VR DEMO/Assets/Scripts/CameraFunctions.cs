using UnityEngine;
using System.Collections;

public class CameraFunctions : MonoBehaviour {

	[SerializeField]
	private Camera camera;

	private float colorLerpDuration = 1.5f;
	private float colorLerpT = 0f;



	// Use this for initialization
	void Start () {
		//Start by setting clear flag to solid color and make it white
		//camera.clearFlags = CameraClearFlags.SolidColor;
		//camera.fieldOfView = 180f;
	}
	
	// Update is called once per frame
	void Update () {
		//camera.backgroundColor = Color.Lerp (Color.white, Color.black, colorLerpT);
		//if (colorLerpT < 1) {colorLerpT += Time.deltaTime / colorLerpDuration;}

		//camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 0f, 1.3f * Time.deltaTime); //for body swapping effect
		//Debug.Log(camera.fieldOfView);
		//if(camera.fieldOfView < 1) { Debug.Log ("reset camera fov"); camera.ResetFieldOfView(); }
	}

}
