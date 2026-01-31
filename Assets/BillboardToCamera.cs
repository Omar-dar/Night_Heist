using UnityEngine;

public class BillboardToCamera : MonoBehaviour
{
    void LateUpdate()
    {
        Camera cam = Camera.main;
        if (cam == null) cam = FindObjectOfType<Camera>();
        if (cam == null) return;

        transform.forward = cam.transform.forward;
    }
}
