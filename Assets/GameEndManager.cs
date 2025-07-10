using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameEndManager : MonoBehaviourPunCallbacks
{
    public static GameEndManager Instance;


    [Header("게임 종료 처리")]
    public string returnSceneName = "Ingame Scene"; 
    public float delayBeforeExit = 5f;

    private int alivePlayers = 0;
    private bool isBossDead = false;
    private bool gameEnded = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        alivePlayers = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    // 플레이어 사망 시 외부에서 호출
    public void NotifyPlayerDied()
    {
        if (gameEnded) return;

        alivePlayers--;
        Debug.Log($"[게임] 플레이어 사망. 남은 인원: {alivePlayers}");

        if (!isBossDead && alivePlayers <= 0)
        {
            EndGame(false); // 게임 오버
        }
    }

    // 보스 사망 시 외부에서 호출
    public void NotifyBossDied()
    {
        if (gameEnded) return;

        isBossDead = true;
        Debug.Log("보스 사망! 게임 종료 조건 충족.");

        EndGame(true); // 클리어 성공
    }

    // 종료 처리 (모든 클라이언트 동기화)
    [PunRPC]
    private void EndGame(bool isClear)
    {
        if (gameEnded) return;

        gameEnded = true;

        Debug.Log(isClear ? " 승리! 클리어 성공!" : " 전멸. 게임 오버.");

        // 결과 UI 표시 등 추가 가능


        
        StartCoroutine(LeaveRoomAfterDelay());
    }

    // 외부에서 호출해 전체 종료
    public void TriggerGameEnd(bool isClear)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("EndGame", RpcTarget.All, isClear);
        }
    }

    private IEnumerator LeaveRoomAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeExit);

        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(returnSceneName);
    }
}