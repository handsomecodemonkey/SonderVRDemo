using UnityEngine;
using System.Collections;

public class WaterFountainScripts : VRInteractiveItem {

	[SerializeField]FirstPersonBradSceneController sceneController;

	[SerializeField]
	private ParticleSystem water;

	private AudioSource audio;

	[SerializeField]private GameObject m_Reticle;
	[SerializeField]private Sprite m_Sprite;

	private Sprite m_defaultSprite;
	private SpriteRenderer m_SpriteRenderer;
	private bool over = false;

	public bool waterOn = false;
	private int numTimesOn = 0;

	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource> ();
		this.OnOver += HandleOver;
		this.OnOut += HandleOut;

		m_SpriteRenderer = m_Reticle.GetComponent<SpriteRenderer> ();
		m_defaultSprite = m_SpriteRenderer.sprite;
	}
	
	// Update is called once per frame
	void Update () {

		if (over && Input.GetButtonDown("Fire1")) {
			if (!water.isPlaying) {
				audio.volume = 1;
				water.Play ();
				waterOn = true;
				if (++numTimesOn == 1) {
					sceneController.sendKelseyToPositionOne ();
				}
			} else {
				audio.volume = 0;
				water.Stop ();
				waterOn = false;
			}
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
