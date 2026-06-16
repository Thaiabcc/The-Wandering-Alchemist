using UnityEngine;

public class KnightPuzzleAudio : MonoBehaviour
{
    [Header("Audio References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activateClip;
    [SerializeField] private AudioClip failClip;
    [SerializeField] private AudioClip successClip;

    [Header("Settings")]
    public float pitchStep = 0.15f;

    public void PlayNote(int index)
    {
        if (audioSource && activateClip)
        {
            float master = PlayerPrefs.GetFloat("MasterVol", 1f);
            float sfx = PlayerPrefs.GetFloat("SFXVol", 1f);
            audioSource.pitch = 1.0f + (index * pitchStep);
            audioSource.PlayOneShot(activateClip, 1f * sfx * master);
        }
    }

    public void PlayFail()
    {
        if (audioSource && failClip)
        {
            float master = PlayerPrefs.GetFloat("MasterVol", 1f);
            float sfx = PlayerPrefs.GetFloat("SFXVol", 1f);
            audioSource.pitch = 0.8f;
            audioSource.PlayOneShot(failClip, 1f * sfx * master);
        }
    }

    public void PlaySuccess()
    {
        if (audioSource && successClip)
        {
            float master = PlayerPrefs.GetFloat("MasterVol", 1f);
            float sfx = PlayerPrefs.GetFloat("SFXVol", 1f);
            audioSource.pitch = 1.0f;
            audioSource.PlayOneShot(successClip, 1f * sfx * master);
        }
    }
}