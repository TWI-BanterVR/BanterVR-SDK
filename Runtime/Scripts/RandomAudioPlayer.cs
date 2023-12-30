using UnityEngine;
using System.Collections;

public class RandomAudioPlayer : MonoBehaviour
{
    [Tooltip("List of audio clips to play from")]
    public AudioClip[] audioClips;


    [Tooltip("Minimum volume range for playing clips")]
    [Range(0f, 1f)]
    public float minVolume = 0.5f; // New

    [Tooltip("Maximum volume range for playing clips")]
    [Range(0f, 1f)]
    public float maxVolume = 1.0f; // New

    [Tooltip("Minimum pitch range for playing clips")]
    [Range(-3f, 3f)]
    public float minPitch = 0.8f;

    [Tooltip("Maximum pitch range for playing clips")]
    [Range(-3f, 3f)]
    public float maxPitch = 1.2f;

    [Tooltip("Minimum stereo pan range for playing clips")]
    [Range(-1f, 1f)]
    public float minStereoPan = -0.75f;

    [Tooltip("Maximum stereo pan range for playing clips")]
    [Range(-1f, 1f)]
    public float maxStereoPan = 0.25f;


    [Tooltip("Minimum spatial blend range for playing clips (0 is 2D, 1 is 3D)")]
    [Range(0f, 1f)]
    public float minSpatialBlend = 0f; // New

    [Tooltip("Maximum spatial blend range for playing clips (0 is 2D, 1 is 3D)")]
    [Range(0f, 1f)]
    public float maxSpatialBlend = 0f; // New


    [Tooltip("If true, start playing audio clips when the object starts")]
    public bool playOnStart = false;


    [Tooltip("If true, play audio clips at random intervals; if false, play only once")]
    public bool useRandomTime = true;


    [Tooltip("Minimum time range in seconds for playing clips")]
    public float minTime = 5f;

    [Tooltip("Maximum time range in seconds for playing clips")]
    public float maxTime = 10f;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (playOnStart)
        {
            PlayRandomClip();
        }
    }

    public void PlayRandomClip()
    {
        if (audioClips.Length == 0)
        {
            Debug.LogWarning("No audio clips assigned!");
            return;
        }

        // Cancel any scheduled play to prevent overlap
        CancelInvoke(nameof(PlayRandomClip));

        // Select a random clip
        int randomIndex = Random.Range(0, audioClips.Length);
        AudioClip clipToPlay = audioClips[randomIndex];

        // Randomly set the volume within the specified range
        float randomVolume = Random.Range(minVolume, maxVolume);
        audioSource.volume = randomVolume;


        // Randomly choose left (-1) or right (1) speaker
        float randomPan = Random.Range(minStereoPan, maxStereoPan);
        audioSource.panStereo = randomPan;

        // Randomly set the pitch within the specified range
        float randomPitch = Random.Range(minPitch, maxPitch);
        audioSource.pitch = randomPitch;


        // Randomly set the spatial blend within the specified range
        float randomSpatialBlend = Random.Range(minSpatialBlend, maxSpatialBlend);
        audioSource.spatialBlend = randomSpatialBlend;

        // Play the clip
        audioSource.clip = clipToPlay;
        audioSource.Play();

        // Schedule the next play if useRandomTime is true
        if (useRandomTime)
        {
            StartCoroutine(WaitAndScheduleNextPlay(clipToPlay.length));
        }
    }

    private IEnumerator WaitAndScheduleNextPlay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!audioSource.isPlaying)
        {
            float randomTime = Random.Range(minTime, maxTime);
            Invoke(nameof(PlayRandomClip), randomTime);
        }
    }
}
