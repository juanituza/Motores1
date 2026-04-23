using UnityEngine;

public class SystemDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private bool doorOpen = false;
    [SerializeField] private float doorOpenAngle = 95f;
    [SerializeField] private float doorCloseAngle = 0.0f;
    [SerializeField] private float smooth = 3.0f; // velocidad rotacion

    [SerializeField] private AudioClip openDoor;
    [SerializeField] private AudioClip closeDoor;

    public void Interact()
    {
        doorOpen = !doorOpen;
    }

    void Update()
    {
        if (doorOpen)
        {
            Quaternion targetRotation = Quaternion.Euler(0, doorOpenAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
        }
        else
        {
            Quaternion targetRotation2 = Quaternion.Euler(0, doorCloseAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation2, smooth * Time.deltaTime);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!doorOpen) Interact();
        }

       
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if (openDoor != null) AudioSource.PlayClipAtPoint(openDoor, transform.position, 1);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
        if (other.CompareTag("Enemy"))
        {
            if (doorOpen) Interact();
        }

        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if (closeDoor != null) AudioSource.PlayClipAtPoint(closeDoor, transform.position, 1);
        }
    }
}