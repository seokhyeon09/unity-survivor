using UnityEngine;

// 모든 무기들이 상속받을 기본 클래스
public abstract class Weapon : MonoBehaviour
{
    [Header("무기 기본 설정")]
    // 💡 [참고]: 이전에 WeaponStats.cs를 만드셨다면, 거기에 damage, fireRate가 있으니 
    // 나중에는 WeaponStats 컴포넌트를 직접 참조하는 방식으로 합치셔도 됩니다!
    // 지금은 기존 구조를 존중하여 그대로 유지합니다.
    public float damage = 10f;          // 무기 자체 데미지 (WeaponStats와 병행 사용 가능)
    public float fireRate = 0.5f;       // 발사 쿨타임
    public float bulletSpeed = 15f;     // 총알 속도
    public GameObject bulletPrefab;     // 전용 총알 프리팹
    public Transform firePoint;         // 전용 총구 위치

    protected float nextFireTime = 0f;

    public bool CanShoot()
    {
        return Time.time >= nextFireTime;
    }

    // 특수 무기 능력치 초기화를 위한 가상 함수
    public virtual void ResetWeapon()
    {
    }

    // -------------------------------------------------------------------
    // 1. [수정됨] 괄호 안에 float finalDamage 를 받도록 입구를 뚫어줍니다.
    // -------------------------------------------------------------------
    public abstract void Shoot(float finalDamage);

    // -------------------------------------------------------------------
    // 2. [수정됨] 총알 생성 시, 기존의 고정 damage 대신 finalDamage를 전달받아 넘겨줍니다.
    // -------------------------------------------------------------------
    protected void SpawnBullet(Vector3 position, Quaternion rotation, float finalDamage)
    {
        GameObject bulletObj = Instantiate(bulletPrefab, position, rotation);

        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            // 전달받은 합산 데미지(플레이어 스탯 + 무기 스탯)를 총알에 세팅!
            bulletScript.Setup(finalDamage);
        }

        Rigidbody2D rb = bulletObj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = bulletObj.transform.right * bulletSpeed;
        }
    }
}