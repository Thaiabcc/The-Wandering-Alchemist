using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopupGenerator : MonoBehaviour
{
    public static DamagePopupGenerator Instance { get; private set; }

    [SerializeField] private Transform pfDamagePopup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void Create(Vector3 position, int damageAmount, bool isCriticalHit = false, bool isPoison = false)
    {
        if (pfDamagePopup == null) return;

        Transform damagePopupTransform = Instantiate(pfDamagePopup, position, Quaternion.identity);
        DamagePopup popup = damagePopupTransform.GetComponent<DamagePopup>();

        if (popup != null)
        {
            popup.Setup(damageAmount, isCriticalHit, isPoison);
        }
    }
}