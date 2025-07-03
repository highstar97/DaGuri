using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(LineRenderer))]
public class ControllerTracking : MonoBehaviour
{
    #region Variables
    public float delayAfterRelease = 3f;
    public InputActionProperty triggerAction;       // 트리거 버튼 액션
    public event System.Action OnTrackingFinished;

    private LineRenderer lineRenderer;
    private List<Vector3> positions = new List<Vector3>();

    private bool isTracking = false;
    private Coroutine stopTrackingCoroutine;
    #endregion

    #region Properties
    public List<Vector3> Positions => positions;
    #endregion

    #region Unity Functions
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
            if (!isTracking)    // 트리거 처음 누름
            {
                InitTracking();
            }

            TrackPosition();    // Line Renderer에 현재 위치 추가

            if (stopTrackingCoroutine != null)     // 버튼 누르는 동안은 리셋 타이머 중지
            {
                StopCoroutine(stopTrackingCoroutine);
                stopTrackingCoroutine = null;
            }
        }
        else if (!isPressed && isTracking && stopTrackingCoroutine == null)    // 트리거 안누름 + 트래킹 중 => 3초 후 trail 제거 시작
        {
            stopTrackingCoroutine = StartCoroutine(StopTrakingAfterDelay()); 
        }
    }
    #endregion

    #region User Functions
    void InitTracking()
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

    IEnumerator StopTrakingAfterDelay()
    {
        OnTrackingFinished?.Invoke();
        yield return new WaitForSeconds(delayAfterRelease);
        ResetTracking();
    }

    void ResetTracking()
    {
        isTracking = false;
        positions.Clear();
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
    }
    #endregion
}