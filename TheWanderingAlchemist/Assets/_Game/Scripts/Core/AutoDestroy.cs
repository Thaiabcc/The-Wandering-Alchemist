using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            Destroy(gameObject, stateInfo.length);
        }
        else
        {
            Destroy(gameObject, 0.3f);
        }
    }
}