using UnityEngine;

public class DummyMonster : MonoBehaviour
{
    [Header("몬스터 상태 설정")]
    public float hp = 100f;
    public float attackDamage = 10f;

    // [추가됨] 중복 사망 방지용 스위치
    private bool isDead = false;
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float damageAmount)
    {
        // 1. 이미 죽은 상태면, 더 이상 데미지를 안 받게 튕겨냅니다! (총알 2~5번째 관통 무시)
        if (isDead) return;

        AudioManager.Instance.PlaySFX("MonsterHit");

        hp -= damageAmount;
        if (anim != null) anim.SetTrigger("Hit");
        Debug.Log("몬스터 피격! 남은 체력: " + hp);

        if (hp <= 0)
        {
            // 2. 체력이 0이 되는 순간 사망 스위치를 켭니다.
            isDead = true;
            Die();
        }
    }

    void Die()
    {
        Debug.Log("몬스터 사망!");

        // 사망 애니메이션 실행
        if (anim != null) anim.SetBool("Dead", true);
        
        // 물리 충돌 제거 (시체가 밀리거나 플레이어를 막지 않게)
        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null) coll.enabled = false;
        
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        if (rigid != null) rigid.simulated = false;

        MonsterItemDrop dropper = GetComponent<MonsterItemDrop>();
        if (dropper != null)
        {
            dropper.DropItem(); 
        }

        // 비석(사망 모션)을 1초간 보여준 뒤 삭제
        Invoke("OnDead", 1f);
    }

    void OnDead()
    {
        Destroy(gameObject);
    }

    // ----------------------------------------------------
    // 2. 공격 로직 (플레이어와 물리적으로 부딪혔을 때 실행)
    // ----------------------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("누군가와 부딪힘! 대상 이름: " + collision.gameObject.name);
            // 부딪힌 대상의 태그가 "Player"인지 확인
            if (collision.gameObject.CompareTag("Player"))
            {
                // 플레이어의 PlayerHealth 스크립트를 가져와서 데미지 전달
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                    Debug.Log($"몬스터가 플레이어를 때렸습니다! (-{attackDamage})");
                }
            }
        }
}