using UnityEngine;

public class MachineGun : Weapon
{
    [Header("기관총 특화 설정")]
    public float inaccuracy = 5f; // 연사 시 탄퍼짐(오차) 각도

    public override void Shoot(float finalDamage)
    {
        if (CanShoot())
        {
            AudioManager.Instance.PlaySFX("MachineGun");
            // 탄착군을 약간 흩어지게 하기 위한 랜덤 오차 적용
            float randomSpread = Random.Range(-inaccuracy, inaccuracy);
            Quaternion spreadRotation = firePoint.rotation * Quaternion.Euler(0, 0, randomSpread);

            // [수정됨] firePoint.rotation 대신 계산해둔 spreadRotation을 넣습니다!
            SpawnBullet(firePoint.position, spreadRotation, finalDamage);

            nextFireTime = Time.time + fireRate;
        }
    }
}