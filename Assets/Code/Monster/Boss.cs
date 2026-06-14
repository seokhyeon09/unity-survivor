using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    [Header("# Boss Stats")]
    public float health;
    public float maxHealth = 500f;
    public float speed = 2f;
    public float dashSpeed = 20f;
    public float damage = 20f;
    public float dashDamageMultiplier = 2f;
    public float giantDamageMultiplier = 1.5f;

    [Header("# Pattern Settings")]
    public float minPatternInterval = 3f;
    public float maxPatternInterval = 5f;
    bool isDashing = false;
    bool isGiant = false;
    bool isPatternRunning = false;

    [Header("# Giant Pattern Settings")]
    public float normalScale = 1f;
    public float giantScale = 1.8f;
    public float scaleSpeed = 3f;
    public float giantDuration = 10f;

    // ✅ 추가: 거대화 속도 배율
    public float giantSpeedMultiplier = 1.5f;

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

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) target = player.GetComponent<Rigidbody2D>();
        }
        if (!isLive) Init();
    }

    public void Init(float multiplier = 1f)
    {
        health = maxHealth * multiplier;
        isLive = true;

        if (rigid == null) rigid = GetComponent<Rigidbody2D>();
        rigid.simulated = true;
        rigid.gravityScale = 0;
        rigid.mass = 5f;

        spriter.color = Color.white;
        transform.localScale = new Vector3(normalScale, normalScale, 1f);
        isDashing = false;
        isGiant = false;
        isPatternRunning = false;

        StopAllCoroutines();
        StartCoroutine(PatternBrain());
    }

    void FixedUpdate()
    {
        if (!isLive || target == null || isDashing || isPatternRunning) return;

        Vector2 dirVec = target.position - rigid.position;

        // ✅ 거대화 상태일 때 속도 증가 적용
        float currentSpeed = speed;
        if (isGiant)
            currentSpeed *= giantSpeedMultiplier;

        Vector2 nextVec = dirVec.normalized * currentSpeed * Time.fixedDeltaTime;

        rigid.MovePosition(rigid.position + nextVec);
        rigid.linearVelocity = Vector2.zero;

        if (dirVec.x != 0)
        {
            spriter.flipX = dirVec.x < 0;
        }
    }

    IEnumerator PatternBrain()
    {
        yield return new WaitForSeconds(2f);

        while (isLive)
        {
            if (target == null) { yield return null; continue; }

            int randomPattern = Random.Range(0, 2);

            if (randomPattern == 0)
                yield return StartCoroutine(TwoStepDashRoutine());
            else
                yield return StartCoroutine(GiantRoutine());

            float randomInterval = Random.Range(minPatternInterval, maxPatternInterval);
            yield return new WaitForSeconds(randomInterval);
        }
    }

    IEnumerator TwoStepDashRoutine()
    {
        isPatternRunning = true;

        float hpRatio = health / maxHealth;

        yield return StartCoroutine(SingleDashAction());

        if (hpRatio <= 0.5f)
        {
            spriter.color = Color.white;
            isDashing = false;
            yield return new WaitForSeconds(0.6f);

            yield return StartCoroutine(SingleDashAction());
        }

        spriter.color = Color.white;
        isDashing = false;
        isPatternRunning = false;
    }

    IEnumerator SingleDashAction()
    {
        if (target == null || !isLive) yield break;

        Vector2 dashDirection = (target.position - rigid.position).normalized;

        spriter.color = new Color(1f, 0.2f, 0.2f);
        yield return new WaitForSeconds(0.6f);

        isDashing = true;
        float dashDuration = 0.5f;
        float timer = 0f;

        while (timer < dashDuration)
        {
            if (!isLive) yield break;
            timer += Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + (dashDirection * dashSpeed * Time.fixedDeltaTime));
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator GiantRoutine()
    {
        isPatternRunning = true;
        isGiant = true;

        float t = 0f;
        while (t < 1f)
        {
            if (!isLive) yield break;
            t += Time.deltaTime * scaleSpeed;
            float currentScale = Mathf.Lerp(normalScale, giantScale, t);
            transform.localScale = new Vector3(currentScale, currentScale, 1f);
            spriter.color = Color.Lerp(Color.white, new Color(1f, 0.4f, 0.2f), t);
            yield return null;
        }

        transform.localScale = new Vector3(giantScale, giantScale, 1f);
        spriter.color = new Color(1f, 0.4f, 0.2f);

        isPatternRunning = false;

        yield return new WaitForSeconds(giantDuration);

        isPatternRunning = true;

        t = 0f;
        while (t < 1f)
        {
            if (!isLive) yield break;
            t += Time.deltaTime * scaleSpeed;
            float currentScale = Mathf.Lerp(giantScale, normalScale, t);
            transform.localScale = new Vector3(currentScale, currentScale, 1f);
            spriter.color = Color.Lerp(new Color(1f, 0.4f, 0.2f), Color.white, t);
            yield return null;
        }

        transform.localScale = new Vector3(normalScale, normalScale, 1f);
        spriter.color = Color.white;

        isGiant = false;
        isPatternRunning = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isLive) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                float finalDamage = damage;

                if (isDashing)
                    finalDamage *= dashDamageMultiplier;
                else if (isGiant)
                    finalDamage *= giantDamageMultiplier;

                playerHealth.TakeDamage(finalDamage);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isLive || isDashing) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                float currentDamage = isGiant ? damage * giantDamageMultiplier : damage;
                playerHealth.TakeDamage(currentDamage * Time.deltaTime);
            }
        }
    }

    public void TakeDamage(float dmg)
    {
        if (!isLive) return;
        AudioManager.Instance.PlaySFX("MonsterHit");
        health -= dmg;
        if (anim != null) anim.SetTrigger("Hit");
        if (health <= 0) Die();
    }

    void Die()
    {
        if (!isLive) return;
        isLive = false;
        rigid.simulated = false;

        // 충돌체를 끕니다.
        Collider2D[] colls = GetComponents<Collider2D>();
        foreach (var c in colls) c.enabled = false;

        // 즉시 사망 모션 강제 재생
        if (anim != null) anim.SetBool("Dead", true);

        // 비석이 가려지지 않게 렌더링 순위를 가장 위로 강제 조정
        if (spriter != null)
        {
            spriter.sortingOrder = 999;
        }

        // 보스가 죽었음을 Spawner에 알림
        Spawner spawner = FindFirstObjectByType<Spawner>();
        if (spawner != null) spawner.ResetBossStatus();

        MonsterItemDrop itemDrop = GetComponent<MonsterItemDrop>();
        if (itemDrop != null)
        {
            itemDrop.DropItem();
        }

        Invoke("OnDead", 3f);
    }

    void OnDead() { Destroy(gameObject); }
}