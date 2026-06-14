using UnityEngine;

public class CraftingStation : MonoBehaviour, IInteractable
{
    [Header("제작할 아이템 설정")]
    [Tooltip("어떤 아이템을 만들 것인지 선택합니다.")]
    public ItemType craftItemType = ItemType.HealItem; 
    
    [Tooltip("한 번 상호작용 시 몇 개를 얻을 것인지 설정합니다.")]
    public int craftAmount = 1;                        

    [Header("UI 설정")]
    public float successTextDisplayTime = 2f; // 텍스트가 표시될 시간 (초)

    [Header("필요한 재료 설정")]
    public int requiredMaterialA = 1; // 톱니
    public int requiredMaterialB = 0; // 철판
    public int requiredMaterialC = 0; // 화약

    public void Interact(GameObject player)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory == null) return;

        // 재료가 모두 충분한지 검사
        if (inventory.matACount >= requiredMaterialA &&
            inventory.matBCount >= requiredMaterialB &&
            inventory.matCCount >= requiredMaterialC)
        {
            // 1. 재료 소모
            if (requiredMaterialA > 0) inventory.UseItem(ItemType.MaterialA, requiredMaterialA);
            if (requiredMaterialB > 0) inventory.UseItem(ItemType.MaterialB, requiredMaterialB);
            if (requiredMaterialC > 0) inventory.UseItem(ItemType.MaterialC, requiredMaterialC);

            // 2. 결과물 아이템 지급
            AudioManager.Instance.PlaySFX("Upgrade");
            inventory.AddItem(craftItemType, craftAmount);
            
            Debug.Log($"[{craftItemType}] 제작 성공! (+{craftAmount}개)");

            // 3. UI 텍스트 출력 ("Success!")
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                // PlayerStats.cs를 수정하여 메시지를 자유롭게 띄울 수 있게 만들었습니다.
                playerStats.ShowSuccessText("Success!", successTextDisplayTime);
            }
        }
        else
        {
            Debug.Log("재료가 부족하여 아이템을 제작할 수 없습니다.");
            PlayerStats ps = player.GetComponent<PlayerStats>();
            if (ps != null) ps.ShowSuccessText("<color=red>Not Enough!</color>", 1f);
        }
    }
}
