using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class AllObject : MonoBehaviour
{
    Vector3 zoomOutScale;
    Vector3 zoomInScale;

    Vector3 zoomInStartPos;
    Vector3 zoomInEndPos;

    Vector3 zoomOutStartPos;
    Vector3 zoomOutEndPos;

    float timeElapsed = 0;
    float animationDuration = 0;
    bool animate = false;
    bool reverse = false;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnActivateQuiz += MoveCameraToRight;
        GameManager.OnDeactivateQuiz += MoveCameraToLeft;
        GameManager.OnStartScene += StartScene;

        zoomInScale = transform.localScale;
        zoomOutScale = transform.localScale * 0.31f;

        zoomInEndPos = transform.position;
        zoomInStartPos = zoomInEndPos;
        zoomInStartPos.x = -20f;

        zoomOutStartPos = transform.position;
        zoomOutStartPos.y = -4.2f;

        transform.localScale = zoomOutScale;
        transform.position = zoomOutStartPos;

        zoomOutEndPos = zoomOutStartPos;
        zoomOutEndPos.y = -56.35f;
    }

    // Update is called once per frame
    void Update()
    {
        if (animate)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed < animationDuration)
            {
                if (!reverse)
                    transform.position = Vector3.Lerp(zoomOutStartPos, zoomOutEndPos, timeElapsed / animationDuration);
                else
                    transform.position = Vector3.Lerp(zoomOutEndPos, zoomOutStartPos, timeElapsed / animationDuration);

            }
            else
            {
                animate = false;
                if (!reverse)
                    transform.position = zoomOutEndPos;
                else
                    transform.position = zoomOutStartPos;

            }
        }
    }

    void MoveCameraToRight()
    {
        transform.position = new Vector2(transform.position.x + GameManager.cameraMoveDistance, transform.position.y);
    }

    void MoveCameraToLeft(int c)
    {
        transform.position = new Vector2(transform.position.x - GameManager.cameraMoveDistance, transform.position.y);
    }

    void StartScene()
    {
        StartCoroutine(StartSceneCoroutine());
    }

    IEnumerator StartSceneCoroutine()
    {
        animationDuration = GameManager.cameraMoveDuration;
        timeElapsed = 0;
        reverse = false;
        animate = true;

        yield return new WaitForSeconds(animationDuration + GameManager.delayStopDuration);

        animationDuration = GameManager.cameraMoveDuration;
        timeElapsed = 0;
        reverse = true;
        animate = true;

        yield return new WaitForSeconds(animationDuration + GameManager.delayStopDuration);
        transform.localScale = zoomInScale;
        transform.position = zoomInStartPos;
    }
}
