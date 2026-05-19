using UnityEngine;
using UnityEngine.UI;

public class BuffIconUI : MonoBehaviour
{
    private string buffId;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image fillImage; 

    private float timer;
    private float maxDuration;
    private BuffUIManager manager;

    public void Setup(string id, Sprite icon, float duration, BuffUIManager uiManager)
    {
        buffId = id;
        iconImage.sprite = icon;
        timer = duration;
        maxDuration = duration;
        manager = uiManager;
        
        if (fillImage != null) 
            fillImage.fillAmount = (maxDuration > 0) ? 1f : 0f;
    }

    private void Update()
    {
        if (maxDuration <= 0) return; 

        timer -= Time.deltaTime;
        if (fillImage != null) 
        {
            fillImage.fillAmount = timer / maxDuration;
        }

        if (timer <= 0)
        {
            manager.RemoveBuff(buffId);
        }
    }
}