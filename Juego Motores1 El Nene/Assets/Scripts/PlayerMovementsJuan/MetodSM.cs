using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class MetodSM : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float sprintSpeed = 8f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float airControl = 0.3f;

    [Header("Mouse")]
    [SerializeField] float sensitivity = 0.1f;
    [SerializeField] float maxLookAngle = 85f;

    [Header("Salto / Gravedad")]
    [SerializeField] float jumpForce = 1.5f;
    [SerializeField] float gravity = -9.81f;

    [Header("Referencias")]
    [SerializeField] Transform cameraPivot;

    Vector2 moveInput;
    Vector2 lookInput;
    bool sprint;

    float yVelocity;
    float yaw;
    float pitch;
    Vector3 velocity;

    CharacterController controller;

    public void OnMove(InputValue v) => moveInput = v.Get<Vector2>();
    public void OnLook(InputValue v) => lookInput = v.Get<Vector2>();
    public void OnSprint(InputValue v) => sprint = v.isPressed;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Look();
        Move();
    }

    void Look()
    {
        yaw += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity;
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        transform.rotation = Quaternion.Euler(0, yaw, 0);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    void Move()
    {
        float targetSpeed = sprint ? sprintSpeed : walkSpeed;

        Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y);
        inputDir = transform.TransformDirection(inputDir);

        float control = controller.isGrounded ? 1f : airControl;

        velocity.x = Mathf.Lerp(velocity.x, inputDir.x * targetSpeed, acceleration * control * Time.deltaTime);
        velocity.z = Mathf.Lerp(velocity.z, inputDir.z * targetSpeed, acceleration * control * Time.deltaTime);

        if (controller.isGrounded)
        {
            if (yVelocity < 0) yVelocity = -2f;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                yVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
            }
        }
        else
        {
            yVelocity += gravity * Time.deltaTime;
        }

        velocity.y = yVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
}