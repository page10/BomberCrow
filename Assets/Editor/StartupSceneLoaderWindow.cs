using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class StartupSceneLoaderWindow : EditorWindow
{
    private bool startCurrentScene = false;

    [MenuItem("Debug/StartCurrentScene")]
    public static void ShowWindow()
    {
        GetWindow<StartupSceneLoaderWindow>("StartCurrentScene");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();

        startCurrentScene = EditorGUILayout.Toggle("Start Current Scene", startCurrentScene);

        if (GUILayout.Button("Apply"))
        {
            EditorPrefs.SetBool("StartCurrentScene", startCurrentScene);
        }
    }

    private void OnEnable()
    {
        startCurrentScene = EditorPrefs.GetBool("StartCurrentScene", false);
    }

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            if (EditorPrefs.GetBool("StartCurrentScene", false))
            {
                return;
            }

            // 加载默认启动场景
            int startupSceneIndex = 0;
            if (EditorBuildSettings.scenes.Length > 0)
            {
                startupSceneIndex = EditorBuildSettings.scenes[0].enabled ? 0 : -1;
                if (startupSceneIndex < 0)
                    for (int i = 1; i < EditorBuildSettings.scenes.Length; i++)
                    {
                        if (EditorBuildSettings.scenes[i].enabled)
                        {
                            startupSceneIndex = i;
                            break;
                        }
                    }
            }

            if (startupSceneIndex != -1)
            {
                string startupScenePath = SceneUtility.GetScenePathByBuildIndex(startupSceneIndex);
                SceneManager.LoadScene(startupScenePath);
                //EditorSceneManager.OpenScene(startupScenePath);
            }
        }
    }
}