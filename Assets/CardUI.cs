using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour
{
    public Card card;
    public TMP_Text cardName;
    public TMP_Text cardDescription;
    public TMP_Text cartType;
    public Image cardSprite;

    private GameManager gameManager;

    private Vector3 initialPos;
    private bool startTimer;
    public bool posSet = false;

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
    }

    public void PointerEnter()
    {
        transform.localScale = new Vector2(1.3f, 1.3f);
    }

    public void PointerExit()
    {
        transform.localScale = new Vector2(1f, 1f);
    }

    public void PointerDown()
    {

        if (!posSet) initialPos = transform.position;
        posSet = true;
        gameManager.selectedCard = this;
        print(":D");
    }

    public void Drag()
    {
        if (gameManager.haste <= 0) return;

        transform.localScale = new Vector2(1.3f, 1.3f);

        transform.position = Input.mousePosition;
    }

    public void EndDrag()
    {
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
