using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 80f;
    [SerializeField] private Transform playerBody;
    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = 0f;
        float mouseY = 0f;

       
        if (Mouse.current != null)
        {
            mouseX = Mouse.current.delta.x.ReadValue() * mouseSensitivity * Time.deltaTime;
            mouseY = Mouse.current.delta.y.ReadValue() * mouseSensitivity * Time.deltaTime;
        }

       
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        playerBody.Rotate(Vector3.up * mouseX);
    }
}