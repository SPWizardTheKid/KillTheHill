using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CharacterSelect : MonoBehaviour
{
    public Image berserkerSprite;
    public Image lockedSprite;
    public TMP_Text specName;
    public Button selectSwordsman;
    public Button selectBerserker;

    public Character swordsman;
    public Character berserker;

    private Character selectedCharacter;
    private Animator animator;
    
    private MainSceneManager mainSceneManager;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        mainSceneManager = FindObjectOfType<MainSceneManager>();
    }

    private void Start()
    {
        animator.Play("FallingSword");

        if (PlayerPrefs.HasKey("BerserkerUnlocked"))
        {

            selectBerserker.gameObject.SetActive(true);
            specName.text = "Berserker";
        }

        else
        {
            selectBerserker.gameObject.SetActive(false);
            berserkerSprite.sprite = lockedSprite.sprite;
            specName.text = "???";
        }
    }


    public void Select(int index)
    {
        Audio.instance.Play(mainSceneManager.clickSound);
        if (index == 0)
        {
            selectSwordsman.interactable = false;
            selectBerserker.interactable = true;
            selectedCharacter = swordsman;
        }

        else
        {
            selectBerserker.interactable = false;
            selectSwordsman.interactable = true;
            selectedCharacter = berserker;
            
        }
    }

    public void SelectCharacter()
    {
        if (selectedCharacter == null) return;
        gameObject.SetActive(false);
        mainSceneManager.Embark(selectedCharacter);
    }



}
