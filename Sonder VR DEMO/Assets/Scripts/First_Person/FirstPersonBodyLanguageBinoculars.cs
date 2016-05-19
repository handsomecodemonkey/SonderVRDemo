using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FirstPersonBodyLanguageBinoculars : VRInteractiveItem {

	[SerializeField]private GameObject m_Reticle;
	[SerializeField]private Image m_Background;
	[SerializeField]public Text m_Description;
	[SerializeField]private Canvas bodyLanguageDescription;

	private Sprite m_defaultSprite;
	private SpriteRenderer m_SpriteRenderer;
	[SerializeField]private Sprite m_Sprite;

	private bool over = false;

	void Start () {
		bodyLanguageDescription.gameObject.SetActive (false);
		this.OnOver += HandleOver;
		this.OnOut += HandleOut;

		m_SpriteRenderer = m_Reticle.GetComponent<SpriteRenderer> ();
		m_defaultSprite = m_SpriteRenderer.sprite;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Fire3") && over && !bodyLanguageDescription.gameObject.activeSelf) { //Press A Button
			//show body language binoculars description
			bodyLanguageDescription.gameObject.SetActive(true);
		} else if (Input.GetButtonDown("Fire3") && over && bodyLanguageDescription.gameObject.activeSelf){
			//hide body language binoculars
			bodyLanguageDescription.gameObject.SetActive(false);
		}
	}

	//Handle the Over event
	private void HandleOver()
	{
		over = true;
		m_SpriteRenderer.sprite = m_Sprite;
	}

	//Handle the Out Event
	private void HandleOut()
	{
		over = false;
		m_SpriteRenderer.sprite = m_defaultSprite;
	}
}
