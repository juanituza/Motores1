using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoNino : MonoBehaviour
{
    private CharacterController _controller;
    private Vector2 _moveInput;
    private Vector3 _direction;

    public float walkSpeed = 100f;
    public float runSpeed = 150f;
    public float gravity = -70f;
    public float jumpForce = 10f;
    private float _verticalVelocity;
    private bool _isRunning;

    [SerializeField] private AudioSource _footstepAudioSource;
    [SerializeField] private AudioClip _footstepSound;
    [SerializeField] private float _walkStepInterval = 0.5f;
    [SerializeField] private float _runStepInterval = 0.3f;
    private float _stepTimer = 0f;


    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed) _isRunning = true;
        if (context.canceled) _isRunning = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && _controller.isGrounded)
        {
            _verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }
    void Update()
    {
        float currentSpeed = _isRunning ? runSpeed : walkSpeed;
        _direction = transform.right * _moveInput.x + transform.forward * _moveInput.y;
        _controller.Move(_direction * currentSpeed * Time.deltaTime);

        if (_controller.isGrounded)
        {
            if (_verticalVelocity < 0)
            {
                _verticalVelocity = -50f;
            }
        }
        else
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 fallMovement = new Vector3(0, _verticalVelocity, 0);
        _controller.Move(fallMovement * Time.deltaTime);

        HandleFootsteps();
    }

  
    private void HandleFootsteps()
    {
        bool isMoving = _controller.isGrounded && _moveInput.magnitude > 0.1f;

        if (isMoving)
        {
            _stepTimer += Time.deltaTime;
            float currentInterval = _isRunning ? _runStepInterval : _walkStepInterval;

            if (_stepTimer >= currentInterval)
            {
                PlayFootstep();
                _stepTimer = 0f;
            }
        }
        else
        {
            _stepTimer = _walkStepInterval;
        }
    }

    private void PlayFootstep()
    {
        if (_footstepSound != null && _footstepAudioSource != null)
        {
            _footstepAudioSource.pitch = Random.Range(0.85f, 1.15f);
            _footstepAudioSource.PlayOneShot(_footstepSound);
        }
    }
}

