using UnityEngine;

public class AlchemyAudio : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip cookingSFX;
    [SerializeField] private AudioClip successSFX;
    [SerializeField] private AudioClip failSFX;

    public void PlayCooking() => AudioManager.Instance.PlaySFX(cookingSFX, 1f);
    public void PlaySuccess() => AudioManager.Instance.PlaySFX(successSFX, 1f);
    public void PlayFail() => AudioManager.Instance.PlaySFX(failSFX, 1f);
}