using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("처음 시작할 씬 이름")]
    public string startSceneName = "Shelter";

    private string currentSceneName = "";
    private bool isSwitching = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Core 씬에 배치될 것이므로 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        if (pauseMenu != null && pauseMenu.transform.parent != null)
        {
            Destroy(pauseMenu.transform.parent.gameObject);
        }
    }

    public static string targetStartScene = ""; // 외부에서 시작할 씬을 지정할 때 사용
    
    private bool isPaused = false;
    private GameObject pauseMenu;

    void Start()
    {
        CreatePauseMenu();
        
        // [추가] BGM 재생
        AudioManager.Instance.PlayBGM("BGM");

        // 지정된 씬이 있으면 그걸 로드, 없으면 기본 startSceneName
        string sceneToLoad = string.IsNullOrEmpty(targetStartScene) ? startSceneName : targetStartScene;
        targetStartScene = ""; // 일회성 소비
        SwitchScene(sceneToLoad);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    private void TogglePauseMenu()
    {
        if (pauseMenu == null) return;
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f; // 게임 일시정지
    }

    private void CreatePauseMenu()
    {
        // 일시정지 캔버스 동적 생성
        GameObject canvasGO = new GameObject("PauseCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        DontDestroyOnLoad(canvasGO);

        pauseMenu = new GameObject("PausePanel");
        pauseMenu.transform.SetParent(canvasGO.transform, false);
        UnityEngine.UI.Image panelImg = pauseMenu.AddComponent<UnityEngine.UI.Image>();
        panelImg.color = new Color(0, 0, 0, 0.8f);
        RectTransform rt = pauseMenu.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;

        System.Action<string, int, UnityEngine.Events.UnityAction> CreateBtn = (name, yPos, action) => {
            GameObject btnGO = new GameObject(name);
            btnGO.transform.SetParent(pauseMenu.transform, false);
            UnityEngine.UI.Image bg = btnGO.AddComponent<UnityEngine.UI.Image>();
            bg.color = new Color(0.1f, 0.3f, 0.3f, 1f);
            UnityEngine.UI.Button btn = btnGO.AddComponent<UnityEngine.UI.Button>();
            btn.onClick.AddListener(() => AudioManager.Instance.PlaySFX("Button"));
            btn.onClick.AddListener(action);
            btn.onClick.AddListener(TogglePauseMenu); // 클릭 시 메뉴 닫기
            RectTransform btnRT = btnGO.GetComponent<RectTransform>();
            btnRT.sizeDelta = new Vector2(250, 60);
            btnRT.anchoredPosition = new Vector2(0, yPos);

            GameObject txtGO = new GameObject("Text");
            txtGO.transform.SetParent(btnGO.transform, false);
            UnityEngine.UI.Text txt = txtGO.AddComponent<UnityEngine.UI.Text>();
            txt.text = name;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.fontStyle = FontStyle.Bold;
            // 시스템 폰트 할당
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txtGO.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 60);
        };

        CreateBtn("Resume", 80, () => { /* 이미 TogglePauseMenu가 닫아줌 */ });
        CreateBtn("Main Menu", 0, () => {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        });
        CreateBtn("Quit Game", -80, () => {
            Time.timeScale = 1f;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });

        pauseMenu.SetActive(false);
        
        CreateGameOverMenu(canvasGO);
    }

    private GameObject gameOverMenu;

    private void CreateGameOverMenu(GameObject canvasGO)
    {
        gameOverMenu = new GameObject("GameOverPanel");
        gameOverMenu.transform.SetParent(canvasGO.transform, false);
        UnityEngine.UI.Image panelImg = gameOverMenu.AddComponent<UnityEngine.UI.Image>();
        panelImg.color = new Color(0.3f, 0, 0, 0.8f); // 어두운 붉은색
        RectTransform rt = gameOverMenu.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;

        // Title Text
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(gameOverMenu.transform, false);
        UnityEngine.UI.Text titleTxt = titleGO.AddComponent<UnityEngine.UI.Text>();
        titleTxt.text = "YOU DIED";
        titleTxt.alignment = TextAnchor.MiddleCenter;
        titleTxt.color = Color.white;
        titleTxt.fontSize = 60;
        titleTxt.fontStyle = FontStyle.Bold;
        titleTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        RectTransform titleRT = titleGO.GetComponent<RectTransform>();
        titleRT.sizeDelta = new Vector2(500, 100);
        titleRT.anchoredPosition = new Vector2(0, 100);

        // Main Menu Button
        GameObject btnGO = new GameObject("MainMenuBtn");
        btnGO.transform.SetParent(gameOverMenu.transform, false);
        UnityEngine.UI.Image bg = btnGO.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        UnityEngine.UI.Button btn = btnGO.AddComponent<UnityEngine.UI.Button>();
        btn.onClick.AddListener(() => {
            AudioManager.Instance.PlaySFX("Button");
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        });
        RectTransform btnRT = btnGO.GetComponent<RectTransform>();
        btnRT.sizeDelta = new Vector2(300, 70);
        btnRT.anchoredPosition = new Vector2(0, -50);

        GameObject txtGO = new GameObject("Text");
        txtGO.transform.SetParent(btnGO.transform, false);
        UnityEngine.UI.Text txt = txtGO.AddComponent<UnityEngine.UI.Text>();
        txt.text = "메인으로 돌아가기";
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.fontSize = 30;
        txt.fontStyle = FontStyle.Bold;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txtGO.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 70);

        gameOverMenu.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (gameOverMenu != null)
        {
            AudioManager.Instance.PlaySFX("Lose");
            gameOverMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void SwitchScene(string targetSceneName)
    {
        // 이미 씬 전환 중이거나, 같은 씬으로 이동하려 하면 무시
        if (isSwitching || currentSceneName == targetSceneName) return;
        StartCoroutine(SwitchSceneRoutine(targetSceneName));
    }

    private IEnumerator SwitchSceneRoutine(string targetSceneName)
    {
        isSwitching = true;

        // 1. 기존 배경 씬이 있다면 메모리에서 내림(Unload)
        if (!string.IsNullOrEmpty(currentSceneName))
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentSceneName);
            while (unloadOp != null && !unloadOp.isDone)
            {
                yield return null;
            }
        }

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
        if (loadOp == null)
        {
            Debug.LogError($"[LevelManager] 씬 로드 실패! '{targetSceneName}' 씬이 Build Profiles에 등록되지 않았습니다.");
            isSwitching = false;
            yield break; // 에러 방지를 위해 여기서 중단
        }

        while (!loadOp.isDone)
        {
            yield return null;
        }

        // 3. 방금 로드한 씬을 Active Scene으로 설정 (조명 셋팅이나 새 오브젝트 생성을 위해)
        Scene loadedScene = SceneManager.GetSceneByName(targetSceneName);
        if (loadedScene.IsValid())
        {
            SceneManager.SetActiveScene(loadedScene);
        }

        currentSceneName = targetSceneName;
        isSwitching = false;
        
        Debug.Log($"[LevelManager] {targetSceneName} 씬으로 완벽하게 전환되었습니다!");
    }
}
