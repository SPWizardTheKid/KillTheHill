using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public EndScreen endScreen;
    private StatManager statManager;
    private PlayerStatsUI playerStatsUI;

    private void Awake()
    {
        statManager = FindObjectOfType<StatManager>();
        playerStatsUI = FindObjectOfType<PlayerStatsUI>();
    }

    public void DisplayChestRewards()
    {
        endScreen.gameObject.SetActive(true);
        endScreen.cardReward.gameObject.SetActive(false);
        endScreen.goldReward.gameObject.SetActive(false);
        statManager.relicLibrary.Shuffle();

        if (statManager.relicLibrary[0].relicName == "Yamato") statManager.relicLibrary.Shuffle();

        endScreen.relicReward.gameObject.SetActive(true);
        

        endScreen.relicReward.DisplayRelic(statManager.relicLibrary[0]);
        statManager.relics.Add(statManager.relicLibrary[0]);
        statManager.relicLibrary.Remove(statManager.relicLibrary[0]);
        playerStatsUI.DisplayRelics();
    }
}
