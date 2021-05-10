using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatorHelper : MonoBehaviour
{
    public static AnimatorHelper Instance;

    public delegate void WhateverType();
    // declare delegate type
    protected WhateverType callbackFct;
    // to store the function

    void Start()
    {
        Instance = this;
    }

    public void Move(GameObject obj, Vector3 startPos, Vector3 endPos, float speed, WhateverType myCallback)
    {
        StartCoroutine(MoveCor(obj, startPos, endPos, speed, myCallback));
    }

    IEnumerator MoveCor(GameObject obj, Vector3 startPos, Vector3 endPos, float speed, WhateverType myCallback)
    {
        float startTime = Time.time;
        if (LevelManager.THIS.gameStatus == GameState.PreWinAnimations)
            speed = 10;
        float distance = Vector3.Distance(startPos, endPos);
        float fracJourney = 0;
        if (distance > 0.5f)
        {
            while (fracJourney < 1)
            {
                // speed += 0.2f;
                float distCovered = (Time.time - startTime) * speed;
                fracJourney = distCovered / distance;
                obj.transform.position = Vector2.Lerp(startPos, endPos, fracJourney);
                print(obj.transform.position);
                yield return new WaitForFixedUpdate();

            }
        }

        callbackFct = myCallback;
        callbackFct();

    }

    public void Scale(GameObject obj, Vector3 startPos, Vector3 endPos, float speed, WhateverType myCallback)
    {
        StartCoroutine(MoveCor(obj, startPos, endPos, speed, myCallback));
    }


    IEnumerator ScaleCor(GameObject obj, Vector3 startPos, Vector3 endPos, float speed, WhateverType myCallback)
    {
        float startTime = Time.time;
        if (LevelManager.THIS.gameStatus == GameState.PreWinAnimations)
            speed = 10;
        float distance = Vector3.Distance(startPos, endPos);
        float fracJourney = 0;
        if (distance > 0.5f)
        {
            while (fracJourney < 1)
            {
                // speed += 0.2f;
                float distCovered = (Time.time - startTime) * speed;
                fracJourney = distCovered / distance;
                obj.transform.position = Vector2.Lerp(startPos, endPos, fracJourney);
                print(obj.transform.position);
                yield return new WaitForFixedUpdate();

            }
        }

        callbackFct = myCallback;
        callbackFct();

    }

    public void Shake(GameObject obj, WhateverType myCallback)
    {
        StartCoroutine(ShakeCor(obj, myCallback));
    }

    IEnumerator ShakeCor(GameObject obj, WhateverType myCallback)
    {
        float startTime = Time.time;
        Vector3 originPosition = obj.transform.position;
        Quaternion originRotation = obj.transform.rotation;
        float shake_intensity = 0.3f;
        float shake_decay = 0.002f;
        while (Time.time - startTime < 2)
        {
            obj.transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
            obj.transform.rotation = Quaternion.Euler(
                originRotation.x + Random.Range(-shake_intensity, shake_intensity) * 0.2f,
                originRotation.y + Random.Range(-shake_intensity, shake_intensity) * 0.2f,
                originRotation.z + Random.Range(-shake_intensity, shake_intensity) * 0.2f);
            shake_intensity -= shake_decay;
            yield return new WaitForFixedUpdate();
        }
        callbackFct = myCallback;
        callbackFct();

    }


}