using UnityEngine;
using LightMaster; // Mismo namespace que tu LightController

namespace LightMaster
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Light))]
    [AddComponentMenu("Light /Flashlight Controller")]
    public class FlashlightController : MonoBehaviour, IInteractable
    {
        [Header("Configuración")]
        public KeyCode toggleKey = KeyCode.F;

        [Header("Batería")]
        [Range(0f, 200f)] public float maxBattery = 100f;
        public float drainRate = 2f;
        public float rechargeRate = 5f;
        public bool rechargesWhenOff = true;

        [Header("Batería baja")]
        public float lowBatteryThreshold = 20f;
        public float flickerSpeed = 0.05f;

        // Estado
        private Light _thisLight;
        private float _currentBattery;
        private bool _isOn = false;
        private float _flickerTimer;

        private void Awake()
        {
            _thisLight = GetComponent<Light>();
            _currentBattery = maxBattery;
            _thisLight.enabled = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
                Interact(); // Reutiliza IInteractable igual que tu sistema

            HandleBattery();
            HandleFlicker();
        }

        // Implementa IInteractable — compatible con tu sistema existente
        public void Interact()
        {
            if (!_isOn && _currentBattery <= 0f) return;

            _isOn = !_isOn;
            _thisLight.enabled = _isOn;
        }

        private void HandleBattery()
        {
            if (_isOn)
            {
                _currentBattery -= drainRate * Time.deltaTime;
                _currentBattery = Mathf.Clamp(_currentBattery, 0f, maxBattery);

                if (_currentBattery <= 0f)
                {
                    _isOn = false;
                    _thisLight.enabled = false;
                }
            }
            else if (rechargesWhenOff)
            {
                _currentBattery += rechargeRate * Time.deltaTime;
                _currentBattery = Mathf.Clamp(_currentBattery, 0f, maxBattery);
            }
        }

        private void HandleFlicker()
        {
            if (!_isOn || _currentBattery > lowBatteryThreshold) return;

            _flickerTimer -= Time.deltaTime;
            if (_flickerTimer <= 0f)
            {
                _thisLight.enabled = !_thisLight.enabled;
                float ratio = _currentBattery / lowBatteryThreshold;
                _flickerTimer = flickerSpeed + (flickerSpeed * ratio * 3f);
            }
        }

        // Para la UI
        public float GetBatteryPercent() => (_currentBattery / maxBattery) * 100f;
        public bool IsOn() => _isOn;
    }
}