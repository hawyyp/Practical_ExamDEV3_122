using UnityEngine;

namespace Ratchanon
{
    /// <summary>
    /// Manages player score, time, and game result state across scenes.
    /// Singleton pattern with DontDestroyOnLoad.
    /// </summary>
    public class GameScoreManager : MonoBehaviour
    {
        public static GameScoreManager Instance { get; private set; }

        [Header("Runtime State")]
        public int currentScore = 0;
        public float timeRemaining = 30f;
        public string lastPlayedStage = "03_Stage1";
        public bool isGameWon = false;
        public const int WIN_SCORE_THRESHOLD = 10000;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void ResetSession(string stageName = "03_Stage1")
        {
            currentScore = 0;
            timeRemaining = 30f;
            lastPlayedStage = stageName;
            isGameWon = false;
        }

        public void AddScore(int amount)
        {
            currentScore += amount;
        }

        public void AddTime(float seconds)
        {
            timeRemaining = Mathf.Max(0f, timeRemaining + seconds);
        }

        public void CompleteGame(bool won, int finalScore)
        {
            isGameWon = won;
            currentScore = finalScore;
        }
    }
}
