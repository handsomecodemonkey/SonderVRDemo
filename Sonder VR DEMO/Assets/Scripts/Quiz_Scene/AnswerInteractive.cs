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

		if (Input.GetButtonDown ("Fire1") && over) { //Press A Button
			m_Controller.chooseAnswer(m_Answer.text);
		}

	}

	//Handle the Over event
	private void HandleOver()
	{
		over = true;
		m_SpriteRenderer.sprite = m_Sprite;
		m_Background.color = Color.blue;
	}

	//Handle the Out Event
	private void HandleOut()
	{
		over = false;
		m_SpriteRenderer.sprite = m_defaultSprite;
		m_Background.color = new Color(0.322F,0.443F,0.796F,0.392F);
	}
}
