using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class WaitWhileFall : CustomYieldInstruction
{
    private IHighlightableComponent[] items;

    public override bool keepWaiting
    {
        get
        {
            var ii = items.WhereNotNull().Any(i => i.Rigidbody2D?.velocity.magnitude > 1);
            return ii;
        }
    }

    public WaitWhileFall()
    {
        items = GameObject.FindObjectsOfType<IHighlightableComponent>().Where(i => i.selectable).ToArray();
    }
}


public class WaitCurves : CustomYieldInstruction
{
    private CurveYield[] actionList;

    int finished;

    public override bool keepWaiting
    {
        get
        {
            var ii = finished < actionList.Length;
            if (!ii)
            {
                foreach (var action in actionList)
                {
                    action.OnFinished -= OnFinished;
                }
            }
            return ii;
        }
    }

    IEnumerator WaitCurvesCor()
    {
        foreach (var action in actionList)
        {
            yield return action;
        }
    }

    void OnFinished()
    {
        finished++;
    }

    public WaitCurves(CurveYield[] yields)
    {
        actionList = yields;
        foreach (var action in actionList)
        {
            action.OnFinished += OnFinished;
        }
    }
}

public class WaitCurve : CurveYield
{
    private float lastStep;

    public WaitCurve(MonoBehaviour mono, AnimationCurve curveX, AnimationCurve curveY, Action<Vector2> update, Func<float, bool> stepCondition = null, Action stepCallback = null, Action onFinished = null)
    {
        mono.StartCoroutine(WaitCurveCor(curveX, curveY, update, stepCondition, stepCallback, onFinished));
    }

    IEnumerator WaitCurveCor(AnimationCurve curveX, AnimationCurve curveY, Action<Vector2> update, Func<float, bool> stepCondition = null, Action stepCallback = null, Action onFinished = null)
    {
        var startTime = Time.time;
        var pos1 = new Vector2(curveX.keys[0].value, curveY.keys[0].value);
        Keyframe keyframeLastX = curveX.keys[curveX.keys.Length - 1];
        Keyframe keyframeLastY = curveY.keys[curveY.keys.Length - 1];
        var pos2 = new Vector2(keyframeLastX.value, keyframeLastY.value);
        float duration = Mathf.Max(keyframeLastX.time, keyframeLastY.time);
        var journeyLength = Vector3.Distance(pos1, pos2);
        float speed = journeyLength / duration;
        float timePassed = 0;
        while (startTime + duration >= Time.time)
        {
            if (stepCondition != null)
            {
                if (lastStep == 0 && stepCondition(timePassed))
                {
                    lastStep = timePassed;
                    stepCallback?.Invoke();
                }
            }
            // float distCovered = (Time.time - startTime) * speed;
            timePassed += Time.deltaTime;
            if (!float.IsNaN(timePassed) && !float.IsNaN(curveX.Evaluate(timePassed)) && !float.IsNaN(curveY.Evaluate(timePassed)))
                update(new Vector2(curveX.Evaluate(timePassed), curveY.Evaluate(timePassed)));
            yield return new WaitForEndOfFrame();
        }
        stopWait = false;
        onFinished?.Invoke();
        Finished();
    }
}

public class WaitNoCurve : CurveYield
{
    public WaitNoCurve(MonoBehaviour mono, Vector2 v1, Vector2 v2, float duration, Action<Vector2> update)
    {
        var curveX = new AnimationCurve(new Keyframe(0, v1.x), new Keyframe(duration, v2.x));
        var curveY = new AnimationCurve(new Keyframe(0, v1.y), new Keyframe(duration, v2.y));
        mono.StartCoroutine(WaitNoCurveCor(mono, curveX, curveY, update));
    }

    IEnumerator WaitNoCurveCor(MonoBehaviour mono, AnimationCurve curveX, AnimationCurve curveY, Action<Vector2> update)
    {
        yield return new WaitCurve(mono, curveX, curveY, update);
        stopWait = false;
        Finished();
    }
}

public class CurveYield : CustomYieldInstruction
{
    public delegate void DelegateEvent();
    public event DelegateEvent OnFinished;
    public bool stopWait;

    protected void Finished()
    {
        OnFinished?.Invoke();

    }

    public override bool keepWaiting => stopWait;
    public CurveYield()
    {
        stopWait = true;
    }

}