using UnityEngine;

public class AudioVolumeController : MonoBehaviour
{
    private AudioSource mySource;
    public enum AudioType { BGM, SFX }
    public AudioType type;

    private void Awake()
    {
        mySource = GetComponent<AudioSource>();
        if (mySource != null)
        {
            Debug.Log($"<color=yellow>[Controller] Awake trên Object [{gameObject.name}] -> Trạng thái AudioSource ban đầu: Volume={mySource.volume}, IsPlaying={mySource.isPlaying}</color>");
        }
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
        Debug.Log($"<color=orange>[Controller] ApplyVolume trên Object [{gameObject.name}] ({type}) -> Đặt Volume thực tế = {mySource.volume} (Base: {baseVolume} * Master: {master})</color>");

        if (mySource.loop && !mySource.isPlaying && mySource.volume > 0)
        {
            Debug.Log($"<color=orange>[Controller] [{gameObject.name}] tự động Play vì có loop và volume > 0</color>");
            mySource.Play();
        }
    }
}