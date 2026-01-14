using UnityEngine;

public abstract class PlayerWeapons : MonoBehaviour
{
    [Header("Base Weapon Settings")]
    [SerializeField] protected Transform _firePoint;
    [SerializeField] protected float _fireRate;


    protected float _lastFireTime;

    public virtual void TryShoot()
    {
        if (Time.time < _lastFireTime + _fireRate)
            return;

        _lastFireTime = Time.time;
        Shoot();
    }

    public abstract void Shoot();
}
