using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Fighter : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    private Enemy enemy;
    private GameManager gameManager;
    public FighterHealthBar fighterHealthBar;
    public GameObject damageIndicator;

    public int parryValue;

    public Sprite strengthSprite;
    public Sprite vulnerableSprite;
    public Sprite weakSprite;
    public Sprite parrySprite; 

    [Header("Effects")]
    public Effect strength;
    public Effect vulnerable;
    public Effect weak;
    public Effect parry;
    public Effect vanish;
    public Effect summon;
    public Effect poison;
    public Effect punishment;
    public Effect lamp;

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

        parryValue = 5;

        strength.effectIcon = strengthSprite;
        vulnerable.effectIcon = vulnerableSprite;
        weak.effectIcon = weakSprite;
        parry.effectIcon = parrySprite;


        if (isPlayer)
            gameManager.DisplayHealth(currentHealth, currentHealth);
    }

    public void TakeDamage(int value)
    {
        if (isDefending) value = (int)(value * 0.5);

        Debug.Log($"dealt {value} damage");

        var indicator = Instantiate(damageIndicator, this.transform.position, Quaternion.identity, this.transform).GetComponent<DamageIndicator>();
        indicator.SetDamageText(value);

        currentHealth -= value;
        UpdateHealthUI(currentHealth);

        if (currentHealth <= 0)
        {
            //var leftSummons = gameManager.enemies.Where(s => s.isSummon).ToList();

            foreach (var enemy in gameManager.enemies)
            {
                if (!enemy.isSummon)
                {
                    gameManager.EndFight(true);
                    break;
                }

                else if (gameManager.enemies.Count == 0)
                {
                    gameManager.EndFight(true);
                }


            }

            if (isPlayer) gameManager.EndFight(false);

        }
                

         Destroy(gameObject);
         gameManager.enemies.Remove(enemy);

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

        if (type == Effect.Type.weak)
        {
            if (weak.effectValue <= 0)
            {
                weak.effectDisplay = Instantiate(effectPrefab, effectParent).GetComponent<EffectUI>();
            }
            weak.effectValue += value;
            weak.effectDisplay.DisplayEffect(weak);
        }

        if (type == Effect.Type.strength)
        {
            if (strength.effectValue <= 0)
            {
                //create new buff object
                strength.effectDisplay = Instantiate(effectPrefab, effectParent).GetComponent<EffectUI>();
            }
            strength.effectValue += value;
            strength.effectDisplay.DisplayEffect(strength);
        }

        if (type == Effect.Type.parry)
        {
            if (parry.effectValue <= 0)
            {
                //create new buff object
                parry.effectDisplay = Instantiate(effectPrefab, effectParent).GetComponent<EffectUI>();
            }
            parry.effectValue = parryValue;
            parry.effectDisplay.DisplayEffect(parry);
        }

        if (type == Effect.Type.vanish)
        {
            if (vanish.effectValue <= 0)
            {
                //create new buff object
                vanish.effectDisplay = Instantiate(effectPrefab, effectParent).GetComponent<EffectUI>();
            }
            vanish.effectValue += value;
            vanish.effectDisplay.DisplayEffect(vanish);
        }

        if (type == Effect.Type.poison)
        {
            if (poison.effectValue <= 0)
            {
                //create new buff object
                poison.effectDisplay = Instantiate(effectPrefab, effectParent).GetComponent<EffectUI>();
            }
            poison.effectValue += value;
            poison.effectDisplay.DisplayEffect(poison);
        }

        if (type == Effect.Type.summon)
        {
            summon.effectDisplay = Instantiate(effectPrefab, effectParent).GetComponent<EffectUI>();
            summon.effectDisplay.DisplayEffect(summon);
            summon.effectDisplay.effectText.gameObject.SetActive(false);
        }

        if (type == Effect.Type.punishment)
        {
            punishment.effectDisplay = Instantiate(effectPrefab, effectParent).GetComponent<EffectUI>();
            punishment.effectValue += value;
            punishment.effectDisplay.DisplayEffect(punishment);
            punishment.effectDisplay.effectText.gameObject.SetActive(false);
        }

        if (type == Effect.Type.lamp)
        {
            lamp.effectDisplay = Instantiate(effectPrefab, effectParent).GetComponent<EffectUI>();
            lamp.effectValue += 3;
            lamp.effectDisplay.DisplayEffect(lamp);
            
        }
    }

    public void EvaluateEffectsAtTurnEnd()
    {
        if (vulnerable.effectValue > 0)
        {
            vulnerable.effectValue -= 1;
            vulnerable.effectDisplay.DisplayEffect(vulnerable);

            if (vulnerable.effectValue <= 0)
                Destroy(vulnerable.effectDisplay.gameObject);
        }
        if (weak.effectValue > 0)
        {
            weak.effectValue -= 1;
            weak.effectDisplay.DisplayEffect(weak);

            if (weak.effectValue <= 0)
                Destroy(weak.effectDisplay.gameObject);
        }

        if (vanish.effectValue > 0)
        {
            vanish.effectValue -= 1;
            vanish.effectDisplay.DisplayEffect(vanish);

            if (vanish.effectValue <= 0)
                Destroy(vanish.effectDisplay.gameObject);
        }

        if (poison.effectValue > 0)
        {
            TakeDamage(poison.effectValue);
            poison.effectValue -= 1;
            poison.effectDisplay.DisplayEffect(poison);

            if (poison.effectValue <= 0)
                Destroy(poison.effectDisplay.gameObject);

        }

        if (lamp.effectValue > 0)
        {
            lamp.effectValue -= 1;
            lamp.effectDisplay.DisplayEffect(lamp);

            if (lamp.effectValue <= 0 && lamp.effectDisplay != null)
            {
                AddEffect(Effect.Type.strength, 10);
                Destroy(lamp.effectDisplay.gameObject);
            }

        }


    }

    public void ResetBuffs()
    {
        if (vulnerable.effectValue > 0)
        {
            vulnerable.effectValue = 0;
            Destroy(vulnerable.effectDisplay.gameObject);
        }
        else if (weak.effectValue > 0)
        {
            weak.effectValue = 0;
            Destroy(weak.effectDisplay.gameObject);
        }
        else if (strength.effectValue > 0)
        {
            strength.effectValue = 0;
            Destroy(strength.effectDisplay.gameObject);
        }


        isDefending = false;
    }

    public void UpdateHealthUI(int newAmount)
    {
        currentHealth = newAmount;
        fighterHealthBar.DisplayHealth(newAmount);

        if (isPlayer) gameManager.DisplayHealth(newAmount, maxHealth);

    }
}
