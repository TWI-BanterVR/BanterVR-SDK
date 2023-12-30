using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AudioFadeTrigger : MonoBehaviour
{
    [Tooltip("The first audio source (e.g., Audio A). When only A is set with InsideTrigger = true it will only fade in and out audio A")]
    public AudioSource audioSourceA;

    [Tooltip("The second audio source (e.g., Audio B)")]
    public AudioSource audioSourceB;

    [Tooltip("Default duration in seconds for the fade effect between audio sources")]
    public float defaultFadeDuration = 1.0f;

    [Tooltip("If true, fades from A to B when entering the trigger and from B to A when exiting. If false, uses directional system on exit.")]
    public bool InsideTrigger = true;

    private Vector3 entryDirection;
    private Coroutine currentFadeCoroutine;
    private float initialVolumeA;
    private float initialVolumeB;

    private void Awake()
    {
        initialVolumeA = audioSourceA ? audioSourceA.volume : 0;
        initialVolumeB = audioSourceB ? audioSourceB.volume : 0;
    }

    private void StartFade(AudioSource from, AudioSource to, float duration)
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(FadeAudio(from, to, duration));
    }

    public void BlendFromAToB(float fadeDuration)
    {
        StartFade(audioSourceA, audioSourceB, fadeDuration);
    }

    public void BlendFromBToA(float fadeDuration)
    {
        StartFade(audioSourceB, audioSourceA, fadeDuration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHead"))
        {
            entryDirection = other.transform.position - transform.position;

            if (InsideTrigger)
            {
                if (audioSourceB)
                {
                    StartFade(audioSourceA, audioSourceB, defaultFadeDuration);
                }
                else if (audioSourceA)
                {
                    StartFade(null, audioSourceA, defaultFadeDuration);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHead"))
        {
            if (InsideTrigger)
            {
                if (audioSourceA && audioSourceB)
                {
                    StartFade(audioSourceB, audioSourceA, defaultFadeDuration);
                }
                else if (audioSourceA)
                {
                    StartFade(audioSourceA, null, defaultFadeDuration);
                }
                else if (audioSourceB)
                {
                    StartFade(audioSourceB, null, defaultFadeDuration);
                }
            }
            else
            {
                Vector3 exitDirection = other.transform.position - transform.position;
                bool exitedForward = Vector3.Dot(transform.forward, exitDirection) > 0;
                bool enteredForward = Vector3.Dot(transform.forward, entryDirection) > 0;

                if (exitedForward != enteredForward) // Player moved through the trigger
                {
                    if (audioSourceA && audioSourceB)
                    {
                        StartFade(exitedForward ? audioSourceA : audioSourceB, exitedForward ? audioSourceB : audioSourceA, defaultFadeDuration);
                    }
                    else if (audioSourceA)
                    {
                        StartFade(null, audioSourceA, defaultFadeDuration);
                    }
                    else if (audioSourceB)
                    {
                        StartFade(null, audioSourceB, defaultFadeDuration);
                    }
                }
            }
        }
    }


    System.Collections.IEnumerator FadeAudio(AudioSource from, AudioSource to, float duration)
    {
        float elapsedTime = 0;

        float targetVolumeFrom = 0;
        float targetVolumeTo = (to == audioSourceA) ? initialVolumeA : (to == audioSourceB) ? initialVolumeB : 1;

        float initialVolumeFrom = from?.volume ?? 0;
        float initialVolumeTo = to?.volume ?? 0;

        to?.Play();

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            if (from) from.volume = Mathf.Lerp(initialVolumeFrom, targetVolumeFrom, t);
            if (to) to.volume = Mathf.Lerp(initialVolumeTo, targetVolumeTo, t);

            yield return null;
        }

        from?.Stop();
    }
}
