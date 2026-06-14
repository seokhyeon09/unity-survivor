using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("플레이어 기본 능력치")]
    public float maxHealth = 100f;   // 최대 체력
    public float moveSpeed = 5f;     // 이동 속도
    public float attackPower = 10f;  // 공격력

    // 원본 스탯 저장용
    private float originalMaxHealth;
    private float originalMoveSpeed;
    private float originalAttackPower;

    [Header("UI 연결 (강화 성공)")]
    public TMPro.TMP_Text upgradeSuccessText;

    public void ShowSuccessText(string message, float displayTime)
    {
        if (upgradeSuccessText != null)
        {
            StopAllCoroutines(); // 이미 떠있으면 타이머 초기화
            StartCoroutine(ShowSuccessTextCoroutine(message, displayTime));
        }
    }

    private IEnumerator ShowSuccessTextCoroutine(string message, float time)
    {
        if (upgradeSuccessText == null) yield break;
        upgradeSuccessText.text = message;
        
        // 캔버스가 꺼져있을 수 있으므로 부모도 켬
        if (upgradeSuccessText.transform.parent != null)
            upgradeSuccessText.transform.parent.gameObject.SetActive(true);
            
        upgradeSuccessText.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        upgradeSuccessText.gameObject.SetActive(false);
    }

    void Awake()
    {
        originalMaxHealth = maxHealth;
        if (upgradeSuccessText == null) {
            var tmps = GetComponentsInChildren<TMPro.TMP_Text>(true);
            foreach(var t in tmps) { if (t.gameObject.name == "SuccessText") { upgradeSuccessText = t; break; } }
        }
        originalMoveSpeed = moveSpeed;
        originalAttackPower = attackPower;
    }

    public void ResetStats()
    {
        maxHealth = originalMaxHealth;
        moveSpeed = originalMoveSpeed;
        attackPower = originalAttackPower;
        Debug.Log("플레이어 스탯이 초기화되었습니다.");
    }

    // ----------------------------------------------------
    // 강화소에서 능력치를 올릴 때 부르는 함수들
    // ----------------------------------------------------
    public void AddMaxHealth(float amount)
    {
        maxHealth += amount;
        Debug.Log($"최대 체력 증가! (현재: {maxHealth})");

        // 체력을 올려주면서, PlayerHealth 스크립트의 현재 체력도 채워주면 좋습니다.
        PlayerHealth health = GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.Heal(amount); // (참고: 이 Heal 함수는 아래 2단계에서 만들 겁니다)
        }
    }

    public void AddAttackPower(float amount)
    {
        attackPower += amount;
        Debug.Log($"공격력 증가! (현재: {attackPower})");
    }

    public void AddMoveSpeed(float amount)
    {
        moveSpeed += amount;
        Debug.Log($"이동 속도 증가! (현재: {moveSpeed})");
    }
}