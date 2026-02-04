using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float viewRadius = 6f;
    [SerializeField] private float viewAngle = 120f;

    [Tooltip("Player + Wall layerlarini içermeli")]
    [SerializeField] private LayerMask visionMask;
    [SerializeField] private Transform eye;
    [SerializeField] private Transform player;

    [Header("Memory Settings")]
    [SerializeField] private float memoryDuration = 0.5f;
    private float lastSeenTime = -Mathf.Infinity; // Eğer burada -Mathf.Infinity kullanmazsak 22. satırda olan 
                                                  // Time.time (0.02 veya 0.01) gibi bir süreyle başlayacağı için şöyle bir matematik işlemi olmuş oluyor
                                                  // Time.time (0.05) - lastSeenTime (0) = 0.05, 0.05 < memoryDuration (0.5) → TRUE
    public Vector2 LastKnownPosition { get; private set; }

    //Görüyor mu ya da hafızasında mı ? 
    public bool CanSeeOrRememberPlayer => canSeePlayer || Time.time - lastSeenTime < memoryDuration;

    public bool canSeePlayer { get; private set; }
    public Transform Player => player;

    private void Update()
    {
        canSeePlayer = false;

        // Enemy -> Player yönü
        Vector2 dirToPlayer = player.position - eye.position;
        float distance = dirToPlayer.magnitude;
        if (distance > viewRadius)
            return;

        Vector2 lookDir = dirToPlayer.normalized;

        // 👁 Eye rotation, eye empty objesini player yönüne döndür. Atan2 → yönü açıya çevirir
        float rotZ = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        eye.rotation = Quaternion.Euler(0, 0, rotZ);

        // 🔥 DOT PRODUCT ANGLE CHECK, görüş açısı kontrolü
        float dot = Vector2.Dot(eye.up, lookDir);
        float threshold = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);

        if (dot < threshold)
            return;

        // Asıl önemli kısım
        RaycastHit2D hit = Physics2D.Raycast(
            eye.position,
            lookDir,
            viewRadius,
            visionMask // Player + Wall
        );

        // Debug Çizgisi
        Debug.DrawRay(
            eye.position,
            lookDir * viewRadius,
            hit ? Color.red : Color.green
        );

        if (!hit.collider)
            return;
        // İlk çarpılan Ne ? 

        if (hit.collider.CompareTag("Player"))
        {
            canSeePlayer = true;
            lastSeenTime = Time.time;
            LastKnownPosition = player.position;
        }
        //Eğer Wall ise hiçbir şey yapma -> memory süresi bitince durur
    }
}
