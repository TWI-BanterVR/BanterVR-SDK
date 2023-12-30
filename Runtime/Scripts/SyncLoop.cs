using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SyncLoop : MonoBehaviour
{
    [Tooltip("Interval in seconds")]
    public long interval = 10;

    [Tooltip("Run once and then stop")]
    public bool RunOnce;


    [Tooltip("Event to trigger when things are in sync")]
    public UnityEvent OnSync;
    bool hasRun;
    bool readyToTrigger;

    void Update() {
        if(interval > 0 && !hasRun) {
            var nowInMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var intervalMs = interval * 1000;
            var timeSinceLast = nowInMs % intervalMs;
            if(timeSinceLast > (intervalMs - 1000) && !readyToTrigger) {
                readyToTrigger = true;
            }
            if(timeSinceLast < 1000 && readyToTrigger) {
                readyToTrigger = false;
                OnSync?.Invoke();
                if (RunOnce) {
                    hasRun = true;
                }
            }
        }
    }
}
