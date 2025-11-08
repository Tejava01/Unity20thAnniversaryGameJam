using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxHealth;

    private int health;

    public Action onDamage;
    public Action onHeal;
    public Action onDeath;

    private void Awake()
    {
        health = maxHealth;
    }


    public void takeDamage(int amount)
    {
        if (amount <= 0) return;

        health -= amount;
        if (health <= 0) onDeath?.Invoke();

        onDamage?.Invoke();
    }

    public void gainHealth(int amount)
    {
        if (amount <= 0) return;

        health += amount;
        if (health > maxHealth) health = maxHealth;

        onHeal?.Invoke();
    }

    public int getHealth() => health;
    public float getHealthRatio() => (float)health / maxHealth;
}
