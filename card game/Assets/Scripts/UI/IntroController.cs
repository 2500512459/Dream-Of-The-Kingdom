using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class IntroController : MonoBehaviour
{
    public PlayableDirector director;

    public ObjectEventSO loadMenuEvent;
    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
        director.stopped += OnPlayableDirectorStoped;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && director.state == PlayState.Playing)
        {
            director.Stop();
        }
    }

    private void OnPlayableDirectorStoped(PlayableDirector director)
    {
        loadMenuEvent.RaiseEvent(null, this);
    }
}
