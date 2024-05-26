using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class callScene : MonoBehaviour
{
    // Start is called before the first frame update
    public void carregaCena(string nome)

    {
        SceneManager.LoadScene(nome);
    }

    public void MultiplayerGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
