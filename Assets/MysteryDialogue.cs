using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MysteryDialogue : MonoBehaviour
{
	public float delay = 0.05f;
	
	private TMP_Text fullText;
	private string currentText = "";

	// Use this for initialization
	void Start()
	{
		
		StartCoroutine(ShowText());
	}

	public IEnumerator ShowText()
	{
		fullText = GetComponent<TMP_Text>();
		var text = fullText.text;


		for (int i = 0; i < text.Length + 1; i++)
		{
			currentText = text.Substring(0, i);
			this.GetComponent<TMP_Text>().text = currentText;
			yield return new WaitForSeconds(delay);
		}
	}
}
