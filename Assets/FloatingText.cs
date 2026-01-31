using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float floatSpeed = 1.2f;
    [SerializeField] private float lifeTime = 1.0f;

    private float timer;
    private Color startColor;

    private void Awake()
    {
        // Try find TMP text automatically
        if (text == null)
            text = GetComponentInChildren<TMP_Text>(true);

        if (text == null)
        {
            Debug.LogError("FloatingText: No TMP_Text found in children. Add a TMP text to the prefab and assign it.", this);
            enabled = false;
            return;
        }

        startColor = text.color;
    }

    public void SetText(string value)
    {
        if (text != null)
            text.text = value;
    }

    private void Update()
    {
        transform.position += Vector3.up * (floatSpeed * Time.deltaTime);

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / lifeTime);

        var c = startColor;
        c.a = Mathf.Lerp(startColor.a, 0f, t);
        text.color = c;

        if (timer >= lifeTime)
            Destroy(gameObject);
    }
}
