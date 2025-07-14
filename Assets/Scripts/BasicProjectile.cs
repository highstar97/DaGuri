using UnityEngine;
using Photon.Pun;
using System;

public class BasicProjectile : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    #region Variables
    public float moveSpeed = 3f;
    public float lifeTime = 5f;
    public JobParticle jobParticleType;
    public Transform targetTransform;
    public StatComponent ownerStat;

    public Action<BasicProjectile> OnProjectileTriggeredWithBoss;
    #endregion

    #region Unity Functions
    private void Start()
    {
        targetTransform = GameObject.Find("Boss Player(Clone)").transform;
    }

    private void OnEnable()
    {
        Invoke(nameof(Release), lifeTime);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Update()
    {
        if (targetTransform == null) return;

        Vector3 direction = (targetTransform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return; // 소유자만 처리
        if (!other.CompareTag("Boss")) return;

        ITakeDamageable victim = other.GetComponent<ITakeDamageable>();
        if (victim == null) return;

        victim.BroadcastTakeDamage(ownerStat.attack.BaseValue, ownerStat.gameObject);

        OnProjectileTriggeredWithBoss?.Invoke(this);

        Release();
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.photonView.RPC("AttachParticle", RpcTarget.All);
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
    private void AttachParticle() 
    {
        ParticleSystem particleSystem = ParticleManager.instance.GetParticleSystem(jobParticleType);
        
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
