using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class BossFireBallSkill : MonoBehaviourPun
{
    // --- 인스펙터에서 설정할 변수들 ---
    [Header("Line Renderer")]
    public LineRenderer lineRenderer; // 궤적을 표시할 Line Renderer
    public GameObject landingMarkerPrefab; // 착지점 마커 프리팹 (원형 데칼 등)

    public float maxRaycastDistance = 20f; // 직선 레이저가 뻗어나갈 최대 거리
    public LayerMask collisionLayers; // 발사체가 충돌할 레이어 (바닥, 벽 등)

    [Header("Input Actions")]
    public InputActionReference triggerAction; // 궤적 예측을 활성화/비활성화할 입력 액션

    [Header("FireBall")]
    public Transform firePoint; // 발사 시작 지점
    private Vector3 hitPoint; // 발사 도착 지점
    public string fireBall; // 화염구 프리팹

    private GameObject curFireBall; // 화염구 프리팹
    public float fireBallSpeed = 20f;

    // --- 내부 사용 변수 ---
    private GameObject currentLandingMarker; // 현재 인스턴스화된 착지점 마커
    private bool isSkillActive = false; // 스킬 시전 중인지
    private bool isHitFloor; // 현제 레이저가 바닥에 닿았는지

    void Awake()
    {
        lineRenderer.enabled = false; // 시작 시에는 비활성화

        // 착지점 마커 프리파일이 있다면 인스턴스화
        if (landingMarkerPrefab != null)
        {
            currentLandingMarker = Instantiate(landingMarkerPrefab);
            currentLandingMarker.SetActive(false);
        }
    }

    void OnEnable()
    {
        triggerAction.action.performed += ShowFireBallLine;
    }

    void OnDisable()
    {
        // 입력 액션 구독 해제
        triggerAction.action.performed -= ShowFireBallLine;
    }


    void Update()
    {
        if (isSkillActive)
        {
            UpdatePrediction();
        }
    }

    private void ShowFireBallLine(InputAction.CallbackContext context)
    {
        // 스킬 시전 중이면서
        if (isSkillActive)
        {
            // 조준선이 바닥에 닿았으면
            if (!isHitFloor) return;
            // 화염구 발사
            OnDeactivatePrediction();
        }
        else
        {
            // 스킬 시전
            OnActivatePrediction();
        }
    }

    private void OnActivatePrediction()
    {
        isSkillActive = true;
        lineRenderer.enabled = true;
        if (currentLandingMarker != null) currentLandingMarker.SetActive(true);

        curFireBall = PhotonNetwork.Instantiate(fireBall, firePoint.position, Quaternion.identity);
        curFireBall.transform.SetParent(firePoint);
    }

    private void OnDeactivatePrediction()
    {
        isSkillActive = false;
        lineRenderer.enabled = false;
        if (currentLandingMarker != null) currentLandingMarker.SetActive(false);


       BossFireBall fireBall = curFireBall.GetComponent<BossFireBall>();
        curFireBall.transform.SetParent(null);
        // 화염구 발사
        fireBall.Fire(firePoint.position, hitPoint, fireBallSpeed);
    }

    private void UpdatePrediction()
    {
        Vector3 rayOrigin = firePoint.position;
        Vector3 rayDirection = firePoint.forward;

        RaycastHit hitInfo;

        // 바닥에 닿았는지 파악
        isHitFloor = Physics.Raycast(rayOrigin, rayDirection, out hitInfo, maxRaycastDistance, collisionLayers);

        // Line Renderer 업데이트
        lineRenderer.positionCount = 2; // 시작점과 끝점
        lineRenderer.SetPosition(0, rayOrigin);

        if (isHitFloor)
        {
            hitPoint = hitInfo.point;

            lineRenderer.SetPosition(1, hitInfo.point); // 충돌 지점까지 라인 그림
            if (currentLandingMarker != null)
            {
                currentLandingMarker.transform.position = hitInfo.point + hitInfo.normal * 0.01f; // 지면에 살짝 띄워 표시
                currentLandingMarker.transform.rotation = Quaternion.LookRotation(hitInfo.normal); // 지면 법선 방향으로 정렬
                currentLandingMarker.transform.Rotate(90, 0, 0); // 필요에 따라 마커의 로컬 X축을 기준으로 90도 회전하여 평면에 눕히기
                currentLandingMarker.SetActive(true);
            }
        }
        else
        {
            // 충돌하지 않았다면, 최대 거리까지 라인 그림
            lineRenderer.SetPosition(1, rayOrigin + rayDirection * maxRaycastDistance);
            if (currentLandingMarker != null)
            {
                currentLandingMarker.SetActive(false); // 마커 숨김
            }
        }
    }
}
