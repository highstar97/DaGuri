using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class SkillShooter : MonoBehaviourPunCallbacks
{
    public GameObject bulletPrefab;
    public Transform firePoint; // 손끝 위치
    public LineRenderer lineRenderer;



    public float maxDistance = 5f;   // 조준선 길이
    public float bombSpeed = 20f;

    private bool isAiming = false;

    private void Awake()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false; // 시작 시 라인렌더러 비활성화
        }
    }
    private void Update()
    {
        if(!photonView.IsMine)
        {
            return; 
        }
        if(isAiming && lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, firePoint.position + firePoint.forward * maxDistance);
            Debug.Log("Line Renderer active: " + lineRenderer.enabled + ", isAiming: " + isAiming + " at " + Time.frameCount);
        }
        else if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    //버튼 누를때
    public void OnStartAiming()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        isAiming = true;
    }
    public void Fire()
    {
        isAiming = false;
        if (photonView.IsMine)
        {
            //마스터 클라이언트에게 폭탄 발사 요청하는 RPC 호출
           photonView.RPC("ShootBomb", RpcTarget.MasterClient, firePoint.position, firePoint.rotation, firePoint.forward * 20f);

         }
    }




    //public void Fire()
    //{
    //    if (photonView.IsMine)
    //    {
    //        //마스터 클라이언트에게 폭탄 발사 요청하는 RPC 호출
    //        photonView.RPC("ShootBomb", RpcTarget.MasterClient, firePoint.position, firePoint.rotation, firePoint.forward * 20f);

    //    }
    //}

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
        //if( bomb.GetComponent<PhotonView>() != null)
        //    {
        //        bomb.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient.ActorNumber);
        //    }
                PhotonView view = bomb.GetComponent<PhotonView>();
        if (view != null)
        {
            view.TransferOwnership(PhotonNetwork.MasterClient.ActorNumber);
        }

    }

}
