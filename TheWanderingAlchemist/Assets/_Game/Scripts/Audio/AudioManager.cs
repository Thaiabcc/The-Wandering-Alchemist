using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("--- NHẠC NỀN ---")]
    public AudioClip backgroundMusic;

    [Header("--- NGƯỜI CHƠI (HÀNH ĐỘNG) ---")]
    public AudioClip footstep;      
    public AudioClip pickupItems;     
    public AudioClip potionUse;     

    [Header("--- NGƯỜI CHƠI (CHIẾN ĐẤU) ---")]
    public AudioClip swordSwing;      
    public AudioClip stoneThrow;     
    public AudioClip stoneBreak;     
    public AudioClip deflectSuccess;  
    public AudioClip playerTakeDamage;
    public AudioClip playerDie;       

    [Header("--- KẺ ĐỊCH ---")]
    public AudioClip enemyHit;        
    public AudioClip enemyDie;       

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
        PlayMusic(backgroundMusic, 0.028f);
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

    // Hàm SFX
    public void PlaySFX(AudioClip clip, float volume = 1f, bool randomPitch = false)
    {
        if (clip == null || sfxSource == null) return;

        if (randomPitch)
        {
            sfxSource.pitch = Random.Range(0.85f, 1.15f);
        }
        else
        {
            sfxSource.pitch = 1f;
        }

        sfxSource.PlayOneShot(clip, volume);
    }
}