using UnityEngine;
using UnityEngine.UI;

namespace Ratchanon
{
    /// <summary>
    /// Controls Stage Selection navigation.
    /// Fulfills Question 1 (Scenes Management - 02_StageSelect).
    /// </summary>
    public class StageSelectUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button stage1Button;
        [SerializeField] private Button stage2Button;
        [SerializeField] private Button backButton;

        private void Start()
        {
            if (stage1Button != null)
                stage1Button.onClick.AddListener(OnStage1Clicked);

            if (stage2Button != null)
                stage2Button.onClick.AddListener(OnStage2Clicked);

            if (backButton != null)
                backButton.onClick.AddListener(OnBackClicked);
        }

        private void OnStage1Clicked()
        {
            Debug.Log("[StageSelectUI] Stage 1 clicked -> Loading Stage1");
            if (SceneController.Instance != null)
                SceneController.Instance.LoadStage1();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("03_Stage1");
        }

        private void OnStage2Clicked()
        {
            Debug.Log("[StageSelectUI] Stage 2 clicked -> Loading Stage2");
            if (SceneController.Instance != null)
                SceneController.Instance.LoadStage2();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("04_Stage2");
        }

        private void OnBackClicked()
        {
            Debug.Log("[StageSelectUI] Back clicked -> Loading MainMenu");
            if (SceneController.Instance != null)
                SceneController.Instance.LoadMainMenu();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("01_MainMenu");
        }
    }
}
