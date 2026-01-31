using UnityEngine;

public class FlashlightDetector : MonoBehaviour
{
    [Header("Beam Settings")]
    public float range = 12f;
    public float sphereRadius = 0.25f;   // makes detection easier than thin ray
    public LayerMask hitMask = ~0;       // what can be hit (default everything)

    [Header("References")]
    public Light flashlightLight;        // drag your Spot Light here
    public NPCWanderChase npc;           // drag the NPC here

    private void Awake()
    {
        if (flashlightLight == null)
            flashlightLight = GetComponentInChildren<Light>();
    }

    private void Update()
    {
        if (flashlightLight == null || npc == null) return;

        // only detect when flashlight is ON
        if (!flashlightLight.enabled) return;

        Vector3 origin = flashlightLight.transform.position;
        Vector3 dir = flashlightLight.transform.forward;

        // spherecast makes it forgiving
        if (Physics.SphereCast(origin, sphereRadius, dir, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            // if we hit the NPC or one of its children
            if (hit.collider.GetComponentInParent<NPCWanderChase>() == npc)
            {
                npc.StartChase();
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (flashlightLight == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(flashlightLight.transform.position, flashlightLight.transform.forward * range);
    }
#endif
}
