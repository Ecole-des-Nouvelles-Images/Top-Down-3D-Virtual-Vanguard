using UnityEngine;

public class Alien: MonoBehaviour
{
    [Header("Gameplay")]
    public int MaxHealth;

    private float _currentHealth;
        
    public float CurrentHealth
    {
        get => _currentHealth ;
        set
        {
            _currentHealth = value;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);

            if (_currentHealth <= 0)
                Destroy(this.gameObject);
        }
    }

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }
}
