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
        goldAmountText.text = statManager.goldAmount.ToString();

    }

    public void DisplayRelics()
    {
        foreach (Transform c in relicParent)
            Destroy(c.gameObject);

        foreach (Relic r in statManager.relics)
            Instantiate(relicPrefab, relicParent).GetComponent<Image>().sprite = r.relicIcon;
    }



}
