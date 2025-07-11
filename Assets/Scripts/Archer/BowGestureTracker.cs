using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowGestureTracker : MonoBehaviour
{

    public Bow bow;

    public Transform rightHandControllerTransform;

    public float minBowDrawDistance = 0.05f;
    public float maxBosDrawDistance = 0.7f;

    //public event Action<List<Vector3>, bool> OnBowDrawGestureRecognized;

    private List<Vector3> currentGesturePositions = new List<Vector3>();
    private Vector3 drawStartPosition;

    private bool isTrakingGesture = false;
    private bool gestureAlreadyInvoked = false;


    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (bow == null || bow.CurrentState != Bow.State.Ready)
        {
            if (isTrakingGesture)
            {
                StopTrackingGesture();
            }
            return;
        }

        if (bow.CurrentState == Bow.State.Ready && bow.GetIsDrawBow())
        {
            if (!isTrakingGesture)
            {
                StartTrackingGesture();
            }
            TrackGesturePosition();
            CheckBowDrawGesture();

        }
        else if (isTrakingGesture)
        {
            float dist = Vector3.Distance(drawStartPosition, rightHandControllerTransform.position);

            // draw 거리 일정 이상이면 추적 유지
            if (dist < 0.05f) // 너무 가까이 가면 끊는다 (튜닝 가능)
            {
                StopTrackingGesture();
            }

        }
    }

    private void StartTrackingGesture()
    {
        isTrakingGesture = true;
        gestureAlreadyInvoked = false; // <<< 추가된 부분: 새로운 제스처 시작 시 플래그 초기화
        currentGesturePositions.Clear();
        drawStartPosition = bow.GetDrawStartPosition();

        if (rightHandControllerTransform != null)
        {
            currentGesturePositions.Add(rightHandControllerTransform.position);
        }
    }

    void TrackGesturePosition()
    {
        if (currentGesturePositions.Count == 0 ||
     Vector3.Distance(currentGesturePositions[currentGesturePositions.Count - 1], rightHandControllerTransform.position) > 0.005f)
        {
            currentGesturePositions.Add(rightHandControllerTransform.position);
        }
    }

    void StopTrackingGesture()
    {
        if (isTrakingGesture)
        {
            isTrakingGesture = false;
        }
    }

    private void CheckBowDrawGesture()
    {
        if (bow == null || !isTrakingGesture)
        {
            return;
        }

        if (currentGesturePositions.Count < 2)
        {
            return;
        }

        Vector3 currentPos = rightHandControllerTransform.position;
        float currentDrawDistance = Vector3.Distance(drawStartPosition, currentPos);

        // 오른손 트리거가 떼어지고, 충분히 당겨지면 제스쳐 인식 완료
        // 이전 코드: bool rightTriggerPressed = bow.righthandTrigger.action.ReadValue<float>() > 0.1f;
        // 이전 코드: if(!rightTriggerPressed && currentDrawDistance >= minBowDrawDistance && !gestureAlreadyInvoked)
        // <<< 수정된 부분: !rightTriggerPressed 조건을 제거하여, 트리거 눌림과 관계없이 제스처 인식을 전달합니다.
        if (currentDrawDistance >= minBowDrawDistance && !gestureAlreadyInvoked)
        {
            bool isLineGesture = GestureUtils.IsLineGesture(currentGesturePositions);
            bow.OnDrawGestureFinished(currentGesturePositions, isLineGesture);
            gestureAlreadyInvoked = true;
        }
    }
}