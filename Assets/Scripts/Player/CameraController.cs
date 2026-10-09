using UnityEngine;

namespace Ratchanon
{
    /// <summary>
    /// Smooth third-person follow camera for Capsule Player and general targets.
    /// Supports Question 2 (Capsule Player Configuration).
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Target Tracking")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 3.5f, -6f);
        [SerializeField] private float smoothSpeed = 10f;
        [SerializeField] private float lookAtHeightOffset = 1.2f;

        [Header("Camera Collision / Occlusion Check")]
        [SerializeField] private bool autoFindPlayer = true;

        private void Start()
        {
            if (target == null && autoFindPlayer)
            {
                FindPlayerTarget();
            }
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                if (autoFindPlayer) FindPlayerTarget();
                return;
            }

            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            Vector3 lookTarget = target.position + Vector3.up * lookAtHeightOffset;
            transform.LookAt(lookTarget);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void FindPlayerTarget()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                var controller = FindAnyObjectByType<CapsulePlayerController>();
                if (controller != null)
                {
                    target = controller.transform;
                }
            }
        }
    }
}
