using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public GameObject summonPivot;
    private List<Transform> positions = new List<Transform>();

    public bool smallBird;
    public bool bigBird;
    public bool mice;
    public bool miceSummon;
    public bool king;
    public bool lord;
    public bool gabriel;
    public bool finalBoss;

    public bool isSummon;
    public bool shuffleActions;
    public bool midTurn;

    public TMP_Text enemyName;

    public Fighter currentEnemy;

    public Image intentIcon;
    public TMP_Text intentValue;
    public EffectUI intentUi;

    private int turnsWithoutSummons;
    private Fighter player;
    private GameManager gameManager;
    private Animator animator;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        player = gameManager.player;
        currentEnemy = GetComponent<Fighter>();
        animator = GetComponent<Animator>();

        summonPivot = GameObject.FindGameObjectWithTag("Pivot");


        foreach (Transform transform in summonPivot.transform)
        {
            positions.Add(transform);
        }

        if (smallBird) currentEnemy.AddEffect(Effect.Type.vulnerable, 3);

        if (shuffleActions) GenerateTurns();
    }

    private void Start()
    {
        //LoadEnemy();
        if (gabriel) animator.SetBool("Gabriel", true);

    }


    public void LoadEnemy()
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
                ApplyDebuffToPlayer(turns[turnNumber].type);
                StartCoroutine(ApplyEffect());
                break;
            case EnemyAction.IntentType.AttackDebuff:
                StartCoroutine(AttackPlayer());
                StartCoroutine(ApplyDebuff());
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
            
            for (int i = 0; i < action.chance; i++)
            {
                turns.Add(action);
            }
        }

        //foreach (var turn in turns)
        //{
        //    if (turn.chance < 1) turn.chance = 1;
        //}
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
                Audio.instance.Play(gameManager.attackSound);
                yield return new WaitForSeconds(0.5f);
                if (player.parry.effectValue > 0 && temp <= player.parryValue)
                {
                    Audio.instance.Play(gameManager.parrySound);
                    currentEnemy.TakeDamage(temp);
                    if (currentEnemy.currentHealth <= 0)
                    {
                        gameManager.enemies.Remove(this);
                        Destroy(gameObject);
                        if (gameManager.enemies.Count == 0)
                        {
                            gameManager.EndFight(true);
                        }
                        midTurn = false;
                    }    
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
            Audio.instance.Play(gameManager.attackSound);
            yield return new WaitForSeconds(0.5f);
            if (player.parry.effectValue > 0 && totalDamage <= player.parryValue)
            {
                Audio.instance.Play(gameManager.parrySound);
                currentEnemy.TakeDamage(totalDamage);
                if (currentEnemy.currentHealth <= 0)
                {
                    gameManager.enemies.Remove(this);
                    Destroy(gameObject);
                    if (gameManager.enemies.Count == 0)
                    {
                        gameManager.EndFight(true);
                    }
                    midTurn = false;
                }
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
    private IEnumerator ApplyDebuff()
    {
        yield return new WaitForSeconds(1f);
    }

    private void ApplyBuffToSelf(Effect.Type type)
    {
        currentEnemy.AddEffect(type, turns[turnNumber].amount);
    }
    private void ApplyDebuffToPlayer(Effect.Type type)
    {
        if (player == null)
            LoadEnemy();

        var multiply = turns[turnNumber].quantity;
        if (multiply < 1) multiply = 1;

        if (type == Effect.Type.burn)
        {
            for (int i = 0; i < multiply; i++)
            {
                player.AddEffect(type, turns[turnNumber].debuffAmount * currentEnemy.burnValue);
            }
        }
        else
        {
            for (int i = 0; i < multiply; i++)
            {
                player.AddEffect(type, turns[turnNumber].debuffAmount);
            }
            
        }
        
    }


    private void Summon()
    {
        if (summons.Count == 0) return;
        if (!lord)
        { 
            foreach (var summon in summons)
            {
                var newSummon = Instantiate(summon, gameManager.summonParent);
                //newSummon.gameObject.SetActive(true);
                newSummon.GetComponent<Enemy>().isSummon = true;
                newSummon.GetComponent<Fighter>().AddEffect(Effect.Type.summon, 1);
                gameManager.enemies.Add(newSummon.GetComponent<Enemy>());
            }
        }
        else
        {
            for (int i = 0; i < summons.Count; i++)
            {
                var newSummon = Instantiate(summons[i], positions[i].position, Quaternion.identity, positions[i].transform);
                newSummon.GetComponent<Enemy>().isSummon = true;
                newSummon.GetComponent<Fighter>().AddEffect(Effect.Type.summon, 1);
                gameManager.enemies.Add(newSummon.GetComponent<Enemy>());
            }
        }
    }

    public void Defend()
    {
        currentEnemy.isDefending = true;
    }

    public void DisplayIntent()
    {

        //if (turns.Count == 0) LoadEnemy();       
        if (currentEnemy == null) return;


        intentValue.enabled = true;
        intentIcon.sprite = turns[turnNumber].icon;

        if (turns[turnNumber].intentType == EnemyAction.IntentType.Defend
            || turns[turnNumber].intentType == EnemyAction.IntentType.Summon
            || turns[turnNumber].type == Effect.Type.vanish
            || turns[turnNumber].type == Effect.Type.punishment
            || turns[turnNumber].type == Effect.Type.lamp 
            ||turns[turnNumber].type == Effect.Type.holyFlame) intentValue.enabled = false;

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

    public void WrapUpTurn()
    {
        var summons = gameManager.enemies.Where(e => e.isSummon).ToList();

        turnNumber++;

        if (lord && summons.Count <= 0 && turnsWithoutSummons > 2)
        {
            turnNumber = 0;
            turnsWithoutSummons = 0;
        }

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

        

        if (lord && summons.Count > 0)
        {
            turnNumber = 1;
        }    

        else if (lord && summons.Count <= 0 && turnsWithoutSummons < 2)
        {
            turnNumber = 2;
            turnsWithoutSummons++;
            print(turnsWithoutSummons);

        }
        

        if (turnNumber == turns.Count && bigBird)
        {
            turnNumber = 1;
        }

        if (turnNumber == turns.Count && finalBoss)
        {
            turnNumber = 1;
        }

        if (turnNumber == turns.Count)
            turnNumber = 0;

        
        
        midTurn = false;
    }

}
