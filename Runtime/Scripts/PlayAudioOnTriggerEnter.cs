using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class PlayAudioOnTriggerEnter : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Vector2 pitchMinMax = new Vector2(0.9f, 1.1f);

    private void OnEnable()
    {
        Debug.Log("enabled");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided");

        audioSource.pitch = Random.Range(pitchMinMax.x, pitchMinMax.y);
        audioSource.PlayOneShot(audioSource.clip);
    }
}
