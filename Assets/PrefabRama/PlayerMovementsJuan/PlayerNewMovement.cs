using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(CharacterController))]
public class PlayerNewMovement : MonoBehaviour
{
    [Header("Referencias de Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference crouchAction;
    // NUEVO: Referencia para la acción de saltar
    [SerializeField] private InputActionReference jumpAction;

    [Header("Referencias de Objetos")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform visualModel;

    [Header("Configuración de Velocidad")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float sprintSpeed = 4f;
    [SerializeField] private float crouchSpeed = 1f;

    // NUEVO: Configuración de la altura del salto
    [Header("Configuración de Salto")]
    [SerializeField] private float jumpHeight = 1.0f; // Altura en metros

    [Header("Configuración de Cámara")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float maxLookAngle = 80f;

    [Header("Configuración de Agacharse")]
    [SerializeField] private float standingHeight = 1.2f;
    [SerializeField] private float crouchHeight = 0.6f;
    [SerializeField] private float standingCameraY = 1.0f;
    [SerializeField] private float crouchCameraY = 0.5f;
    [SerializeField] private float crouchScaleFactorY = 0.5f;
    [SerializeField] private float crouchTransitionSpeed = 10f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Físicas")]
    [SerializeField] private float gravity = -9.81f;

    [Header("Stamina")]
    [SerializeField] private float maxSprintTime = 5f;
    private float currentStamina;

    [Header("Efectos URP")]
    [SerializeField] private Volume globalVolume;
    [SerializeField][Range(0f, 1f)] private float crouchVignetteIntensity = 0.55f;
    [SerializeField] private float vignetteFadeSpeed = 2f;// Arrastrá tu Global Volume acá
    private Vignette vignette;

    public bool IsHidden { get; private set; }

    private CharacterController controller;
    private Vector2 currentMovementInput;
    private Vector2 currentLookInput;
    private Vector3 velocity;

    private float xRotation = 0f;
    private bool isCrouching = false;
    private bool isSprinting = false;
    private bool isExhausted = false;
    private Coroutine crouchCoroutine;

    private Vector3 originalVisualScale;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        controller.height = standingHeight;
        controller.center = new Vector3(0, standingHeight / 2f, 0);

        if (visualModel)
        {
            originalVisualScale = visualModel.localScale;
        }

        SetCameraHeight(standingCameraY);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Llenar stamina
        currentStamina = maxSprintTime;

        if (globalVolume == null)
        {
            // Busca cualquier objeto en la escena que tenga el componente Volume
            globalVolume = FindAnyObjectByType<Volume>();
        }
        // Capturar la viñeta
        if (globalVolume != null && globalVolume.profile.TryGet(out Vignette v))
        {
            vignette = v;
        }
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        sprintAction.action.Enable();
        crouchAction.action.Enable();

        // NUEVO: Habilitamos y nos suscitamos al salto
        jumpAction.action.Enable();
        jumpAction.action.performed += PerformJump;

        sprintAction.action.performed += ctx => isSprinting = true;
        sprintAction.action.canceled += ctx => isSprinting = false;
        crouchAction.action.performed += ctx => StartCrouchTransition(true);
        crouchAction.action.canceled += ctx => StartCrouchTransition(false);
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        sprintAction.action.Disable();
        crouchAction.action.Disable();

        // NUEVO: Nos desuscribimos del salto
        jumpAction.action.performed -= PerformJump;
        jumpAction.action.Disable();
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
        CheckHiddenState();
        HandleVignette();
    }

    private void HandleLook()
    {
        currentLookInput = lookAction.action.ReadValue<Vector2>();
        float lookX = currentLookInput.x * mouseSensitivity;
        transform.Rotate(Vector3.up * lookX);
        float lookY = currentLookInput.y * mouseSensitivity;
        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        cameraTarget.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        currentMovementInput = moveAction.action.ReadValue<Vector2>();
        bool isTryingToSprint = isSprinting;

        if (isTryingToSprint && currentMovementInput.magnitude > 0.1f)
        {
            currentStamina -= Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isTryingToSprint = false; // Forzamos a caminar
            }
        }
        else
        {
            // Regenerar stamina si no corre
            currentStamina += Time.deltaTime;
            if (currentStamina > maxSprintTime) currentStamina = maxSprintTime;
        }

        bool actualSprinting = isSprinting && !isExhausted && currentMovementInput.magnitude > 0.1f && !isCrouching;
        if (actualSprinting)
        {
            // Gastamos estamina
            currentStamina -= Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isExhausted = true; // Se quedó sin aire por completo
            }
        }
        else
        {
            // Recuperamos estamina
            currentStamina += Time.deltaTime;

            // Solo se le va el agotamiento cuando la barra se llena al 100%
            if (currentStamina >= maxSprintTime)
            {
                currentStamina = maxSprintTime;
                isExhausted = false;
            }
        }


        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateStamina(currentStamina / maxSprintTime);
        }

        //Asignacion de Velocidad 
        float currentSpeed = walkSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;
        else if (actualSprinting) currentSpeed = sprintSpeed;

        Vector3 moveDirection = transform.right * currentMovementInput.x + transform.forward * currentMovementInput.y;


        // FÍSICAS: Reseteo de gravedad al tocar el piso
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move((moveDirection * currentSpeed + velocity) * Time.deltaTime);
    }

    // NUEVO: Método que se ejecuta al presionar el botón de salto
    private void PerformJump(InputAction.CallbackContext context)
    {
        // Condición: Solo saltamos si estamos tocando el piso y NO estamos agachados
        if (controller.isGrounded && !isCrouching)
        {
            // Fórmula matemática de Unity: v = raíz cuadrada de (h * -2 * g)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void StartCrouchTransition(bool wantsToCrouch)
    {
        if (crouchCoroutine != null) StopCoroutine(crouchCoroutine);
        crouchCoroutine = StartCoroutine(CrouchSmoothing(wantsToCrouch));
    }

    private IEnumerator CrouchSmoothing(bool wantsToCrouch)
    {
        float targetHeight = wantsToCrouch ? crouchHeight : standingHeight;
        float targetCameraY = wantsToCrouch ? crouchCameraY : standingCameraY;

        Vector3 targetModelScale = originalVisualScale;

        if (wantsToCrouch)
        {
            targetModelScale.y = originalVisualScale.y * crouchScaleFactorY;
        }

        if (!wantsToCrouch)
        {
            Vector3 rayOrigin = transform.position + (Vector3.up * 0.1f);
            if (Physics.Raycast(rayOrigin, Vector3.up, standingHeight, obstacleLayer))
            {
                isCrouching = true;
                yield break;
            }
        }

        isCrouching = wantsToCrouch;

        while (Mathf.Abs(controller.height - targetHeight) > 0.01f)
        {
            float newHeight = Mathf.Lerp(controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
            controller.height = newHeight;
            controller.center = new Vector3(0, newHeight / 2f, 0);

            Vector3 camPos = cameraTarget.localPosition;
            camPos.y = Mathf.Lerp(camPos.y, targetCameraY, crouchTransitionSpeed * Time.deltaTime);
            cameraTarget.localPosition = camPos;

            if (visualModel)
            {
                visualModel.localScale = Vector3.Lerp(visualModel.localScale, targetModelScale, crouchTransitionSpeed * Time.deltaTime);
            }

            yield return null;
        }

        controller.height = targetHeight;
        controller.center = new Vector3(0, targetHeight / 2f, 0);
        SetCameraHeight(targetCameraY);
        if (visualModel) visualModel.localScale = targetModelScale;
    }

    private void SetCameraHeight(float height)
    {
        Vector3 camPos = cameraTarget.localPosition;
        camPos.y = height;
        cameraTarget.localPosition = camPos;
    }

    private void CheckHiddenState()
    {
        if (isCrouching && Physics.Raycast(transform.position, Vector3.up, standingHeight, obstacleLayer)) IsHidden = true;
        else IsHidden = false;
    }

    private void HandleVignette()
    {
        if (vignette == null) return;

        // Decidimos cuál es el valor al que queremos llegar
        float targetVignette = isCrouching ? crouchVignetteIntensity : 0f;

        // Hacemos un Lerp continuo e independiente de la física
        vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, targetVignette, vignetteFadeSpeed * Time.deltaTime);
    }
}