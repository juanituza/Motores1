using UnityEngine;

public class SwitchAnimator : MonoBehaviour
{
    public Transform switchTransform;
    public float velocidad = 5f;

    private bool encendido = false;
    // Rotación base X:-45, Y:-90, Z:0
    private Quaternion rotacionApagado = Quaternion.Euler(-45f, -90f, 0f);
    private Quaternion rotacionEncendido = Quaternion.Euler(45f, -90f, 0f); 
    private Quaternion targetRotation;

    void Start()
    {
        targetRotation = rotacionApagado;
    }

    public void ToggleSwitch()
    {
        encendido = !encendido;
        targetRotation = encendido ? rotacionEncendido : rotacionApagado;
    }

    void Update()
    {
        switchTransform.localRotation = Quaternion.Lerp(
            switchTransform.localRotation,
            targetRotation,
            Time.deltaTime * velocidad
        );
    }
}