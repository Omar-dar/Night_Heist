using UnityEngine;
using UnityEngine.AI;

public class NPCWanderChase : MonoBehaviour
{
    public enum NPCState { Wander, Chase }

    public AudioSource scream;

    [Header("State")]
    public NPCState State = NPCState.Wander;

    [Header("References")]
    public Transform Player;

    [Header("Wander Mode")]
    [Tooltip("If assigned, NPC will patrol these waypoints instead of random wander.")]
    public Transform WaypointsRoot;

    [Tooltip("Go through waypoints in order. If false -> pick random waypoint each time.")]
    public bool PatrolInOrder = true;

    [Tooltip("Wait at waypoint before moving to the next.")]
    public float WaypointWaitTime = 1.5f;

    [Tooltip("If no waypoints are assigned, NPC will wander randomly within this radius.")]
    public float WanderRadius = 6f;

    [Tooltip("Wait time between random wander destinations.")]
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

    private Transform[] _waypoints;
    private int _currentWaypointIndex = 0;

    // ---- Flashlight hook ----
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
        if (_agent == null)
        {
            Debug.LogError("NPCWanderChase: NavMeshAgent missing!");
            enabled = false;
            return;
        }

        // Auto-find player by tag if not assigned
        if (Player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) Player = p.transform;
        }

        CacheWaypoints();

        _wanderTimer = 0f; // move immediately
        BeginWander();
    }

    void Update()
    {
        if (State == NPCState.Wander)
        {
            _agent.speed = WanderSpeed;

            // If we have waypoints -> patrol
            if (HasWaypoints())
            {
                HandleWaypointPatrol();
            }
            // else -> random wander
            else
            {
                HandleRandomWander();
            }
        }
        else // Chase
        {
            HandleChase();
        }

        UpdateAnimator();
    }

    // -----------------------------
    // Wander (Waypoints)
    // -----------------------------
    void HandleWaypointPatrol()
    {
        // Wait timer counts down when we reached destination
        if (!_agent.pathPending && _agent.remainingDistance <= Mathf.Max(_agent.stoppingDistance, 0.15f))
        {
            _wanderTimer -= Time.deltaTime;
            if (_wanderTimer <= 0f)
            {
                GoToNextWaypoint();
            }
        }
    }

    void GoToNextWaypoint()
    {
        if (!HasWaypoints()) return;

        Transform target;

        if (PatrolInOrder)
        {
            target = _waypoints[_currentWaypointIndex];
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
        }
        else
        {
            int idx = Random.Range(0, _waypoints.Length);
            target = _waypoints[idx];
            _currentWaypointIndex = idx;
        }

        if (target != null)
        {
            _agent.SetDestination(target.position);
            _wanderTimer = WaypointWaitTime;
        }
    }

    // -----------------------------
    // Wander (Random)
    // -----------------------------
    void HandleRandomWander()
    {
        _wanderTimer -= Time.deltaTime;

        bool reached = (!_agent.pathPending && _agent.remainingDistance <= Mathf.Max(_agent.stoppingDistance, 0.15f));

        if (reached && _wanderTimer <= 0f)
        {
            PickNewRandomWanderPoint();
            _wanderTimer = WanderWaitTime;
        }
    }

    void PickNewRandomWanderPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * WanderRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, WanderRadius, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
    }

    // -----------------------------
    // Chase
    // -----------------------------
    void HandleChase()
    {
        if (Player == null)
        {
            State = NPCState.Wander;
            BeginWander();
            return;
        }

        _agent.speed = ChaseSpeed;
        _agent.SetDestination(Player.position);

        _chaseTimer -= Time.deltaTime;
        if (_chaseTimer <= 0f)
        {
            State = NPCState.Wander;
            BeginWander();
        }
    }

    public void StartChase()
    {
        State = NPCState.Chase;
        _chaseTimer = ChaseDuration;

        if (scream != null)
            scream.Play();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    // -----------------------------
    // Helpers
    // -----------------------------
    void BeginWander()
    {
        CacheWaypoints();

        // Move immediately based on what we have
        if (HasWaypoints())
        {
            _wanderTimer = 0f;
            GoToNextWaypoint();
        }
        else
        {
            _wanderTimer = 0f;
            PickNewRandomWanderPoint();
        }
    }

    void CacheWaypoints()
    {
        if (WaypointsRoot == null)
        {
            _waypoints = null;
            return;
        }

        // collect children as waypoints (ignore the root itself)
        int childCount = WaypointsRoot.childCount;
        if (childCount <= 0)
        {
            _waypoints = null;
            return;
        }

        _waypoints = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
            _waypoints[i] = WaypointsRoot.GetChild(i);

        _currentWaypointIndex = 0;
    }

    bool HasWaypoints()
    {
        return _waypoints != null && _waypoints.Length > 0;
    }

    void UpdateAnimator()
    {
        if (_anim == null) return;

        // StarterAssetsThirdPerson expects these parameters
        float speed = _agent.velocity.magnitude;

        _anim.SetFloat("Speed", speed);
        _anim.SetFloat("MotionSpeed", speed > 0.05f ? 1f : 0f);
    }
}