using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speed = 10f;
    private float gravity = -9.81f;

    
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float sphereRadius = 0.3f; 
    [SerializeField] private LayerMask groundMask;

    private bool isGrounded;
    private Vector3 velocity;

    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private Animator animator;

   
    [SerializeField] private UnityEvent OnPlayerJump;
    [SerializeField] private UnityEvent OnPlayerLand;

    void Update()
    {
        Debug.Log("isGrounded: " + isGrounded + " | velocityY: " + velocity.y);
        bool wasGrounded = isGrounded; 
        isGrounded = Physics.CheckSphere(groundCheck.position, sphereRadius, groundMask);

        // 
        if (!wasGrounded && isGrounded)
        {
            OnPlayerLand?.Invoke(); 
        }

        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -5f;
        }

       
        float x = 0f;
        float z = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed) x += 1f; // derecha
            if (Keyboard.current.aKey.isPressed) x -= 1f; // izquierda
            if (Keyboard.current.wKey.isPressed) z += 1f; // adelante
            if (Keyboard.current.sKey.isPressed) z -= 1f; // atras
        }

        
        if (animator != null) 
        {
            animator.SetFloat("VelX", x);
            animator.SetFloat("VelZ", z);
        }

        Vector3 move = transform.right * x + transform.forward * z;
       

        
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            OnPlayerJump?.Invoke(); 
        }
        velocity.y += gravity * Time.deltaTime;
        Vector3 finalMove = (move * speed) + new Vector3(0, velocity.y, 0);
        characterController.Move(finalMove * Time.deltaTime);
    }
}