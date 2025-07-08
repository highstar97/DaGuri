using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GestureUtils
{
    /// <summary>
    /// 주어진 좌표 리스트가 원을 그렸는지 판별합니다.
    /// </summary>
    /// <param name="positions">궤적 위치 리스트</param>
    /// <param name="radiusTolerance">반지름 허용 오차 (작을수록 정밀)</param>
    /// <param name="closureThreshold">시작/끝 점 간 거리 허용치</param>
    /// <returns>원형 제스처로 인식되면 true</returns>
    public static bool IsCircleGesture(
        List<Vector3> positions, 
        float radiusTolerance = 0.05f, 
        float closureThreshold = 0.2f, 
        float tolerance = 0.02f, 
        float allowedOutliersRatio = 0.1f, 
        float angleThreshold = 300f, 
        float maxSharpTurnAngle = 120f, 
        int maxSharpTurnsAllowed = 1)
    {
        if (positions.Count < 5)
        {
            Debug.Log($"점의 수가 작음 {positions.Count}");
            return false; // 점이 너무 적으면 원으로 보기 어려움
        }

        // 시작과 끝 점이 가까운지 확인 (닫힌 경로 여부)
        Vector3 start = positions[0];
        Vector3 end = positions[positions.Count - 1];
        float closure = Vector3.Distance(start, end);
        if (closure > closureThreshold)
        {
            Debug.Log("시작과 끝이 너무 멈");
            return false;
        }

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
        {
            Debug.Log("반지름 오차가 너무 큼");
            return false;
        }

        // 중심을 기준으로 연속 벡터 간 회전 각도를 누적, 회전량
        float totalAngle = 0f;
       
        for (int i = 1; i < positions.Count - 1; i++)
        {
            Vector3 from = (positions[i] - center).normalized;
            Vector3 to = (positions[i + 1] - center).normalized;

            float angle = Vector3.Angle(from, to);
            totalAngle += angle;
        }

        // 충분히 회전했는가? (300도 이상 권장)
        if (totalAngle < angleThreshold)
        {
            Debug.Log($"충분히 회전하지 못함 {totalAngle} < {angleThreshold}");
            return false;
        }

        // 급격한 꺾임 분석
        int sharpTurnCount = 0;
        for (int i = 1; i < positions.Count - 1; i++)
        {
            Vector3 from = (positions[i - 1] - positions[i]).normalized;
            Vector3 to = (positions[i + 1] - positions[i]).normalized;

            float angle = Vector3.Angle(from, to);

            if (angle < maxSharpTurnAngle)
            {
                ++sharpTurnCount;
            }
        }

        // 꺾인 부분이 거의 없어야 함 (sharp turn이 많으면 원 아님)
        if (sharpTurnCount >= maxSharpTurnsAllowed)
        {
            Debug.Log($"꺾인부분이 많음 {sharpTurnCount}");
            return false;
        }
        return true;
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
