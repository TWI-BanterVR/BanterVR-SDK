using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrigger : MonoBehaviour
{
    // Public variable for gravity strength
    public float timeScale = 1f;

    public bool isTrigger = true;

    public void SetTimeScale(float scale) {
        Time.timeScale = scale;
    }

    // Set gravity when player enters trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isTrigger)
        {
            // Set the scene timescale 
            Time.timeScale = timeScale;
        }
    }

    // Reset gravity when player exits trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isTrigger)
        {
            // Reset the scene timescale
            Time.timeScale = 1f;
        }
    }
}
