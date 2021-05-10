using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityStandardAssets._2D;
using UnityEngine.Timeline;

public class Diamond : MonoBehaviour, ICollectable, IPoolable, ITransformMove
{
    PlayableDirector director;
    public bool Collected;

    private Vector3 saveScale;
    private AnimationCurve curveX;
    private AnimationCurve curveY;

    private void Awake()
    {
        saveScale = transform.localScale;
    }

    // Use this for initialization
    void Start()
    {
        PrepareFinishAnim();

    }

    public void PrepareFinishAnim()
    {
        director = GetComponent<PlayableDirector>();
        // var target = Resources.FindObjectsOfTypeAll<TargetGUI>().First().transform;
        // var transformTweenTrack = director.playableAsset.outputs.Select(i => i.sourceObject).OfType<TransformTweenTrack>().FirstOrDefault();
        // TransformTweenClip clip = null;
        // foreach (var marker in transformTweenTrack.GetClips())
        //     clip = marker.asset as TransformTweenClip;
        // clip.startLocation = new ExposedReference<Transform> { defaultValue = transform };
        // clip.endLocation = new ExposedReference<Transform> { defaultValue = target };
        // GetComponent<TransformTweenParameters>().startTransform = transform;
        // GetComponent<TransformTweenParameters>().endTransform = target.transform;
    }

    public void DelayedCollect(float sec)
    {
        GetComponent<Rigidbody2D>().simulated = false;
        transform.localScale /= 1.1f;
        Invoke("Collect", sec);
    }

    public void StartFall()
    {
        StartCoroutine(StartAnim());
    }

    IEnumerator StartAnim()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<Rigidbody2D>().simulated = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("chain"))
        {
            Collect();
        }
    }

    public void Collect()
    {
        if (!IsCollected())
        {
            if (LevelManager.THIS.TargetDiamonds + 1 == Counter_.totalCount) LevelManager.THIS.gameStatus = GameState.WaitBeforeWin;
            Collected = true;
            GetComponent<CircleCollider2D>().enabled = false;
            Camera.main.GetComponent<Camera2DFollow>().target = null;
            transform.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "UI";
            transform.GetComponentInChildren<SpriteRenderer>().sortingOrder = 3;
            transform.GetComponentsInChildren<SpriteRenderer>().ForEachY(i => i.color = Color.white);
            transform.GetComponentInChildren<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
            GetComponent<Rigidbody2D>().simulated = false;
            director.Play();
            //            LevelManager.THIS.StartFlyTarget(gameObject, () => { OnAnimationFinished(); });
        }
    }

    public void OnAnimationFinished()
    {
        LevelManager.THIS.TargetDiamonds++;
        LevelManager.THIS.CheckWin();
        director.Stop();
        gameObject.SetActive(false);
    }

    public bool IsCollected()
    {
        return Collected;
    }

    public void OnGetFromPool(Vector2 pos)
    {
        transform.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Default";
        if (saveScale.magnitude > 0)
            transform.localScale = saveScale;
        Collected = false;
        GetComponent<CircleCollider2D>().enabled = true;
        transform.GetComponentsInChildren<SpriteRenderer>().ForEachY(i => i.color = Color.white);
        transform.GetComponentInChildren<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

    }

    public void OnPrecessFrame(float normalisedTime)
    {
        transform.position = new Vector2(curveX.Evaluate(normalisedTime), curveY.Evaluate(normalisedTime));
        transform.Rotate(Vector3.back*100);
    }

    public void OnStartClip()
    {
        var targetPos = FindObjectsOfType<TargetGUI>().First().transform.position;
        Vector3 centerPos = Camera.main.ViewportToWorldPoint(Vector2.one * 0.5f);
        curveX = new AnimationCurve(new Keyframe(0, transform.position.x), new Keyframe(0.5f, centerPos.x), new Keyframe(1, targetPos.x));
        curveY = new AnimationCurve(new Keyframe(0, transform.position.y), new Keyframe(0.5f, centerPos.y), new Keyframe(1, targetPos.y));
    }
}

public interface ICollectable
{
    bool IsCollected();
    void Collect();
}
