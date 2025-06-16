using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SCSIA
{
    [InitializeOnLoad]
    public static class PlayFromBootstrapScene
    {
        //############################################################################################
        // FIELDS
        //############################################################################################
        private const string _startScenePath = "Assets/SCSIA/Scenes/BootstrapScene.unity";

        //############################################################################################
        // PRIVATE METHODS
        //############################################################################################
        static PlayFromBootstrapScene()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                SceneAsset startSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(_startScenePath);

                if (startSceneAsset == null)
                {
                    Debug.LogWarning($"Start scene not found at: {_startScenePath}");
                    return;
                }

                EditorSceneManager.playModeStartScene = startSceneAsset;
                Debug.Log($"Set play mode start scene: {startSceneAsset.name}");
            }

            if (state == PlayModeStateChange.EnteredEditMode)
            {
                EditorSceneManager.playModeStartScene = null;
            }
        }
    }
}