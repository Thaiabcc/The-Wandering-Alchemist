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
        Debug.Log($"<color=cyan>[AudioManager] LoadVolumes gọi từ PlayerPrefs -> Master: {master}, BGM: {music}, SFX: {sfx}</color>");
        UpdateVolume(master, music, sfx);
    }

    public void UpdateVolume(float masterVol, float musicVol, float sfxVol)
    {
        if (musicSource != null) 
        {
            musicSource.volume = musicVol * masterVol;
            Debug.Log($"<color=cyan>[AudioManager] Cập nhật Music Source Volume thực tế = {musicSource.volume}</color>");
        }
        if (sfxSource != null) 
        {
            sfxSource.volume = sfxVol * masterVol;
            Debug.Log($"<color=cyan>[AudioManager] Cập nhật SFX Source Volume thực tế = {sfxSource.volume}</color>");
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
        Debug.Log($"<color=blue>[AudioManager] PlaySFX -> Clip: {clip.name}, Vol tính toán: {volume * sfx * master}</color>");
    }

    public void StopMusic()
    {
        if (musicSource != null) 
        {
            Debug.Log("<color=blue>[AudioManager] StopMusic được gọi</color>");
            musicSource.Stop();
        }
    }
}