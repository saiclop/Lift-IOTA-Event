using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int floor = 1;
    public static int destinationFloor = 26;
    public static int roundPerTurn = 5;

    public static float cameraMoveDistance = 4.45f;
    public static float durationMovePerLevel = 0.5f;
    public static float distancePerLevel = 7.71f;

    static float pinaltyDuration = 10f;
    public static float getOutAnimationDuration = 1f;
    public static float liftOpenAnimationDuration = 0.5f;
    public static float liftEscapeAnimationDuration = pinaltyDuration - (2 * getOutAnimationDuration) - (2 * liftOpenAnimationDuration);

    // Start scene
    public static float cameraMoveDuration = 2f;
    public static float delayStopDuration = 0.5f;
    public static float characterMoveToLiftDuration = 3f;

    public static bool over = false;

    public static Action<string, float> OnPopUpMessage;
    public static Action OnActivateQuiz;
    public static Action<int> OnDeactivateQuiz;
    public static Action<int> OnFinishRound;
    public static Action OnBeginRound;
    public static Action OnAllAnswersWrong;
    public static Action OnLiftEscape;
    public static Action OnStartScene;
    public static Action OnStartGame;
    public static Action<bool> OnGameOver;
    public static Action OnMoveToClass;

    public GameObject mainCanvas;
    public TextMeshProUGUI timerText;
    public GameObject popUpUI;
    public Slider slider;

    float gameTime = 240;
    string sMinutes, sSeconds;

    bool start = false;

    TextMeshProUGUI popUpText;

    float timeElapsed = 0;
    float animationDuration = 0;
    bool animateSlider = false;
    float sliderStartValue = 0;
    float sliderStopValue = 0;

    // Start is called before the first frame update
    void Start()
    {
        popUpText = popUpUI.GetComponentInChildren<TextMeshProUGUI>();
        OnPopUpMessage += PopUpMessage;
        OnFinishRound += MoveSlider;
        OnStartGame += StartGame;
        OnGameOver += GameOver;

        slider.value = 0;

        StartCoroutine(StartSceneCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (start && !over)
        {
            gameTime -= Time.deltaTime;
            int minutes = (int)gameTime / 60;
            int seconds = (int)gameTime % 60;
            sMinutes = "0" + minutes.ToString();
            if (seconds >= 10)
                sSeconds = seconds.ToString();
            else
                sSeconds = "0" + seconds.ToString();

            if (gameTime > 0)
                timerText.SetText(sMinutes + ":" + sSeconds);
            else
                OnGameOver?.Invoke(false);
        }

        if(animateSlider && !over)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed < animationDuration)
            {
                slider.value = sliderStartValue + ((sliderStopValue - sliderStartValue) * timeElapsed / animationDuration);
            }
            else
            {
                animateSlider = false;
                slider.value = sliderStopValue;
            }
        }
    }

    void PopUpMessage(string message, float seconds)
    {
        StartCoroutine(PopUpMessageCoroutine(message, seconds));
    }

    IEnumerator PopUpMessageCoroutine(string message, float seconds)
    {
        popUpText.text = message;
        popUpUI.SetActive(true);
        yield return new WaitForSeconds(seconds);
        popUpUI.SetActive(false);
    }

    void MoveSlider(int correctAnswer)
    {
        if (correctAnswer > 0)
        {
            sliderStartValue = slider.value;
            sliderStopValue = (floor - 1) * 1f / (destinationFloor - 1) * 1f;
            animationDuration = correctAnswer * durationMovePerLevel;
            timeElapsed = 0;
            animateSlider = true;
        }
    }

    IEnumerator StartSceneCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        OnStartScene?.Invoke();
    }

    void StartGame()
    {
        start = true;
        mainCanvas.SetActive(true);
    }

    void GameOver(bool win)
    {
        over = true;
        StopAllCoroutines();

        if (win)
        {
            popUpText.SetText("Kamu Berhasil Masuk Kelas!");
        }
        else
        {
            popUpText.SetText("Kamu Terlambat Masuk Kelas!");
        }

        popUpUI.SetActive(true);
    }
}
