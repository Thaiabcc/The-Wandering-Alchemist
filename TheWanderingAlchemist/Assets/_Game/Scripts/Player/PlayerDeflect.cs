using System.Collections;
using UnityEngine;

public class PlayerDeflect : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Gameobject con chứa Collider")]
    [SerializeField] private GameObject deflectHitbox;
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private float activeTime = 0.2f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private float offsetDistance = 1.0f;

    [Header("Stamina Cost")]
    [SerializeField] private float staminaCost = 20f;

    private bool isDeflecting;
    private bool canDeflect = true;

    private void Awake()
    {
        if (deflectHitbox) deflectHitbox.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire2") && canDeflect && !isDeflecting)
        {
            if (PlayerStats.Instance.TryConsumeStamina(staminaCost))
            {
                StartCoroutine(DeflectRoutine());
            }
            else
            {
                Debug.Log("Not enough stamina to deflict !");
            }
        }
    }

    private IEnumerator DeflectRoutine()
    {
        isDeflecting = true;
        canDeflect = false;

        // Rotate Zone to Mouse
        RotateHitboxToMouse();

        // 1. Run Anim Deflict
        if (animator) animator.SetTrigger("Deflect");

        // 2. Turn Zone
        if (deflectHitbox) deflectHitbox.SetActive(true);

        // 3. Exist Time
        yield return new WaitForSeconds(activeTime);

        // 4. Off Zone
        if (deflectHitbox) deflectHitbox.SetActive(false);
        isDeflecting = false;

        // 5. CoolDown
        yield return new WaitForSeconds(cooldown - activeTime);
        canDeflect = true;
    }

    private void RotateHitboxToMouse()
    {
        if (deflectHitbox == null) return;

        // 1. Position Mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // 2. Caculate Mouse -> Player
        Vector3 direction = (mousePos - transform.position).normalized;

        // 3. Position Hitbox
        deflectHitbox.transform.position = transform.position + (direction * offsetDistance);

        // 4. Flip Hitbox
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        deflectHitbox.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}