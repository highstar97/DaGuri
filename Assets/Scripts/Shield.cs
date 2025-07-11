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
        //플레이어 앞 위치 계산
        Vector3 sheildPos = transform.position + transform.TransformDirection(shieldOffset);
        Quaternion shieldRot = transform.rotation;

        if (photonView.IsMine)
        {
            activeShield = PhotonNetwork.Instantiate("Shield", sheildPos, shieldRot);
            StartCoroutine(RemoveShieldAfterTime(shieldDuration));
            return;
        }

        activeShield.transform.SetParent(leftHandTargetTransform); //플레이어 왼손 움직일 때 같이
    }

    IEnumerator RemoveShieldAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        PhotonNetwork.Destroy(activeShield);
    }
}
