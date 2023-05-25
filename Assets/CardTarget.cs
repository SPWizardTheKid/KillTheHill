using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTarget : MonoBehaviour
{
    private GameManager gameManager;
    private Fighter enemyFighter;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        enemyFighter = GetComponent<Fighter>();
    }

    public void PointerEnter()
    {
        //if (enemyFighter == null)
        //{
        //    gameManager = FindObjectOfType<GameManager>();
        //    enemyFighter = GetComponent<Fighter>();
        //}

        if (gameManager.selectedCard != null && gameManager.selectedCard.card.cardType == "Attack")
        {
            //target == enemy
            gameManager.cardTarget = enemyFighter;
            Debug.Log("set target");
        }
    }

    public void PointerExit()
    {
        gameManager.cardTarget = null;
    }
}
