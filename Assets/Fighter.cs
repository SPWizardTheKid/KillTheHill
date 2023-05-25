using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    private Enemy enemy;
    public GameManager gameManager;
    public FighterHealthBar fighterHealthBar;

    public bool isPlayer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        currentHealth = maxHealth;
        fighterHealthBar.healthSlider.maxValue = maxHealth;
        fighterHealthBar.DisplayHealth(currentHealth);
        if (isPlayer)
            gameManager.DisplayHealth(currentHealth, currentHealth);
    }

    public void TakeDamage(int amount)
    {

        Debug.Log($"dealt {amount} damage");

        currentHealth -= amount;
        UpdateHealthUI(currentHealth);

        if (currentHealth <= 0)
        {
            if (enemy != null)
                gameManager.EndFight(true);
            else
                gameManager.EndFight(false);

            Destroy(gameObject);
        }
    }

    public void UpdateHealthUI(int newAmount)
    {
        currentHealth = newAmount;
        fighterHealthBar.DisplayHealth(newAmount);

        if (isPlayer) gameManager.DisplayHealth(newAmount, maxHealth);

    }
}
