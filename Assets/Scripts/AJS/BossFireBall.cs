using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFireBall : MonoBehaviour
{
    private Vector3 startPosition;    // 이동을 시작할 위치
    private Vector3 endPosition;      // 이동을 마칠 위치
    private float moveSpeed = 5f;     // 이동 속도 (초당 유니티 단위)

    private bool isMoving = false;   // 현재 오브젝트가 이동 중인지 여부

    /// <summary>
    /// 오브젝트를 지정된 시작점에서 도착점으로 일정한 속도로 이동시킵니다.
    /// </summary>
    /// <param name="startPos">이동 시작 위치</param>
    /// <param name="endPos">이동 도착 위치</param>
    /// <param name="speed">초당 이동 속도 (유니티 단위)</param>
    public void Fire(Vector3 startPos, Vector3 endPos, float speed)
    {
        startPosition = startPos;
        endPosition = endPos;
        moveSpeed = speed;

        isMoving = true;       // 이동 플래그 활성화

        // 이동 시작 시 오브젝트 위치를 시작 위치로 바로 설정
        transform.position = startPosition;
    }

    void Update()
    {
        // isMoving 플래그가 true일 때만 이동 로직을 실행
        if (isMoving)
        {
            // 현재 위치에서 도착 위치까지의 남은 거리를 계산
            float distanceToTarget = Vector3.Distance(transform.position, endPosition);

            // 일정거리 내에 도착하면 
            if (distanceToTarget <= 0.1f)
            {
                isMoving = false; // 이동 플래그 비활성화
                // Destroy(gameObject);
            }

            // 이번 프레임에 이동할 최대 거리를 계산
            float step = moveSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, endPosition, step);
        }
    }
}
