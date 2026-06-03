using UnityEngine;

public class SystemDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private bool doorOpen = false;
    [SerializeField] private float doorOpenAngle = 95f;
 
    [SerializeField] private float smooth = 3.0f;
    [SerializeField]private float initialAngle;

    [SerializeField] private AudioClip openDoor;
    [SerializeField] private AudioClip closeDoor;
   
    [Header("Sistema de Bloqueo")]
    [SerializeField] private bool isLocked = false; 
    [SerializeField] private AudioClip lockedDoorSound; 

    void Start()
    {
        //initialAngle = transform.localEulerAngles.y;
    }


    public void Interact()
    {
        if (isLocked)
        {
            if (lockedDoorSound != null) AudioSource.PlayClipAtPoint(lockedDoorSound, transform.position, 1f);
            return;
        }

        doorOpen = !doorOpen; 

        if (doorOpen)
        {
            if (openDoor != null) AudioSource.PlayClipAtPoint(openDoor, transform.position, 1f);
        }
        else
        {
            if (closeDoor != null) AudioSource.PlayClipAtPoint(closeDoor, transform.position, 1f);
        }
    }
    public void UnlockDoor() // llama a la nota
    {
        isLocked = false;
        Debug.Log("Puerta desbloqueada");
    }

    void Update()
    {
        if (doorOpen)
        {
            Quaternion targetRotation = Quaternion.Euler(0, initialAngle + doorOpenAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
        }
        else
        {
            Quaternion targetRotation2 = Quaternion.Euler(0, initialAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation2, smooth * Time.deltaTime);
        }
    }
}