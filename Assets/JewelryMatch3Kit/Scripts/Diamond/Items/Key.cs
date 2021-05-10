using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Key : MonoBehaviour
{
    public static List<Lock> targets = new List<Lock>();
    public ParticleSystem ParticleSystem;
    public void StartKey()
    {
        ParticleSystem.Play();
        Lock target = FindLock();
        targets.Add(target);
        if (target == null) OnFinished(null);
        StartCoroutine(MoveToLock(target.gameObject));
        // iTween.MoveTo(gameObject, iTween.Hash("position", target.transform.position, "time", 1, "oncomplete", "OnFinished", "oncompleteparams", target));
    }

    private static Lock FindLock()
    {
        return FindObjectsOfType<Lock>().Where(i => i.opened == false && !targets.Any(Target => Target == i)).FirstOrDefault();
    }

    void OnFinished(GameObject target)
    {
        target?.GetComponent<Lock>().Open();
        gameObject.SetActive(false);

    }


    IEnumerator MoveToLock(GameObject target)
    {
        AnimationCurve curveX = new AnimationCurve(new Keyframe(0, transform.position.x), new Keyframe(0.4f, target.transform.position.x));
        AnimationCurve curveY = new AnimationCurve(new Keyframe(0, transform.position.y), new Keyframe(0.5f, target.transform.position.y));
        curveY.AddKey(0.2f, transform.position.y + UnityEngine.Random.Range(-2, 0.5f));
        float startTime = Time.time;
        Vector3 startPos = transform.position;
        float speed = UnityEngine.Random.Range(0.4f, 0.6f);
        float distCovered = 0;
        while (distCovered < 0.5f)
        {
            distCovered = (Time.time - startTime) * speed;
            transform.position = new Vector3(curveX.Evaluate(distCovered), curveY.Evaluate(distCovered), 0);
            transform.Rotate(Vector3.back, Time.deltaTime * 1000);
            yield return new WaitForFixedUpdate();
        }

        OnFinished(target);
    }
}
