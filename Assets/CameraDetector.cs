using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    public Transform player;
    public float viewDistance = 7f;
    public float viewAngle = 45f;
    public AudioSource alarm;

    private bool alarmTriggered = false;

    void Update()
    {
        if (CanSeePlayer())
        {
            if (!alarmTriggered)
            {
                alarmTriggered = true;
                alarm.Play();
            }
        }
        else
        {
            alarmTriggered = false;
            alarm.Stop();
        }
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > viewDistance)
            return false;
        Debug.Log($"Distance check passed: Distance to player: {distance}");

        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > viewAngle / 2)
            return false;
        Debug.Log($"Angle check passed: Angle to player: {angle}");

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, viewDistance))
        {
            Debug.Log($"Raycast hit: {hit.transform.name}");

            if (hit.transform.CompareTag("Player"))
                Debug.Log($"Raycast hit the player!: {hit.transform.name}");
                return true;
        }

        return false;
    }


    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position;
        Vector3 dir = transform.forward;
        
        // Draw the beam line
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origin, dir * viewDistance);
        
        // Draw spheres along the path to show the actual detection volume
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        for (float d = 0; d <= viewDistance; d += 1f)
        {
            Gizmos.DrawWireSphere(origin + dir * d, 2f);
        }
        
        // Draw the end sphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin + dir * viewDistance, 2f);
    }
    #endif
}
