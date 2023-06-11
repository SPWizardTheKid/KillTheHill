using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuElement : MonoBehaviour
{
    public AudioClip hoverSound;

    public void PointerEnter()
    {
        Audio.instance.Play(hoverSound);
        transform.localScale = new Vector2(1.1f, 1.1f);

        
    }

    public void PointerExit()
    {

        transform.localScale = new Vector2(1f, 1f);
    }
}
