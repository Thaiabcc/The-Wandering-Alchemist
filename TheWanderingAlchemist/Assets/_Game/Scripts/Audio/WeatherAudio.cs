using UnityEngine;

public class WeatherAudio : MonoBehaviour
{
    [Header("Audio References")]
    [SerializeField] private AudioSource weatherAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;   
    [Header("Clips")]
    [SerializeField] private AudioClip rainSFX;
    [SerializeField] private AudioClip stormSFX;
    [SerializeField] private AudioClip thunderSFX; 

    public void PlayRain() => PlayLoop(rainSFX);
    public void PlayStorm() => PlayLoop(stormSFX);
    public void StopLoop() => weatherAudioSource?.Stop();

    public void PlayThunder() 
    {
        if (sfxAudioSource != null && thunderSFX != null)
        {
            float master = PlayerPrefs.GetFloat("MasterVol", 1f);
            float sfx = PlayerPrefs.GetFloat("SFXVol", 1f);
            sfxAudioSource.PlayOneShot(thunderSFX, 1f * sfx * master);
        }
    }

    private void PlayLoop(AudioClip clip)
    {
        if (weatherAudioSource == null || clip == null) return;
        
        float master = PlayerPrefs.GetFloat("MasterVol", 1f);
        float music = PlayerPrefs.GetFloat("BGMVol", 1f);
        weatherAudioSource.volume = music * master;

        weatherAudioSource.clip = clip;
        weatherAudioSource.loop = true;

        if (weatherAudioSource.volume > 0.05f)
        {
            weatherAudioSource.Play();
        }
    }
}