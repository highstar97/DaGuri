using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float lifeTime = 5f; // 폭탄 수명

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            Debug.Log("타겟에 맞음!");
            // 맞았을 때 효과 처리(데미지, 이펙트 등) 추가 가능

            Destroy(gameObject);
        }
    }
}
