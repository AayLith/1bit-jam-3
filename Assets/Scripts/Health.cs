using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;
    public float MaxHealth
    {
        get { return maxHealth; }
    }
    private float currentHealth;
    public float CurrentHealth
    {
        get { return currentHealth; }
    }
    public delegate void OnDeath();
    public OnDeath onDeath;
    // Start is called before the first frame update
    void Start()
    {
        ResetHealth();
    }
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UnityEngine.Debug.Log($"Damage taken{damage} current health is now {currentHealth}");
        if (currentHealth <= float.Epsilon)
        {
            onDeath?.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
