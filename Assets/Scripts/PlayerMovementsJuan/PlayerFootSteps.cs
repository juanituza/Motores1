using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El CharacterController del jugador para leer su velocidad")]
    [SerializeField] private CharacterController controller;
    [Tooltip("El sonido de paso que querés reproducir")]
    [SerializeField] private AudioClip footstepClip;

    [Header("Ritmo de los Pasos (Tiempo)")]
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float sprintStepInterval = 0.35f;
    [SerializeField] private float crouchStepInterval = 0.8f; // Muy pausado para el sigilo

    [Header("Velocidad del Audio (Pitch Sutil)")]
    [SerializeField] private float walkPitch = 1.0f;
    [SerializeField] private float sprintPitch = 1.1f;        // Sutilmente más rápido
    [SerializeField] private float crouchPitch = 0.9f;        // Sutilmente más lento/grave

    [Header("Volumen Base (Para Sigilo)")]
    [SerializeField] private float walkVolume = 1.0f;
    [SerializeField] private float sprintVolume = 1.0f;
    [SerializeField] private float crouchVolume = 0.4f;       // Más bajito al estar agachado

    [Header("Detección de Estado")]
    [Tooltip("Si la velocidad supera este número, asume que corre")]
    [SerializeField] private float sprintSpeedThreshold = 2.5f;
    [Tooltip("Si la velocidad es menor a este número (pero mayor a 0.1), asume que está agachado")]
    [SerializeField] private float crouchSpeedThreshold = 1.5f;

    private AudioSource audioSource;
    private float stepTimer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (controller == null)
        {
            controller = GetComponentInParent<CharacterController>();
        }
    }

    private void Update()
    {
        HandleFootsteps();
    }

    private void HandleFootsteps()
    {
        if (!controller.isGrounded) return;

        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        float currentSpeed = horizontalVelocity.magnitude;

        if (currentSpeed > 0.1f)
        {
            // Determinamos el estado basado en la velocidad física
            bool isSprinting = currentSpeed > sprintSpeedThreshold;
            bool isCrouching = currentSpeed < crouchSpeedThreshold;

            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstepSound(isSprinting, isCrouching);

                // Reiniciamos el reloj dependiendo de la acción
                if (isSprinting) stepTimer = sprintStepInterval;
                else if (isCrouching) stepTimer = crouchStepInterval;
                else stepTimer = walkStepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    private void PlayFootstepSound(bool isSprinting, bool isCrouching)
    {
        float basePitch = walkPitch;
        float baseVolume = walkVolume;

        // Asignamos los valores base según el estado
        if (isSprinting)
        {
            basePitch = sprintPitch;
            baseVolume = sprintVolume;
        }
        else if (isCrouching)
        {
            basePitch = crouchPitch;
            baseVolume = crouchVolume;
        }

        // Variación aleatoria súper sutil (+- 0.03) para que no suene robótico
        audioSource.pitch = basePitch + Random.Range(-0.03f, 0.03f);

        // El volumen también varía un poquito hacia abajo para darle textura
        audioSource.volume = baseVolume * Random.Range(0.85f, 1f);

        audioSource.PlayOneShot(footstepClip);
    }
}