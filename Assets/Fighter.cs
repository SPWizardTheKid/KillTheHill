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
    public int burnValue = 1;

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
    public Effect regen;
    public Effect burn;
    public Effect holyFlame;
    public Effect counterPlay;
    public Effect enrage;
    

    public GameObject effectPrefab;
    public Transform effectParent;

    private StatManager statManager;
    public bool isDefending;
    public bool isPlayer;

    private bool ablaze;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        gameManager = FindObjectOfType<GameManager>();
        statManager = FindObjectOfType<StatManager>();

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
        if (isDefending) 
        {
            value = (int)(value * 0.5);
            if (gameManager.enemies[0].king)
            {
                gameManager.player.AddEffect(Effect.Type.burn, 2);
            }
        }

        Debug.Log($"dealt {value} damage");

        var indicator = Instantiate(damageIndicator, this.transform.position, Quaternion.identity, this.transform).GetComponent<DamageIndicator>();
        indicator.SetDamageText(value);

        currentHealth -= value;
        UpdateHealthUI(currentHealth);

        if (!isPlayer && gameManager.enemies[0].king && currentHealth <= 35 && !ablaze) 
        {
            var burn = Instantiate(damageIndicator, this.transform.position, Quaternion.identity, this.transform).GetComponent<DamageIndicator>();
            indicator.SetDamageText("BLAZE UP");
            currentHealth = 50;
            burnValue = 2;
            UpdateHealthUI(currentHealth);
            ablaze = true;
        }

        if (!isPlayer && gameManager.enemies[0].finalBoss && enrage.effectValue > 0)
        {
            AddEffect(Effect.Type.strength, 2);
        }    

        if (currentHealth <= 0 && isPlayer)
        {
            gameManager.EndFight(false);
            Destroy(gameObject);
            return;
        }


        if (currentHealth <= 0 && !enemy.isSummon)
        {
            var summons = gameManager.enemies.Where(e => e.isSummon).ToList();

            if (enemy.mice)
            {
                    var mice1 = Instantiate(enemy.summons[0], gameManager.enemyParent);
                    var mice2 = Instantiate(enemy.summons[0], gameManager.enemy2Parent);
                    var mice3 = Instantiate(enemy.summons[0], gameManager.enemy3Parent);
                    gameManager.enemies.Add(mice1.GetComponent<Enemy>());
                    gameManager.enemies.Add(mice2.GetComponent<Enemy>());
                    gameManager.enemies.Add(mice3.GetComponent<Enemy>());
                    mice1.GetComponent<Enemy>().miceSummon = true;
                    mice2.GetComponent<Enemy>().miceSummon = true;
                    mice3.GetComponent<Enemy>().miceSummon = true;
                    enemy.mice = false;
            }
            
        

            if (isPlayer) gameManager.EndFight(false);

            Destroy(gameObject);
            gameManager.enemies.Remove(enemy);

            if (gameManager.enemies.Count == 0 || summons.Count == gameManager.enemies.Count)
            {
                foreach (var summon in summons)
                {
                    Destroy(summon.gameObject);
                }
                gameManager.EndFight(true);
            }



        }
        else if (currentHealth <= 0 && enemy.isSummon)
        {
            Destroy(gameObject);
            gameManager.enemies.Remove(enemy);
            
        }

        if (gameManager.enemies.Count == 0)
        {
            print("count");
            gameManager.EndFight(true);
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

        if (type == Effect.Type.enrage)
        {
            if (enrage.effectValue <= 0)
            {
                //create new buff object
                enrage.effectDisplay = Instantiate(effectPrefab, effectParent).GetComponent<EffectUI>();
            }
            enrage.effectValue += value;
            enrage.effectDisplay.DisplayEffect(enrage);
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

        if (type == Effect.Type.regen)
        {
            if (regen.effectValue <= 0)
            {
                //create new buff object
                regen.effectDisplay = Instantiate(effectPrefab, effectParent).GetComponent<EffectUI>();
            }
            regen.effectValue += value;
            regen.effectDisplay.DisplayEffect(regen);
        }

        if (type == Effect.Type.burn)
        {
            if (burn.effectValue <= 0)
            {
                //create new buff object
                burn.effectDisplay = Instantiate(effectPrefab, effectParent).GetComponent<EffectUI>();
            }
            burn.effectValue += value * burnValue;
            burn.effectDisplay.DisplayEffect(burn);
        }

        if (type == Effect.Type.holyFlame)
        {

            gameManager.player.holyFlame.effectDisplay = Instantiate(effectPrefab, effectParent).GetComponent<EffectUI>();
            gameManager.player.holyFlame.effectValue += burn.effectValue;
            gameManager.player.burn.effectValue = 0;
            gameManager.player.holyFlame.effectDisplay.DisplayEffect(holyFlame);
            Destroy(gameManager.player.burn.effectDisplay.gameObject);
        }

        if (type == Effect.Type.counterPlay)
        {
            counterPlay.effectDisplay = Instantiate(effectPrefab, effectParent).GetComponent<EffectUI>();
            counterPlay.effectValue += value;
            counterPlay.effectDisplay.DisplayEffect(counterPlay);
            counterPlay.effectDisplay.effectText.gameObject.SetActive(false);
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
            var summons = gameManager.enemies.Where(e => e.isSummon).ToList();

            if (enemy.lord && summons.Count > 0)
            {
                vanish.effectValue = 1;
                vanish.effectDisplay.DisplayEffect(vanish);
            }

            vanish.effectValue -= 1;
            vanish.effectDisplay.DisplayEffect(vanish);

            if (vanish.effectValue <= 0)
                Destroy(vanish.effectDisplay.gameObject);
        }

        if (poison.effectValue > 0)
        {
            var summons = gameManager.enemies.Where(e => e.isSummon).ToList();
            if (poison.effectValue >= currentHealth)
            {
                currentHealth = 0;
            }
            else
            {

                TakeDamage(poison.effectValue);
            }
            
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

        if (regen.effectValue > 0)
        {
            var heal = currentHealth + regen.effectValue;
            if (heal >= maxHealth) heal = maxHealth;
            UpdateHealthUI(heal);
            regen.effectValue -= 1;
            regen.effectDisplay.DisplayEffect(regen);

            if (regen.effectValue <= 0)
                Destroy(regen.effectDisplay.gameObject);

        }

        if (burn.effectValue > 0)
        {
            var damage = burn.effectValue * burnValue;
            if (statManager.PlayerHasRelic("Water Shield"))
            {
                damage = (int)(damage / 2);
            }
            TakeDamage(damage);
            burn.effectValue -= 1;
            burn.effectDisplay.DisplayEffect(burn);

            if (burn.effectValue <= 0)
                Destroy(burn.effectDisplay.gameObject);

        }

        if (holyFlame.effectValue > 0)
        {
            TakeDamage(holyFlame.effectValue);
            holyFlame.effectDisplay.DisplayEffect(holyFlame);


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
