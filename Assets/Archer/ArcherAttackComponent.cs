using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArcherAttackComponent : MonoBehaviourPun
{
    public Transform ArrowOffset; //화살 발사 지점
    public ProjectileSpawner arrowSpawner; //화살 스포너
    public ControllerTracking controllerTracking; //컨트롤러 트래킹
 
    private void Start()
    {
        if (controllerTracking != null)
        {
            controllerTracking.OnTrackingFinished += CheckGesture;
        }
        else
        {
            this.enabled = false;
        }
    }

    private void CheckGesture()
    {
        var trail = controllerTracking.Positions;

        if (GestureUtils.IsLineGesture(trail))
        {
            GameObject projectile = arrowSpawner.SpawnProjectile("Arrow", ArrowOffset.position, this.transform.forward, this.gameObject);

            // 발사체의 PhotonView ID 추출
            int projectileViewID = projectile.GetComponent<PhotonView>().ViewID;

            // 모든 클라이언트에 이펙트 생성하라고 RPC 호출
            photonView.RPC("AttachArrowParticle", RpcTarget.All, projectileViewID);
        }
    }

    [PunRPC]
    void AttachArrowParticle(int viewID)
    {
        PhotonView pv = PhotonView.Find(viewID);
        if (pv == null) return;

        GameObject projectile = pv.gameObject;

        // 1. 파티클 프리팹 획득
        ParticleSystem particlePrefab = ParticleManager.instance.GetParticleSystem(JobParticle.ArcherBasicAttack);
        // 파티클 생성해서 projectile에 붙이기
        ParticleSystem ps = Instantiate(particlePrefab, projectile.transform);
        ps.transform.localPosition = Vector3.zero;
        ps.transform.rotation = projectile.transform.rotation;
        ps.Play();
    }

}