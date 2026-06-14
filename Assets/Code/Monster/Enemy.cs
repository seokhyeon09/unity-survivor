using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("# Stats")]
    public float speed;
    public float health;
    public float maxHealth;
    public float attackDamage = 10f; // 충돌 시 데미지

    [Header("# Reward")]
    public int expValue;

    [Header("# Visuals")]
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;
    public bool isLive;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    public void Init(SpawnData data)
    {
        if (data.spriteIndex < animCon.Length)
            anim.runtimeAnimatorController = animCon[data.spriteIndex];

        speed = data.speed;
        maxHealth = data.health;
        health = data.health;

        float timeBonus = GameManager.instance.gameTime / 60f; // 1분당 1씩 증가
        float healthMultiplier = 1f + (timeBonus * 0.2f); // 분당 20% 복리/단리 설정

        maxHealth = data.health * healthMultiplier;

        health = maxHealth;
        isLive = true;
        rigid.simulated = true;
        spriter.color = Color.white;
        anim.SetBool("Dead", false);
    }

    void FixedUpdate()
    {
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;

        // 플레이어 쪽으로 밀고 들어가는 이동
        rigid.MovePosition(rigid.position + nextVec);
        rigid.linearVelocity = Vector2.zero; // 물리 반동으로 인한 밀림 방지
    }

    void LateUpdate()
    {
        if (!isLive) return;
        spriter.flipX = target.position.x < rigid.position.x;
    }

    public void TakeDamage(float damage)
    {
        if (!isLive) return;

        AudioManager.Instance.PlaySFX("MonsterHit");

        health -= damage;

        if (health > 0)
        {
            anim.SetTrigger("Hit");
            // 넉백 로직 제거 (밀리지 않음)
        }
        else
        {
            Die();
        }
    }

    void Die()
    {
        if (!isLive) return;
        isLive = false;
        rigid.simulated = false;

        // 충돌체를 꺼서 Reposition(텔레포트)나 추가 피격을 방지합니다.
        Collider2D[] colls = GetComponents<Collider2D>();
        foreach (var c in colls) c.enabled = false;

        // 트랜지션 지연 없이 즉시 사망 애니메이션(비석)을 강제 재생합니다.
        if (anim != null) anim.SetBool("Dead", true);

        // 비석이 다른 오브젝트나 맵에 가려지지 않게 렌더링 순위를 가장 위로 강제 조정
        if (spriter != null)
        {
            spriter.sortingOrder = 999;
        }

        Spawner.killCount++;
        Spawner.currentMonsterCount--;

        if (GameManager.instance != null)
        {
            GameManager.instance.GetKill();
        }

        MonsterItemDrop itemDrop = GetComponent<MonsterItemDrop>();
        if (itemDrop != null)
        {
            itemDrop.DropItem();
        }

        Invoke("OnDead", 3f);
    }

    void OnDead()
    {
        Destroy(gameObject);
    }

    // ----------------------------------------------------
    // 1. 처음 부딪혔을 때 즉시 데미지
    // ----------------------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isLive) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log("플레이어와 충돌! 즉시 데미지 발생");
            }
        }
    }

    // ----------------------------------------------------
    // 2. 부딪히고 있는 동안 실시간 지속 데미지
    // ----------------------------------------------------
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isLive) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // 초당 attackDamage만큼 깎이도록 설계
                playerHealth.TakeDamage(attackDamage * Time.deltaTime);
            }
        }
    }
}