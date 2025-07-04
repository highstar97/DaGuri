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
    public State state { get; private set; } // 현재 활의 상태
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
    public Transform ArrowTransform; //화살이 발사될 위치
    public float ArrowDistance = 50f; //화살이 날라가는 거리
    public Transform leftHandIKTarget; //왼손이 활을 잡을 부분
    public Transform rightHandIKTarget; //오른손이 시위을 잡을 부분
    public float minBowDrawDistance = 0.3f;//오른손이 당기는 최소거리
    public float maxBowDrawDistance = 0.5f; //오른손이 당기는 최대거리

    private Vector3 drawStartPosition; //활 당기기 시작 위치
    private bool IsDrawBow = false; //활 당기는지 여부
    private bool IsGesture = false; //오른손 직선 당기기 제스쳐가 유효한지 확인

    private void Awake()
    {
        state = State.Idle; //초기상태는 Idle;
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

    //활 상태를 변경하고 필요한 로직 형성(현재 사용 X)
    private void SetBowState(State newState)
    {
        state = newState;
        Debug.Log($"Bow State : {state}");
    }

    //활시위를 당길때 선이 되는지 확인하고 나오는 함수(여기서는 화살을 날리는 기능을 넣어짐)
    public void ShootCheck()
    {
        if (state == State.Ready && IsDrawBow)
        {
            IsGesture = true;
            Debug.Log("활 발사 준비완료");
        }
    }

    void Update()
    {
        // 1. 손뻗기 제스쳐 감지
        IsLeftHandExtended = Vector3.Distance(leftHandControllerTransform.position, headTransform.position) >= handExtension;
        IsRightHandExtended = Vector3.Distance(rightHandControllerTransform.position, headTransform.position) >= handExtension;

        //2. 현재 Index Trigger 눌림상태 확인
        bool leftTriggerPressed = lefthandTrigger.action.IsPressed();
        bool rightTriggerPressed = righthandTrigger.action.IsPressed();

        //3. Ready 상태 진입
        if (IsLeftHandExtended && IsRightHandExtended && leftTriggerPressed && rightTriggerPressed && state == State.Idle)
        {
            SetBowState(State.Ready);
            m_anim.SetBool("Ready", true);
            drawStartPosition = rightHandControllerTransform.position; //활당기기 시작점
            IsDrawBow = true; // 활당기기 시작 플래그 활성화
            IsGesture = false; // 직선 제스쳐 초기화
        }
        //조건이 해당되지 않으면
        else if (state == State.Ready)
        {
            if (!IsLeftHandExtended || !IsRightHandExtended || !leftTriggerPressed || !rightTriggerPressed)
            {
                SetBowState(State.Idle);
                m_anim.SetBool("Ready", false);
                IsDrawBow = false;
                IsGesture = false;
                m_anim.SetBool("Shoot", false);
            }
        }

        //4. 활 당기기 및 발사 로직
        if (state == State.Ready && IsDrawBow)
        {
            float currentDrawDistance = Vector3.Distance(drawStartPosition, rightHandControllerTransform.position);
            if (currentDrawDistance >= minBowDrawDistance && IsGesture && !rightTriggerPressed)
            {
                SetBowState(State.Shoot);
                m_anim.SetBool("Shoot", true);
                IsDrawBow = false; //발사되었을때는 Line그려지기 해제
                IsGesture = false; //발사 직후 제스쳐 해제

                Debug.Log("화살 발사");
            }
        }
    }
    void OnAnimatorIK(int layIndex)
    {
        if (m_anim == null || leftHandControllerTransform == null || rightHandControllerTransform == null || leftHandIKTarget == null || rightHandIKTarget)
        {
            return;
        }
        if (state == State.Ready || state == State.Shoot)
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
