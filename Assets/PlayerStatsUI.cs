using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    public TMP_Text healthDisplayText;
    public TMP_Text goldAmountText;
    public Transform relicParent;
    public GameObject relicPrefab;
    private StatManager statManager;

    private void Awake()
    {
        statManager = FindObjectOfType<StatManager>();
        
    }

    private void Start()
    {
        goldAmountText.text = statManager.goldAmount.ToString();
        

    }



}
