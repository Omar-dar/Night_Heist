using System.Collections;
using UnityEngine;

public class CameraGameOverTrigger : MonoBehaviour
{
    [Header("Detect")]
    public Transform player;
    public float viewDistance = 7f;
    [Range(1f, 179f)] public float viewAngle = 45f;

    [Header("Vision Blocking")]
    public LayerMask obstaclesMask;
    public Vector3 eyeOffset = new Vector3(0, 0.2f, 0);

    [Header("Alarm + Game Over")]
    public AudioSource alarm;              // drag AlarmSound AudioSource here
    public float gameOverDelay = 2f;       // delay before Game Over UI shows

    private bool triggered = false;

    void Update()
    {
        if (triggered) return;
        if (player == null || GameManager.Instance == null) return;

        Vector3 eyePos = transform.position + transform.TransformDirection(eyeOffset);
        Vector3 toPlayer = (player.position - eyePos);
        float dist = toPlayer.magnitude;
        if (dist > viewDistance) return;

        Vector3 dir = toPlayer.normalized;

        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > viewAngle * 0.5f) return;

        // if something blocks the view -> cannot see player
        if (Physics.Raycast(eyePos, dir, dist, obstaclesMask))
            return;

        // Seen!
        triggered = true;
        StartCoroutine(AlarmThenGameOver());
    }

    private IEnumerator AlarmThenGameOver()
    {
        // play alarm first (if assigned)
        if (alarm != null)
        {
            if (!alarm.isPlaying)
                alarm.Play();

            // If for any reason it didn't start playing, do NOT game over.
            // (This matches your request: only game over when alarm plays)
            yield return null; // wait 1 frame so isPlaying can update
            if (!alarm.isPlaying)
            {
                triggered = false; // allow detection again
                yield break;
            }
        }
        else
        {
            // No alarm assigned -> don't game over (because you asked alarm must happen)
            triggered = false;
            yield break;
        }

        // wait before showing Game Over screen
        float t = 0f;
        while (t < gameOverDelay)
        {
            t += Time.deltaTime;
            yield return null;
        }

        GameManager.Instance.GameOver();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}
