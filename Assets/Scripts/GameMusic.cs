using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameMusic : MonoBehaviour
{
    [Header("Music")]
    public AudioClip backgroundMusic;
    public AudioClip victoryMusic;
    public AudioClip defeatMusic;

    [Header("Volume")]
    [Range(0f, 1f)]
    public float musicVolume = 0.3f;

    public float fadeDuration = 0.2f;

    [Header("Game References")]
    public PlayerHealth playerHealth;
    public WaveManager waveManager;

    private AudioSource musicSource;
    private bool gameFinished = false;

    void Awake()
    {
        musicSource = GetComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;
        musicSource.volume = musicVolume;
    }

    void Start()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    void Update()
    {
        if (gameFinished)
            return;

        if (playerHealth != null && playerHealth.IsDead())
        {
            gameFinished = true;
            StartCoroutine(ChangeMusic(defeatMusic));
        }
        else if (waveManager != null && waveManager.GameWon)
        {
            gameFinished = true;
            StartCoroutine(ChangeMusic(victoryMusic));
        }
    }

    IEnumerator ChangeMusic(AudioClip newMusic)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = fadeDuration > 0f
                ? Mathf.Clamp01(elapsed / fadeDuration)
                : 1f;

            musicSource.volume = Mathf.Lerp(startVolume, 0f, t);

            yield return null;
        }

        musicSource.Stop();

        if (newMusic != null)
        {
            musicSource.clip = newMusic;
            musicSource.loop = false;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }
}
