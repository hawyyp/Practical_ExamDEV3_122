using UnityEngine;
using UnityEngine.UI;

namespace Ratchanon
{
    /// <summary>
    /// Controls the UI display on Win (07_Win) and GameOver (08_GameOver) scenes.
    /// Fulfills Question 1 (Scenes Management) and Question 5 (Result screens with Try Again & Main Menu).
    /// </summary>
    public class ResultSceneUI : MonoBehaviour
    {
        [Header("UI Texts")]
        [SerializeField] private Text titleLabel;
        [SerializeField] private Text scoreLabel;
        [SerializeField] private string defaultTitle = "Game Over";

        [Header("Buttons")]
        [SerializeField] private Button tryAgainButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button stageSelectButton;

        private void Start()
        {
            int score = 0;
            if (GameScoreManager.Instance != null)
            {
                score = GameScoreManager.Instance.currentScore;
            }

            if (scoreLabel != null)
            {
                scoreLabel.text = $"Score : {score:N0}";
            }

            if (titleLabel != null && string.IsNullOrEmpty(titleLabel.text))
            {
                titleLabel.text = defaultTitle;
            }

            if (tryAgainButton != null)
            {
                tryAgainButton.onClick.AddListener(OnTryAgainClicked);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            }

            if (stageSelectButton != null)
            {
                stageSelectButton.onClick.AddListener(OnStageSelectClicked);
            }
        }

        private void OnTryAgainClicked()
        {
            if (SceneController.Instance != null)
            {
                SceneController.Instance.RestartLastStage();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("03_Stage1");
            }
        }

        private void OnMainMenuClicked()
        {
            if (SceneController.Instance != null)
            {
                SceneController.Instance.LoadMainMenu();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("01_MainMenu");
            }
        }

        private void OnStageSelectClicked()
        {
            if (SceneController.Instance != null)
            {
                SceneController.Instance.LoadStageSelect();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("02_StageSelect");
            }
        }
    }
}
