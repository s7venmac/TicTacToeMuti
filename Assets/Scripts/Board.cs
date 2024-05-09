using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Board : NetworkBehaviour
{
    Button[,] buttons = new Button[3,3];
    public override void OnNetworkSpawn()
    {
        var cells = GetComponentsInChildren<Button>();
        int n = 0;
        for(int i=0; i<3; i++){
            for(int j=0; j<3; j++){
                buttons[i,j] = cells[n];
                n++;

                int r=i;
                int c=j;

                buttons[i,j].onClick.AddListener(delegate {
                    OnclickCell(r,c);
                });
            }
        }
    }

    [SerializeField] private Sprite xSprite, oSprite;

    private void OnclickCell(int r, int c){
        if(NetworkManager.Singleton.IsHost && GameManager.Instance.Turn.Value == 0){
            buttons[r,c].GetComponent<Image>().sprite =  xSprite;
            buttons[r,c].interactable = false;
            ChangeSpriteClientRpc(r,c);
            checkResult(r,c);
            GameManager.Instance.Turn.Value = 1;
        }
        else if(!NetworkManager.Singleton.IsHost && GameManager.Instance.Turn.Value == 1){
            buttons[r,c].GetComponent<Image>().sprite = oSprite;
            buttons[r,c].interactable = false;
            checkResult(r,c);
            ChangeSpriteServerRpc(r,c);
            
        }
    }

    [ClientRpc]
    private void ChangeSpriteClientRpc(int r, int c){
        buttons[r,c].GetComponent<Image>().sprite = xSprite;
        buttons[r,c].interactable = false;
        GameManager.Instance.Turn.Value = 1;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeSpriteServerRpc(int r, int c){
        buttons[r,c].GetComponent<Image>().sprite = oSprite;
        buttons[r,c].interactable = false;
        GameManager.Instance.Turn.Value = 0;
    }


    private void checkResult(int r, int c){
        if(IsWon(r,c)){
            GameManager.Instance.ShowMsg("won");
        }
        else{
            if(IsGameDraw()){
                GameManager.Instance.ShowMsg("draw");
            }
        }
    }



    public bool IsWon(int r, int c){
        Sprite clickedButton = buttons[r,c].GetComponent<Image>().sprite;
        if( buttons[0,c].GetComponentInChildren<Image>().sprite == clickedButton &&
            buttons[1,c].GetComponentInChildren<Image>().sprite == clickedButton &&
            buttons[2,c].GetComponentInChildren<Image>().sprite == clickedButton){
                return true;
            }
        else if (buttons[r,0].GetComponentInChildren<Image>().sprite == clickedButton &&
                buttons[r,1].GetComponentInChildren<Image>().sprite == clickedButton &&
                buttons[r,2].GetComponentInChildren<Image>().sprite == clickedButton){
                return true;
                }
        else if(buttons[0,0].GetComponentInChildren<Image>().sprite == clickedButton &&
                buttons[1,1].GetComponentInChildren<Image>().sprite == clickedButton &&
                buttons[2,2].GetComponentInChildren<Image>().sprite == clickedButton){
                    return true;
                }
        else if (buttons[0,2].GetComponentInChildren<Image>().sprite == clickedButton &&
                buttons[1,1].GetComponentInChildren<Image>().sprite == clickedButton &&
                buttons[2,0].GetComponentInChildren<Image>().sprite == clickedButton){
                    return true;
                }
        return false;
    }
    private bool IsGameDraw(){
        for(int i=0 ; i<3; i++){
            for(int j=0; j<3; j++){
                if(buttons[i,j].GetComponent<Image>().sprite != xSprite &&
                    buttons[i,j].GetComponent<Image>().sprite != oSprite){
                        return false;
                    }
            }
        }
        return true;
    }
}
