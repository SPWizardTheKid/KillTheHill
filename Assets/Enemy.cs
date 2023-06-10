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
    public List<GameObject> summons = new List<GameObject>();
    public int turnNumber;
    public Transform targetPos;

    public bool smallBird;
    public bool bigBird;

    public bool isSummon;
    public bool shuffleActions;
    public bool midTurn;

    public TMP_Text enemyName;

    public Fighter currentEnemy;

    public Image intentIcon;
    public TMP_Text intentValue;
    public EffectUI intentUi;


    private Vector3 initialPos;
    private Fighter player;
    private GameManager gameManager;
    private Animator animator;

    private void Awake()
    {
        LoadEnemy();
    }

    private void LoadEnemy()
    {
        gameManager = FindObjectOfType<GameManager>();
        player = gameManager.player;
        currentEnemy = GetComponent<Fighter>();
        animator = GetComponent<Animator>();
        initialPos = transform.position;

        if (smallBird) currentEnemy.AddEffect(Effect.Type.vulnerable, 3);

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
                ApplyDebuffToPlayer(turns[turnNumber].type);
                StartCoroutine(ApplyEffect());
                break;
            case EnemyAction.IntentType.AttackDebuff:
                StartCoroutine(AttackPlayer());
                StartCoroutine(ApplyEffect());
                ApplyDebuffToPlayer(turns[turnNumber].type);
                break;
            case EnemyAction.IntentType.Summon:
                StartCoroutine(ApplyEffect());
                Summon();
                break;
                
            default:
                break;
        }
    }

    public void GenerateTurns()
    {
        foreach (EnemyAction action in enemyActions)
        {
            if (action.chance < 1) action.chance = 1;
            for (int i = 0; i < action.chance; i++)
            {
                turns.Add(action);
            }
        }
        turns.Shuffle();
    }

    private IEnumerator AttackPlayer()
    {
        var totalDamage = turns[turnNumber].amount + currentEnemy.strength.effectValue;
        if (player.vulnerable.effectValue > 0)
        {
            float a = totalDamage * 1.5f;
            totalDamage = (int)a;
        }

        if (turns[turnNumber].quantity > 1)
        {
            var temp = totalDamage;
            for (int i = 0; i < turns[turnNumber].quantity; i++)
            {
                animator.Play("EnemyAttack");
                yield return new WaitForSeconds(0.5f);
                if (player.parry.effectValue > 0 && temp <= player.parryValue)
                {
                    currentEnemy.TakeDamage(temp);
                    totalDamage = 0;
                }
                player.TakeDamage(totalDamage);
                yield return new WaitForSeconds(0.5f);
            }
            WrapUpTurn();
        }
        else
        {
            animator.Play("EnemyAttack");
            yield return new WaitForSeconds(0.5f);
            if (player.parry.effectValue > 0 && totalDamage <= player.parryValue)
            {
                currentEnemy.TakeDamage(totalDamage);
                totalDamage = 0;
            }
            player.TakeDamage(totalDamage);
            yield return new WaitForSeconds(0.5f);
            WrapUpTurn();

        }

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


    private void Summon()
    {
        if (summons.Count == 0) return;

        foreach (var summon in summons)
        {
            var newSummon = Instantiate(summon, gameManager.summonParent);
            //newSummon.gameObject.SetActive(true);
            newSummon.GetComponent<Enemy>().isSummon = true;
            newSummon.GetComponent<Fighter>().AddEffect(Effect.Type.summon, 1);
            gameManager.enemies.Add(newSummon.GetComponent<Enemy>());
        }

    }

    public void Defend()
    {
        currentEnemy.isDefending = true;
    }

    public void DisplayIntent()
    {
        if (currentEnemy == null) return;

        if (turns.Count == 0) LoadEnemy();
        

        intentValue.enabled = true;
        intentIcon.sprite = turns[turnNumber].icon;

        if (turns[turnNumber].intentType == EnemyAction.IntentType.Defend
            || turns[turnNumber].intentType == EnemyAction.IntentType.Summon
            || turns[turnNumber].type == Effect.Type.vanish
            || turns[turnNumber].type == Effect.Type.punishment
            || turns[turnNumber].type == Effect.Type.lamp) intentValue.enabled = false;

        if (turns[turnNumber].intentType == EnemyAction.IntentType.Attack || turns[turnNumber].intentType == EnemyAction.IntentType.AttackDebuff)
        {
            //add strength to attack value
            var totalDamage = turns[turnNumber].amount + currentEnemy.strength.effectValue;
            if (player.vulnerable.effectValue > 0)
            {
                totalDamage = (int)(totalDamage * 1.5f);
                //intentValue.text = totalDamage.ToString();
            }

            

            if (turns[turnNumber].quantity > 1)
            { 
                intentValue.text = $"{totalDamage}x{turns[turnNumber].quantity}"; 
            }
            else
            {
                intentValue.text = totalDamage.ToString();
            }
                
        }
        else
        {
            intentValue.text = turns[turnNumber].amount.ToString();
        }


        animator.Play("Intent");
        
    }

    private void WrapUpTurn()
    {
        turnNumber++;

        if (turnNumber == turns.Count && smallBird)
        {
            if (gameManager.enemies.Count > 1)
            {
                Destroy(gameObject);
                gameManager.enemies.Remove(this);
                midTurn = false;
            }
            else
            {
                Destroy(gameObject);
                gameManager.EndFight(true);
            }
        }


        if (turnNumber == turns.Count && bigBird)
        {
            turnNumber = 1;
        }

        if (turnNumber == turns.Count)
            turnNumber = 0;

        
        
        midTurn = false;
    }

}
