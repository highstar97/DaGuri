using UnityEngine;
using Photon.Pun;
using System;

public class BasicProjectile : MonoBehaviourPun
{
    #region Variables
    public float moveSpeed = 3f;
    public float lifeTime = 5f;
    public Transform targetTransform;
    public StatComponent ownerStat;

    public Action<BasicProjectile> OnProjectileTriggeredWithBoss;
    #endregion

    #region Unity Functions
    private void Start()
    {
        targetTransform = GameObject.Find("Boss").transform;
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

        victim.TakeDamage(ownerStat.attack.BaseValue);

        OnProjectileTriggeredWithBoss?.Invoke(this);

        Release();
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
    #endregion
}
