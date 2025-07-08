using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SwordHit : MonoBehaviourPun 
{
    private SwordAttack swordAttack;
    private SwordInput swordInput;

    private PhotonView photonView;

    void Start()
    {
        swordAttack = GetComponentInParent<SwordAttack>();
        swordInput = GetComponentInParent<SwordInput>();

        photonView = GetComponentInParent<PhotonView>();

        if (swordAttack == null) Debug.LogError("❌ SwordAttack 못 찾음");
        if (swordInput == null) Debug.LogWarning("⚠️ SwordInput 못 찾음 (사용 안하면 무시 가능)");
       
    }

    void OnTriggerEnter(Collider other)
    {

        Debug.Log("Something entered trigger: " + other.name);
        if (other.CompareTag("Target"))
        {
            if (swordAttack != null && swordInput != null)
            {
                if (swordAttack.IsSwinging && swordInput.isPressingAttack)
                {
                    Debug.Log("💥 Attack Success!");

                    photonView.RPC("ShowHitEffect", RpcTarget.All, other.transform.position);
                }
            }
        }
    }

    [PunRPC]
    void ShowHitEffect(Vector3 hitPosition)
    {
        Debug.Log("💥 [RPC] Attack hit at: " + hitPosition);

        //  이펙트 보여주기
        // Instantiate(hitEffectPrefab, hitPosition, Quaternion.identity);

        // 또는 타겟의 HP 깎기 처리
    }
}
