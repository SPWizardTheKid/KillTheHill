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
        var totalDamage = card.amount + player.strength.effectValue;
        if (target.vulnerable.effectValue > 0)
        {
            float a = totalDamage * 1.5f;
            Debug.Log("incrased damage from " + totalDamage + " to " + (int)a);
            totalDamage = (int)a;
        }
        target.TakeDamage(totalDamage);
    }
}
