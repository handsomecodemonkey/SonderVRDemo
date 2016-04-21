using UnityEngine;
using System.Collections;

public class TextBoxManagerScript : MonoBehaviour {

	[SerializeField] private TextAsset m_TextFile;
	[SerializeField] private string[] m_TextLines;

	// Use this for initialization
	void Start () {
		if (m_TextFile != null) {
			m_TextLines = m_TextFile.text.Split ('\n');
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
