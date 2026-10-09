using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ratchanon
{
    /// <summary>
    /// Controls scene transitions and high-level game navigation.
    /// Fulfills Question 1 (Scenes Management) requirements.
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        private static SceneController _instance;
        public static SceneController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Object.FindAnyObjectByType<SceneController>();
                    if (_instance == null)
                    {
                        var go = new GameObject("SceneController (Auto)");
                        _instance = go.AddComponent<SceneController>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
            private set => _instance = value;
        }

        public const string SCENE_MAIN_MENU = "01_MainMenu";
        public const string SCENE_STAGE_SELECT = "02_StageSelect";
        public const string SCENE_STAGE_1 = "03_Stage1";
        public const string SCENE_STAGE_2 = "04_Stage2";
        public const string SCENE_OPTIONS = "05_Options";
        public const string SCENE_CREDITS = "06_Credits";
        public const string SCENE_WIN = "07_Win";
        public const string SCENE_GAME_OVER = "08_GameOver";

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void LoadScene(string sceneName)
        {
            Debug.Log($"[SceneController] Loading scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }

        public void LoadSceneByIndex(int sceneIndex)
        {
            Debug.Log($"[SceneController] Loading scene index: {sceneIndex}");
            SceneManager.LoadScene(sceneIndex);
        }

        public void LoadMainMenu()
        {
            LoadScene(SCENE_MAIN_MENU);
        }

        public void LoadStageSelect()
        {
            LoadScene(SCENE_STAGE_SELECT);
        }

        public void LoadStage1()
        {
            if (GameScoreManager.Instance != null)
            {
                GameScoreManager.Instance.ResetSession(SCENE_STAGE_1);
            }
            LoadScene(SCENE_STAGE_1);
        }

        public void LoadStage2()
        {
            if (GameScoreManager.Instance != null)
            {
                GameScoreManager.Instance.ResetSession(SCENE_STAGE_2);
            }
            LoadScene(SCENE_STAGE_2);
        }

        public void LoadOptions()
        {
            LoadScene(SCENE_OPTIONS);
        }

        public void LoadCredits()
        {
            LoadScene(SCENE_CREDITS);
        }

        public void LoadWin()
        {
            LoadScene(SCENE_WIN);
        }

        public void LoadGameOver()
        {
            LoadScene(SCENE_GAME_OVER);
        }

        public void RestartLastStage()
        {
            string stage = SCENE_STAGE_1;
            if (GameScoreManager.Instance != null && !string.IsNullOrEmpty(GameScoreManager.Instance.lastPlayedStage))
            {
                stage = GameScoreManager.Instance.lastPlayedStage;
                GameScoreManager.Instance.ResetSession(stage);
            }
            LoadScene(stage);
        }

        public void QuitGame()
        {
            Debug.Log("[SceneController] QuitGame called");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
