using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float _speed;

    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    public bool _isMoving = true;

    //[SerializeField] private float _bulletSpeed;
    [SerializeField] private PlayerWeapons currentWeapon;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float firePointDistance = 0.6f;
    private Animator animator;



    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentWeapon = GetComponentInChildren<PlayerWeapons>();


        if (currentWeapon == null)
        {
            Debug.Log("Weapon Script not found!");
        }
    }

    // Rigidbody ile hareket eden bir objede hareketi durdurmak istiyorsan mutlaka velocity = 0 yap
    private void Update()
    {
        if (_isMoving == true)
        {
            PlayerMovement();
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            currentWeapon.TryShoot();
            // Fire();
        }

        // Player Objesi Mouse Rotate ederken sürekli mouse a baksın istiyorsan bunu kullanma, Player in rotate i de dursun istiyorsan kullan
        // if (!_isMoving)
        // {
        //     _moveInput = Vector2.zero;
        //     return;
        // }

        UpdateAim();
        UpdateFirePoint();
        //RotateToMouse();
    }

    // Rigidbody ile hareket eden bir objede hareketi durdurmak istiyorsan mutlaka velocity = 0 yap
    private void FixedUpdate()
    {
        if (!_isMoving)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        _rb.linearVelocity = _moveInput * _speed;
    }

    private void UpdateAim()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 aimDir = (mouseWorld - transform.position).normalized;

        animator.SetFloat("AimX", aimDir.x);
        animator.SetFloat("AimY", aimDir.y);
    }

    private void UpdateFirePoint()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 direction = (mouseWorld - transform.position).normalized;

        //firePoint Pozisyonu
        firePoint.position = (Vector2)transform.position + direction * firePointDistance;

        //FirePoint Rotasyonu (2D)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    // GameObject komple mouse a doğru dönüyor
    // private void RotateToMouse()
    // {
    //     Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //     Vector2 direction = mouseWorldPos - transform.position;

    //     float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //     _rb.rotation = angle;
    // }

    public void PlayerMovement()
    {
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");
        _moveInput = _moveInput.normalized;

        UpdateMovementAnimation(_moveInput);

        // animator.SetFloat("InputX", _moveInput.x);
        // animator.SetFloat("InputY", _moveInput.y);
        // animator.SetBool("isWalking", _moveInput != Vector2.zero);

        // if (_moveInput != Vector2.zero)
        // {
        //     animator.SetBool("isWalking", false);
        //     animator.SetFloat("LastInputX", _moveInput.x);
        //     animator.SetFloat("LastInputY", _moveInput.y);
        // }
    }

    private void UpdateMovementAnimation(Vector2 moveInput)
    {
        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.y);
        animator.SetBool("isMoving", moveInput != Vector2.zero);

        if (moveInput != Vector2.zero)
        {
            animator.SetFloat("LastMoveX", moveInput.x);
            animator.SetFloat("LastMoveY", moveInput.y);
        }
    }

    //Yeni input sistemine göre yapılabilir Unity Input Player Input component’inde: //Behavior: Invoke Unity Events Move action → OnMove seçilmesi gerekiyor.
    //Bu metod sadece Input System tarafından çağrılır, Update() içinde asla çağırma
    // public void OnMove(InputAction.CallbackContext context) 
    // {
    //     _moveInput = context.ReadValue<Vector2>();
    //     _moveInput = _moveInput.normalized;

    //     animator.SetFloat("InputX", _moveInput.x);
    //     animator.SetFloat("InputY", _moveInput.y);

    //     animator.SetBool("isWalking", _moveInput != Vector2.zero);

    //     if (context.canceled)
    //     {
    //         animator.SetFloat("LastInputX", _moveInput.x);
    //         animator.SetFloat("LastInputY", _moveInput.y);
    //     }
    // }

    // Rigidbody ile hareket eden bir objede hareketi durdurmak istiyorsan mutlaka velocity = 0 yap 
    public void SetMoving(bool val)
    {
        _isMoving = val;

        if (!_isMoving)
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }

    private void Fire()
    {
        GameObject _mermi = ObjectPooler.Instance.SpawnFromPool("mermi", firePoint.position, Quaternion.identity);

        if (_mermi != null)
        {
            Debug.Log("Spawn pozisyonu: " + _mermi.transform.position);
            //_mermi.transform.position = new Vector3(0, 0, 0);   // ← geçici olarak merkeze taşı testi
        }
    }
}
