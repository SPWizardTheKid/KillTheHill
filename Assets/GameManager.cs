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
    public bool boss;
    public bool elite;

    public List<Card> deck = new List<Card>();
    public List<Card> drawPile = new List<Card>();
    public List<Card> hand = new List<Card>();
    public List<CardUI> handGameObjects = new List<CardUI>();
    public List<Card> discardPile = new List<Card>();
    public List<Enemy> enemies = new List<Enemy>();
    public List<GameObject> possibleEnemies;
    public List<GameObject> possibleElites;
    public List<GameObject> specialEncounters;
    public List<GameObject> bosses;

    [Header("Audio")]
    public AudioClip battleStart;
    public AudioClip battleTheme;
    public AudioClip parrySound;
    public AudioClip attackSound;
    public AudioClip swingSound;
    public AudioClip winSound;
    public AudioClip deathSound;
    public AudioClip finalTheme;

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
    public Transform enemy3Parent;
    public Transform summonParent;
    public EndScreen endScreen;

    public CardUI selectedCard;
    public Fighter cardTarget;
    public Fighter player;
    private CardAction cardAction;

    private StatManager statManager;
    private PlayerStatsUI playerStatsUI;

    public int tempParryValue;

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
                var enemy = Instantiate(specialEncounters[0], enemy2Parent);
            }
        }

        else if (PlayerPrefs.HasKey("Boss"))
        {
            var bossName = PlayerPrefs.GetString("Boss");
            if (bossName == "Golem King")
            {
                var boss = Instantiate(bosses[0], enemy2Parent);
            }

            if (bossName == "Lord of Shades")
            {
                var boss = Instantiate(bosses[1], enemy2Parent);
            }

            if (bossName == "Gabriel")
            {
                var boss = Instantiate(bosses[2], enemy2Parent);
            }
            
        }

        else if (statManager.floorNumber == 3)
        {
            var boss = Instantiate(bosses[3], enemy2Parent);
        }

        else if (PlayerPrefs.HasKey("Elite"))
        {
            var elite = Instantiate(possibleElites[Random.Range(0, possibleElites.Count)], enemy2Parent);
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
                var newEnemy = Instantiate(possibleEnemies[Random.Range(0, possibleEnemies.Count)], enemy2Parent);
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
            enemy.LoadEnemy();
            enemy.DisplayIntent();
        }

        if (statManager.PlayerHasRelic("Steel Tempest"))
        {
            var parry = statManager.cardLibrary.Where(c => c.cardName == "Parry").FirstOrDefault();
            deck.Add(parry);
        }

        if (statManager.PlayerHasRelic("Murasama"))
        {
            maxHaste += 1;
        }

        if (statManager.PlayerHasRelic("Yamato"))
        {
            foreach (var enemy in enemies)
            {
                enemy.currentEnemy.AddEffect(Effect.Type.vulnerable, 2);
                enemy.currentEnemy.AddEffect(Effect.Type.weak, 2);
            }
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

        

        //StartCoroutine(DisplayParryValue(3f));
    }

    private void Start()
    {
        //StartCoroutine(PlayDelayedSound());

        if (statManager.floorNumber < 3)
        {

            StartCoroutine(PlayDelayedSound());
        }
        else
        {
            StartCoroutine(PlayDelayedFinalTheme());
        }

    }

    private IEnumerator PlayDelayedSound()
    {
        //battleStart.
        Audio.instance.Play(battleStart);
        yield return new WaitForSeconds(battleStart.length);
        Audio.instance.Play(battleTheme, true);
        
    }

    private IEnumerator PlayDelayedFinalTheme()
    {
        Audio.instance.Play(battleStart);
        yield return new WaitForSeconds(battleStart.length);
        Audio.instance.Play(finalTheme, true);
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


            if (player.parryValue >= 5)
            {
                player.parryValue -= tempParryValue;
            }

            tempParryValue = 0;

            if (player.parry.effectValue > 0)
            {
                player.parry.effectValue = 0;
                Destroy(player.parry.effectDisplay.gameObject);
            }

            haste = maxHaste;
            hasteText.text = haste.ToString();

            endTurnButton.interactable = true;

            

            player.isDefending = false;
            DrawCards(drawAmount);


        }
    }

    private IEnumerator HandleEnemyTurn()
    {
        yield return new WaitForSeconds(0.4f);

        foreach (Enemy enemy in enemies.ToList())
        {
            if (enemy.miceSummon)
            {
                enemy.miceSummon = false;
                continue;
            }
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

        if (cardUI.card.cardName == "Parry" && player.counterPlay.effectValue > 0)
        {
            player.AddEffect(Effect.Type.strength, 1);
        }

        if (cardUI.card.cardName == "Parry" && player.parry.effectValue > 0)
        {
            player.parryValue += player.parryValue;
            tempParryValue += 5;
        }

        if (cardUI.card.cardType == "Attack")
        {
            Audio.instance.Play(attackSound);
        }
        else
        {
            Audio.instance.Play(swingSound);
            if (statManager.PlayerHasRelic("Ancient Scroll"))
            {
                player.currentHealth += 1;
                if (player.currentHealth > player.maxHealth) player.currentHealth = player.maxHealth;
                player.UpdateHealthUI(player.currentHealth);
            }
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

            Audio.instance.Stop(battleTheme);
            Audio.instance.Play(winSound);

            if (statManager.PlayerHasRelic("Steel Tempest"))
            {
                var parry = deck.Where(c => c.cardName == "Parry").FirstOrDefault();
                statManager.playerDeck.Remove(parry);
            }

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

                    PlayerPrefs.SetString("Menu", "False");
                    PlayerPrefs.Save();
                }
            }

            else if (PlayerPrefs.HasKey("Elite"))
            {
                endScreen.relicReward.gameObject.SetActive(true);
                var goldValue = Random.Range(50, 60);
                if (statManager.relicLibrary.Count > 0)
                {
                    statManager.relicLibrary.Shuffle();
                    endScreen.goldReward.rewardName.text = goldValue.ToString() + " Gold";
                    var relicReward = statManager.relicLibrary.Where(r => r.relicName != "Yamato").FirstOrDefault();
                    if (relicReward == null) return;
                    endScreen.relicReward.gameObject.SetActive(true);
                    endScreen.relicReward.DisplayRelic(relicReward);

                    statManager.relics.Add(relicReward);
                    statManager.relicLibrary.Remove(relicReward);
                }
                endScreen.cardReward.gameObject.SetActive(true);

                playerStatsUI.DisplayRelics();

                PlayerPrefs.DeleteKey("Elite");
                PlayerPrefs.SetString("Menu", "False");
                PlayerPrefs.Save();
            }

            else if (PlayerPrefs.HasKey("Boss"))
            {
                var goldValue = Random.Range(70, 99);
                statManager.floorNumber += 1;
                if (statManager.relicLibrary.Count > 0)
                {
                    statManager.relicLibrary.Shuffle();
                    endScreen.goldReward.rewardName.text = goldValue.ToString() + " Gold";
                    var relicReward = statManager.relicLibrary.Where(r => r.relicName != "Yamato").FirstOrDefault();
                    if (relicReward == null) return;
                    endScreen.relicReward.gameObject.SetActive(true);
                    endScreen.relicReward.DisplayRelic(relicReward);

                    statManager.relics.Add(relicReward);
                    statManager.relicLibrary.Remove(relicReward);
                }
                endScreen.cardReward.gameObject.SetActive(false);
                
                statManager.UpdateGoldValue(goldValue);


                playerStatsUI.DisplayRelics();

                
                statManager.playerCurrentHealth = statManager.playerMaxHealth;

                PlayerPrefs.SetString("Menu", "False");
                PlayerPrefs.Save();

                PlayerPrefs.DeleteKey("Boss");

            }

            else
            {
                endScreen.relicReward.gameObject.SetActive(false);
                endScreen.cardReward.gameObject.SetActive(true);
                var goldValue = Random.Range(12, 45);
                endScreen.goldReward.rewardName.text = goldValue.ToString() + " Gold";
                statManager.UpdateGoldValue(goldValue);

                PlayerPrefs.SetString("Menu", "False");
                PlayerPrefs.Save();
            }

            

        }

        else
        {
            Audio.instance.Stop(battleTheme);
            Audio.instance.Play(deathSound);
            gameOver.gameObject.SetActive(true);
            gameOver.HandleGameOver(statManager.floorNumber);
            gameOver.DisplayRelics();

            PlayerPrefs.SetString("Menu", "True");
            PlayerPrefs.Save();

        }
    }

}
