using UnityEngine;
using TMPro;

public enum ItemType
{
    MaterialA,
    MaterialB,
    MaterialC,
    HealItem,
    TeleportItem,
    MineItem
}

public class PlayerInventory : MonoBehaviour
{
    [Header("재료 정보")]
    public int matACount = 0;
    public int matBCount = 0;
    public int matCCount = 0;

    [Header("사용 아이템 정보")]
    public int healItemCount = 0;
    public int teleportItemCount = 0;
    public int mineItemCount = 0;

    [Header("UI 연결 (재료)")]
    public TMP_Text matAText;
    public TMP_Text matBText;
    public TMP_Text matCText;

    [Header("UI 연결 (사용 아이템)")]
    public TMP_Text healItemText;
    public TMP_Text teleportItemText;
    public TMP_Text mineItemText;

    void Start()
    {
        // 인벤토리 UI들을 한 줄로 주르륵 내려가게 배치 (오른쪽에 있던 걸 위로)
        // 0번이 가장 위쪽, 5번이 가장 아래쪽
        SetupUI(healItemText, 0);
        SetupUI(teleportItemText, 1);
        SetupUI(mineItemText, 2);
        SetupUI(matAText, 3);
        SetupUI(matBText, 4);
        SetupUI(matCText, 5);

        UpdateUI();
    }

    void SetupUI(TMP_Text txt, int index)
    {
        if (txt == null) return;
        RectTransform rt = txt.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0); // 좌측 하단 앵커
        rt.anchorMax = new Vector2(0, 0);
        rt.pivot = new Vector2(0, 0);
        
        // 너비를 더 길게 늘리고(500), 상하 간격을 위해 높이 설정
        rt.sizeDelta = new Vector2(500, 40);
        
        float xPos = 30f; // 한 줄이므로 x 위치 고정
        float yPos = 30f + (5 - index) * 40f; // 상하 간격 40으로 줄임, 아래에서 위로 쌓임
        rt.anchoredPosition = new Vector2(xPos, yPos);
        txt.fontSize = 28;

        // 강제로 줄바꿈 안 되게 설정
        txt.textWrappingMode = TextWrappingModes.NoWrap;
        txt.overflowMode = TextOverflowModes.Overflow;
    }

    public void AddItem(ItemType type, int amount)
    {
        if (type == ItemType.MaterialA) matACount += amount;
        else if (type == ItemType.MaterialB) matBCount += amount;
        else if (type == ItemType.MaterialC) matCCount += amount;
        else if (type == ItemType.HealItem) healItemCount += amount;
        else if (type == ItemType.TeleportItem) teleportItemCount += amount;
        else if (type == ItemType.MineItem) mineItemCount += amount;

        UpdateUI();
        Debug.Log($"아이템 획득 완료 유형: {type} 현재 수량: {amount}");
    }

    public bool UseItem(ItemType type, int amount)
    {
        if (type == ItemType.MaterialA && matACount >= amount) { matACount -= amount; UpdateUI(); return true; }
        if (type == ItemType.MaterialB && matBCount >= amount) { matBCount -= amount; UpdateUI(); return true; }
        if (type == ItemType.MaterialC && matCCount >= amount) { matCCount -= amount; UpdateUI(); return true; }
        if (type == ItemType.HealItem && healItemCount >= amount) { healItemCount -= amount; UpdateUI(); return true; }
        if (type == ItemType.TeleportItem && teleportItemCount >= amount) { teleportItemCount -= amount; UpdateUI(); return true; }
        if (type == ItemType.MineItem && mineItemCount >= amount) { mineItemCount -= amount; UpdateUI(); return true; }

        return false;
    }

    void UpdateUI()
    {
        if (matAText != null) matAText.text = "Gear: " + matACount;
        if (matBText != null) matBText.text = "Plate: " + matBCount;
        if (matCText != null) matCText.text = "Gunpowder: " + matCCount;
        if (healItemText != null) healItemText.text = "Heal: " + healItemCount;
        if (teleportItemText != null) teleportItemText.text = "Teleport: " + teleportItemCount;
        if (mineItemText != null) mineItemText.text = "Main: " + mineItemCount;
    }
}