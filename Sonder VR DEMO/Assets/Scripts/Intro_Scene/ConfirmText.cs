using UnityEngine;
using System.Collections;

public class ConfirmText : MonoBehaviour {

	[SerializeField] private TextAsset m_TextFile;
	[SerializeField] private string[] m_TextLines;
	[SerializeField] private GameObject m_TextBox;
	private TextMesh m_TextMesh;
	private int lineNumber = 0;

	// Use this for initialization
	void Start () {

		if (m_TextFile != null) {
			m_TextLines = m_TextFile.text.Split ('\n');
		}
			
		m_TextMesh = m_TextBox.GetComponent<TextMesh> ();
		m_TextMesh.text = m_TextLines [lineNumber];
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp ("up")) {
			m_TextMesh.text = m_TextLines [++lineNumber];
		}
	}

}
