using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables
    string prefabName = "Mage - XR Interaction Setup";
    #endregion

    #region Unity Functions
    #endregion

    #region User Functions
    public void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity);
    }
    #endregion
}