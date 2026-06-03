using UnityEngine;

public class SwitchAnimator : MonoBehaviour
{
    [Header("References")]
    public Transform switchTransform;

    [Header("Animation")]
    public float speed = 5f;

    [Header("Rotation Settings")]
    public Vector3 offRotation = new Vector3(-45f, -90f, 0f);
    public Vector3 onRotation = new Vector3(45f, -90f, 0f);

    private bool isOn = false;

    private Quaternion rotationOff;
    private Quaternion rotationOn;
    private Quaternion targetRotation;

    void Start()
    {
        rotationOff = Quaternion.Euler(offRotation);
        rotationOn = Quaternion.Euler(onRotation);

        targetRotation = rotationOff;
    }

    public void ToggleSwitch()
    {
        isOn = !isOn;

        targetRotation = isOn
            ? rotationOn
            : rotationOff;
    }

    void Update()
    {
        if (switchTransform == null)
            return;

        switchTransform.localRotation = Quaternion.Lerp(
            switchTransform.localRotation,
            targetRotation,
            Time.deltaTime * speed
        );
    }
}