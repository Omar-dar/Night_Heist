using UnityEngine;

public class DoorInteractEnter : MonoBehaviour
{
    [Header("Animator")]
    public Animator doorAnimator;

    [Tooltip("Animation state name to play when opening")]
    public string openStateName = "Opening";

    [Tooltip("Animation state name to play when closing")]
    public string closeStateName = "Closing";

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.Return; // Enter
    public float interactDistance = 2.0f;

    [Header("Player")]
    public Transform player;

    private bool _isOpen;

    private void Start()
    {
        // Auto-find Player by tag if not assigned
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (doorAnimator == null)
        {
            doorAnimator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (player == null || doorAnimator == null) return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist <= interactDistance && Input.GetKeyDown(interactKey))
        {
            ToggleDoor();
        }
    }

    private void ToggleDoor()
    {
        _isOpen = !_isOpen;

        if (_isOpen)
            doorAnimator.Play(openStateName, 0, 0f);
        else
            doorAnimator.Play(closeStateName, 0, 0f);
    }
}
