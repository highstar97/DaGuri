using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;
using Photon.Pun;
public class Player : MonoBehaviourPun 
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform rightHandController;
    [SerializeField] private Transform rightHandTarget;
    [SerializeField] private Transform leftHandController;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform avatarCameraTarget;
    [SerializeField] private Transform rightElbowHint;  
    [SerializeField] private Animator animator;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;

    private Vector2 moveInput;
    private Rigidbody rb;

    //쉴드 input action
    public Shield shield;
    public SkillShooter skillShooter;

    private Vector3 remoteRightHandPos;
    private Quaternion remoteRightHandRot;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    // Input System에서 호출됨 (Move 액션 이벤트 연결 필요)
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnShield(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (context.performed)
        {
            Debug.Log("🛡️ 쉴드 버튼 눌림!");
            shield.ActivateShield();
        }
       
    }
    //public void OnFire(InputAction.CallbackContext context)
    //{
    //    if (!photonView.IsMine)
    //    {
    //        return;
    //    }
    //    if (context.performed)
    //    {
    //        if (skillShooter != null)
    //        {
    //            skillShooter.Fire();
    //        }
    //        else
    //        {
    //            Debug.LogWarning("🚫 skillShooter가 연결되어 있지 않습니다!");
    //        }
    //    }
    //}
    public void OnFireHold(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine || !context.performed) return;

        if (skillShooter != null)
        {
            skillShooter.OnStartAiming();
        }
    }

    // 손 뗐을 때
    public void OnFireRelease(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine || !context.canceled) return;

        if (skillShooter != null)
        {
            skillShooter.Fire();
        }
    }


    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        // 아바타에서 오른손이 위치할 곳 조정
        rightHandTarget.position = avatarCameraTarget.position + rightHandController.position - cameraTransform.position;
        // 아바타에서 오른손의 회전 크기 조정
        rightHandTarget.rotation = avatarCameraTarget.rotation * Quaternion.Inverse(cameraTransform.rotation) * rightHandController.rotation;

        // 아바타에서 왼손이 위치할 곳 조정
        leftHandTarget.position = avatarCameraTarget.position + leftHandController.position - cameraTransform.position;
        // 아바타에서 왼손의 회전 크기 조정
        leftHandTarget.rotation = avatarCameraTarget.rotation * Quaternion.Inverse(cameraTransform.rotation) * leftHandController.rotation;
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        rb.velocity = inputDir * moveSpeed;

        if (animator != null)
        {
            animator.SetFloat("MoveX", rb.velocity.x);
            animator.SetFloat("MoveZ", rb.velocity.z);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (rightHandTarget == null || animator == null) return;
        // if (animator == null) return;

        // 오른손 위치 IK
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);

        // 왼손 위치 IK
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);

        // 팔꿈치 힌트
        if (rightElbowHint != null)
        {
            animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1f);
            animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowHint.position);
        }
    }
}