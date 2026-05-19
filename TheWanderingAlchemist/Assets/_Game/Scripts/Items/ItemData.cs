using UnityEngine;

public enum ItemType { Ingredient, Potion, Tool, KeyItem }

[CreateAssetMenu(fileName = "NewItem", menuName = "Alchemist/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string id;
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public Sprite buffIcon;

    [Header("Settings")]
    public ItemType itemType; 
    public int maxStackSize = 99;
    public int baseValue = 10;
    public GameObject worldPrefab;

    [Header("Consumable Effects (Instant)")]
    public bool isConsumable;
    public int healthRestore;
    public float staminaRestore;
    public float shieldAmount;
    public float damageBuffAmount;

    [Header("Temporary & Over Time")]
    public float hoTAmount;    
    public float hoTDuration;  
    public float maxHealthPercentBonus; 
    public float buffDuration; 

    [Header("Throwable & Trap")]
    public bool isThrowable;
    public GameObject throwablePrefab;
    public GameObject poisonPuddlePrefab; 
    public float throwDamageMultiplier;  

    public bool UseItem(PlayerStats player)
    {
        if (!isConsumable || player == null) return false;

        bool itemUsed = false;

        if (itemType == ItemType.KeyItem) return true;

        if (shieldAmount > 0)
        {
            player.AddShield(shieldAmount,buffIcon);
            itemUsed = true;
        }

        if (damageBuffAmount > 0)
        {
            player.ApplyBuffDamage(damageBuffAmount, buffDuration,buffIcon);
            itemUsed = true;
        }

        if (hoTAmount > 0)
        {
            player.StartCoroutine(player.HealOverTime(hoTAmount, hoTDuration));
            itemUsed = true;
        }

        if (maxHealthPercentBonus > 0)
        {
            player.ApplyTemporaryMaxHealth(maxHealthPercentBonus, buffDuration,buffIcon);
            itemUsed = true;
        }

        if (isThrowable)
        {
            SpawnThrowable(player);
            itemUsed = true;
        }

        if (healthRestore > 0 && player.currentHealth < player.MaxHealth)
        {
            player.Heal(healthRestore);
            itemUsed = true;
        }

        if (staminaRestore > 0 && player.currentStamina < player.maxStamina)
        {
            player.RegenerateStamina(staminaRestore);
            itemUsed = true;
        }

        return itemUsed;
    }

    private void SpawnThrowable(PlayerStats player)
    {
        if (throwablePrefab == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 throwDirection = (mousePos - player.transform.position).normalized;

        GameObject potion = Instantiate(throwablePrefab, player.transform.position, Quaternion.identity);
        
        ExplosivePotion explosive = potion.GetComponent<ExplosivePotion>();
        if (explosive != null)
        {
            explosive.Setup(throwDirection);
            return;
        }

        PoisonPotion poison = potion.GetComponent<PoisonPotion>();
        if (poison != null)
        {
            float calculatedDamage = player.currentDamage * throwDamageMultiplier; 
            poison.Setup(throwDirection, calculatedDamage);
            return;
        }
    }
}