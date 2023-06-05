using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class PlayerStats
{
	public List<Card> playerDeck = new List<Card>();
	public List<Card> cardLibrary = new List<Card>();
	public List<Relic> relics = new List<Relic>();
	public List<Relic> relicLibrary = new List<Relic>();
	public int floorNumber = 1;
	public int goldAmount;
	public int playerCurrentHealth;
    public int playerMaxHealth;

    public PlayerStats(List<Card> deck, List<Card> library, List<Relic> relicsList, List<Relic> rLibrary, int floor, int gold, int currentHealth, int maxHealth)
    {
        playerDeck = deck;
        cardLibrary = library;
        relics = relicsList;
        relicLibrary = rLibrary;
        floorNumber = floor;
        goldAmount = gold;
        playerCurrentHealth = currentHealth;
        playerMaxHealth = maxHealth;
    }
}


