using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityStandardAssets._2D;
// [ExecuteInEditMode]
public class ItemPhysicsManger : MonoBehaviour, IHittable
{
    public static int activeBodies;
    private float timer;
    public Rigidbody2D rigidBody2D;
    private Collider2D collid;
    private Camera2DFollow camera;
    private Vector2 camRect;
    private IPoolableComponent iPoolable;
    private static Vector2 lowerDinamicTreshold = new Vector2(0, 0);

    float? saveVelocityMagnitude;
    private float startTime;
    private bool forced;
    private bool invoke;
    private bool waitWake;
    private VisibilityListener visibleListener;
    public bool visible;

    public static Vector2 LowerDinamicTreshold
    {
        get
        {
            return lowerDinamicTreshold;
        }

        set
        {
            lowerDinamicTreshold = value;
        }
    }

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        rigidBody2D.sleepMode = RigidbodySleepMode2D.NeverSleep;
        // if (FindObjectOfType<Nested>() || FindObjectOfType<NestedBig>())
        // rigidBody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        collid = GetComponent<Collider2D>();
        camera = Camera.main.GetComponent<Camera2DFollow>();
        camRect = Camera.main.GetCameraWorldBottomBorder();
        iPoolable = GetComponent<IPoolableComponent>();
        lowerDinamicTreshold = new Vector2(0, 100 * Camera2DFollow.direction);
        SetSleep(true);
        visibleListener = GetComponentInChildren<VisibilityListener>();

    }

    private void Start()
    {
        IHighlightableComponent highlightableComponent = GetComponent<IHighlightableComponent>();
        visible = highlightableComponent.spriteRenderers[0].isVisible;
    }

    private void OnEnable()
    {
        forced = false;
        waitWake = false;
        // if (FindObjectOfType<Water>())
        // {
        //     rigidBody2D.bodyType = RigidbodyType2D.Dynamic;
        // }
        // else
        StartCoroutine(DelayedUpdate());

        visibleListener.OnBecameVisib += OnBecameVis;
    }

    private void OnBecameVis(bool isVisible)
    {
        visible = isVisible;
        if (!visible) SetSleep(true);
    }

    IEnumerator DelayedUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (!visible) continue;
            if (!Physics2D.autoSimulation) continue;
            if (LevelManager.THIS.gameStatus == GameState.Playing && !iPoolable.justPooled)
            {
                if (forced) continue;
                if (!camera.needToScroll)
                {
                    rigidBody2D.bodyType = RigidbodyType2D.Dynamic;
                    yield break;
                }

                else if (rigidBody2D.bodyType != RigidbodyType2D.Static)
                {
                    if (saveVelocityMagnitude == null)
                        saveVelocityMagnitude = rigidBody2D.velocity.magnitude;
                    if (startTime + 0.2f < Time.time && rigidBody2D.velocity.magnitude < .5f)
                        SetSleep(true);
                    if (rigidBody2D.velocity.magnitude > 2) PullAbove();
                }
                else
                {
                    if (waitWake)
                        SetSleep(false);
                    else if (!GetColliderBelow())
                        SetSleep(false);
                }
            }
        }
    }

    private bool GetColliderBelow()
    {
        IEnumerable<Collider2D> hits = null;
        var collider2D1 = Physics2D.OverlapCircleAll(transform.position, 1f).Where(i => i != GetComponent<Collider2D>());
        if (collider2D1.Any(i => i.gameObject.layer == 4)) return false;
        // if (Camera2DFollow.direction > 0)
        hits = collider2D1.Where(i => i.transform.position.y + 1 < transform.position.y);
        // else
        // hits = collider2D1.Where(i => i.transform.position.y > transform.position.y);
        foreach (var hit in hits)
        {
            if (hit.isTrigger) continue;
            if (hit != GetComponent<Collider2D>())
                return true;
        }
        return false;
    }

    ItemPhysicsManger[] GetItemsAbove()
    {
        if (Camera2DFollow.direction > 0)
            return Physics2D.BoxCastAll(transform.position, Vector3.one * 2, 0, Vector2.up, 20, 1 << LayerMask.NameToLayer("Item")).Select(i => i.collider.GetComponent<ItemPhysicsManger>()).Where(i => i != null && i.transform.position.y > transform.position.y).ToArray();
        else
            return Physics2D.BoxCastAll(transform.position, Vector3.one * 2, 0, Vector2.down, 20, 1 << LayerMask.NameToLayer("Item")).Select(i => i.collider.GetComponent<ItemPhysicsManger>()).Where(i => i != null && i.transform.position.y < transform.position.y).ToArray();

    }



    public void SetSleep(bool sleep)
    {
        if (sleep)
        {
            activeBodies--;
            if (activeBodies < 0) activeBodies = 0;
            saveVelocityMagnitude = null;
            rigidBody2D.bodyType = RigidbodyType2D.Static;
        }
        else
        {
            if (activeBodies < 40 || !camera.needToScroll)
            {
                waitWake = false;
                activeBodies++;
                startTime = Time.time;
                rigidBody2D.bodyType = RigidbodyType2D.Dynamic;
            }
            else
                waitWake = true;
        }
    }

    public void SetSleepForced(bool sleep)
    {
        forced = sleep;
        SetSleep(sleep);
    }

    public void Hit(Vector2 pos, Action callback)
    {
        // PullAbove();
    }

    public void PullAbove()
    {
        var items = GetItemsAbove().Where(i => i.rigidBody2D.bodyType == RigidbodyType2D.Static);
        // PhysicsOptimizer.THIS.WakeItems(items);
        foreach (var item in items)
        {
            item.SetSleep(false);
        }

    }

    private void OnDisable()
    {
        if (rigidBody2D.bodyType == RigidbodyType2D.Dynamic)
        {
            SetSleep(true);
        }
        visibleListener.OnBecameVisib -= OnBecameVis;
    }
}
