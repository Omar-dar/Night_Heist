using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class FlashlightController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag the Spot Light component here (your Flashlight object)")]
    public Light flashlight;
    
    [Header("Mouse Aim")]
    public Camera aimCamera;
    
    [Header("Toggle")]
    public Key toggleKey = Key.F;
    
    [Header("Cone Constraints")]
    [Tooltip("Maximum angle the light can deviate from camera forward (in degrees)")]
    public float maxConeAngle = 45f;
    
    [Tooltip("Smooth rotation speed")]
    public float rotationSmoothSpeed = 10f;
    
    private bool _isOn = true;
    private Quaternion _targetRotation;

    void Start()
    {
        if (aimCamera == null)
        {
            aimCamera = Camera.main;
        }
        
        if (flashlight == null)
        {
            flashlight = GetComponentInChildren<Light>();
        }
        
        if (flashlight != null)
            _isOn = flashlight.enabled;
            
        _targetRotation = transform.rotation;
    }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current == null || aimCamera == null) return;

        // Toggle flashlight
        if (Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame && flashlight != null)
        {
            _isOn = !_isOn;
            flashlight.enabled = _isOn;
        }

        // Get mouse position and create ray
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = aimCamera.ScreenPointToRay(mousePosition);
        
        // Calculate desired direction (where mouse is pointing)
        Vector3 desiredDirection = ray.direction;
        
        // Get camera forward direction (center of cone)
        Vector3 cameraForward = aimCamera.transform.forward;
        
        // Calculate angle between desired direction and camera forward
        float angleFromCenter = Vector3.Angle(cameraForward, desiredDirection);
        
        // Constrain within cone
        Vector3 constrainedDirection;
        if (angleFromCenter > maxConeAngle)
        {
            // Clamp to cone edge
            // Project onto cone surface
            Vector3 axis = Vector3.Cross(cameraForward, desiredDirection);
            constrainedDirection = Quaternion.AngleAxis(maxConeAngle, axis) * cameraForward;
        }
        else
        {
            // Within cone, use desired direction
            constrainedDirection = desiredDirection;
        }
        
        // Calculate target rotation
        _targetRotation = Quaternion.LookRotation(constrainedDirection);
        
        // Smoothly rotate towards target
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            _targetRotation, 
            rotationSmoothSpeed * Time.deltaTime
        );
#endif
    }
}