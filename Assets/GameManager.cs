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

    private static int numOfCurrentJoinedPlayer = 0;
    #endregion

    #region Unity Functions
    #endregion

    #region User Functions
    public void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate(prefabs[numOfCurrentJoinedPlayer], instantiateTransforms[numOfCurrentJoinedPlayer].position, instantiateTransforms[numOfCurrentJoinedPlayer].rotation);
        ++numOfCurrentJoinedPlayer;

        demonicAlterController.ToggleDemonicAltar();
    }
    #endregion
}