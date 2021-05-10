using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DirectorAutoDisable : MonoBehaviour
{
    PlayableDirector director;
    // Use this for initialization
    void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    private void OnEnable()
    {
        director.stopped += OnStoppped;
    }

    private void OnDisable()
    {
        director.stopped -= OnStoppped;

    }

    private void OnStoppped(PlayableDirector obj)
    {
        gameObject.SetActive(false);
    }

}
