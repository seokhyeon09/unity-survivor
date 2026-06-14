using UnityEngine;

public class DropItem : MonoBehaviour
{
    [Header("아이템 설정")]
    public ItemType itemType;
    public int itemValue = 1;

    [Header("시각 효과 (둥둥 떠다니기)")]
    public float bobbingSpeed = 3f;      // 떠다니는 속도
    public float bobbingHeight = 0.15f;  // 떠다니는 높이
    private Vector3 startPos;

    private bool isCollected = false;

    private void Start()
    {
        // 처음 생성된 위치를 기억해둡니다.
        startPos = transform.position;
    }

    private void Update()
    {
        // 시간에 따라 위아래로 부드럽게 움직이는 수학 공식을 적용합니다.
        float newY = startPos.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;

        if (collision.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                isCollected = true;
                AudioManager.Instance.PlaySFX("item");
                inventory.AddItem(itemType, itemValue);
                Destroy(gameObject);
            }
        }
    }
}