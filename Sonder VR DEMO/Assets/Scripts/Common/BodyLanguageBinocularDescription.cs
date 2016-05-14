using UnityEngine;
using System.Collections;

public class BodyLanguageBinocularDescription : MonoBehaviour {

	public static string m_DescriptionText = "";
	[SerializeField] private TextMesh m_TextBox;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		m_TextBox.text = m_DescriptionText;
	}

}
