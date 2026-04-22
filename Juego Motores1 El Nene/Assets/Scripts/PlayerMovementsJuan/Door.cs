using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public float openAngle = 90f;
    public float speed = 2f;

    // Estos valores los ajustás desde el Inspector
    public Vector3 closedSize = new Vector3(3.79f, 6.05f, 0.71f);
    [SerializeField] public Vector3 openSize = new Vector3(1.5f, 4f, 5f); 
    public Vector3 closedCenter = new Vector3(-2f,0f,0f);

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private BoxCollider _myCollider; // Referencia al collider

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + openAngle, transform.eulerAngles.z);

        // Guardamos el componente para no buscarlo a cada rato
        _myCollider = GetComponent<BoxCollider>();

        // Empezamos con el tamaño cerrado
        if (_myCollider != null) _myCollider.size = closedSize;
    }

    void Update()
    {
        if (isOpen)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, openRotation, Time.deltaTime * speed);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, closedRotation, Time.deltaTime * speed);
        }
    }

    public void Interact()
    {
        isOpen = !isOpen;

        if (_myCollider != null)
        {
            // ACÁ USAMOS LAS VARIABLES:
            // Si está abierta, usa el valor que pusiste en el Inspector para openSize
            // Si está cerrada, usa el de closedSize
            _myCollider.size = isOpen ? openSize : closedSize;

            //// Lo mismo para el centro, si quisieras definir variables para el centro
            //if (isOpen)
            //{
            //    // Podés crear una variable "public Vector3 openCenter" si querés ser más pro
            //    _myCollider.center = new Vector3(-1.2f, 1f, 0.5f);
            //}
            //else
            //{
            _myCollider.center = isOpen ? new Vector3(-1f, 0f, -3f) : closedCenter;
            //}
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_myCollider == null) _myCollider = GetComponent<BoxCollider>();

        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(_myCollider.center, _myCollider.size);
        
    }
}