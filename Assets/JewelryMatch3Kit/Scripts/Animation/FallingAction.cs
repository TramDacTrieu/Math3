using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FallingAction : MonoBehaviour
{
    public Action callback;
    private void Start()
    {
        StartCoroutine(Falling());
    }
    IEnumerator Falling()
    {
        var moveWave = GetComponent<MoveWave>();
        yield return new WaitWhile(() => moveWave.animationGoing);
        if (GetComponent<Collider2D>() != null)
            GetComponent<Collider2D>().enabled = false;
        var rb = gameObject.GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();
        rb.AddRelativeForce(new Vector2(UnityEngine.Random.insideUnitCircle.x * UnityEngine.Random.Range(30, 400), UnityEngine.Random.Range(100, 150)), ForceMode2D.Force);
        float y = Camera.main.GetCameraWorldBottomBorder().y;
        while (y < transform.position.y)
        {
            transform.Rotate(0, 0, 10);
            yield return new WaitForFixedUpdate();
        }
        Destroy(rb);
        callback?.Invoke();
        Destroy(this);
    }
}