using Photon.Pun;
using Photon.Realtime; // Player 클래스용
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Variables
    public List<string> prefabs;
    public List<Transform> instantiateTransforms;
    public DemonicAltar_Controller demonicAlterController;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        if (!(PhotonNetwork.PrefabPool is ProjectilePool))
        {
            PhotonNetwork.PrefabPool = new ProjectilePool();
        }
    }
    #endregion

    #region User Functions
    public override void OnJoinedRoom()
    {
        int actorIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        Debug.Log($"[Photon] 플레이어 {PhotonNetwork.LocalPlayer.NickName} (Actor {actorIndex}) 방에 입장");

        GameObject character = PhotonNetwork.Instantiate(prefabs[actorIndex], instantiateTransforms[actorIndex].position, instantiateTransforms[actorIndex].rotation);

        Debug.Log($"[Photon] {prefabs[actorIndex]} 프리팹을 생성했습니다.");

        bool isMine = character.GetComponent<PhotonView>().IsMine;

        character.GetComponentInChildren<AudioListener>().enabled = isMine;
        character.GetComponentInChildren<Camera>().enabled = isMine;
        
        if(character.GetComponentInChildren<CharacterController>() != null)
        {
            character.GetComponentInChildren<CharacterController>().enabled = isMine;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            demonicAlterController.ToggleDemonicAltar();
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log($"[Photon] 플레이어 입장: {newPlayer.NickName} (ActorNumber: {newPlayer.ActorNumber})");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log($"[Photon] 플레이어 퇴장: {otherPlayer.NickName}");
    }
    #endregion
}
