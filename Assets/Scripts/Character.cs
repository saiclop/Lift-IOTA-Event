using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Character : MonoBehaviour
{
    public GameObject lift;
    
    float timeElapsed = 0;
    float animationDuration = 0;
    bool animate = false;
    Vector3 distanceToLift;

    Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnFinishRound += MoveWithLift;

        startPos = transform.localPosition;
        startPos.x = 20f;

        transform.localPosition = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (animate && !GameManager.over)
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed < animationDuration)
            {
                transform.position = lift.transform.position - distanceToLift;
            }
            else
            {
                transform.position = lift.transform.position - distanceToLift;
                animate = false;
            }
        }
    }

    void MoveWithLift(int correctAnswers)
    {
        if(correctAnswers > 0)
        {
            if(GameManager.floor - correctAnswers == 1 || GameManager.floor == GameManager.destinationFloor)
            {
                animationDuration = GameManager.durationMovePerLevel * correctAnswers;
                distanceToLift = lift.transform.position - transform.position;

                timeElapsed = 0;
                animate = true;
            }
        }
    }

}
