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
            if (!other.CompareTag("Boss")) return;

            ITakeDamageable victim = other.GetComponent<ITakeDamageable>();
            if (victim == null) return;

            victim.BroadcastTakeDamage(damageAmount, this.gameObject);
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
}