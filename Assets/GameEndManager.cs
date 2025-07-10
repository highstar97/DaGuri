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
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 현재 오브젝트 파괴
        }

    }

    private void Start()
    {
        alivePlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log($"[GameEndManager] 초기 플레이어 수: {alivePlayers}");
    }

    // 플레이어 사망 시 외부에서 호출
    public void NotifyPlayerDied()
    {
        if (gameEnded) return;

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RpcPlayerDied", RpcTarget.All);
        }
    }


    // 보스 사망 시 외부에서 호출
    //보스 사망은 마스터 클라이언트에서만 처리하고 RPC로 동기화
    public void NotifyBossDied()
    {
        if (gameEnded) return;

        if (PhotonNetwork.IsMasterClient)
        {
            isBossDead = true;
            Debug.Log("[GameEndManager] 보스 사망");
            photonView.RPC("EndGame", RpcTarget.All ,true);
        }
    }

    // 플레이어 사망을 모든 클라이언트에 동기화하는 RPC
    [PunRPC]
    private void RpcPlayerDied()
    {
        if (gameEnded) return;

        alivePlayers--;
        Debug.Log($"[GameEndManager] 플레이어 사망. 남은 인원: {alivePlayers}");

        if (PhotonNetwork.IsMasterClient)
        {
            if (!isBossDead && alivePlayers <= 0)
            {
                //모든 플레이어 사망 , 보스가 죽지않았다면 패배
                photonView.RPC("EndGame", RpcTarget.All , false); 
            }
        }
    }
  //  게임 종료를 모든 클라이언트에 동기화하는 RPC
    [PunRPC]
    private void EndGame(bool isClear)
    {
        if (gameEnded) return;

        gameEnded = true;

        Debug.Log(isClear ? " 승리! 클리어 성공!" : " 전멸. 게임 오버.");

        if (GameEndUIController.Instance != null)
        {
            GameEndUIController.Instance.ShowResult(isClear);
        }
        else
        {
            Debug.LogError("[GameEndManager] GameEndUIController.Instance가 씬에 없습니다. UI를 표시할 수 없습니다.");
        }

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
        Instance = null;
        SceneManager.LoadScene(returnSceneName);
    }
}