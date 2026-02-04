using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyVision vision;
    [SerializeField] private Transform player;
    private Rigidbody2D _rb;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 3f;
    private Vector2 lastKnownDirection = Vector2.right;

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float waitAtPointTime = 1f;

    private EnemyState currentState = EnemyState.Patrol;
    private int currentPatrolIndex = 0;
    private float waitTimer = 0;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (vision.CanSeeOrRememberPlayer)
        {
            Vector2 directionToPlayer = ((Vector2)vision.LastKnownPosition - (Vector2)transform.position).normalized;
            // 🔥 HER ZAMAN PLAYER'A DÖN
            if (directionToPlayer != Vector2.zero)
                transform.right = directionToPlayer;
        }

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Chase:
                ChasePlayer();
                break;

            case EnemyState.Search:
                SearchLastPosition();
                break;
        }

        StateTransition();

    }

    private void StateTransition()
    {
        if (vision.canSeePlayer)
        {
            currentState = EnemyState.Chase;
            return;
        }

        if (!vision.canSeePlayer && vision.CanSeeOrRememberPlayer)
        {
            currentState = EnemyState.Search;
            return;
        }

        if (!vision.canSeePlayer && !vision.CanSeeOrRememberPlayer)
        {
            currentState = EnemyState.Patrol;
        }
    }

    private void ChasePlayer()
    {
        Vector2 targetPos = vision.LastKnownPosition;
        //Vector2 direction = (vision.Player.position - transform.position).normalized;
        Vector2 direction = (targetPos - _rb.position).normalized;

        _rb.linearVelocity = direction * _moveSpeed;

        // if (direction.x > 0)
        // {
        //     transform.localScale = new Vector3(1, 1, 1);
        // }
        // else
        //     transform.localScale = new Vector3(-1, 1, 1);
    }

    private void Patrol()
    {
        Transform target = patrolPoints[currentPatrolIndex];
        float distance = Vector2.Distance(target.position, transform.position);

        // Hedefe ulaşmadıysa -> hareket et
        if (distance > 0.2f)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            _rb.linearVelocity = dir * _moveSpeed;
            waitTimer = 0f; //Haraket ediyorsa bekleme sıfırlanır
        }
        else
        {
            // Hedefe geldiği zaman tamamen dur
            _rb.linearVelocity = Vector2.zero;
            waitTimer += Time.fixedDeltaTime;
            
            if (waitTimer >= waitAtPointTime)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                waitTimer = 0f;
            }
        }
    }

    private void SearchLastPosition()
    {
        Vector2 dir = (vision.LastKnownPosition - (Vector2)transform.position).normalized;
        _rb.linearVelocity = dir * _moveSpeed;

        if (Vector2.Distance(transform.position, vision.LastKnownPosition) < 0.2f)
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }

}
