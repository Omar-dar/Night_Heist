using UnityEngine;
using UnityEngine.AI;

public class NPCWanderChase : MonoBehaviour
{
    public enum NPCState { Wander, Chase }
    [Header("State")]
    public NPCState State = NPCState.Wander;

    [Header("References")]
    public Transform Player;

    [Header("Wander")]
    public float WanderRadius = 6f;
    public float WanderWaitTime = 2f;

    [Header("Speeds")]
    public float WanderSpeed = 1.8f;
    public float ChaseSpeed = 4.5f;

    [Header("Chase")]
    public float ChaseDuration = 6f;

    private NavMeshAgent _agent;
    private Animator _anim;
    private float _wanderTimer;
    private float _chaseTimer;

    public void OnFlashlightHit()
{
    StartChase();
}


    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
    }

    void Start()
    {
        // safety
        if (_agent == null)
        {
            Debug.LogError("NPCWanderChase: NavMeshAgent missing!");
            enabled = false;
            return;
        }

        // If you forgot to assign Player, auto-find by tag
        if (Player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) Player = p.transform;
        }

        _wanderTimer = WanderWaitTime;
        PickNewWanderPoint(); // start moving immediately
    }

    void Update()
    {
        if (State == NPCState.Wander)
        {
            _agent.speed = WanderSpeed;

            _wanderTimer -= Time.deltaTime;

            // If reached destination OR timer finished, pick new
            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (_wanderTimer <= 0f)
                {
                    PickNewWanderPoint();
                    _wanderTimer = WanderWaitTime;
                }
            }
        }
        else // Chase
        {
            if (Player == null)
            {
                State = NPCState.Wander;
                return;
            }

            _agent.speed = ChaseSpeed;
            _agent.SetDestination(Player.position);

            _chaseTimer -= Time.deltaTime;
            if (_chaseTimer <= 0f)
            {
                State = NPCState.Wander;
                _wanderTimer = 0f; // forces new wander point
            }
        }

        // Drive StarterAssetsThirdPerson animator params (prevents sliding)
        if (_anim != null)
        {
            float speed = _agent.velocity.magnitude;
            _anim.SetFloat("Speed", speed);
            _anim.SetFloat("MotionSpeed", speed > 0.05f ? 1f : 0f);
        }
    }

    void PickNewWanderPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * WanderRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, WanderRadius, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
    }

    // Call this from your flashlight script when NPC is hit by light
    public void StartChase()
    {
        State = NPCState.Chase;
        _chaseTimer = ChaseDuration;
    }
}
