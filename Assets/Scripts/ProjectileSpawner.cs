using UnityEngine;
using Photon.Pun;

public class ProjectileSpawner : MonoBehaviourPun
{
    #region User Functions
    public GameObject SpawnProjectile(string projectilePrefabName, Vector3 position, Vector3 direction, GameObject owner)
    {
        if (!photonView.IsMine) return null; // 소유자만 발사 가능

        GameObject gameObject = PhotonNetwork.Instantiate(projectilePrefabName, position, Quaternion.LookRotation(direction));
        BasicProjectile projectile = gameObject.GetComponent<BasicProjectile>();
        projectile.ownerStat = owner.GetComponent<StatComponent>();
        return gameObject;
    }
    #endregion
}