using System;
using System.Collections;
using UnityEngine;

public class AnimationItem : MonoBehaviour
{
    public float scale = 1.2f;

    public float time = 0.2f;

    private Transform childTransform;

    private void Awake()
    {
        childTransform = transform.GetChild(0);

    }
    public void Selected(Action onFinished = null)
    {
        StartCoroutine(SelectedAnim());
    }

    IEnumerator SelectedAnim(Action onFinished = null)
    {
        var curveX = new AnimationCurve(new Keyframe(0, childTransform.localScale.x, 5, 0), new Keyframe(time, scale, 0, 5));
        var curveY = new AnimationCurve(new Keyframe(0, childTransform.localScale.y, 5, 0), new Keyframe(time, scale, 0, 5));
        yield return new WaitCurve(LevelManager.THIS, curveX, curveY, (v) => { childTransform.localScale = v; }, null, null, onFinished);
    }
    public void DeSelected(Action onFinished = null)
    {
        StartCoroutine(DeSelectedAnim());
    }

    IEnumerator DeSelectedAnim(Action onFinished = null)
    {
        var curveX = new AnimationCurve(new Keyframe(0, childTransform.localScale.x, -5, 0), new Keyframe(time, 1, 0, -5));
        var curveY = new AnimationCurve(new Keyframe(0, childTransform.localScale.y, -5, 0), new Keyframe(time, 1, 0, -5));

        yield return new WaitCurve(LevelManager.THIS, curveX, curveY, (v) => { childTransform.localScale = v; }, null, null, onFinished);
    }

    public void BonusAppear(Action onFinished = null)
    {
        StartCoroutine(BonusAppearAnim(onFinished));
    }

    IEnumerator BonusAppearAnim(Action onFinished = null)
    {
        var curveX = new AnimationCurve(new Keyframe(0, childTransform.localScale.x, 5, 0), new Keyframe(.15f, childTransform.localScale.x * 1.2f, 0, 5));
        var curveY = new AnimationCurve(new Keyframe(0, childTransform.localScale.y, 5, 0), new Keyframe(.15f, childTransform.localScale.x * 1.3f, 0, 5));
        yield return new WaitCurve(LevelManager.THIS, curveX, curveY, (v) => { childTransform.localScale = v; });
        curveX = new AnimationCurve(new Keyframe(0, childTransform.localScale.x, -5, 0), new Keyframe(.3f, childTransform.localScale.x * .5f, 0, -5));
        curveY = new AnimationCurve(new Keyframe(0, childTransform.localScale.y, -5, 0), new Keyframe(.3f, childTransform.localScale.x * .5f, 0, -5));
        yield return new WaitCurve(LevelManager.THIS, curveX, curveY, (v) => { childTransform.localScale = v; });
        curveX = new AnimationCurve(new Keyframe(0, childTransform.localScale.x), new Keyframe(.1f, 1));
        curveY = new AnimationCurve(new Keyframe(0, childTransform.localScale.y), new Keyframe(.1f, 1));
        yield return new WaitCurve(LevelManager.THIS, curveX, curveY, (v) => { childTransform.localScale = v; });
        onFinished?.Invoke();
    }

    public void NoMatches(Action onChange)
    {
        StartCoroutine(NoMatchesAnim(onChange));
    }

    IEnumerator NoMatchesAnim(Action onChange)
    {
        var curveX = new AnimationCurve(new Keyframe(0, childTransform.localScale.x, 5, 0), new Keyframe(time, .1f, 0, 5));
        var curveY = new AnimationCurve(new Keyframe(0, childTransform.localScale.y, 5, 0), new Keyframe(time, .1f, 0, 5));
        onChange();
        yield return new WaitCurve(LevelManager.THIS, curveX, curveY, (v) => { childTransform.localScale = v; });
        curveX = new AnimationCurve(new Keyframe(0, childTransform.localScale.x, -5, 0), new Keyframe(time, 1, 0, -5));
        curveY = new AnimationCurve(new Keyframe(0, childTransform.localScale.y, -5, 0), new Keyframe(time, 1, 0, -5));

        yield return new WaitCurve(LevelManager.THIS, curveX, curveY, (v) => { childTransform.localScale = v; });
    }

    // private void OnGUI()
    // {
    //     if (GUILayout.Button("Selected"))
    //     {
    //         Selected();
    //     }
    //     if (GUILayout.Button("DeSelected"))
    //     {
    //         DeSelected();
    //     }
    //     if (GUILayout.Button("BonusAppear"))
    //     {
    //         BonusAppear();
    //     }
    //     if (GUILayout.Button("NoMatches"))
    //     {
    //         NoMatches();
    //     }
    // }
}