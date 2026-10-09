using UnityEngine;

namespace Ratchanon
{
    /// <summary>
    /// Obstacle component that handles player collision.
    /// Fulfills Question 4 (Items and Obstacles) and Question 12 (Sound Effect - Obstacles).
    /// </summary>
    public class Obstacle : MonoBehaviour
    {
        [Header("Collision Feedback")]
        [SerializeField] private float pushBackForce = 5f;
        [SerializeField] private float hitCooldown = 1f;
        [SerializeField] private AudioClip customHitSFX;

        private float lastHitTime = -10f;

        private void OnCollisionEnter(Collision collision)
        {
            HandlePlayerHit(collision.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            HandlePlayerHit(other.gameObject);
        }

        private void HandlePlayerHit(GameObject target)
        {
            if (Time.time - lastHitTime < hitCooldown) return;

            if (target.CompareTag("Player") || target.GetComponent<CapsulePlayerController>() != null)
            {
                lastHitTime = Time.time;

                // Play obstacle sound
                if (customHitSFX != null && AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(customHitSFX);
                }
                else if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayObstacleHitSFX();
                }

                // Pushback controller if CharacterController exists
                CharacterController cc = target.GetComponent<CharacterController>();
                if (cc != null)
                {
                    Vector3 pushDirection = (target.transform.position - transform.position).normalized;
                    pushDirection.y = 0.2f;
                    cc.Move(pushDirection * pushBackForce * Time.deltaTime * 5f);
                }
            }
        }
    }
}
