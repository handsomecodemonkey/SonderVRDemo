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

	//Customize these settings for Body Language Binoculars Description
	[SerializeField] private String m_NameText;
	[SerializeField] private String m_Mood;
	[SerializeField] private String m_BodyLanguage;
	[SerializeField] private String m_FunFact;

	// Use this for initialization
	void Start () {
		this.OnOver += HandleOver;
		this.OnOut += HandleOut;

		m_SpriteRenderer = m_Reticle.GetComponent<SpriteRenderer> ();
		m_defaultSprite = m_SpriteRenderer.sprite;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Fire3") && over && activated && BodyLanguageBinocularDescription.m_DescriptionText.Length == 0) { //Press X Button
			BodyLanguageBinocularDescription.m_DescriptionText = ("Name: \t" + m_NameText + "\n" +
			"Mood: \t" + m_Mood + "\n" +
			"Body Language: \n" + m_BodyLanguage + "\n" +
			"Fun Fact: " + m_FunFact);
			
		} else if (Input.GetButtonDown ("Fire3") && BodyLanguageBinocularDescription.m_DescriptionText.Length != 0) {
			BodyLanguageBinocularDescription.m_DescriptionText = "";
		}

		if (activated && over) {
			m_SpriteRenderer.sprite = m_Sprite;
		} else {
			m_SpriteRenderer.sprite = m_defaultSprite;
		}
	}

	//Handle the Over event
	private void HandleOver()
	{
		over = true;
	}

	//Handle the Out Event
	private void HandleOut()
	{
		over = false;
	}
}
