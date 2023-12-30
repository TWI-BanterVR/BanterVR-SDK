using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VisibleEvent : MonoBehaviour
{

    [Tooltip("Event invoked when the object becomes visible")]
    public UnityEvent onBecameVisibleEvent;

    [Tooltip("Event invoked when the object becomes invisible")]
    public UnityEvent onBecameInvisibleEvent;

    [Tooltip("Delay in seconds before invoking the visible event")]
    public float visibleDelay;

    [Tooltip("Delay in seconds before invoking the invisible event")]
    public float invisibleDelay;

    void OnBecameVisible(){
        Invoke("_OnBecameVisible", visibleDelay);
    }

    private void _OnBecameVisible() {
        onBecameVisibleEvent?.Invoke();
    }

    void OnBecameInvisible() {
        Invoke("_OnBecameInvisible", invisibleDelay);
    }

    private void _OnBecameInvisible() {
        onBecameInvisibleEvent?.Invoke();
    }
}
