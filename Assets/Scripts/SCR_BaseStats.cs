using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Stat", menuName = "Hangman/Save")]

public class SCR_BaseStats : ScriptableObject
{
    [SerializeField] int totalWins;
    [SerializeField] int totalLosses;
    [SerializeField] int totalGamesPlayed;
    [SerializeField] int fastestTime = 9999;  //IN SECONDS
    [SerializeField] float winRatio;
    

    void OnEnable()
    {
        if (PlayerPrefs.HasKey("HangManStats"))
        {
            string savedata = PlayerPrefs.GetString("HangManStats");
            string[] splitStats = savedata.Split('|');
            totalWins = int.Parse(splitStats[0]);
            totalLosses = int.Parse(splitStats[1]);
            winRatio = float.Parse(splitStats[2]);
            totalGamesPlayed = int.Parse(splitStats[3]);
            fastestTime = int.Parse(splitStats[4]);
        }
    }


    public void SaveStats(bool hasWonGame, int playtime)
    {
        //AssetDatabase.Refresh();
        
        totalWins += (hasWonGame) ? 1 : 0;
        totalLosses += (!hasWonGame) ? 1 : 0;
        totalGamesPlayed = totalWins + totalLosses;


        winRatio = ((float)totalWins / (float)totalGamesPlayed) * 100;
        if (hasWonGame) 
        {
            fastestTime = (playtime >= fastestTime) ? fastestTime : playtime;
        }
        

        //EditorUtility.SetDirty(this);
        //AssetDatabase.SaveAssets();

        string saveData = "";
        saveData += totalWins.ToString() + "|";
        saveData += totalLosses.ToString() + "|";
        saveData += winRatio.ToString() + "|";
        saveData += totalGamesPlayed.ToString() + "|";
        saveData += fastestTime.ToString();

        PlayerPrefs.SetString("HangManStats", saveData);
    }


    public List<int> GetStats()
    {
        //AssetDatabase.Refresh();

        List<int> statsList = new List<int>();
        statsList.Add(totalWins);
        statsList.Add(totalLosses);
        statsList.Add(Mathf.RoundToInt(winRatio));
        statsList.Add(totalGamesPlayed);
        statsList.Add(fastestTime);

        return statsList;
    }


    public void DeleteSave()
    {
        if (PlayerPrefs.HasKey("HabgManStats"))
        {
            PlayerPrefs.DeleteKey("HangManStats");
        }
    }

}
