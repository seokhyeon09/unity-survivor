using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class ChangeFontUtility : EditorWindow
{
    [MenuItem("Tools/Fix All Fonts")]
    public static void ChangeAllFonts()
    {
        Font krFont = Resources.Load<Font>("neodgm");
        if (krFont == null)
        {
            Debug.LogError("[Fix All Fonts] Resources 폴더에 neodgm 폰트가 없습니다.");
            return;
        }

        // 작업 전 현재 씬 저장 여부 묻기
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.LogWarning("[Fix All Fonts] 작업이 취소되었습니다.");
            return;
        }

        int totalCount = 0;

        // Build Settings에 등록된 모든 씬을 순회
        foreach (EditorBuildSettingsScene buildScene in EditorBuildSettings.scenes)
        {
            if (!buildScene.enabled) continue;

            // 씬 열기
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(buildScene.path, OpenSceneMode.Single);
            int sceneCount = 0;

            Text[] allTexts = Resources.FindObjectsOfTypeAll<Text>();
            foreach (Text t in allTexts)
            {
                if (t.gameObject.scene == scene)
                {
                    if (t.font != krFont)
                    {
                        t.font = krFont;
                        EditorUtility.SetDirty(t);
                        sceneCount++;
                        totalCount++;
                    }
                }
            }

            // 변경 사항이 있으면 씬 저장
            if (sceneCount > 0)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"[Fix All Fonts] {scene.name} 씬에서 {sceneCount}개 변경 및 저장 완료.");
            }
        }

        // 프리팹(Assets) 내부의 텍스트도 변경
        int prefabCount = 0;
        Text[] allAssets = Resources.FindObjectsOfTypeAll<Text>();
        foreach (Text t in allAssets)
        {
            if (EditorUtility.IsPersistent(t.transform.root.gameObject))
            {
                if (t.font != krFont)
                {
                    t.font = krFont;
                    EditorUtility.SetDirty(t);
                    prefabCount++;
                    totalCount++;
                }
            }
        }

        Debug.Log($"[Fix All Fonts] 작업 완료! 모든 씬과 프리팹을 합쳐 총 {totalCount}개의 텍스트 폰트를 변경했습니다.");
    }
}
