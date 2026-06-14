using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    // public float maxHealth = 100f; <-- [삭제!] 이제 Stats가 관리합니다.
    public float currentHealth;

    public Image healthBarFill;
    private Animator anim;
    public bool isDead { get; private set; }

    [Header("피격 설정 (무적 시간)")]
    public float invincibilityDuration = 1f; // 무적 지속 시간
    private bool isInvincible = false;
    private SpriteRenderer spriter;

    private PlayerStats stats; // [추가] 능력치 매니저 서류철

    void Start()
    {
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>(); // 스프라이트 가져오기

        // [추가] 내 몸에 붙어있는 능력치 매니저를 찾아서 가져옴
        stats = GetComponent<PlayerStats>();

        // [수정됨] 최대 체력을 stats에서 읽어옵니다.
        currentHealth = stats.maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        // 이미 사망했거나, 무적 상태라면 데미지를 받지 않음
        if (isDead || isInvincible) return;

        AudioManager.Instance.PlaySFX("PlayerHit");

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, stats.maxHealth); // [수정됨] stats.maxHealth 사용

        UpdateHealthBar();

        if (currentHealth <= 0) 
        {
            Die();
        }
        else
        {
            // 체력이 남아있다면 무적 코루틴 시작
            StartCoroutine(InvincibilityRoutine());
        }
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        int flashCount = 5; // 깜빡임 횟수
        float flashDuration = invincibilityDuration / (flashCount * 2);

        for (int i = 0; i < flashCount; i++)
        {
            // 캐릭터를 반투명(또는 흰색 느낌)하게 만들어서 피격 효과 연출
            if (spriter != null) spriter.color = new Color(1f, 1f, 1f, 0.4f);
            yield return new WaitForSeconds(flashDuration);
            // 원래 상태로 복구
            if (spriter != null) spriter.color = Color.white;
            yield return new WaitForSeconds(flashDuration);
        }

        if (spriter != null) spriter.color = Color.white; // 혹시 모를 버그 방지용 확실한 원상복구
        isInvincible = false;
    }

    // [추가] 강화소에서 최대 체력이 늘어났을 때 현재 체력도 덤으로 채워주는 함수
    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, stats.maxHealth);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            // [수정됨] stats.maxHealth 사용
            healthBarFill.fillAmount = currentHealth / stats.maxHealth;
        }
    }

    void Die()
    {
        isDead = true;
        AudioManager.Instance.PlaySFX("Dead");
        if (anim != null) anim.SetTrigger("Death");

        WeaponController weaponController = GetComponentInChildren<WeaponController>();
        if (weaponController != null) weaponController.gameObject.SetActive(false);

        // [추가] 1.5초 뒤에 게임오버 UI 띄우고 주변 멈추기
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        // 쓰러지는 애니메이션을 볼 수 있도록 1.5초 대기
        yield return new WaitForSeconds(1.5f);

        // LevelManager를 통해 게임오버 UI 활성화 및 시간 정지
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.ShowGameOver();
        }
    }

    // [추가] 부활 로직
    public void Revive()
    {
        if (!isDead) return;

        isDead = false;

        // 애니메이션 초기화 (Idle 상태로 강제 복구)
        if (anim != null) anim.Rebind();

        // 무기 컨트롤러 다시 켜기
        WeaponController weaponController = GetComponentInChildren<WeaponController>(true);
        if (weaponController != null) weaponController.gameObject.SetActive(true);

        // 플레이어 스탯 리셋
        if (stats != null) stats.ResetStats();

        // 모든 무기 스탯 리셋
        if (weaponController != null)
        {
            foreach (Weapon weapon in weaponController.GetComponentsInChildren<Weapon>(true))
            {
                WeaponStats wStats = weapon.GetComponent<WeaponStats>();
                if (wStats != null) wStats.ResetStats();
                weapon.ResetWeapon();
            }
        }

        // 체력을 최대로 회복
        Heal(stats.maxHealth);

        // 텔레포트 적용
        GameObject target = GameObject.FindWithTag("TeleportTarget");
        if (target != null)
        {
            transform.position = target.transform.position;
            Debug.Log("부활: TeleportTarget 위치로 이동했습니다.");
        }
        else
        {
            Debug.Log("부활: TeleportTarget을 찾을 수 없어 제자리에서 부활합니다.");
        }
    }
}