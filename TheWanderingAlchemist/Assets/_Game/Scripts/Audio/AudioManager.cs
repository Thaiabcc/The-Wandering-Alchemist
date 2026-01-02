using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource; // Nhạc nền (Loop)
    [SerializeField] private AudioSource sfxSource;   // Tiếng động (One shot)

    [Header("--- NHẠC NỀN ---")]
    public AudioClip backgroundMusic;

    [Header("--- NGƯỜI CHƠI (HÀNH ĐỘNG) ---")]
    public AudioClip footstep;        // Đi bộ
    public AudioClip pickupItems;     // Nhặt đồ
    public AudioClip potionUse;       // Uống thuốc (ực ực)

    [Header("--- NGƯỜI CHƠI (CHIẾN ĐẤU) ---")]
    public AudioClip swordSwing;      // Tiếng chém kiếm
    public AudioClip stoneThrow;      // Tiếng ném đá (vút)
    public AudioClip stoneBreak;      // Tiếng đá vỡ khi chạm đất/quái (bụp/keng)
    public AudioClip deflectSuccess;  // Đỡ đòn thành công (KENG!!!)
    public AudioClip playerTakeDamage;// Người chơi bị đánh (Á!)
    public AudioClip playerDie;       // Người chơi chết

    [Header("--- KẺ ĐỊCH ---")]
    public AudioClip enemyHit;        // Quái bị đánh trúng (Bộp)
    public AudioClip enemyDie;        // Quái chết

    private void Awake()
    {
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
        // Tự động phát nhạc nền khi game bắt đầu
        PlayMusic(backgroundMusic, 0.2f); // Volume nhạc nền nên để vừa phải (0.5)
    }

    // Hàm phát nhạc nền
    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (clip == null || musicSource == null) return;

        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.loop = true;
        musicSource.Play();
    }

    // Hàm phát tiếng động (SFX)
    // Tôi thêm tham số pitch (độ cao) ngẫu nhiên để âm thanh đỡ nhàm chán
    public void PlaySFX(AudioClip clip, float volume = 1f, bool randomPitch = false)
    {
        if (clip == null || sfxSource == null) return;

        if (randomPitch)
        {
            sfxSource.pitch = Random.Range(0.85f, 1.15f); // Đổi giọng chút xíu cho tự nhiên
        }
        else
        {
            sfxSource.pitch = 1f;
        }

        sfxSource.PlayOneShot(clip, volume);
    }
}