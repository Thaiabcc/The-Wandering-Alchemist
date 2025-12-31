using UnityEngine;

public class AlchemyStation : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        AlchemyUI alchemyUI = AlchemyUI.Instance;

        if (alchemyUI == null)
        {
            Debug.LogError("Không tìm thấy Alchemist !...");
            return;
        }

        bool isOpening = !alchemyUI.alchemyPanel.activeSelf;

        if (isOpening)
            alchemyUI.OpenPanel();
        else
            alchemyUI.CloseButtonAction();
    }
}
