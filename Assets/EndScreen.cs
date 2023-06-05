using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    public RewardUI goldReward;
    public RewardUI cardReward;
    public RewardUI relicReward;

    public List<RewardUI> cardRewards;

    private StatManager statManager;

    private void Awake()
    {
        statManager = FindObjectOfType<StatManager>();
    }

    public void HandleCards()
    {
        statManager.cardLibrary.Shuffle();
        cardReward.gameObject.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            cardRewards[i].gameObject.SetActive(true);
            cardRewards[i].DisplayCard(statManager.cardLibrary[i]); 
        }
    }

    public void SelectedCard(int cardIndex)
    {
        print(":D");
        statManager.playerDeck.Add(statManager.cardLibrary[cardIndex]);
        statManager.cardLibrary.Remove(statManager.cardLibrary[cardIndex]);

        for (int i = 0; i < 3; i++)
        {
            cardRewards[i].gameObject.SetActive(false);
        }

        cardReward.gameObject.SetActive(false);

    }
}
