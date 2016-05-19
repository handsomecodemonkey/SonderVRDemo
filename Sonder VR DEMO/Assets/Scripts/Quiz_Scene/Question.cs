using System;
using System.Collections.Generic;

public class Question {

	private string m_Question;
	private List<string> m_Answers;
	private string m_CorrectAnswer;
	private bool m_Correct;
	private string m_UserSelection;

	public Question(string question, List<string> answers, string correctAnswer){ //Constructor with Dependency injection
		m_Question = question;
		m_Answers = answers;
		m_CorrectAnswer = correctAnswer;
		m_Correct = false;
		m_UserSelection = "";
	}

	//Getters
	public string getQuestion(){
		return m_Question;
	}

	public string getAnswer(int index){
		return m_Answers [index]; //Currently no error checking for bad indexes
	}

	public string getCorrectAnswer(){
		return m_CorrectAnswer;
	}

	public string getResult(){
		if (m_Correct) {
			return "Correct";
		} else {
			return "Incorrect";
		}
	}

}
