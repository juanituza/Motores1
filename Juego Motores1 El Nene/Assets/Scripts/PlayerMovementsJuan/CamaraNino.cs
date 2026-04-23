using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class CamaraNino : MonoBehaviour
{
    public float sensibilidad = 20f;
    public Transform cuerpoNino;

    private float rotacionX = 0f;
    private Vector2 inputRaton;
    private Volume _volume;
    private Vignette _vignette;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _volume = GetComponent<Volume>();
        if (_volume != null && _volume.profile.TryGet(out _vignette))
        {
            _vignette.intensity.value = 0;
            _vignette.color.value = Color.red;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        inputRaton = context.ReadValue<Vector2>();
    }

    void LateUpdate()
    {
        float mouseX = inputRaton.x * sensibilidad * Time.deltaTime;
        float mouseY = inputRaton.y * sensibilidad * Time.deltaTime;

        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);
        cuerpoNino.Rotate(Vector3.up * mouseX);
    }

    public void DispararEfecto()
    {
        if (_vignette != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashRojo());
        }
    }

    IEnumerator FlashRojo()
    {
        float tiempo = 0;
        while (tiempo < 0.1f)
        {
            _vignette.intensity.value = Mathf.Lerp(0, 0.5f, tiempo / 0.1f);
            tiempo += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        tiempo = 0;
        while (tiempo < 0.5f)
        {
            _vignette.intensity.value = Mathf.Lerp(0.5f, 0, tiempo / 0.5f);
            tiempo += Time.deltaTime;
            yield return null;
        }
        _vignette.intensity.value = 0;
    }
}