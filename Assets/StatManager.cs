using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StatManager : MonoBehaviour
{
	public List<Card> playerDeck = new List<Card>();
	public List<Card> cardLibrary = new List<Card>();
	public List<Relic> relics = new List<Relic>();
	public List<Relic> relicLibrary = new List<Relic>();
	public int floorNumber = 1;
	public int goldAmount;
    public int playerCurrentHealth;
    public int playerMaxHealth;
    public int defaultHealth = 100;
    public int defaultMaxHealth = 100;
	private PlayerStatsUI playerStatsUI;
    private GameManager gameManager;


    private void Awake()
    {
        //PlayerPrefs.DeleteAll();


        playerStatsUI = FindObjectOfType<PlayerStatsUI>();
        gameManager = FindObjectOfType<GameManager>();

        if (SceneManager.GetActiveScene().name == "Battle Scene")
        {
            if (PlayerPrefs.HasKey("Stats"))
            {
                var statsJson = PlayerPrefs.GetString("Stats");
                var stats = JsonUtility.FromJson<PlayerStats>(statsJson);
                goldAmount = stats.goldAmount;
                playerDeck = stats.playerDeck;
                cardLibrary = stats.cardLibrary;
                relics = stats.relics;
                relicLibrary = stats.relicLibrary;
                playerCurrentHealth = stats.playerCurrentHealth;
                playerMaxHealth = stats.playerMaxHealth;

                gameManager.player.currentHealth = playerCurrentHealth;
                gameManager.player.maxHealth = playerMaxHealth;
                gameManager.player.UpdateHealthUI(playerCurrentHealth);
                gameManager.DisplayHealth(playerCurrentHealth, gameManager.player.maxHealth);
            }

            else
            {
                gameManager.DisplayHealth(gameManager.player.currentHealth, gameManager.player.maxHealth);
            }
        }

        else
        {
            if (PlayerPrefs.HasKey("Stats"))
            {
                var statsJson = PlayerPrefs.GetString("Stats");
                var stats = JsonUtility.FromJson<PlayerStats>(statsJson);
                goldAmount = stats.goldAmount;
                playerDeck = stats.playerDeck;
                cardLibrary = stats.cardLibrary;
                relics = stats.relics;
                relicLibrary = stats.relicLibrary;
                playerCurrentHealth = stats.playerCurrentHealth;
                playerMaxHealth = stats.playerMaxHealth;

                UpdateHealthValue(playerCurrentHealth, playerMaxHealth);
            }

            else
            {
                playerCurrentHealth = defaultHealth;
                playerMaxHealth = defaultMaxHealth;
                UpdateHealthValue(defaultHealth, defaultMaxHealth);
            }
        }
        
        
    }

    private void Start()
    {

    }

    public void UpdateHealthValue(int hp, int maxHp)
    {
        playerStatsUI.healthDisplayText.text = $"{hp} / {maxHp}";
    }
    public void UpdateGoldValue(int newGold)
    {
        goldAmount += newGold;
        playerStatsUI.goldAmountText.text = goldAmount.ToString();
    }

    public void GameOver()
    {

    }    


    public void LoadMainScene()
    {
        SaveStats();

        SceneManager.LoadScene("Main Scene");
    }

    public void LoadBattleScene()
    {
        SaveStats();

        SceneManager.LoadScene("Battle Scene");
    }

    public void SaveStats()
    {
        if (SceneManager.GetActiveScene().name == "Battle Scene")
        {
            var stats = new PlayerStats(playerDeck, cardLibrary, relics, relicLibrary, floorNumber, goldAmount, gameManager.player.currentHealth, gameManager.player.maxHealth);
            var json = JsonUtility.ToJson(stats);

            PlayerPrefs.SetString("Stats", json);
            PlayerPrefs.Save();

        }
        
        else
        {
            var stats = new PlayerStats(playerDeck, cardLibrary, relics, relicLibrary, floorNumber, goldAmount, playerCurrentHealth, playerMaxHealth);
            var json = JsonUtility.ToJson(stats);

            PlayerPrefs.SetString("Stats", json);
            PlayerPrefs.Save();
        }
    }
}
