using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(LineRenderer))]
public class ControllerTrail : MonoBehaviour
{
    public InputActionProperty triggerAction; // 트리거 버튼 액션
    public float delayAfterRelease = 3f;

    private LineRenderer lineRenderer;
    private List<Vector3> positions = new List<Vector3>();

    private bool isTracking = false;
    private Coroutine stopTrailCoroutine;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        float triggerValue = triggerAction.action.ReadValue<float>();
        bool isPressed = triggerValue > 0.1f;

        if (isPressed)
        {
            if (!isTracking)
            {
                // 트리거 처음 누름
                StartTrail();
            }

            TrackPosition();

            // 버튼 누르는 동안은 리셋 타이머 중지
            if (stopTrailCoroutine != null)
            {
                StopCoroutine(stopTrailCoroutine);
                stopTrailCoroutine = null;
            }
        }
        else if (!isPressed && isTracking && stopTrailCoroutine == null)
        {
            // 트리거 떼면 3초 후 trail 제거 시작
            stopTrailCoroutine = StartCoroutine(StopTrailAfterDelay());
        }
    }

    void StartTrail()
    {
        isTracking = true;
        positions.Clear();
        lineRenderer.enabled = true;
    }

    void TrackPosition()
    {
        positions.Add(transform.position);
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }

    IEnumerator StopTrailAfterDelay()
    {
        yield return new WaitForSeconds(delayAfterRelease);
        ResetTrail();
    }

    void ResetTrail()
    {
        isTracking = false;
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
        positions.Clear();
    }
}
