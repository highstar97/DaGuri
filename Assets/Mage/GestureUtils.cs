using System.Collections.Generic;
using UnityEngine;

public static class GestureUtils
{
    /// <summary>
    /// 주어진 좌표 리스트가 앞에서 뒤로 가는 그림을 그렸는지 판별합니다.
    /// </summary>
    /// <param name="positions">궤적 위치 리스트</param>
    /// /// <param name="lineTolerance">직선 허용 오차 (작을수록 정밀)</param>
    /// <returns>원형 제스처로 인식되면 true</returns>
    
    // Archer가 공격을 할때 오른손이 직선이 되는지 판단하는 함수
    public static bool IsLineGesture(List<Vector3> positions, float lineTolerance = 0.05f)
    {
        //직선이지만 점을 3개로 두어 약간의 오차범위를 생각하는 직선으로 판단한다.
        if (positions.Count < 3)
        {
            return false;
        }
        

        Vector3 start = positions[0];
        Vector3 end = positions[positions.Count-1];
        Vector3 lineDirection = (end-start).normalized; //시작점에서 끝점으로 향하는 방향

        float totalDeviation = 0f;
        for(int i = 1; i<positions.Count -1; i++)
        {
            Vector3 point = positions[i];
            //시작점, 끝점까지 수직거리 계산
            Vector3 projectedPoint = start + Vector3.Dot(point - start, lineDirection) * lineDirection;
            totalDeviation += Vector3.Distance(point, projectedPoint);
        }

        //평균을 너무 넘지 않으면 직선으로 간주함.
        float averageDeviation = totalDeviation / (positions.Count - 2);
        if (averageDeviation > lineTolerance)
        {
            return false;
        }

        //충분한 길이의 직선인지 확인
        float minLength = 0.05f; //최소 직선거리
        float currentGestureLength = Vector3.Distance(start, end);
        if(currentGestureLength < minLength)
        {
            return false;
        }
        
        return true;
        //if (Vector3.Distance(start, end) < minLength) return false;
        //return true;
    }


    /// <summary>
    /// 주어진 좌표 리스트가 원을 그렸는지 판별합니다.
    /// </summary>
    /// <param name="positions">궤적 위치 리스트</param>
    /// <param name="radiusTolerance">반지름 허용 오차 (작을수록 정밀)</param>
    /// <param name="closureThreshold">시작/끝 점 간 거리 허용치</param>
    /// <returns>원형 제스처로 인식되면 true</returns>
    public static bool IsCircleGesture(List<Vector3> positions, float radiusTolerance = 0.3f, float closureThreshold = 0.2f, float angleThreshold = 270f)
    {
        if (positions.Count < 5) return false; // 점이 너무 적으면 원으로 보기 어려움

        Vector3 start = positions[0];
        Vector3 end = positions[positions.Count - 1];

        // 시작과 끝 점이 가까운지 확인 (닫힌 경로 여부)
        float closure = Vector3.Distance(start, end);
        if (closure > closureThreshold)
            return false;

        // 중심점 계산 (평균 위치)
        Vector3 center = Vector3.zero;
        foreach (var p in positions) center += p;
        center /= positions.Count;

        // 평균 반지름 계산
        float avgRadius = 0f;
        foreach (var p in positions) avgRadius += Vector3.Distance(center, p);
        avgRadius /= positions.Count;

        // 각 점이 평균 반지름에서 얼마나 벗어났는지 확인 (분산 기반)
        float variance = 0f;
        foreach (var p in positions)
        {
            float dist = Vector3.Distance(center, p);
            variance += Mathf.Abs(dist - avgRadius);
        }

        // 반지름 오차가 작으면 원으로 간주
        variance /= positions.Count;
        if (variance > radiusTolerance)
            return false;

        // 중심을 기준으로 연속 벡터 간 회전 각도를 누적
        float totalAngle = 0f;
        for (int i = 1; i < positions.Count - 1; i++)
        {
            Vector3 from = (positions[i] - center).normalized;
            Vector3 to = (positions[i + 1] - center).normalized;

            float angle = Vector3.Angle(from, to); // 각도 (0 ~ 180)
            Vector3 cross = Vector3.Cross(from, to);

            // z 방향 기준으로 회전 방향 부호 유지
            if (cross.y < 0) angle *= -1;

            totalAngle += angle;
        }

        totalAngle = Mathf.Abs(totalAngle); // 방향 무시하고 절댓값만

        return totalAngle > angleThreshold;
    }

    /// <summary>
    /// 위에서 아래로 빠르게 찌르기 제스처인지 판별합니다.
    /// </summary>
    /// <param name="positions">궤적 위치 리스트</param>
    /// <param name="minDistance">최소 거리 조건 (너무 짧으면 실패)</param>
    /// <param name="maxDuration">시간 제한 (현재 미사용)</param>
    /// <param name="directionTolerance">수직 하강 방향 유사도 (1.0은 완벽하게 아래)</param>
    /// <returns>찌르기 동작이면 true</returns>
    public static bool IsStabDownGesture(List<Vector3> positions, float minDistance = 0.3f, float maxDuration = 0.5f, float directionTolerance = 0.8f)
    {
        if (positions.Count < 2) return false;

        Vector3 start = positions[0];
        Vector3 end = positions[positions.Count - 1];
        float distance = Vector3.Distance(start, end);

        // 거리 조건 만족하지 않으면 실패
        if (distance < minDistance)
            return false;

        // 방향이 아래쪽(Y-)인지 확인
        Vector3 dir = (end - start).normalized;
        if (Vector3.Dot(dir, Vector3.down) < directionTolerance)
            return false;

        return true;
    }
}
