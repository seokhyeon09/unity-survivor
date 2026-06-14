using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Game Control")]
    public float gameTime;
    public float maxGameTime = 2 * 60f;

    [Header("# Player Info")]
    public int killCount;

    [Header("# UI")]
    public TMP_Text killText; // Inspector에서 TextMeshPro 오브젝트 연결

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // UI 배치: 좌측 상단으로 고정하고 크기 키우기
        if (killText != null)
        {
            RectTransform rt = killText.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1); // 좌측 상단 앵커
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(30, -30); // 여백
            killText.fontSize = 40; // 폰트 크기 키움
        }

        // 시작 시 텍스트 초기화
        UpdateKillUI();
    }

    void Update()
    {
        gameTime += Time.deltaTime;
    }

    public void GetKill()
    {
        killCount++;

        // 1. Spawner의 static 변수와 동기화 (보스 소환 조건용)
        Spawner.killCount = killCount;

        // 2. UI 텍스트 업데이트
        UpdateKillUI();
    }

    void UpdateKillUI()
    {
        if (killText != null)
        {
            killText.text = string.Format("KILL: {0}", killCount);
        }
    }
}