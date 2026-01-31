using UnityEngine;
using UnityEngine.InputSystem;

public class ClickCollector : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float maxDistance = 4f;

    private Highlightable currentHighlight;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        // 1) Aim highlight (ray every frame)
        Ray ray = cam.ScreenPointToRay(Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : new Vector2(Screen.width / 2f, Screen.height / 2f));

        Highlightable newHighlight = null;

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
            newHighlight = hit.collider.GetComponentInParent<Highlightable>();

        if (currentHighlight != newHighlight)
        {
            if (currentHighlight != null) currentHighlight.SetHighlighted(false);
            currentHighlight = newHighlight;
            if (currentHighlight != null) currentHighlight.SetHighlighted(true);
        }

        // 2) Click to collect
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            var collectible = hit.collider.GetComponentInParent<Collectible>();
            if (collectible != null)
                collectible.Collect();
        }
    }
}
