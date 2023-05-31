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

                gameManager.player.currentHealth = playerCurrentHealth;
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

                playerStatsUI.healthDisplayText.text = $"{playerCurrentHealth}/{100}";
            }

            else
            {
                playerStatsUI.healthDisplayText.text = $"{100}/{100}";
            }
        }
        
        
    }

    private void Start()
    {


    }
    public void UpdateGoldValue(int newGold)
    {
        goldAmount += newGold;
        playerStatsUI.goldAmountText.text = goldAmount.ToString();
    }


    public void LoadMainScene()
    {
        SaveStats();

        SceneManager.LoadScene("Main Scene");
    }

    public void SaveStats()
    {
        var stats = new PlayerStats(playerDeck, cardLibrary, relics, relicLibrary, floorNumber, goldAmount, gameManager.player.currentHealth);
        var json = JsonUtility.ToJson(stats);

        PlayerPrefs.SetString("Stats", json);
        PlayerPrefs.Save();
    }
}
