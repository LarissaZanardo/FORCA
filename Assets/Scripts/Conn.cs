using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Unity.Mathematics;

public class Conn : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject painelL, painelS;
    [SerializeField]
    private InputField nomeJogador, nomeSala;
    [SerializeField]
    private Text txtNick;
    [SerializeField]
    private GameObject jogador;
    [SerializeField]
    private Text debugText; // Adicione este campo para o componente de texto de depuração

    void Start()
    {
        // Opcionalmente, você pode inicializar o texto de depuração
       // debugText.text = "";
    }

    public void Login()
    {
        PhotonNetwork.NickName = nomeJogador.text;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        painelL.SetActive(false);
        painelS.SetActive(true);
    }

    public void CriarSala()
    {
        PhotonNetwork.JoinOrCreateRoom(nomeSala.text, new RoomOptions(), TypedLobby.Default);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado");
        AppendDebugText("Conectado");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Conectado ao Lobby");
        AppendDebugText("Conectado ao Lobby");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Conexão perdida");
        AppendDebugText("Conexão perdida: " + cause.ToString());
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Não entrou em nenhuma sala");
        AppendDebugText("Não entrou em nenhuma sala: " + message);
    }

    private bool aguardandoSegundoJogador = false;

    public override void OnJoinedRoom()
    {
        Debug.Log("Entrou em uma sala");
        AppendDebugText("Entrou em uma sala");
        Debug.Log("Nome da sala: " + PhotonNetwork.CurrentRoom.Name);
        AppendDebugText("Nome da sala: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Quantidade de jogadores na sala: " + PhotonNetwork.CurrentRoom.PlayerCount);
        AppendDebugText("Quantidade de jogadores na sala: " + PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.Log("Apelido do jogador: " + PhotonNetwork.NickName);
        AppendDebugText("Apelido do jogador: " + PhotonNetwork.NickName);

        txtNick.text = PhotonNetwork.NickName;

        painelS.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 posicaoPrimeiroJogador = new Vector3(-184.0f, -16.0f, 0.0f);
            PhotonNetwork.Instantiate("Player", posicaoPrimeiroJogador, Quaternion.identity, 0);
        }
        else
        {
            Vector3 posicaoSegundoJogador = new Vector3(-184.0f, -120.0f, 0.0f);
            PhotonNetwork.Instantiate("Player", posicaoSegundoJogador, Quaternion.identity, 0);
        }
        StartCoroutine(IniciarJogo());
    }

    private IEnumerator IniciarJogo()
    {
        while (PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            Debug.Log("Aguardando Segundo Jogador");
            AppendDebugText("Aguardando Segundo Jogador");
            yield return null;
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            yield return new WaitForSeconds(5f);
            Debug.Log("Iniciando o jogo e carregando a cena 'Game'...");
            AppendDebugText("Iniciando o jogo e carregando a cena 'Game'...");
            PhotonNetwork.LoadLevel("Game");
        }
    }

    private void AppendDebugText(string message)
    {
        debugText.text += message + "\n";
    }
}
