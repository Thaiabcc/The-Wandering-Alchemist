using UnityEngine;

public class ShopNPC : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Chào mừng quý khách!");
        ShopUI.Instance.OpenShop();
    }
}