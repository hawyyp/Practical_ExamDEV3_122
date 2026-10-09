using UnityEngine;

namespace Ratchanon
{
    /// <summary>
    /// Detects when player falls off the stage platforms and triggers GameOver.
    /// Fulfills Question 3 (Level Design - Fall area) and Question 5 (Losing conditions).
    /// </summary>
    public class FallZone : MonoBehaviour
    {
        private bool hasTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered) return;

            if (other.CompareTag("Player") || other.GetComponent<CapsulePlayerController>() != null)
            {
                hasTriggered = true;
                Debug.Log("[FallZone] Player fell off stage! Triggering Game Over.");

                if (Stage1Manager.Instance != null)
                {
                    Stage1Manager.Instance.TriggerPlayerFell();
                }
                else if (Stage2Manager.Instance != null)
                {
                    Stage2Manager.Instance.TriggerPlayerFell();
                }
                else if (SceneController.Instance != null)
                {
                    SceneController.Instance.LoadGameOver();
                }
            }
        }
    }
}
