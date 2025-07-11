using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks
{
    #region Unity Functions
    private void Start()
    {
        PhotonNetwork.NickName = "Player" + Random.Range(1000, 9999);
        PhotonNetwork.ConnectUsingSettings();
    }
    #endregion

    #region Photon Functions
    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버 연결됨, 방 입장 시도");
        TryJoinOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"방 입장 성공 : {PhotonNetwork.CurrentRoom.Name}");
    }
    #endregion

    #region User Functions
    private void TryJoinOrCreateRoom()
    {
        var roomName = "MyRoom";
        var options = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);

        Debug.Log($"방 입장 또는 생성 시도 : {roomName}");
    }
    #endregion
}
