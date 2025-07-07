using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public Text[] playerNameTexts;
    public Text[] playerReadyTexts;

    private const int MaxPlayers = 4;
    private const string GameVersion = "0.1";
    private const string RoomName = "Boss vs Player!";
    private const string ReadyKey = "IsReady";
    private const string IngameSceneName = "Ingame";

    private void Start()
    {
        PhotonNetwork.GameVersion = GameVersion;
        PhotonNetwork.NickName = "Player" + Random.Range(100, 999);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버에 연결됨");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        RoomOptions options = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = MaxPlayers
        };
        PhotonNetwork.JoinOrCreateRoom(RoomName, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방에 입장 완료");
        // Vector2 originPosition = Random.insideUnitCircle * 2f;
        // PhotonNetwork.Instantiate("Player", new Vector3(originPosition.x, 0, originPosition.y), Quaternion.identity);

        // 초기 Ready 상태 false로 설정
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { ReadyKey, false }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        UpdatePlayerUI();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerUI();
    }

    /// <summary>
    /// 로컬 플레이어 Ready 상태 변경 함수
    /// 버튼 등에 연결 가능
    /// </summary>
    public void ToggleReady()
    {
        bool isReady = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(ReadyKey) &&
                       (bool)PhotonNetwork.LocalPlayer.CustomProperties[ReadyKey];

        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { ReadyKey, !isReady }
        });
    }

    /// <summary>
    /// 다른 플레이어의 커스텀 프로퍼티가 변경될 때 호출
    /// </summary>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey(ReadyKey))
        {
            Debug.Log($"플레이어 {targetPlayer.NickName} Ready 상태: {changedProps[ReadyKey]}");

            // 마스터 클라이언트만 확인
            UpdatePlayerUI();
            if (PhotonNetwork.IsMasterClient)
            {
                CheckAllPlayersReady();
            }
        }
    }
    private void CheckAllPlayersReady()
    {
        if (PhotonNetwork.PlayerList.Length != MaxPlayers) return;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.ContainsKey(ReadyKey) || !(bool)player.CustomProperties[ReadyKey])
            {
                Debug.Log($"플레이어 {player.NickName} 아직 준비 안됨");
                return;
            }
        }

        Debug.Log("모든 플레이어 Ready 상태 - 게임 시작!");
        PhotonNetwork.LoadLevel(1);
    }

    private void UpdatePlayerUI()
    {
        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            if (i < players.Length)
            {
                Player player = players[i];
                bool isLocal = player == PhotonNetwork.LocalPlayer;

                // 닉네임 텍스트 세팅
                playerNameTexts[i].text = player.NickName;

                // 색상 지정 (로컬이면 빨간색, 아니면 흰색)
                playerNameTexts[i].color = isLocal ? Color.red : Color.white;

                // Ready 상태 텍스트
                bool isReady = player.CustomProperties.ContainsKey(ReadyKey) && (bool)player.CustomProperties[ReadyKey];

                playerReadyTexts[i].text = isReady ? "Ready" : "Not Ready";
            }
            else
            {
                playerNameTexts[i].text = "---";
                playerReadyTexts[i].text = "";
                playerNameTexts[i].color = Color.white;
            }
        }
    }

}