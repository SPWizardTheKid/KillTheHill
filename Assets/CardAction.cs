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

            default:
                break;
        }
    }

    private void AttackEnemy()
    {
        var totalDamage = card.amount;

        target.TakeDamage(totalDamage);
    }
}
