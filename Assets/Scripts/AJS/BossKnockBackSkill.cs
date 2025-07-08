//#define LOCAL_TEST
#define PHOTON

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit; // Input System 사용
[RequireComponent(typeof(Rigidbody))]
public class BossKnockBackSkill : MonoBehaviourPun
{
    public XRDirectInteractor directInteractor;
    // --- Input Actions 설정 ---
    [Header("Input Actions")]
    public InputActionReference controllerPosition;       // 이 컨트롤러의 위치
    public InputActionReference gripAction;       // 그립버튼

    // --- 스킬 발동 조건 설정 ---
    [Header("Skill Conditions")]
    public float maxDetectionTime;       // 트리거 누른 후 바닥에 닿을 수 있는 최대 시간 (초)
    public float minHeightDifference;    // 최초 누른 Y값과 닿은 Y값의 최소 차이 (미터)
    public LayerMask floorLayer;                // Floor 오브젝트의 Layer Mask

    // --- 내부 변수 ---
    private float initialControllerY; // 트리거 처음 눌렀을 때, 컨트롤로 Y 좌표
    private Coroutine smashDetectionCoroutine;
    private bool isKnockBackCheck = false; // 넉백스킬을 체크 중인지

    private void Reset()
    {     
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        floorLayer = LayerMask.GetMask("Floor");

        maxDetectionTime = 1f;
        minHeightDifference = 0.25f;
    }

    private void OnEnable()
    {

        // 이 스킬이 로컬 플레이어에 의해서만 시작되도록 합니다.
#if LOCAL_TEST
        bool isLocalPlayer = true;
#elif PHOTON       
        bool isLocalPlayer = photonView.IsMine;
#endif
        if (!isLocalPlayer) return;

        // 그립 액션 누르면, 넉백 스킬 체크 시작
        gripAction.action.performed += OnKnockBackStart;
        // 그립 액션 떼면, 넉백 스킬 체크 종료
        gripAction.action.canceled += OnKnockBackEnd;
    }
    private void OnDisable()
    {

#if LOCAL_TEST
        bool isLocalPlayer = true;
#elif PHOTON       
        bool isLocalPlayer = photonView.IsMine;
#endif
        if (!isLocalPlayer) return;

        gripAction.action.performed -= OnKnockBackStart;
        gripAction.action.canceled -= OnKnockBackEnd;
    }
    // 물건을 집으면 넉백체크 종료
    public void OnSelectKnockBackEnd()
    {
        if (directInteractor.hasSelection)
        {
            // 물건을 잡고 있는 상태면 넉백 체크 종료
            isKnockBackCheck = false;
            if (smashDetectionCoroutine != null)
            {
                // 스킬이 발동되지 않고 코루틴이 종료되면 초기화
                StopCoroutine(smashDetectionCoroutine);
                smashDetectionCoroutine = null;
                Debug.Log("넉백 체크 종료");
            }
        }
    }

    // --- 컨트롤러 눌림 감지 ---
    public void OnKnockBackStart(InputAction.CallbackContext context)
    {
        isKnockBackCheck = true;

        // 이미 스매쉬 코루틴이 실행 중이면 중복 방지 (이전 코루틴 중지 및 재시작)
        if (smashDetectionCoroutine != null)
        {
            StopCoroutine(smashDetectionCoroutine);
        }

        // 처음 눌렀을 때, 컨트롤러 높이
        initialControllerY = controllerPosition.action.ReadValue<Vector3>().y;

        smashDetectionCoroutine = StartCoroutine(SmashDetectionTimeoutCoroutine()); // 시간 초과 코루틴만 시작
        Debug.Log("검지 누름 Initial Y: " + initialControllerY);
    }

    // --- 컨트롤러 떼기 감지 ---
    public void OnKnockBackEnd(InputAction.CallbackContext context)
    {
        isKnockBackCheck = false;
        if (smashDetectionCoroutine != null)
        {
            // 스킬이 발동되지 않고 코루틴이 종료되면 초기화
            StopCoroutine(smashDetectionCoroutine);
            smashDetectionCoroutine = null;
            Debug.Log("넉백 체크 종료");
        }
    }

    // --- 트리거 충돌 감지 (Is Trigger가 체크된 Collider) ---
    private void OnTriggerEnter(Collider other)
    {
#if LOCAL_TEST
        bool isLocalPlayer = true;
#elif PHOTON       
        bool isLocalPlayer = photonView.IsMine;
#endif
        if (!isLocalPlayer) return;

        // 넉백 스킬 시전 중이고, 충돌한 오브젝트의 Layer가 Floor Layer에 속하는지 확인
        if (isKnockBackCheck && (floorLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            // 현재 컨트롤러 높이 체크
            Vector3 currentControllerVector = controllerPosition.action.ReadValue<Vector3>();
            float currentControllerY = currentControllerVector.y;

            // 높이 차이가 충분하면
            if (Mathf.Abs(initialControllerY - currentControllerY) >= minHeightDifference)
            {
                Debug.Log($"성공! 시작 Y: {initialControllerY}, 현재 Y: {currentControllerY}, 차이: {initialControllerY - currentControllerY}");
                ActivateKnockbackSkill(currentControllerVector); // 스킬 발동

                if (smashDetectionCoroutine != null)
                {
                    StopCoroutine(smashDetectionCoroutine); // 시간 초과 코루틴 중지
                    smashDetectionCoroutine = null;
                }
                isKnockBackCheck = false; // 스킬 발동 후 상태 초기화
            }
            else
            {
                // Hack: 디보그용, 제거가능
                Debug.Log($"실패!! 시작 Y: {initialControllerY}, 현재 Y: {currentControllerY}, 차이: {initialControllerY - currentControllerY}");
            }
        }
    }

    // --- 시간 초과를 처리하는 코루틴 (충돌이 발생하지 않을 경우) ---
    private IEnumerator SmashDetectionTimeoutCoroutine()
    {
        yield return new WaitForSeconds(maxDetectionTime);

        // maxDetectionTime 안에 스킬이 발동되지 않았을 경우
        if (smashDetectionCoroutine != null)
        {
            // 넉백 스킬 종류
            Debug.Log("시간 초과, 넉백실패");
            smashDetectionCoroutine = null;
            isKnockBackCheck = false;
        }
    }

    // --- 넉백 스킬 발동 로직 ---
    private void ActivateKnockbackSkill(Vector3 impactPoint)
    {
        Debug.Log("넉백 발동");
        // 넉백 성공시 진동
        directInteractor.xrController.SendHapticImpulse(1f, 0.2f);
    }
}
