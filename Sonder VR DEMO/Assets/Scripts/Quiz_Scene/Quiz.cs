using System;
using System.Collections.Generic;

public class Quiz {

	private List<Question> m_QuestionList;
	public int m_NumCorrect;
	public int m_NumIncorrect;
	public int m_TotalNumQuestions;
	public int m_NumQuestionsLeft;
		
	public Quiz () { //Constructor
		m_QuestionList = new List<Question>();
		m_NumCorrect = 0;
		m_NumIncorrect = 0;
		m_TotalNumQuestions = 0;
		m_NumQuestionsLeft = 0;
	}

	public void addQuestion(Question q){
		m_QuestionList.Add (q);
		m_TotalNumQuestions++;
		m_NumQuestionsLeft++;
	}

	public Question getNextQuestion(){ //Returns the next question in sequence
		Question returnQuestion = m_QuestionList [0];
		m_QuestionList.RemoveAt(0);
		m_NumQuestionsLeft--;
		return returnQuestion;
	}

	public Question getRandomQuestion(){ //Returns a random question
		//Get random index from 0 till range of questions
		Random rand = new Random();
		int index = rand.Next (m_QuestionList.Count);

		//remove the question from the list using index
		Question returnQuestion = m_QuestionList [index];
		m_QuestionList.RemoveAt(index);

		//return the question
		m_NumQuestionsLeft--;
		return returnQuestion;
	}

	//These functions below are used for analytics later
	public void exportResults(){
	}

	public void printResults(){
	}

}

