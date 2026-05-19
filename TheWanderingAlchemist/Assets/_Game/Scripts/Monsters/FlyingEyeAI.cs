using System.Collections;
using UnityEngine;

public class FlyingEyeAI : EnemyAI
{
    [Header("Explosion Settings")]
    [SerializeField] private float fuseTime = 0.7f;
    [SerializeField] private float explosionRadius = 3.5f;
    [SerializeField] private float minDamageRadius = 1.0f;
    [SerializeField] private float explosionDamage = 60f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Juice (VFX & Audio)")]
    [SerializeField] private GameObject warningGlow;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fuseSFX;      
    [SerializeField] private AudioClip explosionSFX; 

    private bool isTriggered = false;

    protected override void Start()
    {
        base.Start(); 
        if (warningGlow != null) warningGlow.SetActive(false);
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    protected override void FixedUpdate()
    {
        if (isDead || isTriggered) return; 
        base.FixedUpdate();
    }

    protected override void PerformAttack()
    {
        if (!isTriggered) StartCoroutine(ExplodeRoutine());
    }

    #region Combat Death (Khi bị Player đánh chết)
    public override void TriggerDeath()
    {
        if (isDead) return;
        isDead = true;

        StopMoving();
        if (warningGlow != null) warningGlow.SetActive(false);

        animator.enabled = false; 
        rb.gravityScale = 3f;    
        rb.constraints = RigidbodyConstraints2D.None; 
        
        rb.velocity = new Vector2(Random.Range(-3f, 3f), 5f);
        rb.AddTorque(Random.Range(-500f, 500f)); 

        if (mainCollider != null) mainCollider.enabled = false;
        StartCoroutine(DeathFadeOutRoutine());
    }

    private IEnumerator DeathFadeOutRoutine()
    {
        yield return new WaitForSeconds(0.6f); 

        float timer = 0f;
        float fadeDuration = 0.8f;
        Color startColor = spriteRenderer.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
    #endregion

    #region Kamikaze Logic (Tự phát nổ)
    private IEnumerator ExplodeRoutine()
    {
        isTriggered = true;
        StopMoving();

        float timer = 0f;
        float nextBlinkTime = 0f;
        bool isGlowOn = false;
        Vector3 originalGlowScale = warningGlow != null ? warningGlow.transform.localScale : Vector3.one;

        while (timer < fuseTime)
        {
            if (isDead) yield break; 
            timer += Time.deltaTime;

            float currentBlinkSpeed = Mathf.Lerp(0.15f, 0.02f, timer / fuseTime);
            if (timer >= nextBlinkTime)
            {
                isGlowOn = !isGlowOn;
                if (warningGlow != null) warningGlow.SetActive(isGlowOn);
                if (isGlowOn && audioSource != null && fuseSFX != null) audioSource.PlayOneShot(fuseSFX);
                nextBlinkTime = timer + currentBlinkSpeed;
            }

            if (warningGlow != null && isGlowOn)
            {
                float pulse = 1f + Mathf.Sin(timer * 25f) * 0.3f; 
                warningGlow.transform.localScale = originalGlowScale * pulse;
            }

            float shake = Mathf.Lerp(0.02f, 0.08f, timer / fuseTime);
            transform.position += new Vector3(Random.Range(-shake, shake), Random.Range(-shake, shake), 0);

            yield return null;
        }

        if (isDead) yield break;

        if (explosionSFX != null) AudioSource.PlayClipAtPoint(explosionSFX, transform.position);
        if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                float finalDamage = explosionDamage;

                if (distance > minDamageRadius)
                {
                    float lerpVal = (distance - minDamageRadius) / (explosionRadius - minDamageRadius);
                    finalDamage = Mathf.Lerp(explosionDamage, explosionDamage * 0.2f, lerpVal);
                }

                hit.SendMessage("TakeDamage", finalDamage, SendMessageOptions.DontRequireReceiver);
            }
        }

        spriteRenderer.enabled = false;
        if (warningGlow != null) warningGlow.SetActive(false);
        if (mainCollider != null) mainCollider.enabled = false;

        EnemyHealth myHealth = GetComponent<EnemyHealth>();
        if (myHealth != null) myHealth.TakeDamage(9999f); 
    }
    #endregion

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDamageRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}