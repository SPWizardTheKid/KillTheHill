using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public TMP_Text gameOverText;
    public Transform relicParent;
    public GameObject relicPrefab;

    private StatManager statManager;

    private void Awake()
    {
        statManager = FindObjectOfType<StatManager>();

    }

    private void Start()
    {
        DisplayRelics();
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

    public void HandleGameOver(int floorNumber)
    {
        gameOverText.text = "You have died on " + floorNumber + " floor.\n Better luck next time, I guess...";
    }
}
