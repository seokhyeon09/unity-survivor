using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    [Header("아이템 효과 설정")]
    public float healAmount = 30f;
    public GameObject minePrefab;
    public string teleportTargetTag = "TeleportTarget";

    [Header("지뢰 설치 위치 조정")]
    public float mineOffsetY = -0.75f;

    private PlayerInventory inventory;
    private PlayerHealth playerHealth;

    void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseHealItem();
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseTeleportItem();
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseMineItem();
    }

    void UseHealItem()
    {
        if (inventory != null && inventory.UseItem(ItemType.HealItem, 1))
        {
            if (playerHealth != null)
            {
                AudioManager.Instance.PlaySFX("heal");
                playerHealth.Heal(healAmount);
                Debug.Log("회복약을 사용했습니다.");
            }
        }
        else Debug.Log("회복약이 부족합니다.");
    }

    void UseTeleportItem()
    {
        if (inventory != null && inventory.UseItem(ItemType.TeleportItem, 1))
        {
            GameObject target = GameObject.FindWithTag(teleportTargetTag);
            if (target != null)
            {
                AudioManager.Instance.PlaySFX("telepor");
                transform.position = target.transform.position;
                Debug.Log($"텔레포트 아이템 사용: {teleportTargetTag} 위치로 순간이동했습니다.");
            }
            else
            {
                Debug.LogWarning("텔레포트 목표물(TeleportTarget)을 씬에서 찾을 수 없습니다!");
            }
        }
        else Debug.Log("순간이동 아이템이 부족합니다.");
    }

    void UseMineItem()
    {
        if (inventory != null && inventory.UseItem(ItemType.MineItem, 1))
        {
            if (minePrefab != null)
            {
                AudioManager.Instance.PlaySFX("mine");
                Vector3 spawnPosition = transform.position + new Vector3(0f, mineOffsetY, 0f);
                Instantiate(minePrefab, spawnPosition, Quaternion.identity);
                Debug.Log("지뢰를 설치했습니다.");
            }
        }
        else Debug.Log("지뢰 아이템이 부족합니다.");
    }
}