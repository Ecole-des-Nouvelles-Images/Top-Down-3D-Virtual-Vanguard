using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager: MonoBehaviour
{
    public static Action<Slider, float> UpdateBattery;
    public static Action<Image> UpdateChargeStatus;

    private Image _currentChargeIndicator;

    private void OnEnable() {
        UpdateBattery += OnUpdateBattery;
        UpdateChargeStatus += OnUpdateChargeStatus;
    }

    public void OnDisable() {
        UpdateBattery -= OnUpdateBattery;
        UpdateChargeStatus -= OnUpdateChargeStatus;
    }

    private void OnUpdateBattery(Slider battery, float charge)
    {
        if (!battery)
        {
            Debug.LogWarning("Warning: Battery Slider not set on a module");
            return;
        }
        battery.value = charge;
    }
    
    private void OnUpdateChargeStatus(Image indicator)
    {
        if (_currentChargeIndicator != null)
            _currentChargeIndicator.color = Color.black;

        if (indicator == null) {
            return;
        }
        
        indicator.color = Color.white;
        _currentChargeIndicator = indicator;
    }
}
