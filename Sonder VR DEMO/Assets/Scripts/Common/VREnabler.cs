using UnityEngine;
using UnityEngine.VR;
using System.Collections;

public class VREnabler : MonoBehaviour {

	[SerializeField] private bool enabled = false;

	// Use this for initialization
	void Start () {
		VRSettings.enabled = enabled;
	}

}
