using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kampus : MonoBehaviour
{
    Vector3 startPos;
    Vector3 stopPos;

    float timeElapsed = 0;
    float animationDuration = 3f;
    bool animate = false;

    float liftPosX = 20f;
    float kelasPosX = 0f;

    bool walkToClass = false;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnAllAnswersWrong += GetOutFromLift;
        GameManager.OnStartScene += StartScene;
        GameManager.OnMoveToClass += MoveToClass;
    }

    // Update is called once per frame
    void Update()
    {
        if (animate && !GameManager.over)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed < animationDuration)
            {
                transform.localPosition = Vector3.Lerp(startPos, stopPos, timeElapsed / animationDuration);
            }
            else
            {
                animate = false;
                transform.localPosition = stopPos;
                if (walkToClass)
                    GameManager.OnGameOver?.Invoke(true);
            }
        }
    }

    void GetOutFromLift()
    {
        StartCoroutine(GetOutFromLiftCoroutine());
    }

    IEnumerator GetOutFromLiftCoroutine()
    {
        startPos = transform.localPosition;
        stopPos = new Vector2(transform.localPosition.x - 6f, transform.localPosition.y);
        timeElapsed = 0;
        animationDuration = GameManager.getOutAnimationDuration;
        animate = true;

        yield return new WaitForSeconds(animationDuration);

        GameManager.OnLiftEscape?.Invoke();

        yield return new WaitForSeconds(GameManager.liftOpenAnimationDuration * 2 + GameManager.liftEscapeAnimationDuration);

        stopPos = startPos;
        startPos = transform.localPosition;
        timeElapsed = 0;
        animationDuration = GameManager.getOutAnimationDuration;
        animate = true;

        yield return new WaitForSeconds(animationDuration);

        GameManager.OnActivateQuiz?.Invoke();
    }

    void StartScene()
    {
        StartCoroutine(StartSceneCoroutine());
    }

    IEnumerator StartSceneCoroutine()
    {
        yield return new WaitForSeconds(GameManager.cameraMoveDuration * 2 + GameManager.delayStopDuration * 3);

        startPos = transform.localPosition;
        stopPos = transform.localPosition;
        stopPos.x = liftPosX;
        timeElapsed = 0;
        animationDuration = GameManager.characterMoveToLiftDuration;
        animate = true;

        yield return new WaitForSeconds(GameManager.characterMoveToLiftDuration);

        GameManager.OnStartGame?.Invoke();
        GameManager.OnActivateQuiz?.Invoke();
    }

    void MoveToClass()
    {
        walkToClass = true;
        startPos = transform.localPosition;
        stopPos = transform.localPosition;
        stopPos.x = kelasPosX;
        timeElapsed = 0;
        animationDuration = GameManager.characterMoveToLiftDuration;
        animate = true;
    }
}
