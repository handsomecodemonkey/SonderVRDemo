using System;
using UnityEngine;

public class BodyLanguageBinoculars : VRInteractiveItem
{

	[SerializeField]private GameObject m_Reticle;

	private Sprite m_defaultSprite;
	private SpriteRenderer m_SpriteRenderer;
	[SerializeField]private Sprite m_Sprite;

	// Use this for initialization
	void Start () {
		this.OnOver += HandleOver;
		this.OnOut += HandleOut;

		m_SpriteRenderer = m_Reticle.GetComponent<SpriteRenderer> ();
		m_defaultSprite = m_SpriteRenderer.sprite;
	}

	// Update is called once per frame
	void Update () {

	}

	//Handle the Over event
	private void HandleOver()
	{
		m_SpriteRenderer.sprite = m_Sprite;
	}

	//Handle the Out Event
	private void HandleOut()
	{
		m_SpriteRenderer.sprite = m_defaultSprite;
	}
}
