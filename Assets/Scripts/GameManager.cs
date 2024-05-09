using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public NetworkVariable<int> Turn = new NetworkVariable<int>(0);

    private void Awake(){
        if(Instance != null && Instance != this){
            Destroy(gameObject);
        }
        else{
            Instance = this;
        }
    }
    private void Start(){
        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>{
            Debug.Log(clientId + " joined");
            if(NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClients.Count == 2){
                Debug.Log("Tic tac toe");
                SpwanBoard();
            }
        };


    }
    [SerializeField] private GameObject board;
    private GameObject newBoard;
    private void SpwanBoard(){
        newBoard = Instantiate(board);
        newBoard.GetComponent<NetworkObject>().Spawn();
    }

    public void StartHost(){
        NetworkManager.Singleton.StartHost();
        
    }
    public void StartClient(){
        NetworkManager.Singleton.StartClient();
    }

    [SerializeField] private GameObject gameEndPanel;
    [SerializeField] private Text msgText;

    public void ShowMsg(string msg){
        if(msg.Equals("won")){
            msgText.text = "You Won";
            gameEndPanel.SetActive(true);
            ShowOpponentMsg("You Lose");
        }
        else if (msg.Equals("draw")){
            msgText.text = "Game Draw";
            gameEndPanel.SetActive(true);
            ShowOpponentMsg("Game Draw");
        }
    }


    private void ShowOpponentMsg(string msg){
        if(IsHost){
            OpponentMsgClientRpc(msg);
        }
        else {
            OpponentMsgServerRpc(msg);
        }
    }

    [ClientRpc]
    private void OpponentMsgClientRpc(string msg){
        if(IsHost) return;
        msgText.text = msg;
        gameEndPanel.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OpponentMsgServerRpc(string msg){
        msgText.text = msg;
        gameEndPanel.SetActive(true);
    }


    public void Restart(){
        if(!IsHost){
            RestartServerRpc();
            gameEndPanel.SetActive(false);
        }
        else {
            Destroy(newBoard);
            SpwanBoard();
            RestartClientRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RestartServerRpc(){
        Destroy(newBoard);
        SpwanBoard();
        gameEndPanel.SetActive(false);
    } 

    [ClientRpc]
    private void RestartClientRpc(){
        gameEndPanel.SetActive(false);
    }



}
