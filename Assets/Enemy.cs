using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public List<EnemyAction> enemyActions;
    public List<EnemyAction> turns = new List<EnemyAction>();
    public int turnNumber;

    public bool shuffleActions;
    public bool midTurn;

    public Fighter currentEnemy;

    public Image intentIcon;
    public TMP_Text intentValue;
    public EffectUI intentUi;


    private Fighter player;
    private GameManager gameManager;





    private void Awake()
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
                Defend();
                StartCoroutine(ApplyEffect());
                break;
            case EnemyAction.IntentType.SelfBuff:
                ApplyBuffToSelf(turns[turnNumber].type);
                StartCoroutine(ApplyEffect());        
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


        var totalDamage = turns[turnNumber].amount + currentEnemy.strength.effectValue;
        if (player.vulnerable.effectValue > 0)
        {
            float a = totalDamage * 1.5f;
            totalDamage = (int)a;
        }

        yield return new WaitForSeconds(0.5f);
        player.TakeDamage(totalDamage);
        yield return new WaitForSeconds(0.5f);
        WrapUpTurn();
    }

    private IEnumerator ApplyEffect()
    {
        yield return new WaitForSeconds(1f);
        WrapUpTurn();
    }

    private void ApplyBuffToSelf(Effect.Type type)
    {
        currentEnemy.AddEffect(type, turns[turnNumber].amount);
    }
    private void ApplyDebuffToPlayer(Effect.Type type)
    {
        if (player == null)
            LoadEnemy();

        player.AddEffect(type, turns[turnNumber].debuffAmount);
    }

    public void Defend()
    {
        currentEnemy.isDefending = true;
    }

    public void DisplayIntent()
    {

        if (turns.Count == 0) LoadEnemy();

        intentValue.enabled = true;
        intentIcon.sprite = turns[turnNumber].icon;

        if (turns[turnNumber].intentType == EnemyAction.IntentType.Defend) intentValue.enabled = false;

        if (turns[turnNumber].intentType == EnemyAction.IntentType.Attack)
        {
            //add strength to attack value
            var totalDamage = turns[turnNumber].amount + currentEnemy.strength.effectValue;
            if (player.vulnerable.effectValue > 0)
            {
                totalDamage = (int)(totalDamage * 1.5f);
            }
            intentValue.text = totalDamage.ToString();
        }
        else
        {
            intentValue.text = turns[turnNumber].amount.ToString();
        }

        print("intent");

        //intent spawn animation
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
