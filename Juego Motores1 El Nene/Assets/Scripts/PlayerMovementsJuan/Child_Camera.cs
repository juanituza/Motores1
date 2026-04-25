using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class Child_Camera : MonoBehaviour
{
    public float sensitivity = 20f;
    public Transform childBody;

    private float _xRotation = 0f;
    private Vector2 _mouseInput;
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
        _mouseInput = context.ReadValue<Vector2>();
    }

    void LateUpdate()
    {
        float mouseX = _mouseInput.x * sensitivity * Time.deltaTime;
        float mouseY = _mouseInput.y * sensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        childBody.Rotate(Vector3.up * mouseX);
    }

    public void TriggerEffect()
    {
        if (_vignette != null)
        {
            StopAllCoroutines();
            StartCoroutine(RedFlashEffect());
        }
    }

    private IEnumerator RedFlashEffect()
    {
        float time = 0;

        while (time < 0.1f)
        {
            _vignette.intensity.value = Mathf.Lerp(0, 0.5f, time / 0.1f);
            time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        time = 0;
        while (time < 0.5f)
        {
            _vignette.intensity.value = Mathf.Lerp(0.5f, 0, time / 0.5f);
            time += Time.deltaTime;
            yield return null;
        }

        _vignette.intensity.value = 0;
    }
}