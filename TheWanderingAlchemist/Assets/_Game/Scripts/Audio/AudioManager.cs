using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource; // Để phát nhạc nền (Loop)
    [SerializeField] private AudioSource sfxSource;   // Để phát tiếng động (One shot)

    [Header("Clips (Kéo file âm thanh vào đây)")]
    public AudioClip backgroundMusic;
    public AudioClip swordSwing;
    public AudioClip enemyHit;
    public AudioClip footstep;

    private void Awake()
    {
        // Singleton Bất Tử
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        AudioManager.Instance.PlayMusic(backgroundMusic, 0.1f);
    }

    // Hàm phát nhạc nền (Nhạc nền chỉ có 1 bài chạy loop)
    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.loop = true; // Lặp lại
        musicSource.Play();
    }

    // Hàm phát tiếng động (Tiếng đánh, tiếng bước chân...)
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        // PlayOneShot cho phép chỉnh volumeScale
        sfxSource.PlayOneShot(clip, volume);
    }
}