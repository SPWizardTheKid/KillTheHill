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
    }
}
