using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    public TMP_Text healthDisplayText;
    public TMP_Text goldAmountText;
    public TMP_Text heroName;
    public Transform relicParent;
    public GameObject relicPrefab;
    public Button showDeckButton;
    private StatManager statManager;

    private void Awake()
    {
        statManager = FindObjectOfType<StatManager>();
        
    }

    private void Start()
    {
        DisplayRelics();
        if (statManager.character != null) heroName.text = "The " + statManager.character.name;
        goldAmountText.text = statManager.goldAmount.ToString();
        healthDisplayText.text = $"{statManager.playerCurrentHealth} / {statManager.playerMaxHealth}";

    }

    public void DisplayRelics()
    {
        if (statManager == null) statManager = FindObjectOfType<StatManager>();

        if (statManager.relics.Count == 0) return;

        foreach (Transform c in relicParent)
            Destroy(c.gameObject);

        foreach (Relic r in statManager.relics)
            Instantiate(relicPrefab, relicParent).GetComponent<Image>().sprite = r.relicIcon;
    }



}
