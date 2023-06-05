using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardUI : MonoBehaviour
{
    public Image rewardImage;
    public TMP_Text rewardName;
    public TMP_Text rewardDescription;
    public TMP_Text rewardType;
    public Transform rarityBorder;

    public void DisplayRelic(Relic relic)
    {
        rewardImage.sprite = relic.relicIcon;
        rewardName.text = relic.relicName;
        rewardDescription.text = relic.relicDescription;
    }
    public void DisplayCard(Card card)
    {
        rewardImage.sprite = card.cardSprite;
        rewardName.text = card.cardName;
        rewardDescription.text = card.cardDescription;
        rewardType.text = card.cardType;

        foreach (Transform border in rarityBorder)
        {
            border.GetComponent<Image>().color = card.rarity;
        }
    }

    public void PointerEnter()
    {

        transform.localScale = new Vector2(1.3f, 1.3f);
    }

    public void PointerExit()
    {
        transform.localScale = new Vector2(1f, 1f);
    }
}
