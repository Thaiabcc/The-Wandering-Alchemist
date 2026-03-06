using UnityEngine;

public class ShopNPC : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (ShopUI.Instance != null)
        {
            ShopUI.Instance.ToggleShop();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (ShopUI.Instance != null)
            {
                ShopUI.Instance.CloseShop();
            }
        }
    }
}