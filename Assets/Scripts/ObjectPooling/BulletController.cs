using UnityEngine;


public class BulletController : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed = 10f;
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody2D _rb;
    private float spawnTime;

    // [SerializeField] private Camera mainCamera;
    // [SerializeField] private float screenLeft;
    // [SerializeField] private float screenRight;
    // [SerializeField] private float screenTop;
    // [SerializeField] private float screenBottom;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // mainCamera = Camera.main;

        // //Ekran Sınırlarını hesaplıyoruz(Mermi bu sınırları geçince kayboluyor)
        // screenLeft = mainCamera.transform.position.x - mainCamera.orthographicSize * mainCamera.aspect;
        // screenRight = mainCamera.transform.position.x + mainCamera.orthographicSize * mainCamera.aspect;
        // screenTop = mainCamera.transform.position.y + mainCamera.orthographicSize;
        // screenBottom = mainCamera.transform.position.y - mainCamera.orthographicSize;
    }

    public void Fire(Vector2 direction)
    {
        spawnTime = Time.time;

        _rb.linearVelocity = direction * _bulletSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // gameObject her enable olduğunda çalışıyor
    // private void OnEnable()
    // {

    //     spawnTime = Time.time;

    //     //Mouse pozisyonunu alıyoruz
    //     Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //     mousePos.z = 0f; // 2D olduğu için z = 0

    //     Vector2 direction = (mousePos - transform.position).normalized;

    //     // Hız veriyoruz (Yön x hız)
    //     _rb.linearVelocity = direction * _bulletSpeed;

    //     // Mermiyi mouse yönüne doğru döndür (görsel olarak güzel olur)
    //     float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //     transform.rotation = Quaternion.Euler(0f, 0f, angle);
    // }

    private void Update()
    {
        if (Time.time - spawnTime > lifeTime)
        {
            ReturnToPool();
        }

        // // Border control yapıyoruz: Ekran dışına çıkan mermileri queue(havuza) ya geri gönderiyoruz
        // if (transform.position.x > screenRight ||
        // transform.position.x < screenLeft ||
        // transform.position.y > screenTop ||
        // transform.position.y < screenBottom)
        // {
        //     ReturnToPool();
        // }
    }

    //Çarpışma sırasında ne olacağına karar veriyoruz (Düşmana çarparsa, duvara çarparsa vs.)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            //Düşmana hasar ver, vs.
            Debug.Log("Bullet registered");
            damageable.TakeDamage(1);
        }

        //Her çarpışmada queue'ya geri döner
        ReturnToPool();
    }

    // Havuza geri dönme fonksiyonu (ObjectPooler çağırır)
    private void ReturnToPool()
    {
        _rb.linearVelocity = Vector2.zero; //Hızı sıfırlıyoruz
        transform.rotation = Quaternion.identity; //Dönüşü sıfırlıyoruz

        ObjectPooler.Instance.ReturnToPool("mermi", gameObject);
    }

    private void OnDisable() // Ek: Havuza dönünce temizle
    {
        //Burası otomatik çalışıyor
        _rb.linearVelocity = Vector2.zero;
    }

}
