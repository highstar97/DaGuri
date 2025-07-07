using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables

    public List<string> prefabs;

    public List<Transform> instantiateTransforms;

    public DemonicAltar_Controller demonicAlterController;
    #endregion

    #region Unity Functions
    #endregion

    #region User Functions
    public void OnJoinedRoom()
    {
        int numOfCurrentJoinedPlayer = PhotonNetwork.CurrentRoom.PlayerCount -1;
        PhotonNetwork.Instantiate(prefabs[numOfCurrentJoinedPlayer], instantiateTransforms[numOfCurrentJoinedPlayer].position, instantiateTransforms[numOfCurrentJoinedPlayer].rotation);

        demonicAlterController.ToggleDemonicAltar();
    }
    #endregion
}