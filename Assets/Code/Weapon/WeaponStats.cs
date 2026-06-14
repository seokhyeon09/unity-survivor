using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    [Header("무기 기본 능력치")]
    public float baseDamage = 10f;     // 무기 자체 데미지
    public float fireRate = 0.5f;      // 연사 속도 (총알을 쏘는 간격, 초 단위)
    public float bulletSpeed = 15f;    // 총알이 날아가는 속도

    private float originalBaseDamage;
    private float originalFireRate;

    void Awake()
    {
        originalBaseDamage = baseDamage;
        originalFireRate = fireRate;
    }

    public void ResetStats()
    {
        baseDamage = originalBaseDamage;
        fireRate = originalFireRate;
    }

    // ----------------------------------------------------
    // 강화소에서 무기를 개조(강화)할 때 부르는 함수들
    // ----------------------------------------------------
    public void AddDamage(float amount)
    {
        baseDamage += amount;
        Debug.Log($"무기 데미지 증가! (현재 무기 데미지: {baseDamage})");
    }

    public void ReduceFireRate(float amount)
    {
        fireRate -= amount;
        // 연사 딜레이는 낮을수록 빨리 쏘는 것이므로, 0 이하로 내려가지 않게 최소값 방어
        fireRate = Mathf.Max(0.05f, fireRate);
        Debug.Log($"연사 속도 증가! (현재 발사 간격: {fireRate}초)");
    }
}