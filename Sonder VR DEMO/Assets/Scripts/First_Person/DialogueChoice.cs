using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueChoice : VRInteractiveItem {

	[SerializeField]private GameObject m_Reticle;
	[SerializeField]private Image m_Background;
	[SerializeField]public Text m_Response;
	[SerializeField]private FirstPersonBradSceneController sceneController;

	private Sprite m_defaultSprite;
	private SpriteRenderer m_SpriteRenderer;
	[SerializeField]private Sprite m_Sprite;

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
		if (Input.GetButtonDown ("Fire1") && over) { //Press A Button
			//make choice
			sceneController.handleResponse(m_Response.text);
		}
	}

	//Handle the Over event
	private void HandleOver()
	{
		over = true;
		m_SpriteRenderer.sprite = m_Sprite;
		m_Background.color = new Color (0F, 0F, 0F, 0.8F);
		this.GetComponent<Transform> ().Translate (0,0,-0.3F);
	}

	//Handle the Out Event
	private void HandleOut()
	{
		over = false;
		m_SpriteRenderer.sprite = m_defaultSprite;
		m_Background.color = new Color (0F, 0F, 0F, 0.518F);
		this.GetComponent<Transform> ().Translate (0,0,0.3F);
	}
}
