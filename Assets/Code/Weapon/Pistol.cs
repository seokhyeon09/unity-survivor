using UnityEngine;
public class Pistol : Weapon
{
    public override void Shoot(float finalDamage)
    {
        if (CanShoot())
        {
            AudioManager.Instance.PlaySFX("Pistol");
            // 발사 로직 처리 후, SpawnBullet을 부를 때 finalDamage도 같이 패스해줍니다!
            SpawnBullet(firePoint.position, firePoint.rotation, finalDamage);
            nextFireTime = Time.time + fireRate;
        }
    }
}