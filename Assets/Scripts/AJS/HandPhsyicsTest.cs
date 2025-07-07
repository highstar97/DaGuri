using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPhsyicsTest : MonoBehaviour
{
    public Transform target;
    private Rigidbody rb;

    //public Renderer nonPhysicalHand; // 실제 컨트롤로 위치를 보여줄 랜더러
    public GameObject nonPhysicalHandTest; // Hack: 실제 컨트롤로 위치를 보여줄 랜더러인데 지금은 GameObject
    public float shonwNonPhysicalHandDistance = 0.05f; // 컨트롤 위치를 보여 줄 거리 차이

    private void Start()
    {
       rb = GetComponent<Rigidbody>();          

    }

    private void Update()
    {
        // 손 모델링과 실제 손 사이의 거리가
        float distance = Vector3.Distance(transform.position, target.position);
        // 일정 거리 이상이면
        if (distance > shonwNonPhysicalHandDistance)
        {
            // 손 랜더러 활성화
            // nonPhysicalHand.enabled = true;
            nonPhysicalHandTest.gameObject.SetActive(true); // Hack: GameObject 여서 임시적으로 이렇게 설정
        }
        else
        {
            // nonPhysicalHand.enabled = false;
            nonPhysicalHandTest.gameObject.SetActive(false);
        }
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
