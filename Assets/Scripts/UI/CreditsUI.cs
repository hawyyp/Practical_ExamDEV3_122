using UnityEngine;
using UnityEngine.UI;

namespace Ratchanon
{
    /// <summary>
    /// Displays student credit information and handles Back navigation.
    /// Fulfills Question 1 (Scenes Management - 06_Credits).
    /// </summary>
    public class CreditsUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button backButton;

        private void Start()
        {
            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }
        }

        private void OnBackClicked()
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
    }
}
