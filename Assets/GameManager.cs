using Photon.Pun;
using Photon.Realtime; // Player 클래스용
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Variables
    public List<string> prefabs;
    public List<Transform> instantiateTransforms;
    public DemonicAltar_Controller demonicAlterController;

    public Text currentTimeUI;      // 남은시간 표기

    private float currentTime;      // 현재 시간
    #endregion

    #region Unity Functions
    private void Awake()
    {
        if (!(PhotonNetwork.PrefabPool is ProjectilePool))
        {
            PhotonNetwork.PrefabPool = new ProjectilePool();
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentTime);
        }
        else
        {
            currentTime = (float)stream.ReceiveNext();
        }
    }
    #endregion
    
    #region User Functions
    IEnumerator CoStartTime() {
        while (true)
        {
            yield return new WaitForSeconds(1f);    // 1초 시간 흐름
            currentTime += 1f;

            UpdateTimer();                          // 타이머 UI 업데이트
        }
    }

    public void UpdateTimer()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        currentTimeUI.text = $"{minutes:D2} : {seconds:D2}";
    }

    public override void OnJoinedRoom()
    {
        int index = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        Debug.Log($"[Photon] 플레이어 {PhotonNetwork.LocalPlayer.NickName} (Actor {index}) 방에 입장");

        GameObject character = PhotonNetwork.Instantiate(prefabs[index], instantiateTransforms[index].position, instantiateTransforms[index].rotation);

        Debug.Log($"[Photon] {prefabs[index]} 프리팹을 생성했습니다.");

        bool isMine = character.GetComponent<PhotonView>().IsMine;

        character.GetComponentInChildren<AudioListener>().enabled = isMine;
        character.GetComponentInChildren<Camera>().enabled = isMine;
        
        if(character.GetComponentInChildren<CharacterController>() != null)
        {
            character.GetComponentInChildren<CharacterController>().enabled = isMine;
        }

        demonicAlterController.ToggleDemonicAltar();

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(CoStartTime());
        }
    }
  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log($"[Photon] 플레이어 입장: {newPlayer.NickName} (ActorNumber: {newPlayer.ActorNumber})");

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2) //테스트 플레이어 2명
        {
            Debug.Log("[Photon] 모든 플레이어 입장 완료. 게임 로직 시작 가능.");
            photonView.RPC("StartGame", RpcTarget.All);
        }
    }
  

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log($"[Photon] 플레이어 퇴장: {otherPlayer.NickName}");
    }

    [PunRPC]
    void StartGame()
    {
        Debug.Log("[게임 시작] 모든 플레이어 준비 완료, 게임을 시작합니다.");
        demonicAlterController.ToggleDemonicAltar(); // 예시: 보스 등장
        GameEndManager.Instance.InitializeAlivePlayers();
    }
    #endregion
}