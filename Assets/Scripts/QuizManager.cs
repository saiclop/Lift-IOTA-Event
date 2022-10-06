using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [SerializeField] Quiz[] quiz;

    public GameObject quizPanel;
    public GameObject quizUI;
    public Button trueButton;
    public Button falseButton;

    public Image revealAnswerImage;
    public Sprite correctSprite;
    public Sprite wrongSprite;

    TextMeshProUGUI questionText;
    List<Quiz> quizList = new List<Quiz>();

    int round = 0;
    int correctAnswer = 0;

    private void Start()
    {
        GameManager.OnActivateQuiz += ActivateQuiz;
        GameManager.OnDeactivateQuiz += DeactivateQuiz;

        RandomizeQuestion();

        questionText = quizUI.GetComponentInChildren<TextMeshProUGUI>();

        trueButton.onClick.AddListener(delegate { if (!GameManager.over) Answer(true); });
        falseButton.onClick.AddListener(delegate { if (!GameManager.over) Answer(false); });
    }

    private void Update()
    {
        
    }

    void RandomizeQuestion()
    {
        List<int> randomIndex = new List<int>();
        for (int i = 0; i < quiz.Length; i++)
        {
            randomIndex.Add(i);
        }

        for (int i = 0; i < quiz.Length; i++)
        {
            int idx = Random.Range(0, randomIndex.Count);
            quizList.Add(quiz[randomIndex[idx]]);
            randomIndex.RemoveAt(idx);
        }
    }

    void Answer(bool answer)
    {
        if (answer == quizList[0].answer)
        {
            revealAnswerImage.sprite = correctSprite;
            correctAnswer++;
        }
        else
        {
            revealAnswerImage.sprite = wrongSprite;
            quizList.Add(quizList[0]);
        }

        quizList.RemoveAt(0);

        StartCoroutine(AnsweredCoroutine());
    }

    IEnumerator AnsweredCoroutine()
    {
        quizUI.SetActive(false);
        revealAnswerImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        revealAnswerImage.gameObject.SetActive(false);
        round++;

        if (round == GameManager.roundPerTurn || quizList.Count == 0)
        {
            GameManager.floor += correctAnswer;

            if (correctAnswer > 0)
            {
                GameManager.OnPopUpMessage?.Invoke("Berhasil Naik " + correctAnswer.ToString() + " Lantai", 1f);
            }
            else
            {
                GameManager.OnPopUpMessage?.Invoke("Gagal Naik Lantai", 1f);
            }

            GameManager.OnDeactivateQuiz?.Invoke(correctAnswer);

            round = 0;
            correctAnswer = 0;
        }
        else
        {
            questionText.SetText(quizList[0].question);
        }

        quizUI.SetActive(true);    
    }

    void ActivateQuiz()
    {
        questionText.SetText(quizList[0].question);
        quizPanel.SetActive(true);
    }

    void DeactivateQuiz(int correctAnswer)
    {
        quizPanel.SetActive(false);
    }
}