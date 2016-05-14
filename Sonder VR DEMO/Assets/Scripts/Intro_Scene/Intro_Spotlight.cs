using UnityEngine;
using System.Collections;

//This script is to be applied to a Directional Spotlight, it will give a nice effect by slowly revealing the spotlight and the objects within spotlight range.

public class Intro_Spotlight : MonoBehaviour {

	[SerializeField] private float m_spotAngle;
	[SerializeField] private float m_spotAngleSpeed = 0.3f;
	private Light m_spotLight;

	// Use this for initialization
	void Start () {
		m_spotLight = GetComponent<Light> ();
		m_spotLight.spotAngle = 0f;
	}
	
	// Update is called once per frame
	void Update () {
			if (m_spotLight.spotAngle < m_spotAngle) {m_spotLight.spotAngle += m_spotAngleSpeed;} 
	}

}
