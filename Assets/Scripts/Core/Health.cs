using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    private void Start()
    {
        
    }

    private void TakeDamage(int damage)
    {
        _currentHealth -= _maxHealth;

        if(_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} You are dead!");
        gameObject.SetActive(false);
    }
}
