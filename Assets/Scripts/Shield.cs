using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public GameObject shieldPrefab;
    public float shieldDuration = 10f;
    public Vector3 shieldOffset = new Vector3(0, 1f, 1.5f);

    private GameObject activeShield;

    public void ActivateShield()
    {
        if (activeShield != null)
        {
            return;
        }

        //플레이어 앞 위치 계산
        Vector3 sheildPos = transform.position + transform.TransformDirection(shieldOffset);
        Quaternion shieldRot = transform.rotation;

        activeShield = Instantiate(shieldPrefab, sheildPos, shieldRot);
        activeShield.transform.SetParent(transform); //플레이어 움직일 때 같이

        StartCoroutine(RemoveShieldAfterTime(shieldDuration));
    }

    IEnumerator RemoveShieldAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(activeShield);
    }
}
