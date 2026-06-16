using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip; 
    [SerializeField] private float volume = 0.5f;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            float masterVol = PlayerPrefs.GetFloat("MasterVol", 1f);
            float bgmVol = PlayerPrefs.GetFloat("BGMVol", 1f);
            
            if (masterVol > 0.05f && bgmVol > 0.05f)
            {
                AudioManager.Instance.PlayMusic(musicClip, volume);
            }
            else
            {
                AudioManager.Instance.StopMusic();
            }
        }
    }
}