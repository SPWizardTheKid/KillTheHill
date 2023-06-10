using Map;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool elite;

    public List<Card> deck;
    public List<Card> drawPile = new List<Card>();
    public List<Card> hand = new List<Card>();
    public List<CardUI> handGameObjects = new List<CardUI>();
    public List<Card> discardPile = new List<Card>();
    public List<Enemy> enemies = new List<Enemy>();
    public List<GameObject> possibleEnemies;
    public List<GameObject> possibleElites;
    public List<GameObject> specialEncounters;

    public TMP_Text drawPileText;
    public TMP_Text discardPileText;
    public TMP_Text healthText;
    public TMP_Text hasteText;
    public TMP_Text parryValueText;

    public Animator animator;
    public TMP_Text turnText;

    public Button endTurnButton;

    public int drawAmount = 5;
    public int haste;
    public int maxHaste = 3;

    public GameOver gameOver;
    public Transform handTransform;
    public Transform enemyParent;
    public Transform enemy2Parent;
    public Transform summonParent;
    public EndScreen endScreen;

    public CardUI selectedCard;
    public Fighter cardTarget;
    public Fighter player;
    private CardAction cardAction;

    private StatManager statManager;
    private PlayerStatsUI playerStatsUI;

    private int tempParryValue;

    public Turn turn;
    public enum Turn {player, enemy};

    private void Awake()
    {

        cardAction = GetComponent<CardAction>();
        statManager = FindObjectOfType<StatManager>();
        playerStatsUI = FindObjectOfType<PlayerStatsUI>();

        foreach (var cardUi in handGameObjects)
        {
            cardUi.gameObject.SetActive(false);
        }

        if(PlayerPrefs.HasKey("Special"))
        {
            var specialEnemy = PlayerPrefs.GetString("Special");

            if (specialEnemy == "Vergil")
            {
                var enemy = Instantiate(specialEncounters[0], enemyParent);
            }
        }

        else if (elite)
        {
            var elite = Instantiate(possibleElites[Random.Range(0, possibleElites.Count)], enemyParent);
        }

        else
        {
            var multipleEnemies = Random.Range(1, 7);

            if (multipleEnemies > 5)
            {
                multipleEnemies = 2;
                
                Instantiate(possibleEnemies[Random.Range(0, possibleEnemies.Count)], enemyParent);
                Instantiate(possibleEnemies[Random.Range(0, possibleEnemies.Count)], enemy2Parent);
                
            }
            else
            {
                var newEnemy = Instantiate(possibleEnemies[Random.Range(0, possibleEnemies.Count)], enemyParent);
            }

        }

        

        deck = statManager.playerDeck;

        discardPile = new List<Card>();
        drawPile = new List<Card>();
        hand = new List<Card>();


            
        var enemyArray = FindObjectsOfType<Enemy>();
        enemies = new List<Enemy>();

        foreach (var enemy in enemyArray)
        {
            enemies.Add(enemy);
        }

        foreach (var enemy in enemies)
        {
            if (statManager.floorNumber > 1)
            {
                //enemy.currentEnemy.UpdateHealthUI(enemy.currentEnemy.maxHealth + 20);
            }
            enemy.DisplayIntent();
        }




        turnText.text = "Battle Begins!";
        animator.Play("announce");
        

        drawPileText.text = drawPile.Count.ToString();
        discardPileText.text = discardPile.Count.ToString();
        haste = maxHaste;
        hasteText.text = haste.ToString();

        discardPile.AddRange(deck);
        ShuffleCards();
        DrawCards(drawAmount);

        if (statManager.PlayerHasRelic("Steel Tempest"))
        {
            var parry = statManager.playerDeck.Where(c => c.cardName == "Parry").FirstOrDefault();
            DisplayHand(parry);
        }

        //StartCoroutine(DisplayParryValue(3f));
    }

    private void Start()
    {
       

    }

    private IEnumerator DisplayParryValue(float time)
    {
        parryValueText.text = "Curremt parry value: " + player.parryValue;
        parryValueText.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        parryValueText.gameObject.SetActive(false);
    }

    private void ShuffleCards()
    {
        discardPile.Shuffle();
        drawPile = discardPile;
        discardPile = new List<Card>();
    }

    public void DrawCards(int drawAmount)
    {
        var cardsDrawn = 0;

        while (cardsDrawn < drawAmount && hand.Count <= 10)
        {
            if (drawPile.Count < 1) ShuffleCards();

            hand.Add(drawPile[0]);
            DisplayHand(drawPile[0]);
            drawPile.Remove(drawPile[0]);
            drawPileText.text = drawPile.Count.ToString();
            discardPileText.text = discardPile.Count.ToString();
            cardsDrawn++;

        }

    }   

    public void DisplayHand(Card card)
    {
        
        CardUI cardUi = handGameObjects[hand.Count - 1];
        cardUi.Populate(card);
        cardUi.gameObject.SetActive(true);
        
    }

    public void ChangeTurn()
    {
        if (turn == Turn.player)
        {
            turn = Turn.enemy;
            endTurnButton.interactable = false;

            turnText.text = "Enemy's Turn";
            animator.Play("announce");

            foreach (Card card in hand)
            {
                discardPile.Add(card);
                discardPileText.text = discardPile.Count.ToString();

            }
            foreach (CardUI cardUI in handGameObjects)
            {
                cardUI.gameObject.SetActive(false);
                hand.Remove(cardUI.card);
            }


            foreach (Enemy enemy in enemies)
            {
                if (enemy.currentEnemy == null)
                {
                    enemy.currentEnemy = enemy.GetComponent<Fighter>();
                }

                enemy.currentEnemy.EvaluateEffectsAtTurnEnd();

                //reset defence
                enemy.currentEnemy.isDefending = false;
            }

            
            player.EvaluateEffectsAtTurnEnd();  
            StartCoroutine(HandleEnemyTurn());
        }
        else
        {

            foreach (Enemy enemy in enemies)
            {
                enemy.DisplayIntent();
            }

            turn = Turn.player;

            player.parryValue -= tempParryValue;

            haste = maxHaste;
            hasteText.text = haste.ToString();

            endTurnButton.interactable = true;

            if (player.parry.effectValue > 0)
            {
                player.parry.effectValue = 0;
                Destroy(player.parry.effectDisplay.gameObject);
            }

            player.isDefending = false;
            DrawCards(drawAmount);


        }
    }

    private IEnumerator HandleEnemyTurn()
    {
        yield return new WaitForSeconds(0.4f);

        foreach (Enemy enemy in enemies.ToList())
        {
                enemy.midTurn = true;
                enemy.TakeTurn();
                while (enemy.midTurn)
                {
                    yield return new WaitForEndOfFrame();
                }      
        }

        turnText.text = "Player's Turn";
        animator.Play("announce");

        Debug.Log("Turn over");
        ChangeTurn();
    }

    public void DisplayHealth(int healthAmount, int maxHealth)
    {
        healthText.text = $"{healthAmount} / {maxHealth}";
    }

    public void PlayCard(CardUI cardUI)
    {
        Debug.Log("played card");
        cardUI.posSet = false;

        if (cardUI.card.cardName == "Parry" && player.parry.effectValue > 0)
        {
            player.parryValue += player.parryValue;
            tempParryValue += 5;
        }

        cardAction.PerformAction(cardUI.card, cardTarget);

        

        foreach (var enemy in enemies)
        {
            if (enemy.bigBird && enemy.currentEnemy.lamp.effectDisplay != null)
            {
                if (cardUI.card.cardType == "Attack" && enemy.currentEnemy.lamp.effectValue > 1)
                {
                    enemy.currentEnemy.lamp.effectValue -= 1;
                    enemy.currentEnemy.lamp.effectDisplay.DisplayEffect(enemy.currentEnemy.lamp);
                }
                else if (cardUI.card.cardType != "Attack")
                {
                    enemy.currentEnemy.lamp.effectValue += 1;
                    enemy.currentEnemy.lamp.effectDisplay.DisplayEffect(enemy.currentEnemy.lamp);
                }
                else
                {
                    enemy.currentEnemy.AddEffect(Effect.Type.strength, 10);
                    enemy.DisplayIntent();
                    Destroy(enemy.currentEnemy.lamp.effectDisplay.gameObject);
                }
            }
        }

        haste -= 1;
        hasteText.text = haste.ToString();

        selectedCard = null;
        cardUI.gameObject.SetActive(false);
        hand.Remove(cardUI.card);
        discardPile.Add(cardUI.card);
        discardPileText.text = discardPile.Count.ToString();

    }

    public void EndFight(bool win)
    {
        if(win)
        {
            endScreen.gameObject.SetActive(true);
            endScreen.goldReward.gameObject.SetActive(true);
            
            if (PlayerPrefs.HasKey("Special"))
            {
                var specialEnemy = PlayerPrefs.GetString("Special");

                if (specialEnemy == "Vergil")
                {
                    var goldValue = 69;
                    endScreen.goldReward.rewardName.text = goldValue.ToString() + " Gold";
                    statManager.UpdateGoldValue(goldValue);

                    var relicReward = statManager.relicLibrary.Where(r => r.relicName == "Yamato").FirstOrDefault();
                    endScreen.relicReward.gameObject.SetActive(true);
                    endScreen.cardReward.gameObject.SetActive(false);
                    endScreen.relicReward.DisplayRelic(relicReward);

                    statManager.relics.Add(relicReward);
                    statManager.relicLibrary.Remove(relicReward);

                    playerStatsUI.DisplayRelics();

                    PlayerPrefs.DeleteKey("Special");
                    
                }
            }

            else
            {
                endScreen.relicReward.gameObject.SetActive(false);
                endScreen.cardReward.gameObject.SetActive(true);
                var goldValue = Random.Range(12, 45);
                endScreen.goldReward.rewardName.text = goldValue.ToString() + " Gold";
                statManager.UpdateGoldValue(goldValue);
            }

            
            
        }


        else
        {
            gameOver.gameObject.SetActive(true);
            gameOver.HandleGameOver(statManager.floorNumber);
            gameOver.DisplayRelics();

        }
    }

}
