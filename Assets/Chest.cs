using Map;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public EndScreen endScreen;
    public AudioClip chestOpen;
    private StatManager statManager;
    private PlayerStatsUI playerStatsUI;


    private void Awake()
    {
        statManager = FindObjectOfType<StatManager>();
        playerStatsUI = FindObjectOfType<PlayerStatsUI>();
    }

    public void DisplayChestRewards()
    {
        if (statManager.relicLibrary.Count == 0) return;

        Audio.instance.Play(chestOpen);
        endScreen.gameObject.SetActive(true);
        endScreen.cardReward.gameObject.SetActive(false);
        endScreen.goldReward.gameObject.SetActive(false);
        statManager.relicLibrary.Shuffle();

        var relicReward = statManager.relicLibrary.Where(r => (r.relicName != "Yamato") && (!r.isBossRelic)).FirstOrDefault();

        endScreen.relicReward.gameObject.SetActive(true);
        

        endScreen.relicReward.DisplayRelic(relicReward);
        statManager.relics.Add(relicReward);
        statManager.relicLibrary.Remove(relicReward);
        playerStatsUI.DisplayRelics();
    }
}
