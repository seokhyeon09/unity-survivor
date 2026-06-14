using UnityEngine;

public class Bullet : MonoBehaviour
{
    float damage;

    public void Setup(float damageValue)
    {
        damage = damageValue;
    }

    // [추가된 기능] 총알이 화면(카메라 시야) 밖으로 나가면 호출됨
    private void OnBecameInvisible()
    {
        Destroy(gameObject); // 화면 밖으로 나가면 삭제하여 메모리 관리
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 일반 적 타격
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
            
            // DummyMonster(튜토리얼 적) 확인
            DummyMonster dummy = collision.GetComponent<DummyMonster>();
            if (dummy != null)
            {
                dummy.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }
        // 2. 보스 타격
        else if (collision.CompareTag("Boss"))
        {
            Boss boss = collision.GetComponent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }
    }
}