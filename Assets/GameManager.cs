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
    public Button endTurnButton;

    public int drawAmount = 5;
    public int haste;
    public int maxHaste = 3;

    public Transform handTransform;
    public Transform enemyParent;
    public EndScreen endScreen;

    public CardUI selectedCard;
    public Fighter cardTarget;
    public Fighter player;
    private CardAction cardAction;

    private StatManager statManager;
    private PlayerStatsUI playerStatsUI;
    

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

        else
        {
            var newEnemy = Instantiate(possibleEnemies[Random.Range(0, possibleEnemies.Count)], enemyParent);
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
            enemy.DisplayIntent();
        }



        drawPileText.text = drawPile.Count.ToString();
        discardPileText.text = discardPile.Count.ToString();
        haste = maxHaste;
        hasteText.text = haste.ToString();

        discardPile.AddRange(deck);
        ShuffleCards();
        DrawCards(drawAmount);
    }

    private void Start()
    {
       

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

    public void EndTurn()
    {
        discardPile.AddRange(hand);

        foreach (var cardUi in handGameObjects)
        {
            cardUi.gameObject.SetActive(false);
            hand.Remove(cardUi.card);
        }
        
        discardPileText.text = discardPile.Count.ToString();
    }

    public void ChangeTurn()
    {
        if (turn == Turn.player)
        {
            turn = Turn.enemy;
            endTurnButton.interactable = false;


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

            haste = maxHaste;
            hasteText.text = haste.ToString();

            endTurnButton.interactable = true;
            player.isDefending = false;
            DrawCards(drawAmount);


        }
    }

    private IEnumerator HandleEnemyTurn()
    {
        yield return new WaitForSeconds(1f);

        foreach (Enemy enemy in enemies)
        {
            enemy.midTurn = true;
            enemy.TakeTurn();
            while (enemy.midTurn)
            {
                yield return new WaitForEndOfFrame();
            }
                
        }

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

        cardAction.PerformAction(cardUI.card, cardTarget);

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
    }

}
