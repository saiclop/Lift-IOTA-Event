using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Lift : MonoBehaviour
{
    Vector3 startPos;
    Vector3 stopPos;
    float timeElapsed = 0;
    float animationDuration = 0;
    float totalAnimationDuration = 0;
    bool animate = false;
    bool first = true;
    bool escape = false;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnFinishRound += MoveLift;
        GameManager.OnLiftEscape += LiftEscape;
    }

    // Update is called once per frame
    void Update()
    { 
        if (!GameManager.over)
        {
            if (animate)
            {
                timeElapsed += Time.deltaTime;
                if (first)
                {
                    if (timeElapsed < animationDuration)
                    {
                        transform.position = Vector2.Lerp(startPos, stopPos, timeElapsed / animationDuration);
                    }
                    else
                    {
                        transform.position = stopPos;
                        first = false;
                        animate = false;
                    }
                }
                else if (GameManager.floor == GameManager.destinationFloor)
                {
                    if (timeElapsed > totalAnimationDuration)
                    {
                        transform.position = stopPos;
                        animate = false;
                    }
                    else if (timeElapsed > (totalAnimationDuration - animationDuration))
                    {
                        transform.position = Vector2.Lerp(startPos, stopPos, (timeElapsed - totalAnimationDuration + animationDuration) / animationDuration);
                    }
                }
            }
            else if (escape)
            {
                timeElapsed += Time.deltaTime;
                if (timeElapsed < animationDuration)
                {
                    transform.position = Vector2.Lerp(startPos, stopPos, timeElapsed / animationDuration);
                }
                else
                {
                    transform.position = stopPos;
                    escape = false;
                }
            }
        }
    }

    void MoveLift(int correctAnswer)
    {
        if (correctAnswer > 0)
        {
            if (first || GameManager.floor == GameManager.destinationFloor)
            {
                totalAnimationDuration = GameManager.durationMovePerLevel * correctAnswer;

                float distance = 1.175f;
                animationDuration = GameManager.durationMovePerLevel / 5f;

                startPos = transform.position;
                stopPos = new Vector2(transform.position.x, transform.position.y + distance);

                timeElapsed = 0;
                animate = true;
            }
        }
    }

    void LiftEscape()
    {
        StartCoroutine(LiftEscapeCoroutine());
    }

    IEnumerator LiftEscapeCoroutine()
    {
        yield return new WaitForSeconds(GameManager.liftOpenAnimationDuration);

        startPos = transform.position;
        if (GameManager.floor < (GameManager.destinationFloor - 1))
            stopPos = new Vector2(transform.position.x, transform.position.y + GameManager.distancePerLevel * 2);
        else
            stopPos = new Vector2(transform.position.x, transform.position.y - GameManager.distancePerLevel * 2);
        timeElapsed = 0;
        animationDuration = 2 * GameManager.durationMovePerLevel;
        escape = true;

        yield return new WaitForSeconds(GameManager.liftEscapeAnimationDuration - animationDuration);

        stopPos = startPos;
        startPos = transform.position;
        timeElapsed = 0;
        animationDuration = 2 * GameManager.durationMovePerLevel;
        escape = true;
    }
}