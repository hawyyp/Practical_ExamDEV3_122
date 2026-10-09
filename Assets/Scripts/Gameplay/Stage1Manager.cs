using UnityEngine;
using UnityEngine.UI;

namespace Ratchanon
{
    /// <summary>
    /// Manages the Stage 1 Game Loop, HUD, Time Attack 30s countdown, and Win/Lose conditions.
    /// Fulfills Question 5 (Winning and losing conditions) and Question 1 (Scene flow integration).
    /// </summary>
    public class Stage1Manager : MonoBehaviour
    {
        public static Stage1Manager Instance { get; private set; }

        [Header("Rules Settings")]
        [SerializeField] private float initialTime = 30f;
        [SerializeField] private int winScoreThreshold = 10000;

        [Header("State")]
        public float timeRemaining = 30f;
        public int currentScore = 0;
        public bool isGameOver = false;

        [Header("HUD References (Optional direct links)")]
        [SerializeField] private Text timerText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text stageLabelText;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            timeRemaining = initialTime;
            currentScore = 0;
            isGameOver = false;

            if (GameScoreManager.Instance != null)
            {
                GameScoreManager.Instance.ResetSession("03_Stage1");
            }

            UpdateHUD();
        }

        private void Update()
        {
            if (isGameOver) return;

            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                OnTimeExpired();
            }

            UpdateHUD();
        }

        public void AddScore(int amount)
        {
            if (isGameOver) return;
            currentScore += amount;
            if (GameScoreManager.Instance != null)
            {
                GameScoreManager.Instance.currentScore = currentScore;
            }
            UpdateHUD();
        }

        public void AddTime(float seconds)
        {
            if (isGameOver) return;
            timeRemaining = Mathf.Max(0f, timeRemaining + seconds);
            if (GameScoreManager.Instance != null)
            {
                GameScoreManager.Instance.timeRemaining = timeRemaining;
            }
            UpdateHUD();
        }

        public void TriggerPlayerFell()
        {
            if (isGameOver) return;
            isGameOver = true;
            Debug.Log("[Stage1Manager] Player fell into abyss! Losing condition met.");

            if (GameScoreManager.Instance != null)
            {
                GameScoreManager.Instance.CompleteGame(false, currentScore);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayGameOverSFX();
            }

            if (SceneController.Instance != null)
            {
                SceneController.Instance.LoadGameOver();
            }
        }

        private void OnTimeExpired()
        {
            if (isGameOver) return;
            isGameOver = true;

            bool isWin = currentScore > winScoreThreshold;
            Debug.Log($"[Stage1Manager] Time Expired! Score: {currentScore} (Threshold: {winScoreThreshold}). Win = {isWin}");

            if (GameScoreManager.Instance != null)
            {
                GameScoreManager.Instance.CompleteGame(isWin, currentScore);
            }

            if (isWin)
            {
                if (AudioManager.Instance != null) AudioManager.Instance.PlayWinSFX();
                if (SceneController.Instance != null) SceneController.Instance.LoadWin();
            }
            else
            {
                if (AudioManager.Instance != null) AudioManager.Instance.PlayGameOverSFX();
                if (SceneController.Instance != null) SceneController.Instance.LoadGameOver();
            }
        }

        private void UpdateHUD()
        {
            if (timerText != null)
            {
                timerText.supportRichText = true;
                timerText.text = $"TIME: {timeRemaining:F1}s";
                if (timeRemaining > 15f)
                    timerText.color = new Color(0.25f, 1f, 0.5f); // Vibrant Emerald
                else if (timeRemaining > 5f)
                    timerText.color = new Color(1f, 0.85f, 0.2f); // Amber Gold
                else
                    timerText.color = new Color(1f, 0.25f, 0.25f); // Urgent Red
            }
            if (scoreText != null)
            {
                scoreText.supportRichText = true;
                if (currentScore > winScoreThreshold)
                {
                    scoreText.text = $"SCORE: {currentScore:N0}  <color=#44FF88>[WIN READY!]</color>";
                }
                else
                {
                    scoreText.text = $"SCORE: {currentScore:N0}  <color=#88CCFF>/ >{winScoreThreshold:N0}</color>";
                }
            }
            if (stageLabelText != null && string.IsNullOrEmpty(stageLabelText.text))
            {
                stageLabelText.text = "STAGE 1 : CAPSULE RUNNER";
            }
        }

        public void SetHUDReferences(Text timer, Text score, Text stageLabel)
        {
            timerText = timer;
            scoreText = score;
            stageLabelText = stageLabel;
            UpdateHUD();
        }
    }
}
