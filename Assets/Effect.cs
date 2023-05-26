using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public struct Effect
{
	public Type type;
	public enum Type { strength, defence, parry, vulnerable, weak, enrage }
	public Sprite effectIcon;
	public int effectValue;
	public EffectUI effectDisplay;
}
