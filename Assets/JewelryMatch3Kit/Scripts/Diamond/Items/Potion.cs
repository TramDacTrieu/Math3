using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class Potion : MonoBehaviour, IHittable
{
    private PlayableDirector director;

    Action callbackMain;

    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    private void OnEnable()
    {
        director.stopped += OnFinished;
    }
    private void OnDisable()
    {
        director.stopped -= OnFinished;
    }

    private void OnFinished(PlayableDirector obj)
    {
        callbackMain?.Invoke();
    }

    public void Hit(Vector2 pos, Action callback)
    {
        director.Play();
        callbackMain = callback;
        var hit = Physics2D.RaycastAll(transform.position, Vector2.down, 10);
        Water water = null;
        foreach (var item in hit)
        {
            water = item.collider.GetComponent<Water>();
            if (water != null) break;
            if (item.collider.GetComponent<LevelSettings>() != null && item.collider.GetType() == typeof(EdgeCollider2D)) break;
        }
        if (water != null) water.FillLevel(5);
        else
        {
            Water[] waters = GameObject.FindObjectsOfType<Water>();
            if (waters.Length > 0)
            {
                // var closestWater = waters.OrderBy(i => Vector2.Distance(i.transform.position, transform.position)).First();
                // closestWater.enabled = true;
                // closestWater.FillLevel(3);
                waters.ForEachY(i => i.FillLevel(5));
            }
        }
    }
}