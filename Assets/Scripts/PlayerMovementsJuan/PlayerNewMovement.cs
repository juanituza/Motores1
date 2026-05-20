using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerNewMovement : MonoBehaviour
{
    [Header("Referencias de Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference crouchAction;

    [Header("Referencias de Objetos")]
    [SerializeField] private Transform cameraTarget; // Objeto vacío a la altura de los ojos
    [SerializeField] private Transform visualModel;   // EL MODELO 3D DEL NIÑO

    [Header("Configuración de Velocidad")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float sprintSpeed = 4f;
    [SerializeField] private float crouchSpeed = 1f;

    [Header("Configuración de Cámara")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float maxLookAngle = 80f;

    [Header("Configuración de Agacharse (Valores en METROS reales)")]
    [SerializeField] private float standingHeight = 1.2f;
    [SerializeField] private float crouchHeight = 0.6f;
    [SerializeField] private float standingCameraY = 1.0f;
    [SerializeField] private float crouchCameraY = 0.5f;
    [SerializeField] private float crouchScaleFactorY = 0.5f;   // Factor: 0.6 altura / 1.2 total
    [SerializeField] private float crouchTransitionSpeed = 10f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Físicas")]
    [SerializeField] private float gravity = -9.81f;

    public bool IsHidden { get; private set; }

    private CharacterController controller;
    private Vector2 currentMovementInput;
    private Vector2 currentLookInput;
    private Vector3 velocity;

    private float xRotation = 0f;
    private bool isCrouching = false;
    private bool isSprinting = false;
    private Coroutine crouchCoroutine;

    // VARIABLE CLAVE: Guardamos la escala visual base
    private Vector3 originalVisualScale;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // FÍSICA: Escala raíz es 1, CC usa valores reales
        controller.height = standingHeight;
        controller.center = new Vector3(0, standingHeight / 2f, 0);

        // VISUAL: Capturamos la escala actual (ej: 20,20,20) en lugar de forzar a 1
        if (visualModel)
        {
            originalVisualScale = visualModel.localScale;
        }

        SetCameraHeight(standingCameraY);
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        sprintAction.action.Enable();
        crouchAction.action.Enable();

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
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
        CheckHiddenState();
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
        float currentSpeed = walkSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;
        else if (isSprinting) currentSpeed = sprintSpeed;
        Vector3 moveDirection = transform.right * currentMovementInput.x + transform.forward * currentMovementInput.y;
        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move((moveDirection * currentSpeed + velocity) * Time.deltaTime);
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

        // Calculamos el vector de escala objetivo basándonos en la escala original
        Vector3 targetModelScale = originalVisualScale;
        if (wantsToCrouch)
        {
            // Solo afectamos el eje Y multiplicándolo por el factor (ej: 20 * 0.5 = 10)
            targetModelScale.y = originalVisualScale.y * crouchScaleFactorY;
        }

        if (!wantsToCrouch)
        {
            // Subimos el origen del rayo 10 centímetros para evitar que choque con el piso
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
            // Suavizar Físicas y Cámara (igual que antes)
            float newHeight = Mathf.Lerp(controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
            controller.height = newHeight;
            controller.center = new Vector3(0, newHeight / 2f, 0);
            Vector3 camPos = cameraTarget.localPosition;
            camPos.y = Mathf.Lerp(camPos.y, targetCameraY, crouchTransitionSpeed * Time.deltaTime);
            cameraTarget.localPosition = camPos;

            // Suavizar VISUALES usando Lerp de Vectores completos
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
}