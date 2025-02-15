using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseEventListener<T> : MonoBehaviour
{
    public BaseEventSO<T> eventSO;

    public UnityEvent<T> response;

    private void OnEnable()
    {
        if (eventSO != null)
        {
            eventSO.onEventRaised += onEventRaised;
        }
    }

    private void OnDisable()
    {
        if (eventSO != null)
        {
            eventSO.onEventRaised -= onEventRaised;
        }
    }

    private void onEventRaised(T value)
    {
        response.Invoke(value);
    }
}
