using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    public CardUI cardPrefab;
    public Transform playerDeck;
    public Chest chest;
    public EndScreen endScreen;
    public MapManager mapManager;
    private StatManager statManager;
    private bool isShowing = false;
    private List<CardUI> cardsInDeck;
    public ScrollNonUI scrollNonUI;
    public PlayerStatsUI playerStatsUI;
    public GameOver gameOver;

    [Header("Audio")]
    public AudioClip clickSound;
    public AudioClip embarkSound;
    public AudioClip mapBattle;
    public AudioClip mapMystery;

    public Card judgementCut;
    public string bossName;
    public Mystery finalScene;
    public Menu mainMenu;
    public CharacterSelect characterSelectionScreen;
    public GameObject restSite;
    public GameObject settings;
    public Shop shop;
    public Transform mysteryParent;
    public List<Mystery> possibleMysteries = new List<Mystery>();
    private bool admin = false;
    public bool judgement;
    public GameObject check;

    private void Awake()
    {
        statManager = GetComponent<StatManager>();

    }

    private void Start()
    {
        

        if (statManager.floorNumber >= 3)
        {
            Instantiate(finalScene.gameObject, mysteryParent);
        }
        else
        {
            HandleMainMenu();
            StartCoroutine(LateStart(0.1f));
        }
        
    }

    public void Admin()
    {
        if (!admin)
        {
            check.gameObject.SetActive(true);
            judgement = true;
            admin = true;
        }
        else
        {
            check.gameObject.SetActive(false);
            judgement = false;
            admin = false;
        }
    }

    private IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        scrollNonUI = FindObjectOfType<ScrollNonUI>();
    }


    public void Embark(Character character)
    {
        playerStatsUI.gameObject.SetActive(true);
        statManager.character = character;
        statManager.playerDeck = character.startingDeck;
        if (judgement) statManager.character.startingDeck.Add(judgementCut);
        statManager.relics.Clear();
        statManager.relics.Add(character.startingRelic);
        playerStatsUI.DisplayRelics();
        playerStatsUI.heroName.text = "The " + character.name;
        playerStatsUI.goldAmountText.text = "100";
        Reset();

        mapManager.GenerateNewMap();

        Audio.instance.Play(embarkSound);

    }

    private void Reset()
    {
        statManager.SaveStats();
        PlayerPrefs.DeleteAll();
        statManager.ResetSavedValues();
        statManager.playerStatsUI.DisplayRelics();
        statManager.floorNumber = 1;
    }

    public void HandleMainMenu()
    {
        if (PlayerPrefs.GetString("LoadMenu") == "False") return;

        mainMenu.gameObject.SetActive(true);

        if (PlayerPrefs.HasKey("Stats"))
        {
            mainMenu.continueButton.gameObject.SetActive(true);

        }
        else
        {
            mainMenu.continueButton.gameObject.SetActive(false);
        }
        PlayerPrefs.SetString("LoadMenu", "False");
        PlayerPrefs.Save();
    }

    public void Continue()
    {
        mainMenu.gameObject.SetActive(false);
        playerStatsUI.gameObject.SetActive(true);

        Audio.instance.Play(clickSound);
    }

    public void NewGame()
    {
        mainMenu.gameObject.SetActive(false);
        playerStatsUI.gameObject.SetActive(false);
        characterSelectionScreen.gameObject.SetActive(true);
        Audio.instance.Play(clickSound);
    }

    public void Settings()
    {
        Audio.instance.Play(clickSound);
        settings.SetActive(true);
    }

    public void Quit()
    {
        Audio.instance.Play(clickSound);
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("LoadMenu");
    }

    public void Close()
    {
        settings.SetActive(false);
    }

    public void LoadRandomMystery()
    {
        var rand = Random.Range(0, 8);
        possibleMysteries.Shuffle();

        switch (rand)
        {
            case 0:
                Instantiate(possibleMysteries[0].gameObject, mysteryParent);
                Audio.instance.Play(mapMystery);
                break;
            case 1:
                Instantiate(possibleMysteries[1].gameObject, mysteryParent);
                Audio.instance.Play(mapMystery);
                break;
            case 2:
                Instantiate(possibleMysteries[2].gameObject, mysteryParent);
                Audio.instance.Play(mapMystery);
                break;
            case 3:
                Instantiate(possibleMysteries[3].gameObject, mysteryParent);
                Audio.instance.Play(mapMystery);
                break;
            case 4:
                statManager.LoadBattleScene();
                break;
            case 5:
                statManager.LoadBattleScene();
                break;
            case 6:
                Audio.instance.Play(mapMystery);
                LoadShopScene();
                break;
            case 7:
                Audio.instance.Play(mapMystery);
                LoadShopScene();
                break;
            case 8:
                Audio.instance.Play(mapMystery);
                LoadChestScene();
                break;

            default:
                break;
        }
    }

    public void LoadRestSite()
    {
        restSite.gameObject.SetActive(true);
    }

    public void Rest()
    {
        var restedHP = statManager.playerCurrentHealth + (int)(statManager.playerMaxHealth * 0.3);

        if (restedHP > statManager.playerMaxHealth) restedHP = statManager.playerMaxHealth;

        statManager.UpdateHealthValue(restedHP, statManager.playerMaxHealth);

        statManager.playerCurrentHealth = restedHP;
        restSite.gameObject.SetActive(false);
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
            if (!interactable)
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
        shop.gameObject.SetActive(true);
    }


}
