using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    #region Variables
    public float moveSpeed = 3f;
    public float lifeTime = 5f;
    public JobParticle jobParticleType;
    public AudioClip audioClip;
    public StatComponent ownerStat;

    public Action<BasicProjectile> OnProjectileTriggeredWithBoss;
    #endregion

    #region Unity Functions

    private void OnEnable()
    {
        Invoke(nameof(Release), lifeTime);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }


    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.photonView.RPC("AttachParticleAndSound", RpcTarget.All);
    }
    #endregion

    #region User Functions
    private void Release()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject); // 커스텀 풀로 반환됨
        }
    }

    [PunRPC]
    private void AttachParticleAndSound()
    {
        ParticleSystem particleSystem = ParticleManager.instance.GetParticleSystem(jobParticleType);
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(audioClip);

        if (particleSystem == null || GetComponentInChildren<ParticleSystem>() != null)
        {
            return;
        }

        // 파티클 생성해서 projectile에 붙이기
        ParticleSystem ps = Instantiate(particleSystem, this.transform);
        ps.transform.localPosition = Vector3.zero;
        ps.transform.rotation = this.transform.rotation;
        ps.Play();
    }
    #endregion
}
