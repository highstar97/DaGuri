using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Input System 사용

public class BossKnockBackSkill : MonoBehaviour
{
    // --- Input Actions 설정 ---
    [Header("Input Actions")]
    public InputActionReference controllerTriggerPress;   // 이 컨트롤러의 트리거 누름
    public InputActionReference controllerTriggerRelease; // 이 컨트롤러의 트리거 떼기
    public InputActionReference controllerPosition;       // 이 컨트롤러의 위치

    // --- 스킬 발동 조건 설정 ---
    [Header("Skill Conditions")]
    public float maxDetectionTime = 1.5f;       // 트리거 누른 후 바닥에 닿을 수 있는 최대 시간 (초)
    public float minHeightDifference = 0.3f;    // 최초 누른 Y값과 닿은 Y값의 최소 차이 (미터)
    public LayerMask floorLayer;                // Floor 오브젝트의 Layer Mask

    // --- 내부 변수 ---
    private float initialControllerY; // 트리거 처음 눌렀을 때, 컨트롤로 Y 좌표
    private Coroutine smashDetectionCoroutine;
    private bool isTriggerPressed = false; // 트리거가 눌려있는지 상태 추적

    // --- 활성화 시 Input Action 콜백 구독 ---
    void OnEnable()
    {
        // InputActionReference를 사용할 때는 .action이 null이 아닌지 확인하는 것이 좋습니다.
        // Inspector에서 할당이 제대로 안 될 경우를 대비합니다.
        if (controllerTriggerPress != null && controllerTriggerPress.action != null)
        {
            controllerTriggerPress.action.performed += OnTriggerPressed;
        }
        if (controllerTriggerRelease != null && controllerTriggerRelease.action != null)
        {
            controllerTriggerRelease.action.performed += OnTriggerReleased;
        }
        // controllerPosition.action은 ReadValue만 하므로, 콜백 등록은 필요 없습니다.
        // 다만, ReadValue 전에 액션이 활성화되어 있는지 확인해야 합니다.
    }

    // --- 비활성화 시 Input Action 콜백 구독 해제 ---
    void OnDisable()
    {
        if (controllerTriggerPress != null && controllerTriggerPress.action != null)
        {
            controllerTriggerPress.action.performed -= OnTriggerPressed;
        }
        if (controllerTriggerRelease != null && controllerTriggerRelease.action != null)
        {
            controllerTriggerRelease.action.performed -= OnTriggerReleased;
        }
    }


    // --- 컨트롤러 트리거 눌림 감지 ---
    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        
        isTriggerPressed = true;
        // 이미 스매쉬 코루틴이 실행 중이면 중복 방지 (이전 코루틴 중지 및 재시작)
        if (smashDetectionCoroutine != null)
        {
            StopCoroutine(smashDetectionCoroutine);
        }
        initialControllerY = controllerPosition.action.ReadValue<Vector3>().y;
        smashDetectionCoroutine = StartCoroutine(SmashDetectionTimeoutCoroutine()); // 시간 초과 코루틴만 시작
        Debug.Log("검지 누름 Initial Y: " + initialControllerY);
    }

    // --- 컨트롤러 트리거 떼기 감지 ---
    private void OnTriggerReleased(InputAction.CallbackContext context)
    {
        isTriggerPressed = false;
        if (smashDetectionCoroutine != null)
        {
            // 스킬이 발동되지 않고 코루틴이 종료되면 초기화
            StopCoroutine(smashDetectionCoroutine);
            smashDetectionCoroutine = null;
            Debug.Log("검지 땜");
        }
    }

    // --- 트리거 충돌 감지 (Is Trigger가 체크된 Collider) ---
    private void OnTriggerEnter(Collider other)
    {
        // 트리거가 눌려있는 상태이고, 충돌한 오브젝트의 Layer가 Floor Layer에 속하는지 확인
        if (isTriggerPressed && (floorLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            Vector3 currentControllerVector = controllerPosition.action.ReadValue<Vector3>();
            float currentControllerY = currentControllerVector.y;

            // 높이 차이 조건 확인
            if (Mathf.Abs(initialControllerY - currentControllerY) >= minHeightDifference)
            {
                Debug.Log($"성공! 시작 Y: {initialControllerY}, 현재 Y: {currentControllerY}, 차이: {initialControllerY - currentControllerY}");
                ActivateKnockbackSkill(currentControllerVector); // 스킬 발동

                if (smashDetectionCoroutine != null)
                {
                    StopCoroutine(smashDetectionCoroutine); // 시간 초과 코루틴 중지
                    smashDetectionCoroutine = null;
                }
                isTriggerPressed = false; // 스킬 발동 후 상태 초기화
            }
            else
            {
                Debug.Log($"실패!! 시작 Y: {initialControllerY}, 현재 Y: {currentControllerY}, 차이: {initialControllerY - currentControllerY}");
            }
        }
    }

    // --- 시간 초과를 처리하는 코루틴 (충돌이 발생하지 않을 경우) ---
    private IEnumerator SmashDetectionTimeoutCoroutine()
    {
        yield return new WaitForSeconds(maxDetectionTime);

        // maxDetectionTime 안에 스킬이 발동되지 않았을 경우
        if (smashDetectionCoroutine != null) // 아직 코루틴이 유효하다면 (즉, OnTriggerEnter에서 중지되지 않았다면)
        {
            Debug.Log("시간 초과, 넉백실패");
            smashDetectionCoroutine = null;
            isTriggerPressed = false; // 상태 초기화
        }
    }

    // --- 넉백 스킬 발동 로직 ---
    private void ActivateKnockbackSkill(Vector3 impactPoint)
    {
        /*        Debug.Log("Knockback Skill Activated at: " + impactPoint);
                Collider[] hitColliders = Physics.OverlapSphere(impactPoint, knockbackRadius);
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Enemy"))
                    {
                        Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            Vector3 knockbackDirection = (hitCollider.transform.position - impactPoint).normalized;
                            knockbackDirection.y = Mathf.Max(0.1f, knockbackDirection.y);
                            rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
                            Debug.Log($"Applied knockback to: {hitCollider.name}");
                        }
                    }
                }*/

        Debug.Log("넉백 발동");
        // 이펙트, 사운드 등 추가
    }
}
