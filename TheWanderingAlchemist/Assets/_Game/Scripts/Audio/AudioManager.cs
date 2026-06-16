using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadVolumes();
    }

    public void LoadVolumes()
    {
        float master = PlayerPrefs.GetFloat("MasterVol", 1f);
        float music = PlayerPrefs.GetFloat("BGMVol", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVol", 1f);
        UpdateVolume(master, music, sfx);
    }

    public void UpdateVolume(float masterVol, float musicVol, float sfxVol)
    {
        if (musicSource != null) 
        {
            musicSource.volume = musicVol * masterVol;
        }
        if (sfxSource != null) 
        {
            sfxSource.volume = sfxVol * masterVol;
        }
    }

    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (clip == null || musicSource == null) return;
    
        musicSource.clip = clip;
    
        float master = PlayerPrefs.GetFloat("MasterVol", 1f);
        float music = PlayerPrefs.GetFloat("BGMVol", 1f);
        musicSource.volume = volume * music * master;
    
        musicSource.loop = true;

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, bool randomPitch = false)
    {
        if (clip == null || sfxSource == null) return;
        
        sfxSource.pitch = randomPitch ? Random.Range(0.85f, 1.15f) : 1f;
        float master = PlayerPrefs.GetFloat("MasterVol", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVol", 1f);
        sfxSource.PlayOneShot(clip, volume * sfx * master);
    }

    public void StopMusic()
    {
        if (musicSource != null) 
        {
            musicSource.Stop();
        }
    }
}