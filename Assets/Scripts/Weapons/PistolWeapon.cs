using UnityEngine;

public class PistolWeapon : PlayerWeapons
{
    public override void Shoot()
    {

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 direction = (mouseWorld - _firePoint.position).normalized;
        Quaternion rotation = Quaternion.identity;
        
        GameObject bullet = ObjectPooler.Instance.SpawnFromPool("mermi", _firePoint.position, rotation);
        
        bullet.GetComponent<BulletController>().Fire(direction);
    }
}
