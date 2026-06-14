using UnityEngine;

public class StatueHealer : MonoBehaviour
{
    [Header("회복 설정")]
    [Tooltip("초당 회복될 체력량")]
    public float healAmountPerSecond = 10f;
    
    private float timer = 0f;

    void OnTriggerStay2D(Collider2D collision)
    {
        // 플레이어가 콜라이더 영역 안에 머물 때
        if (collision.CompareTag("Player"))
        {
            timer += Time.deltaTime;
            
            // 1초마다 회복
            if (timer >= 1f)
            {
                PlayerHealth health = collision.GetComponent<PlayerHealth>();
                PlayerStats stats = collision.GetComponent<PlayerStats>();
                
                if (health != null && stats != null && health.currentHealth < stats.maxHealth)
                {
                    AudioManager.Instance.PlaySFX("heal");
                    health.Heal(healAmountPerSecond);
                    Debug.Log($"석상의 힘으로 체력이 {healAmountPerSecond} 회복되었습니다!");
                }
                timer = 0f; // 타이머 초기화
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // 플레이어가 영역 밖으로 나가면 타이머 초기화
        if (collision.CompareTag("Player"))
        {
            timer = 0f;
        }
    }
}
