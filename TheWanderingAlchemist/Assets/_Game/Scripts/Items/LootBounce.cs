using UnityEngine;
using System.Collections;

public class LootBounce : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(BounceRoutine());
    }

    private IEnumerator BounceRoutine()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(Random.Range(-1.2f, 1.2f), Random.Range(-0.5f, -1f), 0);
        
        float duration = 0.5f; 
        float elapsed = 0f;
        float peakHeight = 1.5f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            currentPos.y += Mathf.Sin(t * Mathf.PI) * peakHeight;

            transform.position = currentPos;
            yield return null;
        }

        transform.position = targetPos;
        
        yield return StartCoroutine(MiniBounce(targetPos));
    }

    private IEnumerator MiniBounce(Vector3 groundPos)
    {
        float elapsed = 0f;
        float duration = 0.2f;
        float peakHeight = 0.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            Vector3 currentPos = groundPos;
            currentPos.y += Mathf.Sin(t * Mathf.PI) * peakHeight;
            transform.position = currentPos;
            yield return null;
        }
        transform.position = groundPos;
    }
}