using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPhsyicsTest : MonoBehaviour
{
    public Transform target;
    private Rigidbody rb;

    private void Start()
    {
       rb = GetComponent<Rigidbody>();       
    }

    private void FixedUpdate()
    {
        rb.velocity = (target.position - transform.position) / Time.fixedDeltaTime;

        // 타켓과 나의 회전량의 차이를 구하기 위한 Quaternion 계산식 
        // 타켓의 rotation에 내 역회전량을 곱한다
        Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
        // rotationDifference 를 각도와 축 정보로 반환
        rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        // 해당 정보로 얼만큼 회전해야 하는지 Vector3  형태로 계산
        Vector3 rotationDifferenceInDegree = angleInDegree * rotationAxis;

        // 해당 정보들로, 자연스럽게 돌게 시킴
        rb.angularVelocity = rotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime;
    }
}
