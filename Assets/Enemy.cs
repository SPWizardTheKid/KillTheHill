using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public List<EnemyAction> enemyActions;
    public List<EnemyAction> turns = new List<EnemyAction>();
    public int turnNumber;

    public bool shuffleActions;
    public bool midTurn;

    public Fighter currentEnemy;

    private Fighter player;
    private GameManager gameManager;





    private void Start()
    {
        LoadEnemy();
    }

    private void LoadEnemy()
    {
        gameManager = FindObjectOfType<GameManager>();
        player = gameManager.player;
        currentEnemy = GetComponent<Fighter>();

        if (shuffleActions) GenerateTurns();
    }

    public void TakeTurn()
    {
        switch (turns[turnNumber].intentType)
        {
            case EnemyAction.IntentType.Attack:
                StartCoroutine(AttackPlayer());
                break;
            case EnemyAction.IntentType.Defend:
               
                break;
            case EnemyAction.IntentType.SelfBuff:
                
                break;
            case EnemyAction.IntentType.Debuff:
                
                break;
            case EnemyAction.IntentType.AttackDebuff:
               
                break;
            default:
                break;
        }
    }

    public void GenerateTurns()
    {
        foreach (EnemyAction action in enemyActions)
        {
            for (int i = 0; i < action.chance; i++)
            {
                turns.Add(action);
            }
        }
        turns.Shuffle();
    }

    private IEnumerator AttackPlayer()
    {
        //animation


        int totalDamage = turns[turnNumber].amount; // + strength
        //if (player.vulnerable.buffValue > 0)
        //{
        //    float a = totalDamage * 1.5f;
        //    totalDamage = (int)a;
        //}

        yield return new WaitForSeconds(0.5f);
        player.TakeDamage(totalDamage);
        yield return new WaitForSeconds(0.5f);
        WrapUpTurn();
    }

    private void WrapUpTurn()
    {
        turnNumber++;
        if (turnNumber == turns.Count)
            turnNumber = 0;


        //currentEnemy.EvaluateBuffsAtTurnEnd();
        midTurn = false;
    }
}
