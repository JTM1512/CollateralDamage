using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public class MusicPlayer : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loop = true;
    [SerializeField] private bool persistAcrossScenes = true;

    [Header("Smooth Loop (Optional)")]
    [SerializeField] private bool fadeLoop = false;
    [SerializeField] private float fadeOutSeconds = 1.0f;
    [SerializeField] private float fadeInSeconds = 0.15f;

    [Header("Mix")]
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    private AudioSource audioSource;
    private float baseVolume;
    private float fadeT;
    private bool isFadingOut;
    private bool isFadingIn;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (persistAcrossScenes)
        {
            DontDestroyOnLoad(gameObject);
        }

        // Force typical "music" settings.
        audioSource.spatialBlend = 0f; // 2D
        audioSource.loop = loop && !fadeLoop; // if fadeLoop, we manually loop
        baseVolume = Mathf.Clamp01(volume);
        audioSource.volume = baseVolume;

        if (musicClip != null)
        {
            audioSource.clip = musicClip;
        }
    }

    private void Update()
    {
        if (!fadeLoop || !loop || audioSource == null || audioSource.clip == null)
        {
            return;
        }

        float clipLength = audioSource.clip.length;
        if (clipLength <= 0.01f)
        {
            return;
        }

        // If audio stopped for any reason, restart it (keeps looping forever).
        if (!audioSource.isPlaying && !isFadingOut && !isFadingIn)
        {
            audioSource.volume = baseVolume;
            audioSource.time = 0f;
            audioSource.Play();
        }

        float timeRemaining = clipLength - audioSource.time;
        if (!isFadingOut && !isFadingIn && timeRemaining <= Mathf.Max(0.01f, fadeOutSeconds))
        {
            isFadingOut = true;
            fadeT = 0f;
        }

        float dt = Time.unscaledDeltaTime;

        if (isFadingOut)
        {
            fadeT += dt;
            float t = Mathf.Clamp01(fadeT / Mathf.Max(0.01f, fadeOutSeconds));
            audioSource.volume = Mathf.Lerp(baseVolume, 0f, t);

            if (t >= 1f)
            {
                audioSource.time = 0f;
                audioSource.Play();

                isFadingOut = false;
                isFadingIn = true;
                fadeT = 0f;
                audioSource.volume = 0f;
            }
        }
        else if (isFadingIn)
        {
            fadeT += dt;
            float t = Mathf.Clamp01(fadeT / Mathf.Max(0.01f, fadeInSeconds));
            audioSource.volume = Mathf.Lerp(0f, baseVolume, t);

            if (t >= 1f)
            {
                isFadingIn = false;
                audioSource.volume = baseVolume;
            }
        }
    }

    private void Start()
    {
        if (playOnStart)
        {
            Play();
        }
    }

    public void Play()
    {
        if (audioSource == null || audioSource.clip == null)
        {
            Debug.LogWarning($"{nameof(MusicPlayer)}: No AudioClip assigned.");
            return;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void Stop()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}

// Created with AI assistance (Cursor + GPT-5.2).

