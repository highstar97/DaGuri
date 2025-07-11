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
        if(!photonView.IsMine)
        {
            return;
        }
        if(((1 << other.gameObject.layer) & projectileLayer ) != 0)
        {
            PhotonView projView = other.GetComponent<PhotonView>();
        
                if (projView != null && projView.IsMine)
                {
                    PhotonNetwork.Destroy(other.gameObject);
                }
                else if (projView == null) // PhotonView 없는 경우 (테스트용)
                {
                    Destroy(other.gameObject);
                }
           
        }
    }


}
