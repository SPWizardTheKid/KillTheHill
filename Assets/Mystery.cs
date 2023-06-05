using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Events;

public class Mystery : MonoBehaviour
{
    public Button proceedButton;

    
    [Header("Mystery UI")]
    public TMP_Text initialDialogue;
    public TMP_Text option1Dialogue;
    public TMP_Text option2Dialogue;
    public TMP_Text option3Dialogue;
    public TMP_Text option4Dialogue;
    public TMP_Text option5Dialogue;
    public TMP_Text option6Dialogue;
    public Button option1Button;
    public Button option2Button;
    public Button option3Button;
    public Button option4Button;
    public Button option5Button;

    public bool cleanse;


    private MainSceneManager mainSceneManager;
    private StatManager statManager;

    private void Awake()
    {
        mainSceneManager = FindObjectOfType<MainSceneManager>();
        statManager = FindObjectOfType<StatManager>();
    }

    public void ReturnToMap()
    {
        this.gameObject.SetActive(false);
    }

    public void Fountain(int optionIndex)
    {
        if (optionIndex == 0)
        {
            initialDialogue.gameObject.SetActive(false);
            option1Dialogue.gameObject.SetActive(true);
            option1Button.gameObject.SetActive(false);
            option2Button.gameObject.SetActive(false);


            statManager.playerCurrentHealth += 20;
            if (statManager.playerCurrentHealth > statManager.playerMaxHealth) statManager.playerCurrentHealth = statManager.playerMaxHealth;
            statManager.UpdateHealthValue(statManager.playerCurrentHealth, statManager.playerMaxHealth);

            proceedButton.gameObject.SetActive(true);

        }

        else
        {
            initialDialogue.gameObject.SetActive(false);
            option2Dialogue.gameObject.SetActive(true);
            option1Button.gameObject.SetActive(false);
            option2Button.gameObject.SetActive(false);

            statManager.playerMaxHealth += 10;
            statManager.UpdateHealthValue(statManager.playerCurrentHealth, statManager.playerMaxHealth);

            proceedButton.gameObject.SetActive(true);
        }
    }

    public void Vergil(int optionIndex)
    {
        if (optionIndex == 0)
        {
            PlayerPrefs.SetString("Special", "Vergil");
            PlayerPrefs.Save();
            SceneManager.LoadScene("Battle Scene");
        }

        else
        {
            initialDialogue.gameObject.SetActive(false);
            option2Dialogue.gameObject.SetActive(true);
            option1Button.gameObject.SetActive(false);
            option2Button.gameObject.SetActive(false);

            proceedButton.gameObject.SetActive(true);
        }
    }

    public void Cleanse(int optionIndex)
    {
        

        if (optionIndex == 0)
        {   
            mainSceneManager.ShowDeck(true);
            statManager.UpdateGoldValue(-50);
            ReturnToMap();
        }
        else if (optionIndex == 1)
        {
            initialDialogue.gameObject.SetActive(false);
            option2Dialogue.gameObject.SetActive(true);
            option1Button.gameObject.SetActive(false);
            option2Button.gameObject.SetActive(false);
            option3Button.gameObject.SetActive(false);

            var unholyStrike = statManager.cardLibrary.Where(c => c.cardName == "Unholy Strike").FirstOrDefault();

            statManager.playerDeck.Add(unholyStrike);

            proceedButton.gameObject.SetActive(true);
        }
        else
        {
            initialDialogue.gameObject.SetActive(false);
            option3Dialogue.gameObject.SetActive(true);
            option1Button.gameObject.SetActive(false);
            option2Button.gameObject.SetActive(false);
            option3Button.gameObject.SetActive(false);

            proceedButton.gameObject.SetActive(true);
        }
    }

    public void CryingGirl(int optionIndex)
    {
        if (optionIndex == 0)
        {
            initialDialogue.gameObject.SetActive(false);
            option1Dialogue.gameObject.SetActive(true);
            option1Button.gameObject.SetActive(false);
            option2Button.gameObject.SetActive(false);
            option3Button.gameObject.SetActive(false);

            option4Button.gameObject.SetActive(true);
            option5Button.gameObject.SetActive(true);

        }
        else if (optionIndex == 1)
        {
            initialDialogue.gameObject.SetActive(false);
            option2Dialogue.gameObject.SetActive(true);
            option1Button.gameObject.SetActive(false);
            option2Button.gameObject.SetActive(false);
            option3Button.gameObject.SetActive(false);


            statManager.playerCurrentHealth -= 15;
            if (statManager.playerCurrentHealth <= 0) statManager.GameOver();
            statManager.UpdateHealthValue(statManager.playerCurrentHealth, statManager.playerMaxHealth);

            proceedButton.gameObject.SetActive(true);
        }

        else if (optionIndex == 2)
        {
            option1Dialogue.gameObject.SetActive(false);
            
            option4Button.gameObject.SetActive(false);
            option5Button.gameObject.SetActive(false);
            option5Dialogue.gameObject.SetActive(true);

            var cry = statManager.cardLibrary.Where(c => c.cardName == "Violent Cry").FirstOrDefault();

            statManager.playerDeck.Add(cry);


            proceedButton.gameObject.SetActive(true);
        }

        else if (optionIndex == 3)
        {
            option1Dialogue.gameObject.SetActive(false);
            
            option4Button.gameObject.SetActive(false);
            option5Button.gameObject.SetActive(false);
            option6Dialogue.gameObject.SetActive(true);

            statManager.playerCurrentHealth += 10;
            statManager.playerMaxHealth += 10;
            statManager.UpdateHealthValue(statManager.playerCurrentHealth, statManager.playerMaxHealth);

            proceedButton.gameObject.SetActive(true);
        }

        else
        {
            initialDialogue.gameObject.SetActive(false);
            option3Dialogue.gameObject.SetActive(true);
            option1Button.gameObject.SetActive(false);
            option2Button.gameObject.SetActive(false);
            option3Button.gameObject.SetActive(false);

            proceedButton.gameObject.SetActive(true);
        }
    }



    private void OnEnable()
    {
        if (statManager.goldAmount < 50 && cleanse) option1Button.interactable = false;
        mainSceneManager.scrollNonUI.freezeY = true;
    }

    private void OnDisable()
    {
        mainSceneManager.scrollNonUI.freezeY = false;
    }
}
