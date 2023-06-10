using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public struct Effect
{
	public Type type;
	public enum Type { strength, defence, parry, vulnerable, weak, enrage, vanish, summon, poison, punishment, lamp }
	public Sprite effectIcon;
	public int effectValue;
	public EffectUI effectDisplay;
}
