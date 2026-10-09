using UnityEngine;

namespace Ratchanon
{
    /// <summary>
    /// Controls Capsule Player movement using CharacterController.
    /// Fulfills Question 2 (Capsule Player Configuration) requirements.
    /// Supports both New Input System and Legacy Input seamlessly.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class CapsulePlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 7f;
        [SerializeField] private float sprintMultiplier = 1.4f;
        [SerializeField] private float rotationSpeed = 12f;
        [SerializeField] private float jumpHeight = 1.5f;
        [SerializeField] private float gravity = -20f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundMask = ~0;

        private CharacterController controller;
        private Vector3 velocity;
        private bool isGrounded;
        private Transform cameraTransform;

        public bool CanMove { get; set; } = true;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        private void Start()
        {
            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            if (!CanMove) return;

            // Ground detection
            isGrounded = controller.isGrounded;
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            // Input fetching with dual-system support
            Vector2 input = GetMovementInput();
            bool jumpPressed = GetJumpInput();
            bool sprintPressed = GetSprintInput();

            // Calculate movement direction relative to camera
            Vector3 moveDirection = Vector3.zero;
            if (cameraTransform != null)
            {
                Vector3 forward = cameraTransform.forward;
                Vector3 right = cameraTransform.right;
                forward.y = 0f;
                right.y = 0f;
                forward.Normalize();
                right.Normalize();

                moveDirection = forward * input.y + right * input.x;
            }
            else
            {
                moveDirection = new Vector3(input.x, 0f, input.y);
            }

            // Move the controller
            float currentSpeed = moveSpeed * (sprintPressed ? sprintMultiplier : 1f);
            if (moveDirection.magnitude > 0.05f)
            {
                controller.Move(moveDirection.normalized * (currentSpeed * Time.deltaTime));

                // Smooth rotation towards movement direction
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Jump
            if (jumpPressed && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // Gravity
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        private Vector2 GetMovementInput()
        {
            float horizontal = 0f;
            float vertical = 0f;

#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                var kb = UnityEngine.InputSystem.Keyboard.current;
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed) vertical += 1f;
                if (kb.sKey.isPressed || kb.downArrowKey.isPressed) vertical -= 1f;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) horizontal += 1f;
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) horizontal -= 1f;
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            if (Mathf.Approximately(horizontal, 0f) && Mathf.Approximately(vertical, 0f))
            {
                try
                {
                    horizontal = Input.GetAxisRaw("Horizontal");
                    vertical = Input.GetAxisRaw("Vertical");
                }
                catch { }
            }
#endif

            return new Vector2(horizontal, vertical).normalized;
        }

        private bool GetJumpInput()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame) return true;
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            try
            {
                if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space)) return true;
            }
            catch { }
#endif
            return false;
        }

        private bool GetSprintInput()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (UnityEngine.InputSystem.Keyboard.current.leftShiftKey.isPressed) return true;
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            try
            {
                if (Input.GetKey(KeyCode.LeftShift)) return true;
            }
            catch { }
#endif
            return false;
        }
    }
}
