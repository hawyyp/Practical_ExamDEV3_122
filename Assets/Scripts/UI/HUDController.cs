using UnityEngine;
using UnityEngine.UI;

namespace Ratchanon
{
    /// <summary>
    /// HUD UI Controller for Stage 1.
    /// Fulfills Question 1 (UI Text labels) and Question 5 (Countdown top-left HUD & Score display).
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("UI Text Displays")]
        public Text timerText;
        public Text scoreText;
        public Text stageLabelText;

        [Header("Buttons")]
        public Button backButton;

        private void Start()
        {
            if (Stage1Manager.Instance != null)
            {
                Stage1Manager.Instance.SetHUDReferences(timerText, scoreText, stageLabelText);
            }

            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }
        }

        private void OnBackClicked()
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
