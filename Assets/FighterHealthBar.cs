using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FighterHealthBar : MonoBehaviour
{
    public TMP_Text healthText;
    public Slider healthSlider;
    public void DisplayHealth(int healthAmount)
    {
        healthText.text = $"{healthAmount}/{healthSlider.maxValue}";
        healthSlider.value = healthAmount;
    }
}
