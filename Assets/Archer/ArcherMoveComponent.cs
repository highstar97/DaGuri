using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class ArcherMoveComponent : MonoBehaviour
{
    #region Variables   
    public float moveSpeed = 3f;

    public InputActionProperty moveAction;

    public Transform cameraTransform;           // 실제 카메라 위치
    public Transform leftController;            // 실제 왼쪽 컨트롤러 위치
    public Transform rightController;           // 실제 오른쪽 컨트롤러 위치

    public Transform avatarCameraTarget;        // 실제 카메라에 비례해서 아바타 매시에 가상 카메라 위치
    public Transform avatarLeftHandTarget;      // 실제 카메라 - 실제 왼쪽 컨트롤러에 비례해서, 아바타 매시에서 왼손의 위치
    public Transform avatarRightHandTarget;     // 실제 카메라 - 실제 오른쪽 컨트롤러에 비례해서, 아바타 매시에서 오른손의 위치

    private int currentIdleIndex = 0;           // 현재 Idle Pose 인덱스 값
    private float moveForward;                  // 앞으로 움직이는 값, 뒤는 음수
    private float moveRight;                    // 오른쪽으로 움직이는 값, 왼쪽은 음수

    private Animator archerAnimator;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        archerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 아바타에서 왼쪽손이 위치할 곳 조정
        avatarLeftHandTarget.position = avatarCameraTarget.position + leftController.position - cameraTransform.position;
        // 아바타에서 외쪽손의 회전 크기 조정
        Quaternion leftRotationOffset = Quaternion.Inverse(cameraTransform.rotation) * leftController.rotation;
        avatarLeftHandTarget.rotation = avatarCameraTarget.rotation * leftRotationOffset;

        // 아바타에서 오른손이 위치할 곳 조정
        avatarRightHandTarget.position = avatarCameraTarget.position + rightController.position - cameraTransform.position;
        // 아바타에서 오른손의 회전 크기 조정
        Quaternion rightRotationOffset = Quaternion.Inverse(cameraTransform.rotation) * rightController.rotation;
        avatarRightHandTarget.rotation = avatarCameraTarget.rotation * rightRotationOffset;
    }

    private void FixedUpdate()
    {
        Vector2 moveValue = moveAction.action.ReadValue<Vector2>();

        archerAnimator.SetFloat("MoveX", moveValue.x);
        archerAnimator.SetFloat("MoveZ", moveValue.y);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (archerAnimator == null || leftController == null) return;

        archerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        archerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        archerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, avatarLeftHandTarget.position);
        archerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, avatarLeftHandTarget.rotation);

        archerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        archerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        archerAnimator.SetIKPosition(AvatarIKGoal.RightHand, avatarRightHandTarget.position);
        archerAnimator.SetIKRotation(AvatarIKGoal.RightHand, avatarRightHandTarget.rotation);
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
            archerAnimator.SetFloat("MoveX", moveForward);

            moveRight = inputVelocity.y;
            archerAnimator.SetFloat("MoveZ", moveRight);

            //mageRigidBody.velocity = new Vector3(moveForward * moveSpeed, 0, moveRight * moveSpeed);
        }
    }

    // Invoke Unity Events
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputVelocity = context.ReadValue<Vector2>();

        if (inputVelocity != null)
        {
            moveForward = inputVelocity.x;
            archerAnimator.SetFloat("MoveX", moveForward);

            moveRight = inputVelocity.y;
            archerAnimator.SetFloat("MoveZ", moveRight);

           // mageRigidBody.velocity = new Vector3(moveForward * moveSpeed, 0, moveRight * moveSpeed);
        }
    }
    #endregion

    #region User Functions
    // Idle Pose를 바꾸어줌.
    //IEnumerator Co_ChangeIdlePose()
    //{
    //    const float changeTime = 5.0f;      // Pose 변경할 시간 interval
    //    while (true)
    //    {
    //        // 랜덤 Idle Index 생성
    //        int nextIdleIndex = 0;
    //        do
    //        {
    //            nextIdleIndex = Random.Range(0, 4);
    //        } while (currentIdleIndex == nextIdleIndex);

    //        // 시간 경과에 따른 부드런 Idle Pose 변경
    //        float elapsedTime = 0f;
    //        float startIndex = currentIdleIndex;
    //        float changingIndex = startIndex;

    //        while (elapsedTime <= 1f)
    //        {
    //            elapsedTime += Time.deltaTime;
    //            changingIndex = Mathf.Lerp(startIndex, nextIdleIndex, elapsedTime);
    //            archerAnimator.SetFloat("Idle Index", changingIndex);
    //            yield return null;
    //        }

    //        currentIdleIndex = nextIdleIndex;

    //        // changeTime 마다 Pose 변경
    //        yield return new WaitForSeconds(changeTime);
    //    }
    //}
    #endregion
}
