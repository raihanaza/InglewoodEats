using System.Collections;
using UnityEngine;

// Handles the plate's animations

public class DishAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    public float animationDuration = 0.4f;
    public float pullDistance = 1.5f;   // how far it flies toward user
    public float pushDistance = 1.5f;   // how far it slides away
    public float pushDropAmount = 1.0f; // how far it drops during push

    public void PlayPullAnimation(System.Action onComplete)
    {
        StartCoroutine(PullRoutine(onComplete));
    }


    public void PlayPushAnimation(System.Action onComplete)
    {
        StartCoroutine(PushRoutine(onComplete));
    }

    private IEnumerator PullRoutine(System.Action onComplete)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, 0, -pullDistance);

        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);

            // EaseIn makes it feel like it's shooting fast
            float eased = t * t;

            transform.position = Vector3.Lerp(startPos, endPos, eased);
            yield return null;
        }

        transform.position = endPos;
        onComplete?.Invoke();
    }

    private IEnumerator PushRoutine(System.Action onComplete)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, -pushDropAmount, pushDistance);

        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);

            // EaseOut makes it feel like it's slowing as it falls
            float eased = 1f - (1f - t) * (1f - t);

            transform.position = Vector3.Lerp(startPos, endPos, eased);
            yield return null;
        }

        transform.position = endPos;
        onComplete?.Invoke();
    }
}