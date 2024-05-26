using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    List<string> solvedList = new List<string>();
    string[] unsolvedWord;

    [Header("Letters")]
    public GameObject letterPrefab;
    public Transform letterHolder;
    List<TMP_Text> letterHolderList = new List<TMP_Text>();

    [Header("Categories")]
    public Category[] categories;
    public TMP_Text categoryText;

    [Header("Timer")]
    public TMP_Text timerText;
    int playTime;

    [Header("Hints")]
    public int maxHints = 3;

    [Header("Mistakes")]
    [Space]
    public Animator[] petalList;

    [SerializeField]
    int maxMistakes;
    int currentMistakes;

    private bool gameOver;


    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        maxMistakes = petalList.Length;
        Initialize();
        StartCoroutine(Timer());
        StartCoroutine(TimerSync());


    }
    private void Initialize()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetInitialWord();
        }
    }
    public void InputFromButton(string requestedLetter, bool isThatAHint)
    {
        // CHECK IF THE GAME IS NOT GAME OVER YET

        // SEARCH MECHANIC FOR SOLVED LIST
        CheckLetter(requestedLetter, isThatAHint);
    }

    private void CheckLetter(string requestedLetter, bool isThatAHint)
    {
        if (gameOver)
        {
            return;
        }

        bool letterFound = false;

        // FIND THE LETTER IN THE SOLVED LIST
        for (int i = 0; i < solvedList.Count; i++)
        {
            if (solvedList[i] == requestedLetter)
            {
                letterHolderList[i].text = requestedLetter;
                unsolvedWord[i] = requestedLetter;
                letterFound = true;
            }
        }

        if (!letterFound && !isThatAHint)
        {
            // MISTAKE STUFF - GRAPHICAL REPRESENTATION
            petalList[currentMistakes].SetTrigger("miss");
            currentMistakes++;

            if (currentMistakes == maxMistakes)
            {
                // DO GAME OVER
                UIHandler.instance.LoseCondition(playTime);
                gameOver = true;
                return;
            }
        }

        // CHECK IF GAME WON
        Debug.Log("Vitoria? " + CheckIfWon());
        gameOver = CheckIfWon();
        if (gameOver)
        {
            // SHOW UI
            UIHandler.instance.WinCondition(playTime);
        }
    }

    private bool CheckIfWon()
    {
        // CHECK MECHANICS
        for (int i = 0; i < unsolvedWord.Length; i++)
        {
            if (unsolvedWord[i] != solvedList[i])
            {
                return false;
            }
        }

        return true;
    }

    public bool GameOver()
    {
        return gameOver;
    }

    private IEnumerator Timer()
    {
        int seconds = 0;
        int minutes = 0;
        timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");

        // Wait for 5 seconds before starting the timer
        yield return new WaitForSeconds(5);

        // Start the timer after 5 seconds
        while (!gameOver)
        {
            yield return new WaitForSeconds(1);
            playTime++;

            seconds = playTime % 60;
            minutes = playTime / 60 % 60;

            timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
        }
    }

    private IEnumerator TimerSync()
    {
        while (!gameOver)
        {
            // Check if it is the master client (host) before sending updates
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("SyncTime", RpcTarget.All, playTime);
            }

            yield return new WaitForSeconds(1f); // Update every second
        }
    }

    [PunRPC]
    private void SyncTime(int time)
    {
        // Update time received from all clients
        playTime = time;

        // Update the time counter on the user interface
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        int seconds = playTime % 60;
        int minutes = playTime / 60 % 60;
        timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }



    private void SetInitialWord()
    {
        int cIndex = Random.Range(0, categories.Length);
        string categoryName = categories[cIndex].name;
        int wIndex = Random.Range(0, categories[cIndex].wordList.Length);
        string pickedWord = categories[cIndex].wordList[wIndex];

        photonView.RPC("SyncInitialWord", RpcTarget.AllBuffered, categoryName, pickedWord);
    }

    [PunRPC]
    private void SyncInitialWord(string categoryName, string pickedWord)
    {
        categoryText.text = categoryName;
        string[] splittedWord = pickedWord.Select(l => l.ToString()).ToArray();
        unsolvedWord = new string[splittedWord.Length];
        solvedList = new List<string>(splittedWord);

        // CREATE THE VISUAL
        for (int i = 0; i < solvedList.Count; i++)
        {
            GameObject tempLetter = Instantiate(letterPrefab, letterHolder, false);
            letterHolderList.Add(tempLetter.GetComponent<TMP_Text>());
        }
    }

}