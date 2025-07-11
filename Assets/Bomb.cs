using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Bomb : MonoBehaviourPunCallbacks
{
    public float lifeTime = 5f; // 폭탄 수명
    public int damageAmount = 10;


    private void Start()
    {
        //마스터 클라이언트에서만 폭탄수명 관리 파괴
        
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(DestroyBombAfterTime(lifeTime));
        }
    }

    private IEnumerator DestroyBombAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
     
        if (PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject); // 폭탄 파괴
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (other.CompareTag("Boss"))
            {
                StatComponent targetStatComponent = other.GetComponent<StatComponent>();
                PhotonView targetPhotonView = other.GetComponent<PhotonView>();

                // StatComponent와 PhotonView가 모두 있는 경우에만 데미지 처리 및 이펙트 호출
                if (targetStatComponent != null && targetPhotonView != null)
                {


                    // 폭발 이펙트 재생 RPC:
                    //  photonView.RPC("PlayExplosionEffect", RpcTarget.All, transform.position); // 폭발 위치도 함께 전달


                    // 마스터 클라이언트는 이 RPC를 받으면 StatComponent.TakeDamage()를 호출합니다.
                    targetPhotonView.RPC("RequestTakeDamage", RpcTarget.MasterClient, damageAmount);

                    // 폭탄 파괴:
                    // 데미지 적용 및 이펙트 호출 후, 마스터 클라이언트가 폭탄을 파괴합니다.
                    // 폭탄의 소유권이 마스터 클라이언트에게 있어야 합니다 (SkillShooter에서 설정됨).
                    if (photonView.IsMine)
                    {
                        PhotonNetwork.Destroy(gameObject);
                    }
                }
             }




            }

        }
    /*
            [PunRPC]
            private void PlayExplosionEffect(Vector3 explosionPosition)
            {
                // 여기에 폭발 이펙트 (파티클, 사운드 등) 재생 코드를 작성하세요.
                Debug.Log($"폭발 이펙트 재생! 위치: {explosionPosition}"); // 테스트를 위한 로그
            }
    */

}
