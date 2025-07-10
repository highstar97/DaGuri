using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameStartManager : MonoBehaviourPun
{
    private int playerReady = 0;
    private bool gameStarted = false;

    public static GameStartManager instance;
    private void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    [PunRPC]
    void StartGame()
    {
        Debug.Log("[게임] StartGame() 호출됨");
        // 게임 시작 연출, 보스 소환 등
    }
}
