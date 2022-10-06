using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Gedung : MonoBehaviour
{
    Vector3 startPos;
    Vector3 stopPos;
    float timeElapsed = 0;
    float animationDuration = 0;
    float pendingAnimationDuration = 0;
    bool animate = false;
    bool first = true;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnFinishRound += MoveGedung;
    }

    // Update is called once per frame
    void Update()
    {
        if (animate && !GameManager.over)
        {
            timeElapsed += Time.deltaTime;
            if (first)
            {
                if (timeElapsed > (pendingAnimationDuration + animationDuration))
                {
                    transform.position = stopPos;
                    first = false;
                    animate = false;

                    GameManager.OnBeginRound?.Invoke();
                }
                else if (timeElapsed > pendingAnimationDuration)
                {
                    transform.position = Vector2.Lerp(startPos, stopPos, (timeElapsed - pendingAnimationDuration) / animationDuration);
                }
            }
            else
            {
                if (timeElapsed < animationDuration)
                {
                    transform.position = Vector2.Lerp(startPos, stopPos, timeElapsed / animationDuration);
                }
                else
                {
                    transform.position = stopPos;
                    animate = false;

                    GameManager.OnBeginRound?.Invoke();
                }
            }
        }
    }

    void MoveGedung(int correctAnswer)
    {
        if(correctAnswer > 0)
        {
            startPos = transform.position;
            stopPos = new Vector2(transform.position.x, transform.position.y - (GameManager.distancePerLevel * correctAnswer));
            animationDuration = GameManager.durationMovePerLevel * correctAnswer;

            if (first || GameManager.floor == GameManager.destinationFloor)
            {
                float distance = 1.175f;
                pendingAnimationDuration = GameManager.durationMovePerLevel / 5f;

                stopPos.y += distance;

                animationDuration -= pendingAnimationDuration;
            }

            timeElapsed = 0;
            animate = true;
        }
    }
}
