using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class DynamicYSorter : MonoBehaviour
{
    private SpriteRenderer sr;
    public int offset = 0;
    
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (sr != null)
        {
            // 단순하게 Transform Y축 기반 정렬 + offset 지원 (역방향 방지)
            sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100) + offset;
        }
    }
}