using UnityEngine;
using System.Collections.Generic;

public class Explosion : MonoBehaviour
{
    public float destroyTime = 0.5f;
    private float explosionDamage;
    
    // 이미 데미지를 입은 몬스터를 기억해서 중복 데미지 방지
    private HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();

    public void Setup(float damage, float scaleSize)
    {
        explosionDamage = damage;
        // 지뢰(Mine.cs)에서 넘겨준 크기로 프리팹(시각적 크기와 콜라이더 크기)을 키움
        transform.localScale = new Vector3(scaleSize, scaleSize, 1f);

        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        // 이미 데미지를 준 대상이면 무시
        if (hitTargets.Contains(hit)) return;

        // 일반 몬스터(Enemy) 타격
        if (hit.CompareTag("Enemy"))
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage);
                hitTargets.Add(hit);
            }
        }
        // 보스(Boss) 타격
        else if (hit.CompareTag("Boss"))
        {
            Boss boss = hit.GetComponent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(explosionDamage);
                hitTargets.Add(hit);
            }
        }
        // 더미 몬스터(Monster) 타격
        else if (hit.CompareTag("Monster"))
        {
            DummyMonster dummy = hit.GetComponent<DummyMonster>();
            if (dummy != null)
            {
                dummy.TakeDamage(explosionDamage);
                hitTargets.Add(hit);
            }
        }
    }
}