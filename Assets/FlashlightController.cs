using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class FlashlightController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag the Spot Light component here (your Flashlight object)")]
    public Light flashlight;

    [Header("Toggle")]
    public Key toggleKey = Key.F;

    [Header("Rotation Keys")]
    public Key rotateLeftKey = Key.Q;
    public Key rotateRightKey = Key.E;

    [Tooltip("Optional: tilt up/down")]
    public Key tiltUpKey = Key.R;
    public Key tiltDownKey = Key.T;

    [Header("Rotation Settings")]
    [Tooltip("Degrees per second")]
    public float rotateSpeed = 90f;

    [Tooltip("Clamp left/right (yaw)")]
    public float yawLimit = 70f;

    [Tooltip("Clamp up/down (pitch). Negative looks down, positive looks up.")]
    public float pitchUpLimit = 35f;
    public float pitchDownLimit = 35f;

    private bool _isOn = true;

    private float _yaw;   // left/right
    private float _pitch; // up/down

    void Start()
    {
        if (flashlight == null)
        {
            flashlight = GetComponentInChildren<Light>();
        }

        // Start from current local rotation
        Vector3 e = transform.localEulerAngles;
        _pitch = NormalizeAngle(e.x);
        _yaw = NormalizeAngle(e.y);

        if (flashlight != null)
            _isOn = flashlight.enabled;
    }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current == null) return;

        // ---- Toggle ON/OFF ----
        if (Keyboard.current[toggleKey].wasPressedThisFrame && flashlight != null)
        {
            _isOn = !_isOn;
            flashlight.enabled = _isOn;
        }

        // ---- Rotate ----
        float yawInput = 0f;
        if (Keyboard.current[rotateLeftKey].isPressed) yawInput -= 1f;
        if (Keyboard.current[rotateRightKey].isPressed) yawInput += 1f;

        float pitchInput = 0f;
        if (Keyboard.current[tiltUpKey].isPressed) pitchInput -= 1f;    // look up (negative pitch in Unity)
        if (Keyboard.current[tiltDownKey].isPressed) pitchInput += 1f;  // look down

        _yaw += yawInput * rotateSpeed * Time.deltaTime;
        _pitch += pitchInput * rotateSpeed * Time.deltaTime;

        // clamp
        _yaw = Mathf.Clamp(_yaw, -yawLimit, yawLimit);
        _pitch = Mathf.Clamp(_pitch, -pitchUpLimit, pitchDownLimit);

        // apply
        transform.localRotation = Quaternion.Euler(_pitch, _yaw, 0f);

#else
        // If you are NOT using the new Input System, you can add legacy input here.
#endif
    }

    private float NormalizeAngle(float angle)
    {
        // Converts 0..360 into -180..180
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }
}
