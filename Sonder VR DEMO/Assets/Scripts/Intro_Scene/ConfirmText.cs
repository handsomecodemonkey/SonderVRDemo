using UnityEngine;
using System.Collections;

public class ConfirmText : MonoBehaviour {

	[SerializeField] private TextAsset m_TextFile;
	[SerializeField] private string[] m_TextLines;
	[SerializeField] private GameObject m_TextBox;
	private TextMesh m_TextMesh;
	private int lineNumber = 0;

	private bool binoculars;

	[SerializeField] GameObject hiddenBinoculars;

	// Use this for initialization
	void Start () {

		BodyLanguageBinoculars.activated = false; //Start by leaving this setting off

		if (m_TextFile != null) {
			m_TextLines = m_TextFile.text.Split ('\n');
		}
			
		m_TextMesh = m_TextBox.GetComponent<TextMesh> ();
		m_TextMesh.text = m_TextLines [lineNumber];
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1")) { //Fire1 Maps to the left mouse click or A button on controller by default
			m_TextMesh.text = m_TextLines [++lineNumber];

			switch (lineNumber)
			{
			case 4:
				hiddenBinoculars.layer = 1;
				break;
			case 6:
				BodyLanguageBinoculars.activated = true; //Turns on the body language binoculars for the intro scene
				hiddenBinoculars.layer = 8;
				break;
			default:
				break;
			}

		} else if (Input.GetButtonDown("Fire2")) { //Mps to the B button on the controller, and right mouse button
			m_TextMesh.text = m_TextLines [lineNumber == 0 ? 0 : --lineNumber];
			switch (lineNumber)
			{
			case 4:
				hiddenBinoculars.layer = 8;
				break;
			case 5:
				BodyLanguageBinoculars.activated = false; 
				break;
			default:
				break;
			}
		}

	}

}
