using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    public CardUI cardPrefab;
    public Transform playerDeck;
    public Chest chest;
    public EndScreen endScreen;
    private StatManager statManager;
    private bool isShowing = false;
    private List<CardUI> cardsInDeck;
    public ScrollNonUI scrollNonUI;
    private PlayerStatsUI playerStatsUI;

    public List<Mystery> possibleMysteries = new List<Mystery>();

    private void Awake()
    {
        statManager = GetComponent<StatManager>();
        playerStatsUI = FindObjectOfType<PlayerStatsUI>();
        
    }

    private void Start()
    {
        StartCoroutine(LateStart(0.1f));
    }

    private IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        scrollNonUI = FindObjectOfType<ScrollNonUI>();
    }

    public void LoadRandomMystery()
    {
        var rand = Random.Range(0, 8);
        possibleMysteries.Shuffle();

        switch (rand)
        {
            case 0:
                possibleMysteries[0].gameObject.SetActive(true);
                break;
            case 1:
                possibleMysteries[1].gameObject.SetActive(true);
                break;
            case 2:
                possibleMysteries[3].gameObject.SetActive(true);
                break;
            case 3:
                possibleMysteries[3].gameObject.SetActive(true);
                break;
            case 4:
                statManager.LoadBattleScene();
                break;
            case 5:
                statManager.LoadBattleScene();
                break;
            case 6:
                LoadShopScene();
                break;
            case 7:
                LoadShopScene();
                break;
            case 8:
                LoadChestScene();
                break;

            default:
                break;
        }
    }
    
    public void LoadRestSite()
    {

    }
    
    public void Rest()
    {
        var restedHP = (int)(statManager.playerMaxHealth * 0.3) + statManager.playerCurrentHealth;

        if (restedHP > statManager.playerMaxHealth) restedHP = statManager.playerMaxHealth;

        statManager.UpdateHealthValue(restedHP, statManager.playerMaxHealth);
    }


    public void ShowDeck(bool interactable)
    {
   
        
        if (!isShowing)
        {
            print("yes");
            cardsInDeck = new List<CardUI>();
            foreach (var card in statManager.playerDeck)
            {
                var cardInDeck = Instantiate(cardPrefab, playerDeck);
                cardsInDeck.Add(cardInDeck);
                cardInDeck.Populate(card);
            }

            if (interactable)
            {
                playerStatsUI.showDeckButton.interactable = false;
                foreach (var card in cardsInDeck)
                {
                    card.interactable = true;

                }
                cardsInDeck = new List<CardUI>();
                
            }
            playerDeck.gameObject.SetActive(true);
            scrollNonUI.freezeY = true;
            isShowing = true;
        }

        else
        {
            if(!interactable)
            {
                foreach (var card in cardsInDeck)
                {
                    Destroy(card.gameObject);
                }
            }
            foreach (var card in cardsInDeck)
            {
                Destroy(card.gameObject);
            }

            foreach (var card in cardsInDeck)
            {
                card.interactable = false;
            }

            cardsInDeck.Clear();
            cardsInDeck = new List<CardUI>();

            playerDeck.gameObject.SetActive(false);
            scrollNonUI.freezeY = false;

            isShowing = false;
        }
        
    }
    public void BackToMap()
    {
        chest.gameObject.SetActive(false);
        endScreen.gameObject.SetActive(false);
    }
    public void LoadChestScene()
    {
        chest.gameObject.SetActive(true);
    }

    public void LoadShopScene()
    {

    }

   
}
