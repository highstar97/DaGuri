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
        int actorIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        GameObject character = PhotonNetwork.Instantiate(prefabs[actorIndex], instantiateTransforms[actorIndex].position, instantiateTransforms[actorIndex].rotation);

        Debug.Log($"[Photon] {prefabs[actorIndex]} 프리팹을 생성했습니다.");

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
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log($"[Photon] 플레이어 퇴장: {otherPlayer.NickName}");
    }
    #endregion
}