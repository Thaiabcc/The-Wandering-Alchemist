using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource footstepSource; 

    [Header("Player Clips")]
    public AudioClip footstep;
    public AudioClip swordSwing;
    public AudioClip takeDamage;
    public AudioClip deflectSFX;
    public AudioClip Die;

    public void UpdateFootstep(bool isMoving, bool isRunning)
    {
        if (footstepSource == null) return;

        if (isMoving)
        {
            float master = PlayerPrefs.GetFloat("MasterVol", 1f);
            float sfx = PlayerPrefs.GetFloat("SFXVol", 1f);
            footstepSource.volume = sfx * master;

            if (!footstepSource.isPlaying && footstepSource.volume > 0.05f) 
                footstepSource.Play();
            
            footstepSource.pitch = isRunning ? 1.5f : 1f; 
        }
        else
        {
            if (footstepSource.isPlaying) footstepSource.Stop();
        }
    }

    public void PlayAttack() => AudioManager.Instance.PlaySFX(swordSwing, 1f, true);
    public void PlayHurt() => AudioManager.Instance.PlaySFX(takeDamage, 1f);
    public void PlayDeflect() => AudioManager.Instance.PlaySFX(deflectSFX, 1.2f);
    public void PlayDie() => AudioManager.Instance.PlaySFX(deflectSFX, 1.2f);
}