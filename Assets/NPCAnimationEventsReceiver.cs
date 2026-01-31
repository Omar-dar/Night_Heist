using UnityEngine;

public class NPCAnimationEventsReceiver : MonoBehaviour
{
    // Matches the animation event name "OnFootstep"
    private void OnFootstep(AnimationEvent animationEvent)
    {
        // do nothing (or add footstep audio later)
    }

    // Matches "OnLand" if any clip has it
    private void OnLand(AnimationEvent animationEvent)
    {
        // do nothing
    }
}
