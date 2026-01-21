using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] private float viewRadius = 6f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private Transform eye;
    [SerializeField] private Transform player;

    public bool canSeePlayer { get; private set; }
    public Transform Player => player;

    private void Update()
    {
        canSeePlayer = false;

        Vector2 dirToPlayer = player.position - eye.position;
        float distance = dirToPlayer.magnitude;
        if (distance > viewRadius)
            return;

        Vector2 lookDir = dirToPlayer.normalized;

        // 👁 Eye rotation
        float rotZ = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        eye.rotation = Quaternion.Euler(0, 0, rotZ);

        // 🔥 DOT PRODUCT ANGLE CHECK
        float dot = Vector2.Dot(eye.up, lookDir);
        float threshold = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);
        if (dot < threshold)
            return;

        RaycastHit2D hit = Physics2D.Raycast(
            eye.position,
            lookDir,
            viewRadius,
            obstacleMask
        );

        Debug.DrawRay(
            eye.position,
            lookDir * viewRadius,
            hit ? Color.red : Color.green
        );

        if (!hit)
            canSeePlayer = true;
    }
}
