using UnityEngine;
using System.Collections;

public class Reticle : MonoBehaviour {

	[SerializeField] private Camera m_Camera;
	[SerializeField] private float m_ReticleDefaultDistance = 2.0f;

	private Vector3 originalScale;

	// Use this for initialization
	void Start () {
		GetOriginalScale ();
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		float distance = 1;
		if (Physics.Raycast (new Ray (m_Camera.transform.position, m_Camera.transform.rotation * Vector3.forward), out hit)) {
			//distance = hit.distance;
		} else {
			//distance = m_Camera.farClipPlane * 0.95f; 
		}
			
		transform.position = m_Camera.transform.position + m_Camera.transform.rotation * Vector3.forward * distance;
		transform.LookAt (m_Camera.transform.position);
		transform.Rotate (0.0f, 180.0f, 0.0f);
		transform.localScale = originalScale * distance * 0.2f;
	}

	public void GetOriginalScale(){
		originalScale = transform.localScale;
	}

	public void changeCamera(Camera newCamera){
		m_Camera = newCamera;
		GetOriginalScale ();
	}
}
