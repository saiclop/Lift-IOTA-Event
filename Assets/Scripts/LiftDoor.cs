using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class LiftDoor : MonoBehaviour
{
    public GameObject rightDoor;
    public GameObject leftDoor;

    float closeScaleX;
    float openScaleX;

    float leftClosePosX;
    float rightClosePosX;
    float leftOpenPosX;
    float rightOpenPosX;

    bool open = false;
    bool animate = false;
    float timeElapsed = 0;

    int correct;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnDeactivateQuiz += CheckNumOfCorrectAnswer;
        GameManager.OnBeginRound += () => { AnimateDoor(true); };
        GameManager.OnLiftEscape += LiftEscape;

        closeScaleX = rightDoor.transform.localScale.x;
        openScaleX = 0;

        rightDoor.SetActive(false);
        leftDoor.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (animate && !GameManager.over)
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed < GameManager.liftOpenAnimationDuration)
            {
                float scale = 0;
                float leftPos = 0;
                float rightPos = 0;
                if (open)
                {
                    scale = closeScaleX - ((closeScaleX - openScaleX) * timeElapsed / GameManager.liftOpenAnimationDuration);
                    leftPos = leftClosePosX - ((leftClosePosX - leftOpenPosX) * timeElapsed / GameManager.liftOpenAnimationDuration);
                    rightPos = rightClosePosX - ((rightClosePosX - rightOpenPosX) * timeElapsed / GameManager.liftOpenAnimationDuration);
                }
                else
                {
                    scale = openScaleX + ((closeScaleX - openScaleX) * timeElapsed / GameManager.liftOpenAnimationDuration);
                    leftPos = leftOpenPosX + ((leftClosePosX - leftOpenPosX) * timeElapsed / GameManager.liftOpenAnimationDuration);
                    rightPos = rightOpenPosX + ((rightClosePosX - rightOpenPosX) * timeElapsed / GameManager.liftOpenAnimationDuration);
                }
                leftDoor.transform.localScale = new Vector2(scale, leftDoor.transform.localScale.y);
                rightDoor.transform.localScale = new Vector2(scale, rightDoor.transform.localScale.y);

                leftDoor.transform.localPosition = new Vector2(leftPos, leftDoor.transform.localPosition.y);
                rightDoor.transform.localPosition = new Vector2(rightPos, rightDoor.transform.localPosition.y);
            }
            else
            {
                if (open)
                {
                    leftDoor.transform.localScale = new Vector2(openScaleX, leftDoor.transform.localScale.y);
                    rightDoor.transform.localScale = new Vector2(openScaleX, rightDoor.transform.localScale.y);

                    leftDoor.transform.localPosition = new Vector2(leftOpenPosX, leftDoor.transform.localPosition.y);
                    rightDoor.transform.localPosition = new Vector2(rightOpenPosX, rightDoor.transform.localPosition.y);

                    if (GameManager.floor == GameManager.destinationFloor)
                        GameManager.OnMoveToClass?.Invoke();
                    else if (correct > 0)
                        GameManager.OnActivateQuiz?.Invoke();

                }
                else
                {
                    leftDoor.transform.localScale = new Vector2(closeScaleX, leftDoor.transform.localScale.y);
                    rightDoor.transform.localScale = new Vector2(closeScaleX, rightDoor.transform.localScale.y);

                    leftDoor.transform.localPosition = new Vector2(leftClosePosX, leftDoor.transform.localPosition.y);
                    rightDoor.transform.localPosition = new Vector2(rightClosePosX, rightDoor.transform.localPosition.y);

                    if (correct > 0)
                        GameManager.OnFinishRound?.Invoke(correct);
                }

                animate = false;
            }
        }
    }

    void AnimateDoor(bool open)
    {
        if (this.open)
        {
            leftClosePosX = leftDoor.transform.localPosition.x + 1.2f;
            rightClosePosX = rightDoor.transform.localPosition.x - 1.2f;
            leftOpenPosX = leftDoor.transform.localPosition.x;
            rightOpenPosX = rightDoor.transform.localPosition.x;
        }
        else
        {
            leftClosePosX = leftDoor.transform.localPosition.x;
            rightClosePosX = rightDoor.transform.localPosition.x;
            leftOpenPosX = leftDoor.transform.localPosition.x - 1.2f;
            rightOpenPosX = rightDoor.transform.localPosition.x + 1.2f;
        }

        leftDoor.SetActive(true);
        rightDoor.SetActive(true);

        timeElapsed = 0;
        this.open = open;
        animate = true;
    }
    
    void CheckNumOfCorrectAnswer(int correctAnswer)
    {
        correct = correctAnswer;
        if (correctAnswer > 0)
            AnimateDoor(false);
        else
            GameManager.OnAllAnswersWrong?.Invoke();
    }

    void LiftEscape()
    {
        StartCoroutine(LiftEscapeCoroutine());
    }

    IEnumerator LiftEscapeCoroutine()
    {
        AnimateDoor(false);
        
        yield return new WaitForSeconds(GameManager.liftOpenAnimationDuration + GameManager.liftEscapeAnimationDuration);

        AnimateDoor(true);
    }
}
