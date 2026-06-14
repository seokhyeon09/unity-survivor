using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour, IInteractable
{
    [Header("이동할 씬 이름")]
    [Tooltip("빌드 셋팅에 등록된 씬 이름(예: Shelter 또는 BattleField)을 정확히 적어주세요.")]
    public string targetSceneName = "BattleField";

    public void Interact(GameObject player)
    {
        AudioManager.Instance.PlaySFX("telepor");
        Debug.Log($"포탈 작동: [{targetSceneName}] 씬으로 이동합니다...");
        
        // 메인 메뉴로 돌아갈 때는 게임 씬(Core)을 완전히 파괴해야 합니다.
        if (targetSceneName == "MainMenu")
        {
            // TimeScale을 원상복구하고, MainMenu 씬을 Single 모드로 새로 불러옵니다.
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            return;
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.SwitchScene(targetSceneName);
        }
        else
        {
            Debug.LogError("LevelManager를 찾을 수 없습니다! Core 씬에서 플레이 중인지 확인하세요.");
        }
    }
}
