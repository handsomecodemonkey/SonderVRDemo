using UnityEngine;
using System.Collections;

//This class should be used with the Main Camera
//it checks if the user's gaze is over an object
//This class is a simplified version of Unity's Sample VR Demo VREyeRayCaster Class

public class VREyeRayCaster : MonoBehaviour {

	public event System.Action<RaycastHit> OnRaycasthit;                   // This event is called every frame that the user's gaze is over a collider.

	[SerializeField] private Transform m_Camera;
	[SerializeField] LayerMask m_ExclusionLayers; // Layers to exclude from raycast
	[SerializeField] private float m_RayLength = 500f;              // How far into the scene the ray is cast.

	private VRInteractiveItem m_CurrentInteractible;                //The current interactive item
	private VRInteractiveItem m_LastInteractible;                   //The last interactive item

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		EyeRaycast ();
	}

	private void EyeRaycast() {
		
		// Create a ray that points forwards from the camera.
		Ray ray = new Ray(m_Camera.position, m_Camera.forward);
		RaycastHit hit;

		// Do the raycast forweards to see if we hit an interactive item
		if (Physics.Raycast(ray, out hit, m_RayLength, ~m_ExclusionLayers))
		{
			VRInteractiveItem interactible = hit.collider.GetComponent<VRInteractiveItem>(); //attempt to get the VRInteractiveItem on the hit object
			m_CurrentInteractible = interactible;

			// If we hit an interactive item and it's not the same as the last interactive item, then call Over
			if (interactible && interactible != m_LastInteractible)
				interactible.Over(); 

			// Deactive the last interactive item 
			if (interactible != m_LastInteractible)
				DeactiveLastInteractible();

			m_LastInteractible = interactible;

			if (OnRaycasthit != null)
				OnRaycasthit(hit);
		}
		else
		{
			// Nothing was hit, deactive the last interactive item.
			DeactiveLastInteractible();
			m_CurrentInteractible = null;
		}
	}

	private void DeactiveLastInteractible()
	{
		if (m_LastInteractible == null)
			return;

		m_LastInteractible.Out();
		m_LastInteractible = null;
	}

	private void HandleUp()
	{
		if (m_CurrentInteractible != null)
			m_CurrentInteractible.Up();
	}


	private void HandleDown()
	{
		if (m_CurrentInteractible != null)
			m_CurrentInteractible.Down();
	}


	private void HandleClick()
	{
		if (m_CurrentInteractible != null)
			m_CurrentInteractible.Click();
	}


	private void HandleDoubleClick()
	{
		if (m_CurrentInteractible != null)
			m_CurrentInteractible.DoubleClick();

	}
}
