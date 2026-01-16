using UnityEngine;

public class PistolWeapon : PlayerWeapons
{
    protected override void Shoot()
    {
        //Quaternion rotation = Quaternion.identity; altta rotation kullanıyorduk onu _firePoint.rotation a çevirdik.
        GameObject bullet = ObjectPooler.Instance.SpawnFromPool("mermi", _firePoint.position, _firePoint.rotation);

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 direction = (mouseWorld - _firePoint.position).normalized;
        bullet.GetComponent<BulletController>().Fire(direction);
    }
}
