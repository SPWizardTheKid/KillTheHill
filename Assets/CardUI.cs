using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CardUI : MonoBehaviour
{
    public Card card;
    public TMP_Text cardName;
    public TMP_Text cardDescription;
    public TMP_Text cartType;
    public Image cardSprite;
    public Transform rarityBorder;

    private GameManager gameManager;
    private Canvas tempCanvas;
    private GraphicRaycaster tempRaycaster;

    private Vector3 initialPos;
    private bool startTimer;
    public bool posSet = false;
    public bool interactable = false;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
     
    }

    public void Populate(Card _card)
    {
        card = _card;
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        cardName.text = card.cardName;
        cardDescription.text = card.cardDescription;
        cartType.text = card.cardType;
        cardSprite.sprite  = card.cardSprite;

        foreach (Transform border in rarityBorder)
        {
            border.GetComponent<Image>().color = card.rarity;
        }
    }

    public void PointerEnter()
    {
        transform.localScale = new Vector2(1.3f, 1.3f);

        if (SceneManager.GetActiveScene().name == "Main Scene") return;
        tempCanvas = gameObject.AddComponent<Canvas>();
        tempCanvas.overrideSorting = true;
        tempCanvas.sortingOrder = 1;
        tempRaycaster = gameObject.AddComponent<GraphicRaycaster>();

        
    }

    public void PointerExit()
    {
        transform.localScale = new Vector2(1f, 1f);
        if (SceneManager.GetActiveScene().name == "Main Scene") return;
        Destroy(tempRaycaster);
        Destroy(tempCanvas);
        
    }

    public void PointerDown()
    {
        if (SceneManager.GetActiveScene().name == "Main Scene" && !interactable) return;

        if (SceneManager.GetActiveScene().name == "Main Scene" && interactable)
        {
            var manager = FindObjectOfType<StatManager>();
            var stats = FindObjectOfType<PlayerStatsUI>();
            manager.playerDeck.Remove(card);
            foreach (Transform card in transform.parent)
            {
                Destroy(card.gameObject);
            }
            transform.parent.gameObject.SetActive(false);
            stats.showDeckButton.interactable = true;
        }

        else
        {

            if (!posSet) initialPos = transform.position;
            posSet = true;
            gameManager.selectedCard = this;
            print(":D");
        }

        
    }

    public void Drag()
    {
        if (SceneManager.GetActiveScene().name == "Main Scene") return;

        if (gameManager.haste <= 0) return;

        transform.localScale = new Vector2(1.3f, 1.3f);

        transform.position = Input.mousePosition;
    }

    public void EndDrag()
    {
        if (SceneManager.GetActiveScene().name == "Main Scene") return;

        Destroy(tempRaycaster);
        Destroy(tempCanvas);

        transform.localScale = new Vector2(1f, 1f);
        StartCoroutine(MoveFunction());

        if (gameManager.cardTarget != null && card.cardType == "Attack" && transform.localPosition.y > 400)
        {
            gameManager.PlayCard(this);
        }
        else if (card.cardType != "Attack" && transform.localPosition.y > 400)
        {
            gameManager.PlayCard(this);
        }
    }

    private IEnumerator MoveFunction()
    {
        var timeSinceStarted = 0f;
        while (true)
        {
            timeSinceStarted += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, initialPos, timeSinceStarted);

            if (transform.position == initialPos)
            {
                yield break;

            }

            yield return null;
        }
    }


}
