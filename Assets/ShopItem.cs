using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public Image itemImage;
    public Image shadow;
    public Image title;
    public TMP_Text itemName;
    public TMP_Text itemDescription;
    public TMP_Text itemType;
    public TMP_Text priceText;
    public Transform rarityBorder;

    public int price;
    public int itemIndex;

    private StatManager statManager;
    private PlayerStatsUI playerStatsUI;

    public bool isCard;
    public bool isRelic;

    private void Awake()
    {
        statManager = FindObjectOfType<StatManager>();
        playerStatsUI = FindObjectOfType<PlayerStatsUI>();
    }

    public void DisplayRelic(Relic relic)
    {
        itemImage.sprite = relic.relicIcon;
        itemImage.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
        this.GetComponent<Image>().enabled = false;
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 20f);
        itemName.gameObject.SetActive(false);
        itemDescription.gameObject.SetActive(false);
        itemType.gameObject.SetActive(false);
        shadow.gameObject.SetActive(false);
        title.gameObject.SetActive(false);
        rarityBorder.gameObject.SetActive(false);
    }

    public void DisplayCard(Card card)
    {
        itemImage.sprite = card.cardSprite;
        itemName.text = card.cardName;
        itemDescription.text = card.cardDescription;
        itemType.text = card.cardType;

        foreach (Transform border in rarityBorder)
        {
            border.GetComponent<Image>().color = card.rarity;
        }
    }

    public void DisplayPrice(int itemPrice)
    {
        price = itemPrice;
        priceText.text = itemPrice.ToString() + " $";
    }

    public void BuyItem()
    {
        if (price > statManager.goldAmount) return;

        statManager.UpdateGoldValue(-price);

        if (isCard) 
        {
            statManager.playerDeck.Add(statManager.cardLibrary[itemIndex]);
            Destroy(gameObject);

        }
        else
        {
            statManager.relics.Add(statManager.relicLibrary[itemIndex]);
            playerStatsUI.DisplayRelics();
            Destroy(gameObject);
        }
        

    }

    public void PointerEnter()
    {
        transform.localScale = new Vector2(1.1f, 1.1f);
    }

    public void PointerExit()
    {
        transform.localScale = new Vector2(1f, 1f);
    }
}
