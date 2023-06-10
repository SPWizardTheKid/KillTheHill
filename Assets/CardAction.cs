using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAction : MonoBehaviour
{
    private Card card;
    public Fighter player;
    public Fighter target;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void PerformAction(Card _card, Fighter _fighter)
    {
        card = _card;
        target = _fighter;

        switch(card.cardName)
        {
            case "Slash":
                AttackEnemy();
                break;
            case "Defend":
                Defend();
                break;
            case "Parry":
                ApplyBuffToSelf(Effect.Type.parry);
                break;
            case "Violent Cry":
                ViolentCry();
                break;
            case "Unholy Strike":
                AttackEnemy();
                ApplyEffect(Effect.Type.vulnerable);
                ApplyEffect(Effect.Type.weak);
                break;
            case "Judgement Cut":
                AttackEnemy();
                break;
            case "Motivated Strike":
                AttackEnemy();
                ApplyBuffToSelf(Effect.Type.strength);
                break;
            case "Steady Posture":
                SteadyPosture();
                break;

            default:
                break;
        }
    }

    private void Defend()
    {
        player.isDefending = true;
    }

    private void AttackEnemy()
    {
        if (target.vanish.effectValue > 0)
        {
            var indicator = Instantiate(target.damageIndicator, target.transform.position, Quaternion.identity, target.transform).GetComponent<DamageIndicator>();
            indicator.SetDamageText("Miss");
            return;
        } 

            
        var totalDamage = card.amount + player.strength.effectValue;

        if (target.punishment.effectValue > 0)
        {
            var temp = totalDamage;
            totalDamage += 10;
            player.TakeDamage(totalDamage);
            totalDamage = temp;
            var indicator = Instantiate(player.damageIndicator, player.transform.position, Quaternion.identity, player.transform).GetComponent<DamageIndicator>();
            indicator.SetDamageText("PUNISHED");
            return;

        }
        if (target.vulnerable.effectValue > 0)
        {
            float a = totalDamage * 1.5f;
            Debug.Log("incrased damage from " + totalDamage + " to " + (int)a);
            totalDamage = (int)a;
        }

        if (player.weak.effectValue > 0)
        {
            float a = totalDamage / 1.5f;
            totalDamage = (int)a;
        }
        target.TakeDamage(totalDamage);
    }

    private void ViolentCry()
    {
        foreach (var enemy in gameManager.enemies)
        {
            enemy.currentEnemy.AddEffect(Effect.Type.weak, 2);
        }
    }

    private void SteadyPosture()
    {
        player.parryValue += card.amount;
    }

    private void ApplyEffect(Effect.Type t)
    {
        target.AddEffect(t, card.effectAmount);
    }
    private void ApplyBuffToSelf(Effect.Type t)
    {
        player.AddEffect(t, card.effectAmount);
    }
    private void AttackSelf(int selfDamage)
    {
        player.TakeDamage(selfDamage);
    }
}
