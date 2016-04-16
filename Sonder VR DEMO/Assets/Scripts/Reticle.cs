using UnityEngine;
using System.Collections;

public class Reticle : MonoBehaviour {

	[SerializeField] private Camera m_Camera;
	[SerializeField] private float m_ReticleDefaultDistance = 2.0f;

	private Vector3 originalScale;

	// Use this for initialization
	void Start () {
		originalScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		float distance;
		if (Physics.Raycast (new Ray (m_Camera.transform.position, m_Camera.transform.rotation * Vector3.forward), out hit)) {
			distance = hit.distance;
		} else { distance = m_ReticleDefaultDistance; }
			
		transform.position = m_Camera.transform.position + m_Camera.transform.rotation * Vector3.forward * distance;
		transform.LookAt (m_Camera.transform.position);
		transform.Rotate (0.0f, 180.0f, 0.0f);
		transform.localScale = originalScale * distance * 0.3f;
	}
}
