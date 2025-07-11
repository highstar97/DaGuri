using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SkillShooter : MonoBehaviourPunCallbacks
{
    public GameObject bulletPrefab;
    public Transform firePoint; // 손끝 위치

    public void Fire()
    {
        if (photonView.IsMine)
        {
            //마스터 클라이언트에게 폭탄 발사 요청하는 RPC 호출
            photonView.RPC("ShootBomb", RpcTarget.MasterClient, firePoint.position, firePoint.rotation, firePoint.forward * 20f);

        }
    }

    [PunRPC]
    private void ShootBomb(Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        if (!PhotonNetwork.IsMasterClient)
            {
                return; // 마스터 클라이언트가 아닌 경우 실행하지 않음
            }

        GameObject bomb = PhotonNetwork.Instantiate(bulletPrefab.name, position, rotation, 0);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
       
        if (rb != null)
            {
                rb.velocity = velocity; // 폭탄에 속도 적용
            }

        //폭탄의 소유권을 마스터 클라이언트에게 명시적으로 부여
        if( bomb.GetComponent<PhotonView>() != null)
            {
                bomb.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient.ActorNumber);
            }

    }

}
