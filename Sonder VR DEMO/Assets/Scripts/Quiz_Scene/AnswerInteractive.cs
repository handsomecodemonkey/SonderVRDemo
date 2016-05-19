using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnswerInteractive : VRInteractiveItem {

	[SerializeField]private GameObject m_Reticle;
	[SerializeField]private Image m_Background;
	[SerializeField]private Text m_Answer;
	[SerializeField]private QuizController m_Controller;

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

		if (Input.GetButtonDown ("Fire1") && over && m_Answer.text != "") { //Press A Button
			m_Controller.chooseAnswer(m_Answer.text);
		}

	}

	//Handle the Over event
	private void HandleOver()
	{
		over = true;
		if (m_Answer.text != "") {
			m_SpriteRenderer.sprite = m_Sprite;
			m_Background.color = new Color (0.129F, 0.588F, 0.953F);
			this.GetComponent<Transform> ().Translate (0,0,-0.3F);
		}
	}

	//Handle the Out Event
	private void HandleOut()
	{
		over = false;
		m_SpriteRenderer.sprite = m_defaultSprite;
		m_Background.color = new Color(0.051F,0.278F,0.631F);
		this.GetComponent<Transform> ().Translate (0,0,0.3F);
	}
}
