using UnityEngine;

namespace Ratchanon
{
    public enum ItemType
    {
        TimeBonus,    // +3 seconds
        TimePenalty,  // -5 seconds
        ScoreBonus,   // +500 points
        ScorePenalty  // -500 points
    }

    /// <summary>
    /// Represents a collectible item in Stage 1 and Stage 2.
    /// Fulfills Question 4 (Items and Obstacles) and Question 11 (Sound Effect - Items).
    /// </summary>
    public class ItemPickup : MonoBehaviour
    {
        [Header("Item Configuration")]
        public ItemType itemType = ItemType.ScoreBonus;

        [Header("Values")]
        [SerializeField] private int scoreValue = 500;
        [SerializeField] private float timeValue = 3f;

        [Header("Animation")]
        [SerializeField] private float rotationSpeed = 90f;
        [SerializeField] private float bobbingSpeed = 2f;
        [SerializeField] private float bobbingHeight = 0.25f;

        [Header("Audio")]
        [SerializeField] private AudioClip customCollectSFX;

        private Vector3 startPosition;
        private bool isCollected = false;

        private void Start()
        {
            startPosition = transform.position;
            ConfigureDefaults();
        }

        private void ConfigureDefaults()
        {
            switch (itemType)
            {
                case ItemType.TimeBonus:
                    timeValue = 3f;
                    break;
                case ItemType.TimePenalty:
                    timeValue = -5f;
                    break;
                case ItemType.ScoreBonus:
                    scoreValue = 500;
                    break;
                case ItemType.ScorePenalty:
                    scoreValue = -500;
                    break;
            }
        }

        private void Update()
        {
            // Rotate item
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

            // Bob up and down
            float newY = startPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isCollected) return;

            // Check if triggered by player
            if (other.CompareTag("Player") || other.GetComponent<CapsulePlayerController>() != null)
            {
                Collect();
            }
        }

        public void Collect()
        {
            if (isCollected) return;
            isCollected = true;

            // Notify Stage1Manager
            if (Stage1Manager.Instance != null)
            {
                switch (itemType)
                {
                    case ItemType.TimeBonus:
                        Stage1Manager.Instance.AddTime(timeValue);
                        PlaySound(true);
                        break;
                    case ItemType.TimePenalty:
                        Stage1Manager.Instance.AddTime(timeValue); // negative value reduces time
                        PlaySound(false);
                        break;
                    case ItemType.ScoreBonus:
                        Stage1Manager.Instance.AddScore(scoreValue);
                        PlaySound(true);
                        break;
                    case ItemType.ScorePenalty:
                        Stage1Manager.Instance.AddScore(scoreValue); // negative value reduces score
                        PlaySound(false);
                        break;
                }
            }
            else
            {
                PlaySound(itemType == ItemType.ScoreBonus || itemType == ItemType.TimeBonus);
            }

            Destroy(gameObject);
        }

        private void PlaySound(bool isPositive)
        {
            if (customCollectSFX != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(customCollectSFX);
            }
            else if (AudioManager.Instance != null)
            {
                if (isPositive)
                {
                    AudioManager.Instance.PlayItemBonusSFX();
                }
                else
                {
                    AudioManager.Instance.PlayItemPenaltySFX();
                }
            }
        }
    }
}
