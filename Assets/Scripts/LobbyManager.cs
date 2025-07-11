using ExitGames.Client.Photon;
using NUnit.Framework;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct PlayerState
{
    public bool isReady;
    public int index;
    public string nickName;

    public ExitGames.Client.Photon.Hashtable ToHashtable()
    {
        return new ExitGames.Client.Photon.Hashtable {
            { "IsReady", isReady },
            { "Index", index },
            { "NickName", nickName }
        };
    }

    public static PlayerState FromHashtable(ExitGames.Client.Photon.Hashtable table)
    {
        return new PlayerState
        {
            isReady = table.ContainsKey("IsReady") && (bool)table["IsReady"],
            index = table.ContainsKey("Index") ? (int)table["Index"] : -1,
            nickName = table.ContainsKey("NickName") ? (string)table["NickName"] : "Unknown"
        };
    }
}
public class LobbyManager : MonoBehaviourPunCallbacks
{
    #region Variables
    public Text[] playerNameTexts;
    public Text[] playerReadyTexts;

    private const int MaxPlayers = 4;
    private const string GameVersion = "0.1";
    private const string RoomName = "Boss vs Player!";
    private const byte SwapEventCode = 10;
    #endregion

    #region Unity Functions
    private void Start()
    {
        PhotonNetwork.GameVersion = GameVersion;
        PhotonNetwork.NickName = "Player" + Random.Range(100, 999);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
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

        PlayerState myState = new PlayerState
        {
            isReady = false,
            index = PhotonNetwork.LocalPlayer.ActorNumber,
            nickName = PhotonNetwork.NickName
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(myState.ToHashtable());
        
        UpdatePlayerUI();
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("IsReady"))
        {
            UpdatePlayerUI();
            if (PhotonNetwork.IsMasterClient)
            {
                CheckAllPlayersReady();
            }
        }
        else if (changedProps.ContainsKey("NickName"))
        {
            UpdatePlayerUI();
        }
        else if (changedProps.ContainsKey("Index"))
        {

        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        UpdatePlayerUI();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        UpdatePlayerUI();
    }
    #endregion

    #region Event Handler
    private void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == SwapEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int aId = (int)data[0];
            int bId = (int)data[1];

            Photon.Realtime.Player a = PhotonNetwork.CurrentRoom.GetPlayer(aId);
            Photon.Realtime.Player b = PhotonNetwork.CurrentRoom.GetPlayer(bId);
            if (a == null || b == null) return;

            PlayerState stateA = PlayerState.FromHashtable(a.CustomProperties);
            PlayerState stateB = PlayerState.FromHashtable(b.CustomProperties);

            if (stateA.isReady || stateB.isReady) return;

            // index만 스왑
            (stateA.index, stateB.index) = (stateB.index, stateA.index);

            a.SetCustomProperties(stateA.ToHashtable());
            b.SetCustomProperties(stateB.ToHashtable());
        }
    }
    #endregion

    #region User Functions
    public void ToggleReady()
    {
        PlayerState state = PlayerState.FromHashtable(PhotonNetwork.LocalPlayer.CustomProperties);
        state.isReady = !state.isReady;

        PhotonNetwork.LocalPlayer.SetCustomProperties(state.ToHashtable());
    }
    public void ToggleRequestSwap(int targetIndex)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            PlayerState state = PlayerState.FromHashtable(player.CustomProperties);
            if (state.index == targetIndex)
            {
                ToggleRequestSwap(PhotonNetwork.LocalPlayer, player);
                break;
            }
        }
    }
    public void ToggleRequestSwap(Photon.Realtime.Player applicant, Photon.Realtime.Player respondent)
    {
        if (applicant == null || respondent == null) return;

        PlayerState aState = PlayerState.FromHashtable(applicant.CustomProperties);
        PlayerState bState = PlayerState.FromHashtable(respondent.CustomProperties);

        if (aState.isReady || bState.isReady) return;

        object[] data = new object[] { applicant.ActorNumber, respondent.ActorNumber };

        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(SwapEventCode, data, options, SendOptions.SendReliable);
    }

    private void CheckAllPlayersReady()
    {
        if (PhotonNetwork.PlayerList.Length != MaxPlayers) return;

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            PlayerState state = PlayerState.FromHashtable(player.CustomProperties);
            if (!state.isReady)
            {
                Debug.Log($"{state.nickName} 아직 준비 안됨");
                return;
            }
        }

        Debug.Log("모든 플레이어 Ready 상태 - 게임 시작!");
        PhotonNetwork.LoadLevel("Ingame");
    }

    private void UpdatePlayerUI()
    {
        // index 기준으로 정렬할 딕셔너리
        Dictionary<int, Photon.Realtime.Player> sortedByIndex = new();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            PlayerState state = PlayerState.FromHashtable(player.CustomProperties);
            if (!sortedByIndex.ContainsKey(state.index))
            {
                sortedByIndex.Add(state.index, player);
            }
        }

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            if (sortedByIndex.TryGetValue(i, out Photon.Realtime.Player player))
            {
                PlayerState state = PlayerState.FromHashtable(player.CustomProperties);

                playerNameTexts[i].text = state.nickName;
                playerReadyTexts[i].text = state.isReady ? "Ready" : "Not Ready";
                playerNameTexts[i].color = player == PhotonNetwork.LocalPlayer ? Color.red : Color.white;
            }
            else
            {
                playerNameTexts[i].text = "---";
                playerReadyTexts[i].text = "";
                playerNameTexts[i].color = Color.white;
            }
        }
    }
    #endregion
}