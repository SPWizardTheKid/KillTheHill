using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyAction
{
    public IntentType intentType;
    public enum IntentType { Attack, Defend, SelfBuff, Debuff, AttackDebuff }
    public int amount;
    public int quantity;
    public int debuffAmount;
    public Effect.Type type;
    public int chance;
    public Sprite icon;
}
