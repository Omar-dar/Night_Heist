using UnityEngine;

public class PopupScale : MonoBehaviour
{
    [SerializeField] private float popDuration = 0.25f;
    [SerializeField] private float overshoot = 1.08f;

    private void OnEnable()
    {
        StopAllCoroutines();
        transform.localScale = Vector3.zero;
        StartCoroutine(Pop());
    }

    private System.Collections.IEnumerator Pop()
    {
        float t = 0f;

        // grow to overshoot
        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / popDuration);
            float s = Mathf.SmoothStep(0f, overshoot, a);
            transform.localScale = Vector3.one * s;
            yield return null;
        }

        // settle to 1
        t = 0f;
        while (t < popDuration * 0.6f)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / (popDuration * 0.6f));
            float s = Mathf.SmoothStep(overshoot, 1f, a);
            transform.localScale = Vector3.one * s;
            yield return null;
        }

        transform.localScale = Vector3.one;
    }
}
