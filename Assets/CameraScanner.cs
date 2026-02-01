using UnityEngine;

public class CameraScanner : MonoBehaviour
{
    public float maxAngle = 45f;
    public float scanSpeed = 0.1f;

    private float startY;

    void Start()
    {
        startY = transform.eulerAngles.y;
    }

    void Update()
    {
        float angle = Mathf.Sin(Time.time * scanSpeed) * maxAngle;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, startY + angle, transform.eulerAngles.z);
    }
}
