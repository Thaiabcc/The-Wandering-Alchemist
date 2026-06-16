using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    public AudioClip hitSFX;
    public AudioClip dieSFX;

    public void PlayHit() => AudioManager.Instance.PlaySFX(hitSFX, 0.8f, true);
    public void PlayDie() => AudioManager.Instance.PlaySFX(dieSFX, 1f);
}