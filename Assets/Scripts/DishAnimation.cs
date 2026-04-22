using System.Collections;
using UnityEngine;

public class DishAnimation : MonoBehaviour
{
    [Header("Pull Animation")]
    public float pullDuration = 0.8f;
    public float pullSpinSpeed = 720f;
    public float pullScaleUp = 2.5f;      // how big it gets as it flies toward you
    public float pullRiseAmount = 0.3f;   // rises up before shooting forward

    [Header("Push Animation")]
    public float pushDuration = 0.5f;
    public float pushDistance = 2.0f;
    public float pushDropAmount = 1.2f;
    public float pushSpinAmount = 45f; 

    [Header("Spawn Animation")]
    public float spawnDuration = 0.6f;

    [Header("Glow Settings")]
    public Color pullColor = new Color(0.2f, 1f, 0.4f, 1f);
    public Color normalColor = Color.black;
    public float glowIntensity = 1.5f;

    [Header("Idle Spin Animation")]
    public float idleRotateSpeed = 45f;

    private Renderer plateRenderer;
    private MaterialPropertyBlock propBlock;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Coroutine idleCoroutine;
    private bool isAnimating = false;

    void Awake()
    {
        plateRenderer = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        originalPosition = transform.position;
        originalScale = transform.localScale;
    }

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }


    public void PlayPullAnimation(System.Action onComplete)
    {
        if (isAnimating) return;
        StopIdle();
        StartCoroutine(PullRoutine(onComplete));
    }

    public void PlayPushAnimation(System.Action onComplete)
    {
        if (isAnimating) return;
        StopIdle();
        StartCoroutine(PushRoutine(onComplete));
    }

    // Plate spawn
    private IEnumerator SpawnRoutine()
    {
        transform.localScale = Vector3.zero;
        Vector3 belowPos = originalPosition - new Vector3(0, 0.3f, 0);
        transform.position = belowPos;

        float elapsed = 0f;
        while (elapsed < spawnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / spawnDuration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            float bounce = Mathf.Sin(t * Mathf.PI) * 0.15f;

            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, eased);
            transform.position = Vector3.Lerp(belowPos, originalPosition, eased)
                                 + new Vector3(0, bounce, 0);

            // Shimmer during spawn
            float shimmer = Mathf.Sin(elapsed * 20f) * 0.5f + 0.5f;
            SetEmissionColor(Color.Lerp(normalColor, pullColor, shimmer * (1f - t)));

            yield return null;
        }

        transform.localScale = originalScale;
        transform.position = originalPosition;
        SetEmissionColor(normalColor);

        StartIdle();
    }

    // Idle animation
    private void StartIdle()
    {
        idleCoroutine = StartCoroutine(IdleRoutine());
    }

    private void StopIdle()
    {
        if (idleCoroutine != null)
            StopCoroutine(idleCoroutine);
    }

    private IEnumerator IdleRoutine()
{
    while (true)
    {
        // 360 degree spin on table
        transform.Rotate(0, idleRotateSpeed * Time.deltaTime, 0);
        yield return null;
    }
}

    //  Fly to camera animation 
    private IEnumerator PullRoutine(System.Action onComplete)
    {
        isAnimating = true;

        Vector3 startPos = transform.position;
        Vector3 startScale = transform.localScale;
        Quaternion startRot = transform.rotation;

        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 facePos = cameraPos + Camera.main.transform.forward * 0.8f;

        // Plate rise up slightly
        float phase1Duration = pullDuration * 0.3f;
        float elapsed = 0f;

        SetEmissionColor(pullColor * glowIntensity);

        while (elapsed < phase1Duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / phase1Duration);
            float eased = 1f - (1f - t) * (1f - t);

            transform.position = Vector3.Lerp(startPos,
                startPos + new Vector3(0, pullRiseAmount, 0), eased);
            transform.localScale = Vector3.Lerp(startScale, startScale * 1.3f, eased);
            transform.Rotate(0, pullSpinSpeed * Time.deltaTime, 0);

            yield return null;
        }

        // Plate shoot toward face, spin, scale up then shrink
        float phase2Duration = pullDuration * 0.7f;
        Vector3 risePos = transform.position;
        elapsed = 0f;

        while (elapsed < phase2Duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / phase2Duration);
            float eased = t * t * t;

            // Scale then shrink
            float scaleCurve = Mathf.Sin(t * Mathf.PI);
            float scaleMultiplier = 1f + scaleCurve * (pullScaleUp - 1f);

            transform.position = Vector3.Lerp(risePos, facePos, eased);
            transform.localScale = startScale * scaleMultiplier;
            transform.Rotate(0, pullSpinSpeed * 1.5f * Time.deltaTime, 0);

            // Flash brighter as it approaches
            float flash = Mathf.Lerp(glowIntensity, glowIntensity * 2f, t);
            SetEmissionColor(pullColor * flash);

            yield return null;
        }

        transform.position = facePos;
        transform.localScale = originalScale;

        isAnimating = false;
        onComplete?.Invoke();
    }

    // Plate tumble away animation
    private IEnumerator PushRoutine(System.Action onComplete)
    {
        isAnimating = true;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Vector3 startScale = transform.localScale;

        // Push away from player
        Vector3 awayDir = (transform.position - Camera.main.transform.position).normalized;
        Vector3 endPos = startPos + awayDir * pushDistance
                         + new Vector3(0, -pushDropAmount, 0);

        Quaternion endRot = startRot * Quaternion.Euler(pushSpinAmount, 0, pushSpinAmount * 1.5f);

        SetEmissionColor(Color.red * glowIntensity);

        float elapsed = 0f;
        while (elapsed < pushDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / pushDuration);
            float eased = 1f - (1f - t) * (1f - t);

            transform.position = Vector3.Lerp(startPos, endPos, eased);
            transform.rotation = Quaternion.Lerp(startRot, endRot, eased);

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t * t);

            yield return null;
        }

        isAnimating = false;
        onComplete?.Invoke();
    }

    // Utility to set glow color
    private void SetEmissionColor(Color color)
    {
        if (plateRenderer == null) return;
        plateRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_EmissionColor", color);
        plateRenderer.SetPropertyBlock(propBlock);
    }
}