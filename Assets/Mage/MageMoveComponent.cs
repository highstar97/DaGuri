using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class MageMoveComponent : MonoBehaviour
{
    #region Variables   
    public float moveSpeed = 3f;
    public Transform cameraTransform; public Transform avatarCameraTarget;
    public Transform leftController; public Transform avatarLeftHandTarget;
    public Transform rightController; public Transform avatarRightHandTarget;

    private int currentIdleIndex = 0;           // 현재 Idle Pose 인덱스 값
    private float moveForward;                  // 앞으로 움직이는 값, 뒤는 음수
    private float moveRight;                    // 오른쪽으로 움직이는 값, 왼쪽은 음수

    private Animator mageAnimator;
    private Rigidbody mageRigidBody;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        mageAnimator = GetComponent<Animator>();
        mageRigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        StartCoroutine(nameof(Co_ChangeIdlePose));

        leftController.transform.position = mageAnimator.GetIKHintPosition(AvatarIKHint.LeftElbow);
        rightController.transform.position = mageAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);
    }

    private void Update()
    {
        avatarLeftHandTarget.position = leftController.position;
        avatarLeftHandTarget.rotation = leftController.rotation;

        avatarRightHandTarget.position = rightController.position;
        avatarRightHandTarget.rotation = rightController.rotation;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (mageAnimator == null || leftController == null) return;

        mageAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        mageAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        mageAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftController.position);
        mageAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftController.rotation);

        mageAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        mageAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        mageAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightController.position);
        mageAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightController.rotation);
    }
    #endregion

    #region Player Input Functions
    // Input System을 이용한 움직임
    // Send Massage
    public void OnMove(InputValue value)
    {
        Vector2 inputVelocity = value.Get<Vector2>();

        if (inputVelocity != null)
        {
            moveForward = inputVelocity.x;
            mageAnimator.SetFloat("Move Forward", moveForward);

            moveRight = inputVelocity.y;
            mageAnimator.SetFloat("Move Right", moveRight);

            mageRigidBody.velocity = new Vector3(moveForward * moveSpeed, 0, moveRight * moveSpeed);
        }
    }

    // Invoke Unity Events
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputVelocity = context.ReadValue<Vector2>();

        if (inputVelocity != null)
        {
            moveForward = inputVelocity.x;
            mageAnimator.SetFloat("Move Forward", moveForward);

            moveRight = inputVelocity.y;
            mageAnimator.SetFloat("Move Right", moveRight);

            mageRigidBody.velocity = new Vector3(moveForward * moveSpeed, 0, moveRight * moveSpeed);
        }
    }
    #endregion

    #region User Functions
    // Idle Pose를 바꾸어줌.
    IEnumerator Co_ChangeIdlePose()
    {
        const float changeTime = 5.0f;      // Pose 변경할 시간 interval
        while (true)
        {
            // 랜덤 Idle Index 생성
            int nextIdleIndex = 0;
            do
            {
                nextIdleIndex = Random.Range(0, 4);
            } while (currentIdleIndex == nextIdleIndex);

            // 시간 경과에 따른 부드런 Idle Pose 변경
            float elapsedTime = 0f;
            float startIndex = currentIdleIndex;
            float changingIndex = startIndex;

            while (elapsedTime <= 1f)
            {
                elapsedTime += Time.deltaTime;
                changingIndex = Mathf.Lerp(startIndex, nextIdleIndex, elapsedTime);
                mageAnimator.SetFloat("Idle Index", changingIndex);
                yield return null;
            }

            currentIdleIndex = nextIdleIndex;

            // changeTime 마다 Pose 변경
            yield return new WaitForSeconds(changeTime);
        }
    }
    #endregion
}
