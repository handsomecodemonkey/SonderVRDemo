using System;
using UnityEngine;

public class BodyLanguageBinoculars : VRInteractiveItem
{

	[SerializeField]private GameObject m_Reticle;

	private Sprite m_defaultSprite;
	private SpriteRenderer m_SpriteRenderer;
	[SerializeField]private Sprite m_Sprite;

	public static bool activated;
	private bool over = false;

	// Use this for initialization
	void Start () {
		this.OnOver += HandleOver;
		this.OnOut += HandleOut;

		m_SpriteRenderer = m_Reticle.GetComponent<SpriteRenderer> ();
		m_defaultSprite = m_SpriteRenderer.sprite;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Fire3") && over) { //Press A Button
			Debug.Log("Please put functionality here...");
		}
	}

	//Handle the Over event
	private void HandleOver()
	{
		over = true;
		if (activated) {
			m_SpriteRenderer.sprite = m_Sprite;
		}
	}

	//Handle the Out Event
	private void HandleOut()
	{
		over = false;
		if (activated) {
			m_SpriteRenderer.sprite = m_defaultSprite;
		}
	}
}
