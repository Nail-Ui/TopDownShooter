using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private EnemyVision vision;
    private Rigidbody2D _rb;
    private Vector2 lastKnownDirection = Vector2.right;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 directionToPlayer = (vision.Player.position - transform.position).normalized;

        // 🔥 HER ZAMAN PLAYER'A DÖN
        if (directionToPlayer != Vector2.zero)
        {
            lastKnownDirection = directionToPlayer;
            transform.right = lastKnownDirection;
        }

        if (!vision.canSeePlayer)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        ChasePlayer();
    }

    private void ChasePlayer()
    {
        Vector2 direction = (vision.Player.position - transform.position).normalized;
        _rb.linearVelocity = lastKnownDirection * _moveSpeed;

        transform.right = direction;


        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

}
