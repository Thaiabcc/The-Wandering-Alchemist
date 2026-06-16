using UnityEngine;

public class AudioVolumeController : MonoBehaviour
{
    private AudioSource mySource;
    public enum AudioType { BGM, SFX }
    public AudioType type;

    private void Awake()
    {
        mySource = GetComponent<AudioSource>();
        ApplyVolume();
    }

    private void OnEnable()
    {
        ApplyVolume();
    }

    public void ApplyVolume()
    {
        if (mySource == null) return;
        
        float master = PlayerPrefs.GetFloat("MasterVol", 1f);
        float baseVolume = (type == AudioType.BGM) 
            ? PlayerPrefs.GetFloat("BGMVol", 1f) 
            : PlayerPrefs.GetFloat("SFXVol", 1f);
            
        mySource.volume = baseVolume * master;

        if (mySource.loop && !mySource.isPlaying && mySource.volume > 0)
        {
            mySource.Play();
        }
    }
}