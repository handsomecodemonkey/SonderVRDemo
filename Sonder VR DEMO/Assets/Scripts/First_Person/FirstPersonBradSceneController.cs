using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FirstPersonBradSceneController : MonoBehaviour {

	[SerializeField]WaterFountainScripts waterFountain;

	//Kelsey's variables
	[SerializeField]private KelseyBehavior kelsey;
	[SerializeField]private Vector3 targetOne;
	[SerializeField]private Vector3 bathroom;
	[SerializeField]private Text kelseysBLDescription;

	//Dialogue Choices
	[SerializeField]DialogueChoice kelseysDialogueResponse;
	[SerializeField]DialogueChoice bradDialogueResponse;

	// Use this for initialization
	void Start () {
		kelseysDialogueResponse.gameObject.SetActive (false);
		bradDialogueResponse.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void sendKelseyToPositionOne(){
		kelsey.setAnimation ("Run");
		kelsey.goToTarget (targetOne);
		StartCoroutine (startDialogue ());
	}

	IEnumerator startDialogue(){
		yield return new WaitForSeconds (2F);
		kelsey.setAnimation ("Idle");

		//Start dialogue
		kelsey.audioSource.clip = kelsey.audioClips[0];
		kelsey.audioSource.Play ();

		yield return new WaitForSeconds (3F);

		kelseysBLDescription.text = "Name: Kelsey\nBody Language: Lost, she is asking you where the bathroom is\nMood: Anxious";

		//Message bubbles will pop up
		kelseysDialogueResponse.gameObject.SetActive (true);
		kelseysDialogueResponse.m_Response.text = "The bathroom is about 10 feet to your left";

		bradDialogueResponse.gameObject.SetActive (true);
		bradDialogueResponse.m_Response.text = "*Point to the bathroom*";
	}

	IEnumerator SecondDialogue(){
		yield return new WaitForSeconds (3F);
		kelsey.audioSource.clip = kelsey.audioClips[1];
		kelsey.audioSource.Play ();
		kelsey.setAnimation ("Stun");
		kelseysBLDescription.text = "Name: Kelsey\nBody Language: Lost, she is asking you where the bathroom is\nMood: Alone";

		yield return new WaitForSeconds (3.5F);

		bradDialogueResponse.gameObject.SetActive (true);
		bradDialogueResponse.m_Response.text = "*Keep pointing to the bathroom*";
	}

	IEnumerator ThirdDialogue(){
		yield return new WaitForSeconds (3F);
		kelsey.audioSource.clip = kelsey.audioClips[2];
		kelsey.audioSource.Play ();
		kelsey.setAnimation ("Win");
		kelseysBLDescription.text = "Name: Kelsey\nBody Language: She is crying because you ignored her\nMood: Sad";

		yield return new WaitForSeconds (3F);

		bradDialogueResponse.gameObject.SetActive (true);
		bradDialogueResponse.m_Response.text = "She's crying, you should turn around";
	}

	IEnumerator EndDialogue(){
		yield return new WaitForSeconds (2F);
		kelsey.setAnimation ("Idle");
		kelsey.audioSource.clip = kelsey.audioClips[4];
		kelsey.audioSource.Play ();
		kelseysBLDescription.text = "Name: Kelsey\nBody Language: She is happy you answered her question\nMood: Grateful/Happy";

		yield return new WaitForSeconds (2F);
		kelsey.setAnimation ("Run");
		kelsey.goToTarget (bathroom);

		yield return new WaitForSeconds (2F);
		SceneManager.LoadScene (2); // Hardcoded to the main menu
	}

	public void handleResponse(string response){ //This function is for handling the user dialogue responses

		switch (response) {
		case "The bathroom is about 10 feet to your left":
			kelseysDialogueResponse.gameObject.SetActive (false);
			StartCoroutine (EndDialogue ());
			break;
		case "*Point to the bathroom*":
			bradDialogueResponse.gameObject.SetActive (false);
			StartCoroutine (SecondDialogue());
			break;
		case "*Keep pointing to the bathroom*":
			bradDialogueResponse.gameObject.SetActive (false);
			StartCoroutine (ThirdDialogue());
			break;
		default:
			break;
		}
	}

}
