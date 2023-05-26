using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EffectUI : MonoBehaviour
{
    public Image effectIcon;
    public TMP_Text effectText;

    public void DisplayEffect(Effect effect)
    {
        effectIcon.sprite = effect.effectIcon;
        effectText.text = effect.effectValue.ToString();
    }
}
