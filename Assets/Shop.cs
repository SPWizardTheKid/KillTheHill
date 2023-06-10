using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public GameObject shopkeeper;
    public GameObject shopBackground;
    public Transform cardGroup;
    public Transform relicGroup;
    public ShopItem shopItem;
    public Button exitShopButton;

    private StatManager statManager;
   
    private bool shownOnce;

    private void Awake()
    {
        statManager = FindObjectOfType<StatManager>();
    }

    public void ShowShopItems()
    {
        shopBackground.SetActive(true);

        if (shownOnce) return;

        statManager.cardLibrary.Shuffle();
        statManager.relicLibrary.Shuffle();

        for (int i = 0; i < 4; i++)
        {
            var shopCard = Instantiate(shopItem, cardGroup);
            shopCard.itemIndex += i;
            shopCard.isCard = true;
            var price = Random.Range(30, 50);
            if(ColorUtility.ToHtmlStringRGB(statManager.cardLibrary[i].rarity) == "FF9D25")
            {
                price += 40;
            }
            else if (ColorUtility.ToHtmlStringRGB(statManager.cardLibrary[i].rarity) == "4BE0E0")
            {
                price += 20;
            }


            shopCard.DisplayCard(statManager.cardLibrary[i]);
            shopCard.DisplayPrice(price);
        }

        for (int i = 0; i < 2; i++)
        {
            var shopRelic = Instantiate(shopItem, relicGroup);
            var price = Random.Range(80, 150);
            shopRelic.itemIndex += i;
            shopRelic.isRelic = true;
            shopRelic.DisplayRelic(statManager.relicLibrary[i]);
            shopRelic.DisplayPrice(price);

        }

        shownOnce = true;
    }

    public void PointerEnter()
    {
        exitShopButton.transform.localScale = new Vector2(1.2f, 1.2f);
    }

    public void PointerExit()
    {
        exitShopButton.transform.localScale = new Vector2(1f, 1f);
    }

    public void Exit()
    {
        shopBackground.SetActive(false);
    }

    public void Proceed()
    {
        gameObject.SetActive(false);
    }
}
