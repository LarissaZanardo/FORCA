using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Category", menuName = "Hangman/Category")]

public class Category : ScriptableObject
{
    public new string name;
    public string[] wordList;

    void OnValidate()
    {
        for (int i = 0; i < wordList.Length; i++) 
        {
            wordList[i] = ConvertTheWord(wordList[i]);
        }
    }

    string ConvertTheWord(string _word)
    {
        string word = _word;
        if(word == "")
        {
            return string.Empty;
        }

        char[] w = word.ToCharArray();
        string newWord = "";
        foreach (char letter in w) 
        {
            newWord += char.ToUpper(letter).ToString();
        }

        return newWord;
    }
}
