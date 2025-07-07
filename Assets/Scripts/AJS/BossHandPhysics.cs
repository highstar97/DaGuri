using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Rigidbody))]
public class BossHandPhysics : MonoBehaviourPun
{
    private Rigidbody rb;
    private Collider[] handColliders; // 손 콜라이더
    [Header("Hand Controller")]
    [SerializeField]
    private Transform targetController;

    [Header("Model Prefab")]
    public GameObject bossHandPresence; // 실제 컨트롤로 위치를 보여주는 오브젝트 (XRController Model 역할)
    public float shonwNonPhysicalHandDistance = 0.05f; // 컨트롤 위치를 보여 줄 거리 차이

    private void Reset()
    {
        shonwNonPhysicalHandDistance = 0.05f;
        if(rb == null)
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false; // 중력 비활성화
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        handColliders = GetComponentsInChildren<Collider>(); // 손에 있는 손가락 콜라이더 컴포넌트 가져오기
    }

    void Update()
    {
        if(!photonView.IsMine) return;

        // 손 모델링과 실제 손 사이의 거리가
        float distance = Vector3.Distance(transform.position, targetController.position);
        // 일정 거리 이상이면
        if (distance > shonwNonPhysicalHandDistance)
        {
            // 모델 Prefab 활성화
            bossHandPresence.SetActive(true);
        }
        else
        {
            // 모델 Prefab 비활성화
            bossHandPresence.SetActive(false);
        }
    }
    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        rb.velocity = (targetController.position - transform.position) / Time.fixedDeltaTime;

        // 타켓과 나의 회전량의 차이를 구하기 위한 Quaternion 계산식 
        // 타켓의 rotation에 내 역회전량을 곱한다
        Quaternion rotationDifference = targetController.rotation * Quaternion.Inverse(transform.rotation);
        // rotationDifference 를 각도와 축 정보로 반환
        rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        // 해당 정보로 얼만큼 회전해야 하는지 Vector3  형태로 계산
        Vector3 rotationDifferenceInDegree = angleInDegree * rotationAxis;

        // 해당 정보들로, 자연스럽게 돌게 시킴
        rb.angularVelocity = rotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime;
    }
    public void DisableHandCollider()
    {
        // 물건을 집을 때, 손 콜라이더 비활성화
        foreach (Collider collider in handColliders)
        {
            collider.enabled = false;
        }
    }

    public void EnableHandCollider()
    {
        // 물건 놓으면 다시 활성화
        foreach(Collider collider in handColliders)
        {
            collider.enabled = true;
        }
    }

    public void EnableHandColliderDelay(float delay)
    {
        // 물건 튕기지 않게. 0.5초 후에 콜라이더 활성화 하기
        Invoke("EnableHandCollider", delay);
    }
}
