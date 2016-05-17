using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class QuizController : MonoBehaviour {

	private Quiz m_Quiz;

	//View Objects (Game objects in use)
	[SerializeField]private Text QuestionText;
	[SerializeField]private Image CorrectImage;
	[SerializeField]private Image IncorrectImage;
	[SerializeField]private Text Answer1Text;
	[SerializeField]private Text Answer2Text;
	[SerializeField]private Text Answer3Text;
	[SerializeField]private Text Answer4Text;
	[SerializeField]private Text ResultsText;
	[SerializeField]private int m_SceneToOpen;

	private Question m_CurrentQuestion;
	private bool QuizOver = false;

	// Use this for initialization
	void Start () {

		//Question Initialization currently hard-coded, eventually refactor into a database of some sort and use a while loop

		//Question one
		string questionOneText = "Who was the story about? Select one.";
		List<string> questionOneAnswers = new List<string> ();
		questionOneAnswers.Add ("Brad");
		questionOneAnswers.Add ("Spencer");
		questionOneAnswers.Add ("Victor");
		questionOneAnswers.Add ("Steve");
		string questionOneCorrectAnswer = "Brad";

		Question questionOne = new Question (questionOneText, questionOneAnswers, questionOneCorrectAnswer);

		//Question two
		string questionTwoText = "What was his problem? Select one.";
		List<string> questionTwoAnswers = new List<string> ();
		questionTwoAnswers.Add ("Talking too loudly");
		questionTwoAnswers.Add ("Not looking at the speaker");
		questionTwoAnswers.Add ("Getting too close");
		questionTwoAnswers.Add ("Staring too long");
		string questionTwoCorrectAnswer = "Not looking at the speaker";

		Question questionTwo = new Question (questionTwoText, questionTwoAnswers, questionTwoCorrectAnswer);

		//Question three
		string questionThreeText = "What did he learn to do about it? Select one.";
		List<string> questionThreeAnswers = new List<string> ();
		questionThreeAnswers.Add ("Stop Staring");
		questionThreeAnswers.Add ("Stand Back");
		questionThreeAnswers.Add ("Speak Softly");
		questionThreeAnswers.Add ("Look at the speaker");
		string questionThreeCorrectAnswer = "Look at the speaker";

		Question questionThree = new Question (questionThreeText, questionThreeAnswers, questionThreeCorrectAnswer);

		//Question four
		string questionFourText = "What tool did Soda give you this lesson? Select one.";
		List<string> questionFourAnswers = new List<string> ();
		questionFourAnswers.Add ("Compass");
		questionFourAnswers.Add ("Remote");
		questionFourAnswers.Add ("Binoculars");
		questionFourAnswers.Add ("Volume Meter");
		string questionFourCorrectAnswer = "Binoculars";

		Question questionFour = new Question (questionFourText, questionFourAnswers, questionFourCorrectAnswer);

		//Put all the questions into the Quiz
		m_Quiz = new Quiz();
		m_Quiz.addQuestion (questionOne);
		m_Quiz.addQuestion (questionTwo);
		m_Quiz.addQuestion (questionThree);
		m_Quiz.addQuestion (questionFour);

		CorrectImage.enabled = false;
		IncorrectImage.enabled = false;
		NextQuestion ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Fire1") && QuizOver == true) { //Press A Button
			SceneManager.LoadScene (m_SceneToOpen);
		}
	}

	private void NextQuestion(){
		m_CurrentQuestion = m_Quiz.getNextQuestion ();
		QuestionText.text = m_CurrentQuestion.getQuestion ();
		Answer1Text.text = m_CurrentQuestion.getAnswer (0);
		Answer2Text.text = m_CurrentQuestion.getAnswer (1);
		Answer3Text.text = m_CurrentQuestion.getAnswer (2);
		Answer4Text.text = m_CurrentQuestion.getAnswer (3);
	}

	public void EndQuiz(){
		QuestionText.text = "";
		Answer1Text.text = "";
		Answer2Text.text = "";
		Answer3Text.text = "";
		Answer4Text.text = "";

		//Show results
		ResultsText.enabled = true;
		ResultsText.text = m_Quiz.getResults () + "Press A to go back to menu";
		QuizOver = true;
	}

	public void chooseAnswer(string answer){
		if (answer == m_CurrentQuestion.getCorrectAnswer()) {
			m_Quiz.m_NumCorrect++;
			StartCoroutine (showCorrect());
		} else {
			m_Quiz.m_NumIncorrect++;
			StartCoroutine (showIncorrect());
		}
	}

	private void advanceQuiz(){
		if (m_Quiz.m_NumQuestionsLeft != 0) {
			NextQuestion ();
		} else {
			EndQuiz ();
		}
	}

	private IEnumerator showCorrect(){
		CorrectImage.enabled = true;
		CorrectImage.GetComponent<AudioSource> ().Play ();
		yield return new WaitForSeconds (1F);
		CorrectImage.enabled = false;
		advanceQuiz ();
	}

	private IEnumerator showIncorrect(){
		IncorrectImage.enabled = true;
		IncorrectImage.GetComponent<AudioSource> ().Play ();
		yield return new WaitForSeconds (1F);
		IncorrectImage.enabled = false;
		advanceQuiz ();
	}

}
