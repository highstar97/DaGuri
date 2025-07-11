using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 

public class Shield : MonoBehaviourPun
{
    public float shieldDuration = 10f;
    public Vector3 shieldOffset = new Vector3(0, 1f, 1.5f);
    public Transform leftHandTargetTransform;

    private GameObject activeShield;

    public void ActivateShield()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        //플레이어 앞 위치 계산
        Vector3 sheildPos = transform.position + transform.TransformDirection(shieldOffset);
        Quaternion shieldRot = transform.rotation;

        activeShield = PhotonNetwork.Instantiate("Shield", sheildPos, shieldRot);
        activeShield.transform.SetParent(leftHandTargetTransform); //플레이어 왼손 움직일 때 같이

        StartCoroutine(RemoveShieldAfterTime(shieldDuration));
    }

    IEnumerator RemoveShieldAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        PhotonNetwork.Destroy(activeShield);
    }
}
