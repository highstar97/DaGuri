using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFireBall : MonoBehaviour
{
    private Vector3 startPosition;    // 이동을 시작할 위치
    private Vector3 endPosition;      // 이동을 마칠 위치
    private float moveSpeed = 5f;     // 이동 속도 (초당 유니티 단위)

    [Header("FireBall Stat")]
    public Vector3 maxScale = new Vector3(5f, 5f, 5f); // 최대 스케일 (이 이상 커지지 않음, 선택 사항)
    public float growthRate = 0.1f; // 탐지 범위 증가값
    public LayerMask targetLayer; // 특정 레이어의 오브젝트만 탐지하고 싶을 때 사용

    private bool isMoving = false;   // 현재 오브젝트가 이동 중인지 여부
    public Vector3 initialScale; // 오브젝트의 초기 스케일 (인스펙터에서 조절 가능)
    private Coroutine radiousIncrease;

    private void Reset()
    {
        targetLayer = LayerMask.GetMask("Player");
    }

    private void Start()
    {
        initialScale = transform.localScale;
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
                Explode(); // 폭발 범위 탐지 및 피해 주기
            }

            // 이번 프레임에 이동할 최대 거리를 계산
            float step = moveSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, endPosition, step);
        }
    }
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

        // 거리 증가 시작
        radiousIncrease = StartCoroutine(DetetioinRadiousIncrease());
    }

    IEnumerator DetetioinRadiousIncrease()
    {
        // 최대 범위 넘으면 더이상 증가하지 않음
        while (transform.localScale.x <= maxScale.x)
        {
            Vector3 increaseAmount = Vector3.one * growthRate;
            transform.localScale += increaseAmount;
            yield return new WaitForSeconds(0.01f);
        }        
        transform.localScale = maxScale;
        yield break;
    }

    private void Explode()
    {
        if (radiousIncrease != null)
        {
            StopCoroutine(radiousIncrease);
        }

        float currentRadius = transform.localScale.x * 0.5f;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, currentRadius, targetLayer);

        foreach (var hitCollider in hitColliders)
        {
            Debug.Log(hitCollider.name + "가 Fireball 범위에 들어왔습니다!");
            ITakeDamageable playerHP = hitCollider.GetComponent<ITakeDamageable>();
            if (playerHP != null)
            {
                Debug.Log(hitCollider.name + "Fireball 피해");
                playerHP.TakeDamage(1);
            }
        }
        Debug.Log(currentRadius);
        Destroy(gameObject); // Hack: 포톤 오브젝트 풀링으로 대체 예정
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x * 0.5f);
    }
}
