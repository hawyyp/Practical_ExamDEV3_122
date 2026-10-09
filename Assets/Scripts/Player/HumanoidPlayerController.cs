using System.Collections.Generic;
using UnityEngine;

namespace Ratchanon
{
    /// <summary>
    /// Controls the Humanoid 3D Character Controller with Animator and E-key Interaction System.
    /// Fulfills Question 6 (Stage 2 - Humanoid Player Configuration)
    /// and Question 8 (Items and obstacles interaction system).
    /// Supports both New Input System and Legacy Input seamlessly.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class HumanoidPlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float sprintMultiplier = 1.4f;
        [SerializeField] private float rotationSpeed = 12f;
        [SerializeField] private float jumpHeight = 1.4f;
        [SerializeField] private float gravity = -20f;

        [Header("Animation")]
        [SerializeField] private Animator animator;
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int GroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int JumpHash = Animator.StringToHash("Jump");

        private CharacterController controller;
        private Vector3 velocity;
        private bool isGrounded;
        private Transform cameraTransform;

        [Header("Interaction State")]
        private readonly List<InteractableItem> nearbyItems = new List<InteractableItem>();
        private InteractableItem currentClosestItem;

        public bool CanMove { get; set; } = true;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }

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
            HandleInteraction();

            if (!CanMove) return;

            // Ground detection
            isGrounded = controller.isGrounded;
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            // Dual input system reading
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
            float targetSpeed = (moveDirection.magnitude > 0.05f) ? (moveSpeed * (sprintPressed ? sprintMultiplier : 1f)) : 0f;
            if (moveDirection.magnitude > 0.05f)
            {
                controller.Move(moveDirection.normalized * (targetSpeed * Time.deltaTime));

                // Smooth rotation towards move direction
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Jump
            if (jumpPressed && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                if (animator != null && HasParameter(animator, JumpHash))
                {
                    animator.SetTrigger(JumpHash);
                }
            }

            // Gravity
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            // Update Animator parameters
            if (animator != null)
            {
                if (HasParameter(animator, SpeedHash))
                {
                    animator.SetFloat(SpeedHash, targetSpeed);
                }
                if (HasParameter(animator, GroundedHash))
                {
                    animator.SetBool(GroundedHash, isGrounded);
                }
            }
        }

        #region Interaction Handling
        public void RegisterNearbyItem(InteractableItem item)
        {
            if (item != null && !nearbyItems.Contains(item))
            {
                nearbyItems.Add(item);
                UpdateClosestItem();
            }
        }

        public void UnregisterNearbyItem(InteractableItem item)
        {
            if (nearbyItems.Contains(item))
            {
                nearbyItems.Remove(item);
                UpdateClosestItem();
            }
        }

        private void UpdateClosestItem()
        {
            // Clean up destroyed items
            nearbyItems.RemoveAll(item => item == null);

            if (nearbyItems.Count == 0)
            {
                currentClosestItem = null;
                if (Stage2Manager.Instance != null)
                {
                    Stage2Manager.Instance.HideInteractionPrompt();
                }
                return;
            }

            // Find closest
            float closestDist = float.MaxValue;
            InteractableItem closest = null;
            Vector3 pos = transform.position;

            foreach (var item in nearbyItems)
            {
                if (item == null) continue;
                float d = Vector3.Distance(pos, item.transform.position);
                if (d < closestDist)
                {
                    closestDist = d;
                    closest = item;
                }
            }

            currentClosestItem = closest;
            if (currentClosestItem != null && Stage2Manager.Instance != null)
            {
                Stage2Manager.Instance.ShowInteractionPrompt(currentClosestItem.GetPromptText());
            }
        }

        private void HandleInteraction()
        {
            UpdateClosestItem();

            if (currentClosestItem != null && GetInteractInput())
            {
                InteractableItem itemToInteract = currentClosestItem;
                itemToInteract.Interact(this);
                UpdateClosestItem();
            }
        }
        #endregion

        #region Input Fetching
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
                var kb = UnityEngine.InputSystem.Keyboard.current;
                if (kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed) return true;
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            try
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) return true;
            }
            catch { }
#endif

            return false;
        }

        private bool GetInteractInput()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                var kb = UnityEngine.InputSystem.Keyboard.current;
                if (kb.eKey.wasPressedThisFrame) return true;
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            try
            {
                if (Input.GetKeyDown(KeyCode.E)) return true;
            }
            catch { }
#endif

            return false;
        }

        private static bool HasParameter(Animator anim, int paramHash)
        {
            if (anim == null) return false;
            foreach (var p in anim.parameters)
            {
                if (p.nameHash == paramHash) return true;
            }
            return false;
        }
        #endregion
    }
}
