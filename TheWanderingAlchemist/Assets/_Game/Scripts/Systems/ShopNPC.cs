using UnityEngine;

public class ShopNPC : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        // Thay vì chỉ gọi OpenShop, ta gọi ToggleShop
        if (ShopUI.Instance != null)
        {
            ShopUI.Instance.ToggleShop();
        }
    }
}