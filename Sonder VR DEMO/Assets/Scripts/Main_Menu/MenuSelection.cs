using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuSelection : VRInteractiveItem {

	[SerializeField]private GameObject m_Reticle;

	private Sprite m_defaultSprite;
	private SpriteRenderer m_SpriteRenderer;
	[SerializeField]private Sprite m_Sprite;
	[SerializeField]private int m_SceneToOpen;

	private static bool over = false;

	// Use this for initialization
	void Start () {
		this.OnOver += HandleOver;
		this.OnOut += HandleOut;

		m_SpriteRenderer = m_Reticle.GetComponent<SpriteRenderer> ();
		m_defaultSprite = m_SpriteRenderer.sprite;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown ("Fire1") && over) { //Press A Button
			SceneManager.LoadScene (m_SceneToOpen);
		}

		if (over) {
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
