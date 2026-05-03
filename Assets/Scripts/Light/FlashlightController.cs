using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace LightMaster
{
    [RequireComponent(typeof(Light))]
    public class FlashlightController : MonoBehaviour
    {
        [Header("Configuración")]
        public bool startOn = false;
        [Range(0f, 100f)] public float maxBattery = 100f;
        public float drainRate = 2f;
        public float lowBatteryThreshold = 20f;

        [Header("Smooth & Flicker")]
        public bool useSmoothSwitch = false;
        public float flickerSpeed = 0.07f;

        private Light _thisLight;
        private float _currentBattery;
        private bool _isOn;
        private float _baseIntensity;
        private float _flickerTimer;

        private void Awake()
        {
            _thisLight = GetComponent<Light>();
            _currentBattery = maxBattery;
            _baseIntensity = _thisLight.intensity;
            _isOn = startOn;
            _thisLight.enabled = startOn;
        }

        private void Update()
        {
            // Verificamos si existe el teclado antes de pedir la tecla
            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
                Interact();

            if (_isOn)
            {
                _currentBattery -= drainRate * Time.deltaTime;
                _currentBattery = Mathf.Max(_currentBattery, 0);

                if (_currentBattery <= 0)
                    TurnOff();
                else if (_currentBattery <= lowBatteryThreshold)
                    DoFlicker();
            }
        }

        public void Interact()
        {
            if (!_isOn && _currentBattery <= 0) return;
            _isOn = !_isOn;
            _thisLight.enabled = _isOn;
            if (_isOn) _thisLight.intensity = _baseIntensity;
        }

        private void TurnOff()
        {
            _isOn = false;
            _thisLight.enabled = false;
        }

        private void DoFlicker()
        {
            _flickerTimer -= Time.deltaTime;
            if (_flickerTimer <= 0)
            {
                _thisLight.enabled = !_thisLight.enabled;
                _flickerTimer = flickerSpeed;
            }
        }

        // SOLO UNA VEZ LOS MÉTODOS DE BATERÍA
        public float GetBatteryPercent() => (_currentBattery / maxBattery) * 100f;
        public float GetBatteryRaw() => _currentBattery;
    }
}