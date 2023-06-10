using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Character : ScriptableObject
{
    public GameObject characterPrefab;
    public Relic startingRelic;
    public Sprite splashArt;
    public List<Card> startingDeck;
}
