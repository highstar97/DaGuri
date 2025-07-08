using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bow : MonoBehaviour
{
    public enum State
    {
        Idle, //그냥 초기상태
        Ready, //준비상태
        Shoot //발사상태
    }

    [Header("활 상태 및 애니메이션")]
    public State CurrentState { get; private set; } // 현재 활의 상태
    public Animator m_anim; //애니메이터에 따른 움직임 설정
    public InputActionProperty lefthandTrigger; // 왼손 IndexTrigger
    public InputActionProperty righthandTrigger; // 오른손 IndexTrigger

    [Header("손뻗기 감지")]
    public Transform leftHandControllerTransform; //왼손 컨트롤러의 위치
    public Transform rightHandControllerTransform; //오른손 컨트롤러의 위치
    public Transform headTransform; //플레이어 머리 위치
    public float handExtension = 0.3f; //손을 뻗었다고 생각되는 거리(30cm정도는 뻗어줘야함.)
    private bool IsLeftHandExtended = false; //왼손을 뻗은지 확인하는 플래그
    private bool IsRightHandExtended = false; //오른손을 뻗은지 확인하는 플래그

    [Header("활 IK 및 활의 특성")]
    public float ArrowForce = 50f; //화살이 날라가는 거리
    public Transform leftHandIKTarget; //왼손이 활을 잡을 부분
    public Transform rightHandIKTarget; //오른손이 시위을 잡을 부분
    public float minBowDrawDistance = 0.1f;//오른손이 당기는 최소거리
    public float maxBowDrawDistance = 0.5f; //오른손이 당기는 최대거리

    private Vector3 drawStartPosition; //활 당기기 시작 위치
    private bool IsDrawBow = false; //활 당기는지 여부
    private bool IsGesture = false; //오른손 직선 당기기 제스쳐가 유효한지 확인
    private bool canFire = true;
    private bool hasFiredThisShot = false;

    private void Awake()
    {
        CurrentState = State.Idle; //초기상태는 Idle;
    }

    private void OnEnable()
    {
        lefthandTrigger.action.Enable();
        righthandTrigger.action.Enable();

    }
    private void OnDisable()
    {
        lefthandTrigger.action.Disable();
        righthandTrigger.action.Disable();
    }

    public Vector3 GetDrawStartPosition()
    {
        return drawStartPosition;
    }

    public bool GetIsDrawBow()
    {
        return IsDrawBow;
    }

    //활 상태를 변경하고 필요한 로직 형성(현재 사용 X)
    private void SetBowState(State newState)
    {
        if (CurrentState == newState)
        {
            return;
        }
        CurrentState = newState;

        switch (CurrentState)
        {
            case State.Idle:
                m_anim.SetBool("Ready", false);
                m_anim.SetBool("Shoot", false);
                IsDrawBow = false;
                IsGesture = false;
                canFire = true;
                hasFiredThisShot = false;
                
                break;
            case State.Ready:
                m_anim.SetBool("Ready", true);
                m_anim.SetBool("Shoot", false);
                IsDrawBow = true;
                IsGesture = false;
                canFire = true;
                hasFiredThisShot = false;
                break;

           case State.Shoot:
                m_anim.SetBool("Ready", false);
                m_anim.SetBool("Shoot", true);
                if (!hasFiredThisShot)
                {
                    hasFiredThisShot = true;
                    canFire = false;
                }

                StartCoroutine(RestartAttack(0.1f));
                    break;  
        }
    }

    IEnumerator RestartAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetBowState(State.Idle);
    }

    public void OnDrawGestureFinished(List<Vector3> trail, bool IsLineGesture)
    {
        if (CurrentState == State.Ready && IsDrawBow)
        {
            if (IsLineGesture)
            {
                Vector3 start = trail[0];
                Vector3 end = trail[trail.Count - 1];
                float dotProduct = Vector3.Dot((end - start).normalized, -rightHandControllerTransform.forward);

                if (dotProduct > 0.0f)
                {
                    IsGesture = true;
                }
                else
                {
                    IsGesture = false;
                }
            }
            else
            {
                IsGesture = false;
            }
        }
        else
        {
            IsGesture = false;
        }
    }

    void Update()
    {
        //bowPivot.position = avaterLeftHandTarget.position;
        // 1. 손뻗기 제스쳐 감지
        IsLeftHandExtended = Vector3.Distance(leftHandControllerTransform.position, headTransform.position) >= handExtension;
        IsRightHandExtended = Vector3.Distance(rightHandControllerTransform.position, headTransform.position) >= handExtension;

        //2. 현재 Index Trigger 눌림상태 확인
        bool leftTriggerPressed = lefthandTrigger.action.ReadValue<float>() > 0.1f;
        bool rightTriggerPressed = righthandTrigger.action.ReadValue<float>() > 0.1f;

        // 3. Ready 상태 진입 및 이탈 로직
        if (CurrentState == State.Idle)
        {
            if (IsLeftHandExtended && IsRightHandExtended && leftTriggerPressed && rightTriggerPressed)
            {
                SetBowState(State.Ready);
                drawStartPosition = rightHandControllerTransform.position;
                // 이전: drawStartPosition = rightHandControllerTransform.position;
            }
        }
        else if (CurrentState == State.Ready)
        {
            if (!IsLeftHandExtended || !IsRightHandExtended || !leftTriggerPressed)
            {
                SetBowState(State.Idle);
            }
            else // Ready 상태를 유지하는 동안 활 당기기 및 발사 로직 체크
            {
                float currentDrawDistance = Vector3.Distance(drawStartPosition, rightHandControllerTransform.position);
                if (currentDrawDistance >= minBowDrawDistance && IsGesture && !rightTriggerPressed && canFire)
                {
                    SetBowState(State.Shoot);
                }
                else if (currentDrawDistance > maxBowDrawDistance) // 오버드로우 방지
                {
                    SetBowState(State.Idle);
                }
            }
        }
    }
        void OnAnimatorIK(int layIndex)
    {
        if (m_anim == null || leftHandControllerTransform == null || rightHandControllerTransform == null || leftHandIKTarget == null || rightHandIKTarget)
        {
            return;
        }
        if (CurrentState == State.Ready || CurrentState == State.Shoot)
        {
            //왼손은 활잡기
            m_anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            m_anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
            m_anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKTarget.position);
            m_anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIKTarget.rotation);

            //오른손은 시위 당기기
            m_anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            m_anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            m_anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandControllerTransform.position);
            m_anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandControllerTransform.rotation);
        }
        //Idle일때는 애니메이션이 손을 제어
        else
        {
            m_anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
            m_anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
            m_anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
            m_anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
        }
    }
}
