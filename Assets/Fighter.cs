using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    private Enemy enemy;
    private GameManager gameManager;
    public FighterHealthBar fighterHealthBar;

    [Header("Effects")]
    public Effect strength;
    public Effect vulnerable;
    public Effect weak;
    public Effect parry;

    public GameObject effectPrefab;
    public Transform effectParent;

    public bool isDefending;
    public bool isPlayer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        gameManager = FindObjectOfType<GameManager>();

        currentHealth = maxHealth;
        fighterHealthBar.healthSlider.maxValue = maxHealth;
        fighterHealthBar.DisplayHealth(currentHealth);
        if (isPlayer)
            gameManager.DisplayHealth(currentHealth, currentHealth);
    }

    public void TakeDamage(int value)
    {
        if (isDefending) value = (int)(value * 0.5);

        Debug.Log($"dealt {value} damage");

        currentHealth -= value;
        UpdateHealthUI(currentHealth);

        if (currentHealth <= 0)
        {
            if (enemy != null) gameManager.EndFight(true);

            else gameManager.EndFight(false);


            Destroy(gameObject);
        }
    }

    public void AddEffect(Effect.Type type, int value)
    {
        if (type == Effect.Type.vulnerable)
        {
            if (vulnerable.effectValue <= 0)
            {
                vulnerable.effectDisplay = Instantiate(effectPrefab, effectParent).GetComponent<EffectUI>();
            }
            vulnerable.effectValue += value;
            vulnerable.effectDisplay.DisplayEffect(vulnerable);
        }
        else if (type == Effect.Type.weak)
        {
            if (weak.effectValue <= 0)
            {
                weak.effectDisplay = Instantiate(effectPrefab, effectParent).GetComponent<EffectUI>();
            }
            weak.effectValue += value;
            weak.effectDisplay.DisplayEffect(weak);
        }
        else if (type == Effect.Type.strength)
        {
            if (strength.effectValue <= 0)
            {
                //create new buff object
                strength.effectDisplay = Instantiate(effectPrefab, effectParent).GetComponent<EffectUI>();
            }
            strength.effectValue += value;
            strength.effectDisplay.DisplayEffect(strength);
        }
    }

    public void UpdateHealthUI(int newAmount)
    {
        currentHealth = newAmount;
        fighterHealthBar.DisplayHealth(newAmount);

        if (isPlayer) gameManager.DisplayHealth(newAmount, maxHealth);

    }
}
