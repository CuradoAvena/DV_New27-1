using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float acceleration = 10f;
    public float rotationSmoothTime = 0.1f;

    [Header("Salto y gravedad")]
    public float jumpHeight = 1.2f;
    public float gravity = -20f;

    [Header("Referencias")]
    public Transform cam;          // Main Camera
    public Animator animator;      // Animator del modelo (puede ser hijo)

    CharacterController controller;
    Vector3 velocity;              // solo se usa el eje Y
    float currentSpeed;
    float rotVelocity;

    static readonly int SpeedHash    = Animator.StringToHash("Speed");
    static readonly int GroundedHash = Animator.StringToHash("Grounded");
    static readonly int JumpHash     = Animator.StringToHash("Jump");

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cam == null && Camera.main != null) cam = Camera.main.transform;
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // --- Input ---
        float h = (kb.dKey.isPressed ? 1 : 0) - (kb.aKey.isPressed ? 1 : 0);
        float v = (kb.wKey.isPressed ? 1 : 0) - (kb.sKey.isPressed ? 1 : 0);
        Vector2 input = new Vector2(h, v).normalized;
        bool running = kb.leftShiftKey.isPressed;

        // --- Dirección relativa a la cámara ---
        Vector3 camF = cam.forward; camF.y = 0; camF.Normalize();
        Vector3 camR = cam.right;   camR.y = 0; camR.Normalize();
        Vector3 moveDir = camF * input.y + camR * input.x;

        float targetSpeed = input.sqrMagnitude > 0.01f
            ? (running ? runSpeed : walkSpeed)
            : 0f;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        // --- Rotación hacia donde caminas ---
        if (moveDir.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,
                                                ref rotVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        // --- Gravedad y salto ---
        bool grounded = controller.isGrounded;
        if (grounded && velocity.y < 0) velocity.y = -2f;

        if (grounded && kb.spaceKey.wasPressedThisFrame)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger(JumpHash);
        }
        velocity.y += gravity * Time.deltaTime;

        // --- Mover ---
        Vector3 move = moveDir.normalized * currentSpeed + Vector3.up * velocity.y;
        controller.Move(move * Time.deltaTime);

        // --- Animator ---
        animator.SetFloat(SpeedHash, currentSpeed);
        animator.SetBool(GroundedHash, grounded);
    }
}
