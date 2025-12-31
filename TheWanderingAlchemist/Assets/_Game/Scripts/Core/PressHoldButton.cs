using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DCDE6B : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent OnPressAction;
    public float holdDelay = 0.5f;
    public float repeatInterval = 0.1f;
    private bool isHeld = false;
    private float nextFireTime;

    // Point Down
    public void OnPointerDown(PointerEventData eventData)
    {
        isHeld = true;
        OnPressAction?.Invoke();
        nextFireTime = Time.time + holdDelay;
    }

    // Point Up
    public void OnPointerUp(PointerEventData eventData)
    {
        isHeld = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHeld = false;
    }
    private void Update()
    {
        if (isHeld)
        {
            if(Time.time >= nextFireTime)
            {
                OnPressAction?.Invoke();
                nextFireTime = Time.time + repeatInterval;
            }
        }
    }
}
