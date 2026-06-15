using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.IO;

public class VercelBuildHelper
{
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        // WebGL 빌드일 때만 실행
        if (target == BuildTarget.WebGL)
        {
            Debug.Log("[VercelBuildHelper] WebGL 빌드 감지됨. Vercel 배포용 설정 파일(vercel.json)을 생성합니다.");

            string vercelJsonPath = Path.Combine(pathToBuiltProject, "vercel.json");
            
            // Vercel에서 유니티 WebGL의 압축 파일(.br, .gz)과 .wasm 파일을 정상적으로 읽을 수 있도록 헤더 설정
            string vercelJsonContent = @"{
  ""cleanUrls"": true,
  ""headers"": [
    {
      ""source"": ""/(.*).gz"",
      ""headers"": [
        { ""key"": ""Content-Encoding"", ""value"": ""gzip"" }
      ]
    },
    {
      ""source"": ""/(.*).br"",
      ""headers"": [
        { ""key"": ""Content-Encoding"", ""value"": ""br"" }
      ]
    },
    {
      ""source"": ""/(.*).wasm"",
      ""headers"": [
        { ""key"": ""Content-Type"", ""value"": ""application/wasm"" }
      ]
    },
    {
      ""source"": ""/(.*).wasm.gz"",
      ""headers"": [
        { ""key"": ""Content-Type"", ""value"": ""application/wasm"" },
        { ""key"": ""Content-Encoding"", ""value"": ""gzip"" }
      ]
    },
    {
      ""source"": ""/(.*).wasm.br"",
      ""headers"": [
        { ""key"": ""Content-Type"", ""value"": ""application/wasm"" },
        { ""key"": ""Content-Encoding"", ""value"": ""br"" }
      ]
    }
  ]
}";

            File.WriteAllText(vercelJsonPath, vercelJsonContent);
            Debug.Log($"[VercelBuildHelper] vercel.json 파일 생성 완료: {vercelJsonPath}");
        }
    }

    // 터미널(CLI) 자동 빌드를 위한 메서드 추가
    public static void PerformWebGLBuild()
    {
        var editorScenes = EditorBuildSettings.scenes;
        var scenePaths = new System.Collections.Generic.List<string>();
        foreach (var scene in editorScenes)
        {
            if (scene.enabled) scenePaths.Add(scene.path);
        }

        string buildPath = "Builds/WebGL";
        Debug.Log($"[VercelBuildHelper] WebGL 자동 빌드를 시작합니다. 대상 경로: {buildPath}");
        
        UnityEditor.BuildPipeline.BuildPlayer(scenePaths.ToArray(), buildPath, BuildTarget.WebGL, BuildOptions.None);
        
        Debug.Log("[VercelBuildHelper] WebGL 빌드 프로세스가 모두 완료되었습니다.");
    }
}
