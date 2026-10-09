using UnityEngine;

namespace Ratchanon
{
    /// <summary>
    /// Represents an interactable item in Stage 2 that requires pressing [E] to collect.
    /// Fulfills Question 8 (Stage 2 - Items and obstacles with Interaction system)
    /// and Question 11 (Sound Effect - Items).
    /// </summary>
    public class InteractableItem : MonoBehaviour
    {
        [Header("Item Type & Values")]
        public ItemType itemType = ItemType.ScoreBonus;
        [SerializeField] private int scoreValue = 500;
        [SerializeField] private float timeValue = 3f;
        [SerializeField] private string itemName = "Score Bonus";

        [Header("Interaction Settings")]
        [SerializeField] private float interactionRadius = 2.5f;

        [Header("Animation")]
        [SerializeField] private float rotationSpeed = 70f;
        [SerializeField] private float bobbingSpeed = 2.2f;
        [SerializeField] private float bobbingHeight = 0.2f;

        [Header("Audio")]
        [SerializeField] private AudioClip customCollectSFX;

        private Vector3 startPosition;
        private bool isCollected = false;

        private void Start()
        {
            startPosition = transform.position;
            ConfigureDefaults();
            EnsureTriggerCollider();
        }

        private void ConfigureDefaults()
        {
            switch (itemType)
            {
                case ItemType.TimeBonus:
                    timeValue = 3f;
                    scoreValue = 0;
                    itemName = "Time Bonus (+3s)";
                    break;
                case ItemType.TimePenalty:
                    timeValue = -5f;
                    scoreValue = 0;
                    itemName = "Time Hazard (-5s)";
                    break;
                case ItemType.ScoreBonus:
                    scoreValue = 500;
                    timeValue = 0f;
                    itemName = "Gold Star (+500 pts)";
                    break;
                case ItemType.ScorePenalty:
                    scoreValue = -500;
                    timeValue = 0f;
                    itemName = "Dark Crystal (-500 pts)";
                    break;
            }
        }

        private void EnsureTriggerCollider()
        {
            SphereCollider sphereCol = GetComponent<SphereCollider>();
            if (sphereCol == null)
            {
                sphereCol = gameObject.AddComponent<SphereCollider>();
            }
            sphereCol.isTrigger = true;
            sphereCol.radius = interactionRadius;
        }

        private void Update()
        {
            if (isCollected) return;

            // Idle floating animation
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            float newY = startPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isCollected) return;

            HumanoidPlayerController player = other.GetComponent<HumanoidPlayerController>();
            if (player == null)
            {
                player = other.GetComponentInParent<HumanoidPlayerController>();
            }

            if (player != null)
            {
                player.RegisterNearbyItem(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            HumanoidPlayerController player = other.GetComponent<HumanoidPlayerController>();
            if (player == null)
            {
                player = other.GetComponentInParent<HumanoidPlayerController>();
            }

            if (player != null)
            {
                player.UnregisterNearbyItem(this);
            }
        }

        public string GetPromptText()
        {
            switch (itemType)
            {
                case ItemType.ScoreBonus:
                    return $"[E] Collect {itemName} <color=#FFDD44>(+500)</color>";
                case ItemType.ScorePenalty:
                    return $"[E] Collect {itemName} <color=#FF4466>(-500)</color>";
                case ItemType.TimeBonus:
                    return $"[E] Collect {itemName} <color=#44FF88>(+3s)</color>";
                case ItemType.TimePenalty:
                    return $"[E] Collect {itemName} <color=#FF4466>(-5s)</color>";
                default:
                    return $"[E] Collect {itemName}";
            }
        }

        public void Interact(HumanoidPlayerController player)
        {
            if (isCollected) return;
            isCollected = true;

            Debug.Log($"[InteractableItem] Interacted with {itemName} (Type: {itemType})");

            // Apply effects
            if (Stage2Manager.Instance != null)
            {
                switch (itemType)
                {
                    case ItemType.ScoreBonus:
                        Stage2Manager.Instance.AddScore(scoreValue);
                        PlaySound(true);
                        break;
                    case ItemType.ScorePenalty:
                        Stage2Manager.Instance.AddScore(scoreValue);
                        PlaySound(false);
                        break;
                    case ItemType.TimeBonus:
                        Stage2Manager.Instance.AddTime(timeValue);
                        PlaySound(true);
                        break;
                    case ItemType.TimePenalty:
                        Stage2Manager.Instance.AddTime(timeValue);
                        PlaySound(false);
                        break;
                }
            }
            else
            {
                PlaySound(itemType == ItemType.ScoreBonus || itemType == ItemType.TimeBonus);
            }

            if (player != null)
            {
                player.UnregisterNearbyItem(this);
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
