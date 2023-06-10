using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTarget : MonoBehaviour
{
    private GameManager gameManager;
    private Fighter enemyFighter;
    private Enemy enemy;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        enemyFighter = GetComponent<Fighter>();
        enemy = GetComponent<Enemy>();
        StartCoroutine(DisplayName(5f));
        
    }


    private IEnumerator DisplayName(float time)
    {
        enemy.enemyName.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        enemy.enemyName.gameObject.SetActive(false);
    }

    public void PointerEnter()
    {
        StartCoroutine(DisplayName(2f));

        if (gameManager.selectedCard != null && gameManager.selectedCard.card.cardType == "Attack")
        {
            gameManager.cardTarget = enemyFighter;
            Debug.Log("set target");
        }
    }

    public void PointerExit()
    {
        //enemy.enemyName.gameObject.SetActive(false);
        gameManager.cardTarget = null;
    }
}
