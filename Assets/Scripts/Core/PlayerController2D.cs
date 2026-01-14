using Unity.Mathematics;
using UnityEngine;

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
    


    private void Awake()
    {
        currentWeapon = GetComponentInChildren<PlayerWeapons>();

        if(currentWeapon == null)
        {
            Debug.Log("Weapon Script not found!");
        }
        _rb = GetComponent<Rigidbody2D>();
    }

    // Rigidbody ile hareket eden bir objede hareketi durdurmak istiyorsan mutlaka velocity = 0 yap
    private void Update()
    {
        if (_isMoving == true)
        {
            PlayerMovement();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            currentWeapon.Shoot();
            // Fire();
        }

        // Player Objesi Mouse Rotate ederken sürekli mouse a baksın istiyorsan bunu kullanma, Player in rotate i de dursun istiyorsan kullan
        // if (!_isMoving)
        // {
        //     _moveInput = Vector2.zero;
        //     return;
        // }

        RotateToMouse();
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

    private void RotateToMouse()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseWorldPos - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _rb.rotation = angle;
    }

    public void PlayerMovement()
    {
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");
        _moveInput = _moveInput.normalized;
    }

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
