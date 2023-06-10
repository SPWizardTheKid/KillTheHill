using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class Card : ScriptableObject
{

    public string cardName;

    [TextArea(20, 50)]
    public string cardDescription;

    public Sprite cardSprite;

    public string cardType;

    public int amount;

    public int effectAmount;

    public Color rarity;



}
