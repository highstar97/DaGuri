using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShieldController : MonoBehaviourPunCallbacks
{
    public LayerMask projectileLayer;


    // Start is called before the first frame update
    void Start()
    {
        Collider collider = GetComponent<Collider>();
        if (collider == null || !collider.isTrigger)
        {
            Debug.LogError("쉴드 프리팹에 Trigger Collider가 필요합니다!");
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null || !rb.isKinematic)
        {
            Debug.LogWarning("쉴드에는 Kinematic Rigidbody가 권장됩니다.");
        }
      
    }
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & projectileLayer) != 0)
        {
            PhotonView projView = other.GetComponent<PhotonView>();

            if (projView != null)
            {
                // PhotonView가 있는 발사체는 마스터 클라이언트에게 파괴 요청
                // 발사체의 소유권과 관계없이 마스터 클라이언트가 파괴하도록 요청
                photonView.RPC("RequestDestroyProjectile", RpcTarget.MasterClient, projView.ViewID);
                Debug.Log($"[ShieldController] Projectile with ViewID {projView.ViewID} hit my shield. Requesting destroy to Master Client.");
            }
            else // PhotonView가 없는 경우 (로컬에서 생성된 발사체이거나 테스트용)
            {
                Destroy(other.gameObject);
                Debug.Log("[ShieldController] Non-networked projectile hit my shield. Destroying locally.");
            }
        }

    }
    [PunRPC]
    private void RequestDestroyProjectile(int viewID)
    {
        // 이 RPC는 마스터 클라이언트에서만 실행되어야 합니다.
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            PhotonNetwork.Destroy(targetView.gameObject);
            Debug.Log($"[ShieldController] Master Client destroyed projectile with ViewID {viewID}.");
        }
        else
        {
            Debug.LogWarning($"[ShieldController] Master Client could not find Projectile with ViewID {viewID} to destroy.");
        }
    }
}
