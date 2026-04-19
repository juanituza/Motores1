using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Playermovement : MonoBehaviour
{

    [SerializeField] private CharacterController characterController;

    [SerializeField] private float speed = 10f;

    private float gravity = -9.81f;


   [SerializeField] private Transform groundCheck;
    [SerializeField] private float sphereRadious = 0.3f;
    [SerializeField] private LayerMask groundMask;
   
    bool isGrounded;
    Vector3 velocity;

    [SerializeField] private float jumpHeight = 3;


  
    void Update()
    {

        isGrounded = Physics.CheckSphere(groundCheck.position,sphereRadious,groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
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

     

        Vector3 move  = transform.right * x + transform.forward * z;

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
      

        characterController.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }
}
