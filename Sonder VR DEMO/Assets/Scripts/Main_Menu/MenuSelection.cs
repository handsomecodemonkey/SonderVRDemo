using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuSelection : VRInteractiveItem {

	[SerializeField]private GameObject m_Reticle;

	private Sprite m_defaultSprite;
	private SpriteRenderer m_SpriteRenderer;
	[SerializeField]private Image m_MenuBackground;
	[SerializeField]private Sprite m_Sprite;
	[SerializeField]private int m_SceneToOpen;

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
		Debug.Log (over);
		if (Input.GetButtonDown ("Fire1") && over) { //Press A Button
			SceneManager.LoadScene (m_SceneToOpen);
		}

	}

	//Handle the Over event
	private void HandleOver()
	{
		Debug.Log ("voer");
		over = true;
		m_SpriteRenderer.sprite = m_Sprite;
		m_MenuBackground.color = Color.white;
		this.GetComponent<Transform> ().Translate (0,0,-0.3F);
	}

	//Handle the Out Event
	private void HandleOut()
	{
		over = false;
		m_SpriteRenderer.sprite = m_defaultSprite;
		m_MenuBackground.color = new Color (1F, 1F, 1F, 0.882F);
		this.GetComponent<Transform> ().Translate (0,0,0.3F);
	}
}
