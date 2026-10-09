using UnityEngine;
using UnityEngine.UI;

namespace Ratchanon
{
    /// <summary>
    /// Controls Main Menu buttons and navigation according to Fig. 1.
    /// Fulfills Question 1 (Scenes Management).
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Menu Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button exitButton;

        private void Start()
        {
            if (startButton != null)
                startButton.onClick.AddListener(OnStartClicked);

            if (optionsButton != null)
                optionsButton.onClick.AddListener(OnOptionsClicked);

            if (creditsButton != null)
                creditsButton.onClick.AddListener(OnCreditsClicked);

            if (exitButton != null)
                exitButton.onClick.AddListener(OnExitClicked);
        }

        private void OnStartClicked()
        {
            Debug.Log("[MainMenuUI] Start Game clicked -> Loading StageSelect");
            if (SceneController.Instance != null)
                SceneController.Instance.LoadStageSelect();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("02_StageSelect");
        }

        private void OnOptionsClicked()
        {
            Debug.Log("[MainMenuUI] Options clicked -> Loading Options");
            if (SceneController.Instance != null)
                SceneController.Instance.LoadOptions();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("05_Options");
        }

        private void OnCreditsClicked()
        {
            Debug.Log("[MainMenuUI] Credits clicked -> Loading Credits");
            if (SceneController.Instance != null)
                SceneController.Instance.LoadCredits();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("06_Credits");
        }

        private void OnExitClicked()
        {
            Debug.Log("[MainMenuUI] Exit clicked");
            if (SceneController.Instance != null)
                SceneController.Instance.QuitGame();
            else
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }
}
