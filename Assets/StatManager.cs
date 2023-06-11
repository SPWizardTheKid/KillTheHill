using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StatManager : MonoBehaviour
{
    public Character character;
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
	public PlayerStatsUI playerStatsUI;
    private GameManager gameManager;


    private void Awake()
    {
        //PlayerPrefs.DeleteAll();


        
        gameManager = FindObjectOfType<GameManager>();

        if (SceneManager.GetActiveScene().name == "Battle Scene")
        {
            if (PlayerPrefs.HasKey("Stats"))
            {
                var statsJson = PlayerPrefs.GetString("Stats");
                var stats = JsonUtility.FromJson<PlayerStats>(statsJson);
                floorNumber = stats.floorNumber;
                character = stats.character;
                goldAmount = stats.goldAmount;
                playerDeck = stats.playerDeck;
                cardLibrary = stats.cardLibrary;
                relics = stats.relics;
                relicLibrary = stats.relicLibrary;
                playerCurrentHealth = stats.playerCurrentHealth;
                playerMaxHealth = stats.playerMaxHealth;

                gameManager.player.currentHealth = playerCurrentHealth;
                gameManager.player.maxHealth = playerMaxHealth;
                gameManager.player.fighterHealthBar.healthSlider.maxValue = playerMaxHealth;
                gameManager.player.UpdateHealthUI(playerCurrentHealth);
                gameManager.DisplayHealth(playerCurrentHealth, gameManager.player.maxHealth);
                playerStatsUI.DisplayRelics();
            }

            else
            {

                ResetSavedValues();
                playerStatsUI.DisplayRelics();
            }
        }

        else
        {
            if (PlayerPrefs.HasKey("Stats"))
            {
                var statsJson = PlayerPrefs.GetString("Stats");
                var stats = JsonUtility.FromJson<PlayerStats>(statsJson);
                floorNumber = stats.floorNumber;
                character = stats.character;
                goldAmount = stats.goldAmount;
                playerDeck = stats.playerDeck;
                cardLibrary = stats.cardLibrary;
                relics = stats.relics;
                relicLibrary = stats.relicLibrary;
                playerCurrentHealth = stats.playerCurrentHealth;
                playerMaxHealth = stats.playerMaxHealth;

                UpdateHealthValue(playerCurrentHealth, playerMaxHealth);
                playerStatsUI.DisplayRelics();
            }

            else
            {
                
                ResetSavedValues();
                playerStatsUI.DisplayRelics();
            }
        }
    }

    public void ResetSavedValues()
    {
        playerCurrentHealth = defaultHealth;
        playerMaxHealth = defaultMaxHealth;
        
        goldAmount = 100;
        UpdateHealthValue(defaultHealth, defaultMaxHealth);
    }

    public bool PlayerHasRelic(string relicName)
    {
        foreach (Relic r in relics)
        {
            if (r.relicName == relicName)
                return true;
        }
        return false;
    }

    public void UpdateHealthValue(int hp, int maxHp)
    {
        if (playerStatsUI.isActiveAndEnabled == false)
        {
            playerStatsUI.gameObject.SetActive(true);
            
            playerStatsUI.healthDisplayText.text = $"{hp} / {maxHp}";
        }
        playerStatsUI.healthDisplayText.text = $"{hp} / {maxHp}";
    }
    public void UpdateGoldValue(int newGold)
    {
        goldAmount += newGold;
        playerStatsUI.goldAmountText.text = goldAmount.ToString();
    }

    public void GameOver()
    {

        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Main Scene");
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
            var stats = new PlayerStats(character, playerDeck, cardLibrary, relics, relicLibrary, floorNumber, goldAmount, gameManager.player.currentHealth, gameManager.player.maxHealth);
            var json = JsonUtility.ToJson(stats);

            PlayerPrefs.SetString("Stats", json);
            PlayerPrefs.Save();

        }
        
        else
        {
            var stats = new PlayerStats(character, playerDeck, cardLibrary, relics, relicLibrary, floorNumber, goldAmount, playerCurrentHealth, playerMaxHealth);
            var json = JsonUtility.ToJson(stats);

            PlayerPrefs.SetString("Stats", json);
            PlayerPrefs.Save();
        }
    }
}
