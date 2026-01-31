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
            // Basic null checks
            if (flashlightLight == null)
            {
                return;
            }
            
            if (npc == null)
            {
                return;
            }
            
            if (!flashlightLight.enabled)
            {
                return;
            }

            Vector3 origin = flashlightLight.transform.position;
            Vector3 dir = flashlightLight.transform.forward;

            // Try the SphereCast
            bool didHit = Physics.SphereCast(origin, sphereRadius, dir, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore);
            
            if (didHit)
            {
                // Check what component it has
                NPCWanderChase hitNPC = hit.collider.GetComponentInParent<NPCWanderChase>();
                
                if (hitNPC != null)
                {
                    if (hitNPC == npc)
                    {
                        npc.StartChase();
                    }
                }
            }
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (flashlightLight == null) return;
            
            Vector3 origin = flashlightLight.transform.position;
            Vector3 dir = flashlightLight.transform.forward;
            
            // Draw the beam line
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(origin, dir * range);
            
            // Draw spheres along the path to show the actual detection volume
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            for (float d = 0; d <= range; d += 1f)
            {
                Gizmos.DrawWireSphere(origin + dir * d, sphereRadius);
            }
            
            // Draw the end sphere
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(origin + dir * range, sphereRadius);
        }
        #endif
    }
