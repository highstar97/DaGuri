using UnityEngine;

public class FollowVRController : MonoBehaviour
{
    public Transform vrControllerTransform; // Inspector에서 Right Controller 오브젝트 연결

    void Update()
    {
        // 스크립트가 매 프레임 실행되는지 확인 (Console 창에 계속 출력됨)
        Debug.Log("Follow VR Controller Update is running.");

        if (vrControllerTransform != null)
        {
            // VR 컨트롤러의 위치와 회전을 IK 타겟에 복사
            transform.position = vrControllerTransform.position;
            transform.rotation = vrControllerTransform.rotation;

            // VR 컨트롤러의 Transform 값이 실제 변화하는지 확인
            Debug.Log($"VR Controller Position: {vrControllerTransform.position}, Rotation: {vrControllerTransform.rotation}");
            Debug.Log($"IK Target Position: {transform.position}, Rotation: {transform.rotation}");
        }
        else
        {
            // vrControllerTransform이 할당되지 않았거나 null이 된 경우
            Debug.LogError("VR Controller Transform is NOT assigned or NULL on " + gameObject.name);
        }
    }
}