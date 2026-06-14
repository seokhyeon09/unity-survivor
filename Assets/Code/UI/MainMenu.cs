using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("시작할 메인 씬 이름")]
    public string firstSceneName = "Core";

    void Start()
    {
        // EventSystem이 없으면 클릭 자체가 안 되므로 없을 경우 자동 생성합니다.
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // 인스펙터 계층구조나 이름이 변경되었더라도 버튼을 찾아 강제 연결합니다.
        UnityEngine.UI.Button[] buttons = GetComponentsInChildren<UnityEngine.UI.Button>(true);
        foreach(var btn in buttons)
        {
            string btnName = btn.gameObject.name.ToLower();
            string btnText = "";
            UnityEngine.UI.Text txt = btn.GetComponentInChildren<UnityEngine.UI.Text>();
            if (txt != null) btnText = txt.text.ToLower();

            if (btnName.Contains("tutorial") || btnText.Contains("tutorial") || btnText.Contains("튜토리얼"))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OnClickTutorial);
            }
            else if (btnName.Contains("start") || btnText.Contains("start") || btnText.Contains("시작"))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OnClickGameStart);
            }
            else if (btnName.Contains("exit") || btnName.Contains("quit") || btnText.Contains("exit") || btnText.Contains("종료"))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OnClickGameExit);
            }
        }
    }

    public void OnClickGameStart()
    {
        AudioManager.Instance.PlaySFX("Button");
        Debug.Log("게임 시작: " + firstSceneName + " 씬을 로드합니다.");
        LevelManager.targetStartScene = ""; // 기본 씬(Shelter) 로드
        SceneManager.LoadScene(firstSceneName, LoadSceneMode.Single);
    }

    public void OnClickTutorial()
    {
        AudioManager.Instance.PlaySFX("Button");
        Debug.Log("튜토리얼 시작: " + firstSceneName + " 씬을 로드하고 Tutorial로 진입합니다.");
        LevelManager.targetStartScene = "Tutorial";
        SceneManager.LoadScene(firstSceneName, LoadSceneMode.Single);
    }

    public void OnClickGameExit()
    {
        AudioManager.Instance.PlaySFX("Button");
        Debug.Log("게임을 종료합니다.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
