using UnityEngine;

[System.Serializable]
public class DropData
{
    [Tooltip("드랍할 아이템 프리팹")]
    public GameObject itemPrefab;
    
    [Tooltip("드랍 확률 (0.0 ~ 1.0)")]
    [Range(0f, 1f)] public float dropRate = 0.3f;
    
    [Tooltip("한 번에 드랍할 개수")]
    public int dropCount = 1;
}

public class MonsterItemDrop : MonoBehaviour
{
    [Header("# Loot Table")]
    public DropData[] dropTable;

    // 몬스터가 죽을 때 실행할 함수
    public void DropItem()
    {
        if (dropTable == null) return;

        foreach (DropData drop in dropTable)
        {
            if (drop.itemPrefab == null) continue;

            // 0.0 ~ 1.0 사이의 랜덤한 숫자 뽑기
            float ran = Random.Range(0f, 1f);

            // 뽑은 숫자가 드랍 확률보다 낮거나 같으면 아이템 생성
            if (ran <= drop.dropRate)
            {
                for (int i = 0; i < drop.dropCount; i++)
                {
                    // 여러 개 떨어질 때 겹치지 않게 오프셋 적용
                    Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0f);
                    Instantiate(drop.itemPrefab, transform.position + randomOffset, Quaternion.identity);
                }
            }
        }
    }
}