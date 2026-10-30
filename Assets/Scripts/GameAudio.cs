using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameAudio : MonoBehaviour
{
    public static GameAudio Instance { get; private set; }

    public AudioClip laserShot;
    public AudioClip enemyHit;
    public AudioClip enemyDeath;
    public AudioClip playerHurt;

    [Range(0f, 1f)]
    public float volume = 0.6f;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void PlayLaser() => Play(laserShot);
    public void PlayEnemyHit() => Play(enemyHit);
    public void PlayEnemyDeath() => Play(enemyDeath);
    public void PlayPlayerHurt() => Play(playerHurt);

    private void Play(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip, volume);
    }
}
