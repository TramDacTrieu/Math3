using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudAnim : MonoBehaviour
{
    public bool dirLeft;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        var startTime = Time.time;
        Vector2 pos = new Vector2(Random.insideUnitCircle.x * 20, transform.position.y);
        Vector2 posFirst = new Vector2(-20, transform.position.y);
        Vector2 posSec = new Vector2(20, transform.position.y);
        Vector2 pos1 = Vector2.zero;
        Vector2 pos2 = Vector2.zero;
        if (!dirLeft)
        {
            pos1 = posFirst;
            pos2 = posSec;
        }
        else
        {
            pos2 = posFirst;
            pos1 = posSec;
        }
        float speed = Random.Range(2, 5);
        var journeyLength = Vector3.Distance(pos, pos2);
        float fracJourney = 0;
        while (fracJourney < 1)
        {
            float distCovered = (Time.time - startTime) * speed;
            fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(pos, pos2, fracJourney);
            yield return new WaitForEndOfFrame();
        }

        while (true)
        {
            startTime = Time.time;
            journeyLength = Vector3.Distance(pos1, pos2);
            fracJourney = 0;
            while (fracJourney < 1)
            {
                float distCovered = (Time.time - startTime) * speed;
                fracJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(pos1, pos2, fracJourney);
                yield return new WaitForEndOfFrame();
            }

            journeyLength = Vector3.Distance(pos2, pos1);
            fracJourney = 0;
            startTime = Time.time;
            while (fracJourney < 1)
            {
                float distCovered = (Time.time - startTime) * speed;
                fracJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(pos2, pos1, fracJourney);
                yield return new WaitForEndOfFrame();
            }
        }

    }
}
