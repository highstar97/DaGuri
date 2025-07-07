using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rightHandController;
    [SerializeField] private Transform rightHandIKTarget;
    [SerializeField] private Transform rightElbowHint;
    [SerializeField] private Animator animator;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;

    private Vector2 moveInput;
    private Rigidbody rb;

    //쉴드 input action
    public Shield shield;
    public SkillShooter skillShooter;

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
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnShield(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("🛡️ 쉴드 버튼 눌림!");
            shield.ActivateShield();
        }
       
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (skillShooter != null)
            {
                skillShooter.Fire();
            }
            else
            {
                Debug.LogWarning("🚫 skillShooter가 연결되어 있지 않습니다!");
            }
        }
    }


    private void Update()
    {
        // VR 컨트롤러 위치를 IK 타겟에 전달
        if (rightHandController != null && rightHandIKTarget != null)
        {
            rightHandIKTarget.position = rightHandController.position + new Vector3(0, 0, 3f);
            rightHandIKTarget.rotation = rightHandController.rotation;
        }
    }

    private void FixedUpdate()
    {
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
        if (animator == null) return;

        // 손 위치 IK
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIKTarget.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandIKTarget.rotation);

        // 팔꿈치 힌트
        if (rightElbowHint != null)
        {
            animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1f);
            animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowHint.position);
        }
    }

    
}
