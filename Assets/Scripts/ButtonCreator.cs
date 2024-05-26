using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System.Security.Cryptography;

public class ButtonCreator : MonoBehaviour
{
    public static ButtonCreator instance;

    public GameObject buttonPrefab;
    public Transform buttonholder1;
    public Transform buttonholder2;
    public Transform buttonholder3;

    string[] lettersToUse = new string[27] 
    {
        "Q","W","E","R","T","Y","U","I","O","P",
        "A","S","D","F","G","H","J","K","L","Ç",
        "Z","X","C","V","B","N","M"
    };

    List<LetterButton> letterList = new List<LetterButton>();

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PopulateKeyboard();
    }

    // Update is called once per frame
    void PopulateKeyboard()
    {
        for (int i = 0; i < lettersToUse.Length; i++)
        {
            if (i < 10)
            {
                GameObject newButton = Instantiate(buttonPrefab, buttonholder1, false);
                newButton.GetComponentInChildren<TMP_Text>().text = lettersToUse[i];
                LetterButton myLetter = newButton.GetComponent<LetterButton>();
                myLetter.SetButton(lettersToUse[i]);

                letterList.Add(myLetter);
            } else if (i < 20)
            {
                GameObject newButton = Instantiate(buttonPrefab, buttonholder2, false);
                newButton.GetComponentInChildren<TMP_Text>().text = lettersToUse[i];
                LetterButton myLetter = newButton.GetComponent<LetterButton>();
                myLetter.SetButton(lettersToUse[i]);

                letterList.Add(myLetter);
            } else
            {
                GameObject newButton = Instantiate(buttonPrefab, buttonholder3, false);
                newButton.GetComponentInChildren<TMP_Text>().text = lettersToUse[i];
                LetterButton myLetter = newButton.GetComponent<LetterButton>();
                myLetter.SetButton(lettersToUse[i]);

                letterList.Add(myLetter);
            }
        }
    }

    public void RemoveLetter(LetterButton theButton)
    {
        letterList.Remove(theButton);
    }

    //FROM THE HINT BUTTON
    public void UseHint()
    {
        if (GameManager.instance.GameOver() || GameManager.instance.maxHints <= 0)
        {
            Debug.Log("Você não tem mais dicas!");
            return;
        }

        GameManager.instance.maxHints--;
        int randomIndex = Random.Range(0, letterList.Count);
        letterList[randomIndex].Sendletter(true);
    }
}
