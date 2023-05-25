using Map;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Card> deck;
    public List<Card> drawPile = new List<Card>();
    public List<Card> hand = new List<Card>();
    public List<CardUI> handGameObjects = new List<CardUI>();
    public List<Card> discardPile = new List<Card>();
    public List<Enemy> enemies = new List<Enemy>();

    public TMP_Text drawPileText;
    public TMP_Text discardPileText;
    public TMP_Text healthText;
    public TMP_Text hasteText;
    public Button endTurnButton;

    public int drawAmount = 5;
    public int haste;
    public int maxHaste = 3;

    public Transform handTransform;

    public CardUI selectedCard;
    public Fighter cardTarget;
    private CardAction cardAction;

    public Turn turn;
    public enum Turn {player, enemy};

    private void Awake()
    {
        cardAction = GetComponent<CardAction>();

        foreach (var cardUi in handGameObjects)
        {
            cardUi.gameObject.SetActive(false);
        }

        discardPile = new List<Card>();
        drawPile = new List<Card>();
        hand = new List<Card>();

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


            //foreach (Enemy e in enemies)
            //{
            //    if (e.thisEnemy == null)
            //        e.thisEnemy = e.GetComponent<Fighter>();

            //    //reset block
            //    e.thisEnemy.currentBlock = 0;
            //    e.thisEnemy.fighterHealthBar.DisplayBlock(0);
            //}


            StartCoroutine(HandleEnemyTurn());
        }
        else
        {
            //foreach (Enemy e in enemies)
            //{
            //    e.DisplayIntent();
            //}

            turn = Turn.player;

            haste = maxHaste;
            hasteText.text = haste.ToString();

            endTurnButton.interactable = true;
            DrawCards(drawAmount);


        }
    }

    private IEnumerator HandleEnemyTurn()
    {
        yield return new WaitForSeconds(1.5f);

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

        cardAction.PerformAction(cardUI.card, cardTarget);

        haste -= cardUI.card.cardCost;
        hasteText.text = haste.ToString();

        selectedCard = null;
        cardUI.gameObject.SetActive(false);
        hand.Remove(cardUI.card);
        discardPile.Add(cardUI.card);
        discardPileText.text = discardPile.Count.ToString();

    }

    public void EndFight(bool win)
    {
        print("ended");
    }

}
