using UnityEngine;

public class Highlightable : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Material highlightMaterial;

    private Material[] originalMats;

    private void Awake()
    {
        if (targetRenderer == null) targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer != null)
            originalMats = targetRenderer.materials;
    }

    public void SetHighlighted(bool on)
    {
        if (targetRenderer == null || highlightMaterial == null || originalMats == null) return;

        if (on)
        {
            // add highlight as extra material (works like overlay)
            var mats = targetRenderer.materials;
            if (mats.Length == originalMats.Length)
            {
                var newMats = new Material[mats.Length + 1];
                for (int i = 0; i < mats.Length; i++) newMats[i] = mats[i];
                newMats[newMats.Length - 1] = highlightMaterial;
                targetRenderer.materials = newMats;
            }
        }
        else
        {
            targetRenderer.materials = originalMats;
        }
    }
}
