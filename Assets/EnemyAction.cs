using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyAction
{
    public IntentType intentType;
    public enum IntentType { Attack, Defend, SelfBuff, Debuff, AttackDebuff }
    public int amount;
    public int debuffAmount;
    public int chance;
    public Sprite icon;
}
