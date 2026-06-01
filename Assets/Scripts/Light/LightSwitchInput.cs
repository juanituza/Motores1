using LightMaster;
using UnityEngine;
public class LightSwitchInput : MonoBehaviour
{
    public LightAction lightAction;
    [Header("Configuración")]
    public float distanciaMaxima = 4f;
    public LayerMask capaInteractuable;

    private SwitchAnimator switchAnimator; // ← agregamos esto

    void Start()
    {
        // Busca el animator pero no explota si no existe
        switchAnimator = GetComponent<SwitchAnimator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray rayo = Camera.main.ViewportPointToRay(
                new Vector3(0.5f, 0.5f, 0)
            );
            RaycastHit hit;
            if (Physics.Raycast(rayo, out hit, distanciaMaxima, capaInteractuable))
            {
                LightSwitchInput interruptor =
                    hit.collider.GetComponentInParent<LightSwitchInput>();

                if (interruptor == this)
                {
                    // Solo anima si tiene SwitchAnimator
                    if (switchAnimator != null)
                        switchAnimator.ToggleSwitch();

                    lightAction.PerformAction();
                }
            }
        }
    }
}