using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SwordAttack : MonoBehaviour
{
    #region Variables
    public Transform SwordOffset;                   // 검 시작지점

    public ProjectileSpawner swordSpawner;          // 검격 스포너

    public SwordInput swordInput;                   // 검 입력

    private bool canAttack = true;
    #endregion

    void Start()
    {
        swordInput.OnPressingFinished += CheckGesture;
    }

    #region User Functions
    private void CheckGesture()
    {
        if (canAttack == false) return;
        
        List<Vector3> trail = new();
        trail.Add(swordInput.StartPosition);
        trail.Add(swordInput.EndPosition);

        // 대각선 체크
        if (GestureUtils.IsDiagonalGesture(trail))
        {
            Debug.Log("대각선 제스처 인식 → 검격 발동");
            swordSpawner.SpawnProjectile("Sword", SwordOffset.position, new Vector3(0f, 2f, 0f), this.gameObject);
        }

        StartCoroutine(CoAttackDelay());
    }

    IEnumerator CoAttackDelay(float delay = 0.5f)
    {
        canAttack = false;
        yield return new WaitForSeconds(delay);
        canAttack = true;
    }
    #endregion
}
