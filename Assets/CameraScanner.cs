using UnityEngine;

public class CameraScanner : MonoBehaviour
{
    public float maxAngle = 45f;
    public float scanSpeed = 2f;

    private float startY;

    void Start()
    {
        startY = transform.eulerAngles.y;
    }

    void Update()
    {
        transform.Rotate(0f, 60f * Time.deltaTime, 0f);
    }
}
