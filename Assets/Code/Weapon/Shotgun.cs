using UnityEngine;

public class Shotgun : Weapon
{
    [Header("샷건 특화 설정")]
    public int pelletCount = 5;      // 한 번에 나갈 파편(총알) 개수
    public float spreadAngle = 30f;  // 부채꼴로 퍼지는 전체 각도

    private int originalPelletCount;

    void Awake()
    {
        originalPelletCount = pelletCount;
    }

    public override void ResetWeapon()
    {
        pelletCount = originalPelletCount;
    }

    public override void Shoot(float finalDamage)
    {
        if (CanShoot())
        {
            AudioManager.Instance.PlaySFX("shotgun");
            // 부채꼴의 시작 각도를 구하기 위해 전체 각도의 절반을 뺍니다. (-15도 ~ +15도)
            float startAngle = -spreadAngle / 2f;

            // 총알 사이의 각도 간격을 계산합니다. (총알이 1개일 때 에러 방지)
            float angleStep = pelletCount > 1 ? spreadAngle / (pelletCount - 1) : 0f;

            // 설정한 파편 개수(pelletCount)만큼 반복해서 총알을 생성!
            for (int i = 0; i < pelletCount; i++)
            {
                // 이번 총알이 날아갈 각도 계산 (-15도, -7.5도, 0도, 7.5도, 15도...)
                float currentAngle = startAngle + (angleStep * i);

                // 총구의 원래 방향에 계산한 각도를 더해서 새로운 회전값을 만듭니다.
                Quaternion pelletRotation = firePoint.rotation * Quaternion.Euler(0, 0, currentAngle);

                // 방향이 틀어진 채로 총알 생성 & 합산 데미지 전달!
                SpawnBullet(firePoint.position, pelletRotation, finalDamage);
            }

            // 다음 발사 쿨타임 적용
            nextFireTime = Time.time + fireRate;
        }
    }
}