using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public Text tutorialText;
    public Transform tutorialItemsParent; // 인스펙터나 코드로 연결

    void Start()
    {
        // 캔버스가 없으면 동적 생성
        if (tutorialText == null)
        {
            GameObject canvasGO = new GameObject("TutorialCanvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            
            GameObject textGO = new GameObject("TutorialText");
            textGO.transform.SetParent(canvasGO.transform, false);
            tutorialText = textGO.AddComponent<Text>();
            Font krFont = Resources.Load<Font>("neodgm");
            tutorialText.font = krFont != null ? krFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            tutorialText.fontSize = 28;
            tutorialText.alignment = TextAnchor.UpperCenter;
            tutorialText.horizontalOverflow = HorizontalWrapMode.Overflow;
            tutorialText.verticalOverflow = VerticalWrapMode.Overflow;
            tutorialText.color = Color.yellow;
            Outline outline = textGO.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(2, -2);
            
            RectTransform rt = textGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.offsetMin = new Vector2(50, -150);
            rt.offsetMax = new Vector2(-50, -20);
        }

        StartCoroutine(TutorialRoutine());
    }

    IEnumerator TutorialRoutine()
    {
        // Step 1: 이동
        tutorialText.text = "1. 이동: W, A, S, D 키를 눌러 이동해보세요.";
        bool hasMoved = false;
        while (!hasMoved)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) hasMoved = true;
            yield return null;
        }
        tutorialText.text = "좋습니다!";
        yield return new WaitForSeconds(1.5f);

        // Step 2: 공격
        tutorialText.text = "2. 공격: 마우스 좌클릭으로 앞의 적을 공격해보세요.";
        bool hasAttacked = false;
        while (!hasAttacked)
        {
            if (Input.GetMouseButtonDown(0)) hasAttacked = true;
            yield return null;
        }
        tutorialText.text = "훌륭합니다!";
        yield return new WaitForSeconds(1.5f);

        // Step 3: 아이템 습득
        tutorialText.text = "3. 습득: 바닥에 떨어진 재료와 아이템에 다가가 획득하세요.";
        
        // 씬에 존재하는 모든 드랍 아이템(DropItem)을 다 먹을 때까지 대기
        while (FindObjectsByType<DropItem>(FindObjectsSortMode.None).Length > 0)
        {
            yield return null;
        }
        
        tutorialText.text = "모두 주웠습니다!";
        yield return new WaitForSeconds(1.5f);

        // Step 4: 제작소/강화소 상호작용
        tutorialText.text = "4. 상호작용: 제작소(또는 강화소)에 다가가 스페이스바를 누르세요.";
        bool hasInteracted = false;
        while (!hasInteracted)
        {
            if (Input.GetKeyDown(KeyCode.Space)) hasInteracted = true;
            yield return null;
        }
        tutorialText.text = "완벽합니다!";
        yield return new WaitForSeconds(1.5f);

        // Step 5: 아이템 사용 (회복 효과를 확인하기 위해 플레이어 체력을 강제로 깎음)
        PlayerHealth ph = FindFirstObjectByType<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(50f);
        }

        tutorialText.text = "5. 아이템 사용: 1번(회복), 2번(텔레포트), 3번(지뢰) 키를 눌러보세요. (체력이 깎였습니다!)";
        bool used1 = false; bool used2 = false; bool used3 = false;
        while (!used1 || !used2 || !used3)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) used1 = true;
            if (Input.GetKeyDown(KeyCode.Alpha2)) used2 = true;
            if (Input.GetKeyDown(KeyCode.Alpha3)) used3 = true;
            yield return null;
        }
        tutorialText.text = "아주 좋습니다!";
        yield return new WaitForSeconds(1.5f);

        // Step 6: 일시정지 안내
        tutorialText.text = "6. 시스템: 인게임에서 ESC 키를 누르면 게임 종료 혹은 메인 메뉴로 나갈 수 있습니다.";
        yield return new WaitForSeconds(4f);

        // Step 7: 포탈 이동
        tutorialText.text = "7. 완료: 우측 끝에 있는 포탈로 이동하여 메인 화면으로 돌아가세요.";
        
        // 포탈 스크립트가 씬 이동을 처리하므로, 여기서는 루프만 돕니다.
        // 포탈에 닿으면 ScenePortal 로직에 의해 씬이 넘어갈 것입니다.
        while (true)
        {
            yield return null;
        }
    }
}
