using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerController2D playerMovement;

    [Header("Camera Settings")]
    [SerializeField] private float lookAheadDistance = 5f;
    [SerializeField] private float lookSmooth = 8f;

    private readonly Vector3 baseOffset = new Vector3(0, 0, -10);
    private Vector3 currentLookOffset;

    private void LateUpdate()
    {

        if (!player) return;

        Vector3 lookOffset = Vector3.zero;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerMovement.SetMoving(false);


            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            Vector3 dir = mouseWorld - player.position;

            // 🔒 CLAMP — kamera artık player’dan kopamaz
            lookOffset = Vector3.ClampMagnitude(dir, lookAheadDistance);
        }
        else
        {
            playerMovement.SetMoving(true);
        }

        // SADECE look offset yumuşatılır
        currentLookOffset = Vector3.Lerp(
            currentLookOffset,
            lookOffset,
            lookSmooth * Time.deltaTime
        );

        // 🚫 DRIFT YOK
        transform.position = player.position + currentLookOffset + baseOffset;
    }
}
